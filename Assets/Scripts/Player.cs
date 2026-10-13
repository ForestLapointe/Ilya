using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Combat;

public class Player : MonoBehaviour,Combat
{

    #region Variables

    #region Combat
    public List<Effect> effects
    {
        get { return _effects; }
        set { _effects = value; }
    }
    List<Effect> _effects;
    public bool IsDead
    {
        get { return health <= 0; }
    }
    [SerializeField] GameObject deathScreen;
    public GameObject[] attacks;
    public UnityEngine.UI.RawImage healthbar;
    public float _health;
    public float health
    {
        get { return _health; }
        set
        {
			float difference = value - _health;
            _health = _health + (difference * (difference > 0? (statHealingPower.x * statHealingPower.y) : ((1/statDefense.x) * statDefense.y)));
            healthbar.rectTransform.sizeDelta = new Vector2 (_health, healthbar.rectTransform.sizeDelta.y);
            if (_health <= 0)
            {
                deathScreen.SetActive (true);
            }
        }
    }
    public int team { get { return _team; } set { _team = value; } }
    public int _team;
    public Attack CurrentAttack { get { return _CurrentAttack; } set { _CurrentAttack = value; } }
    public Attack _CurrentAttack;

    #endregion Combat
    #region Stats
    public Vector2 statDefense;
    public Vector2 statPower;
    public Vector2 statHealingPower;
    public Vector2 statSpeed;
    #endregion Stats
    #region Movement
    Rigidbody CharacterBody;
	
	public float rotationspeed;
	public float JumpPower;
    #endregion Movement

    #endregion Variables

    void Awake() 
	{
		Combat.Teams = new List<List<Combat>>();
	}
    void Start()
	{
		CharacterBody = GetComponent<Rigidbody>();
		Combat.AddMember(this);
		effects = new List<Effect>();
	}
	void Update()
	{
		handleeffects (); 
		bool touchingGround = Physics.Raycast(transform.position,Vector3.down,1.1f);

		if (!IsDead)
		{
			//Controls
			Vector3 moveDirection = Vector3.zero;

			//Strafing
			if (Input.GetKey(KeyCode.Q)) moveDirection += transform.right * (-statSpeed.x * statSpeed.y);  
			if (Input.GetKey(KeyCode.E)) moveDirection += transform.right * (statSpeed.x * statSpeed.y);

			//Forward and Backward
			if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward * (statSpeed.x * statSpeed.y);
			if (Input.GetKey(KeyCode.S)) moveDirection += transform.forward * (-statSpeed.x * statSpeed.y);
			CharacterBody.linearVelocity = new Vector3(moveDirection.x, CharacterBody.linearVelocity.y, moveDirection.z);

			//Rotation
			if (Input.GetKey(KeyCode.A)) transform.eulerAngles += new Vector3(0, -rotationspeed, 0);
			if (Input.GetKey(KeyCode.D)) transform.eulerAngles += new Vector3(0, rotationspeed, 0);

			//Jump
			if (touchingGround && Input.GetKey(KeyCode.Space)) CharacterBody.linearVelocity += new Vector3(0, JumpPower, 0);

			int ButtonPressed = -1;
			if (Input.GetKeyDown(KeyCode.Alpha1)) ButtonPressed = 0;
			if (Input.GetKeyDown(KeyCode.Alpha2)) ButtonPressed = 1;
			if (Input.GetKeyDown(KeyCode.Alpha3)) ButtonPressed = 2;
			if (Input.GetKeyDown(KeyCode.Alpha4)) ButtonPressed = 3;
			if (Input.GetKeyDown(KeyCode.Alpha5)) ButtonPressed = 4;
			if (Input.GetKeyDown(KeyCode.Alpha6)) ButtonPressed = 5;
			if (Input.GetKeyDown(KeyCode.Alpha7)) ButtonPressed = 6;
			if (Input.GetKeyDown(KeyCode.Alpha8)) ButtonPressed = 7;
			if (Input.GetKeyDown(KeyCode.Alpha9)) ButtonPressed = 8;
			if (Input.GetKeyDown(KeyCode.Alpha0)) ButtonPressed = 9;

			//Key 1 Press
			if (ButtonPressed != -1 && CurrentAttack == null)
			{
				if (attacks[ButtonPressed] != null)
				{
					Attack attack = Instantiate(attacks[ButtonPressed]).GetComponent<Attack>();
					attack.Attacker = this;
					attack.transform.position = transform.position + (transform.forward);
				}
			}
		}
	}
	void handleeffects ()
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
                    case Combat.Effect.Type.damageovertime :
						health -= effect.potency;
                        break;
					case Combat.Effect.Type.cripple :
                        statSpeed.y *= effect.potency;
						break;
					case Combat.Effect.Type.powermodifier :
						statPower.y *= effect.potency;
						break;
                    case Combat.Effect.Type.defensemodifier:
                        statDefense.y *= effect.potency;
                        break;
					case Combat.Effect.Type.speedmodifier :
						statSpeed.y *= effect.potency;
						break;
                }
            } 
		}
	}
	public void addeffect (Effect effect)
	{
		Effect currenteffect = findeffect (effect.type);
        if (currenteffect == null)
		{
			effects.Add(effect);
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
}