using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static Combat;

public class EnemyScript : MonoBehaviour,Combat
{
    #region Variables
    Vector3 StartPosition;
    public List<Effect> effects 
    {
        get { return _effects; } 
        set { _effects = value; }
    }
    List<Effect> _effects;
    NavMeshAgent Agent;
    public float health
    {
        get { return _health; }
	    set
        {
            float difference = value - _health;
            if (statDefense.x == 0)
            {
                statDefense.x = .001f;
                Debug.LogError("statdefense.x is 0");
            }
            _health = _health + (difference * (difference > 0 ? (statHealingPower.x * statHealingPower.y) : ((1 / statDefense.x) * statDefense.y)));
            healthbar.rectTransform.sizeDelta = new Vector2 (_health,healthbar.rectTransform.sizeDelta.y);
        }
	}
    public float _health;
    public int team 
    { 
        get { return _team; } 
        set { _team = value; } 
    }
    public int _team;
    [SerializeField]List<Vector3> Path;
    public UnityEngine.UI.RawImage healthbar;
    [SerializeField] Canvas canvas;
    public GameObject[] attacks;
    public Attack CurrentAttack 
    { 
        get { return _CurrentAttack; } 
        set { _CurrentAttack = value; } 
    }
    public Attack _CurrentAttack;
    List<string> ActionName;
    List<object[]> ActionData;
    [SerializeField] float range;

    #region Stats
    public Vector2 statDefense;
    public Vector2 statPower;
    public Vector2 statHealingPower;
    public Vector2 statSpeed;
    #endregion Stats

    #endregion Variables

    void Start()
    {
        effects = new List<Effect>();
        Agent = GetComponent<NavMeshAgent>();
        StartPosition = transform.position;
        Combat.AddMember(this);
        ActionName = new List<string>();
        ActionData = new List<object[]>();
        AddAction("idle");
    }

    void Update()
    {
        healthbar.rectTransform.LookAt(Camera.main.transform.position);
        canvas.transform.position = transform.position;
        handleeffects();
        StateMachine();
        if (_health <= 0) Destroy (gameObject);
    }

    void AddAction(string name,params object[] data) 
    {
        ActionName.Add (name);
        ActionData.Add (data);
    }

    void FinishAction()
    {
        ActionName.RemoveAt (ActionName.Count-1);
        ActionData.RemoveAt (ActionData.Count-1);
    }

    void StateMachine()
    {
        Vector3 Target;
        Transform TargetTF;
        switch (ActionName.Last())
        {
            case "idle":
                AddAction("patrol path");
                break;
            case "move to":
                if (ActionData.Last()[0].GetType() == typeof (Transform))
                {
                    Target = ((Transform) ActionData.Last()[0]).position;
                }
                else
                {
                    Target = (Vector3) ActionData.Last()[0];
                }
                #region interruptable
                bool interruptable = true;
                if (ActionData.Last().Length > 1) interruptable = (bool) ActionData.Last()[1];
                if (interruptable) 
                { 
                    if (Combat.findnearest (transform.position,range,team,out Combat nearestTarget))
                    {
                        FinishAction();
                        AddAction ("attack", nearestTarget.transform);
                        AddAction ("move to", nearestTarget.transform, false);                    
                    }
                }
                #endregion interruptable

                //Move Enemy NavMesh
                Agent.SetDestination (Target);
                if ((transform.position - Target).magnitude < 2)
                {
                   FinishAction();
                }
                break;
            case "attack" :
                    TargetTF = (Transform) ActionData.Last()[0];
                    Vector3 lookposition = TargetTF.position;
                    lookposition.y = transform.position.y;
                    transform.LookAt (lookposition);
                if (CurrentAttack == null)
                {
                    Attack attack = Instantiate(attacks[0]).GetComponent<Attack>();
                    attack.Attacker = this;
                    attack.transform.position = transform.position + (transform.forward);
                    attack.Damage *= statPower.x * statPower.y;
                }
                FinishAction();
                break;
            case "patrol path" :
                FinishAction();
                foreach (Vector3 node in Path)
                {
                    AddAction("move to", node + StartPosition);
                }                   
                break;
        }
    }
    public void addeffect (Effect effect)
    {
        Effect currenteffect = findeffect (effect.type);
        if (currenteffect == null)
        {
            effects.Add (effect);
        }
        else
        {
            currenteffect.time += effect.time;
        }
    }
    Effect findeffect (Effect.Type type)
    {
        foreach (Effect effect in effects)
        {
            if (effect.type == type) return effect;
        }
        return null;
    }
    void handleeffects()
    {
        statSpeed.y = 1;
        foreach (Effect effect in effects)
        {
            effect.currenttime -= Time.deltaTime;
            if (effect.currenttime <= 0)
            {
                effect.currenttime = effect.tickrate;
                switch (effect.type)
                {
                    case Combat.Effect.Type.damageovertime:
                        health -= effect.potency;
                        break;
                    case Combat.Effect.Type.cripple:
                        statSpeed.y *= effect.potency;
                        break;
                    case Combat.Effect.Type.powermodifier:
                        statPower.y *= effect.potency;
                        break;
                    case Combat.Effect.Type.defensemodifier:
                        statDefense.y *= effect.potency;
                        break;
                    case Combat.Effect.Type.speedmodifier:
                        statSpeed.y *= effect.potency;
                        break;
                }
            }
        }
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = new Color(1,1,1,0.3f);
        Gizmos.DrawSphere (transform.position, range);
        Gizmos.color = Color.red;
        for (int i = 0; i<Path.Count ;i++) 
        {
            if (i>0)  
            {
                Gizmos.DrawLine (transform.position+Path[i - 1],transform.position+Path[i]);
            }
            else
            {
                Gizmos.DrawLine(transform.position + Path[Path.Count - 1], transform.position + Path[0]);
            }
        }
    }
}