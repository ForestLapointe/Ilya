using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public List<Combat.Effect> effects;
    List<Combat> AlreadyDamaged;
    [SerializeField] GameObject NextAttack;
    [SerializeField] bool AutoAttack;
    public float lifeSpan;
    public float age;
    public float damageDelay;
    public bool ParentToAttacker;
    public Combat Attacker;
    public Vector2 Damage;
    public Vector3 Offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AlreadyDamaged = new List<Combat>();
        if (ParentToAttacker)
        {
            transform.parent = Attacker.transform;
            transform.localEulerAngles = Vector3.zero;
        }
        transform.localPosition = Offset;
        Attacker.CurrentAttack = this;
    }

    // Update is called once per frame
    void Update()
    {
        age += Time.deltaTime;
        if (age >= lifeSpan) 
        {
            Destroy(gameObject);
            Attacker.CurrentAttack = null;
            if (NextAttack != null && AutoAttack && AlreadyDamaged.Count > 0) 
            { 
                GameObject newAttack = Instantiate(NextAttack);
                newAttack.transform.position = transform.position;
                newAttack.GetComponent<Attack>().Attacker = Attacker;
            }
        }

    }
    private void OnTriggerStay (Collider target)
    {
        if (age >= damageDelay) 
        {
            if (target.TryGetComponent<Combat> (out Combat Target))
            {
                if (!AlreadyDamaged.Contains (Target)) 
                {
                    if (Target.team != Attacker.team) 
                    { 
                        Target.health -= Random.Range (Damage.x,Damage.y); 
                        AlreadyDamaged.Add (Target);
                        
                        foreach (Combat.Effect effect in effects)
                        {
                            Target.addeffect (effect);
                        } 
                    }
                }
            }
        }
    }
}