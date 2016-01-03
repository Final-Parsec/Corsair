using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Animator))]
public class EnemyBase : Agent
{
    /// <summary>
    /// A <see cref="Dictionary{StatusEffects, float}"/> of effects to floats.
    /// Used to track active Coroutines flashing colors on the <see cref="EnemyBase"/>.
    /// </summary>
    private Dictionary<StatusEffects, float> activeStatusFlashCoroutineEndTimes = new Dictionary<StatusEffects, float>();

    /// <summary>
    /// The event is called when the <see cref="EnemyBase"/> is killed.
    /// </summary>
    public delegate void KilledAction();
	public event KilledAction Killed;

    /// <summary>
    /// The damage done to the player.
    /// </summary>
	public int damageValue;

    /// <summary>
    /// The money gained by the player when they kill it.
    /// </summary>
	public int moneyValue;

    /// <summary>
    /// The size of the healthbar.
    /// </summary>
	public Vector2 healthBarSize;

    /// <summary>
    /// A reference to the <see cref="Animator"/> component.
    /// </summary>
	protected Animator animator;

    /// <summary>
    /// The current health of the <see cref="EnemyBase"/>.
    /// </summary>
	protected int health;

    /// <summary>
    /// The max posible health of the <see cref="EnemyBase"/>.
    /// </summary>
	public int maxHealth = 100;

    /// <summary>
    /// The armor.
    /// </summary>
	private int armor = 2;

    /// <summary>
    /// Gets or sets the Armor value.
    /// </summary>
    public int Armor
    {
        get { return this.armor; }
        set
        {
            if (value < 0)
            {
                this.armor = 0;
                return;
            }
            this.armor = value;
        }
    }

	/// <summary>
    /// A list of Debuffs that are effecting the <see cref="EnemyBase"/>.
    /// </summary>
	protected List<IBuff> debuffs = new List<IBuff> ();

    /// <summary>
    /// Gets or sets the health.
    /// </summary>
	public int Health {
		get {
			return this.health;
		}
		private set {
			if (this.health == 0) {
				// We're already dead. Don't do anything.
				// There appeared to some race condition where an enemy would destory itself multiple times.
				return;
			}
		
			if (value < 1) {
			    this.health = 0;
                StartCoroutine(this.DestroyThis ());
			} else if (value > this.maxHealth) {
			    this.health = this.maxHealth;
			} else {
			    this.health = value;
			}
		}
	}

	/// <summary>
    /// Called when object is enabled.
    /// </summary>
	void OnEnable()
	{
		this.objectManager = ObjectManager.GetInstance ();
		this.objectManager.AddEntity (this);
	    this.onNode = this.objectManager.NodeManager.GetClosestNode (this.transform.position);
	    this.SpawnNode = this.onNode;
	    this.InitAttributes();
        this.transform.position = this.objectManager.NodeManager.CorrectPosition(this.transform.position);

        animator.SetTrigger("Alive");
    }	

	/// <summary>
    /// Called once per frame.
    /// </summary>
	void Update ()
	{
        if (Health == 0)
        {
            return;
        }

        this.Move();
	    this.ApplyDebuffs();
    }

    /// <summary>
    /// Initializes the attributes.
    /// </summary>
	protected void InitAttributes()
    {
	    this.minWaypointDisplacement = this.objectManager.MapData.NodeSize.x / 10;
	    this.animator = this.GetComponent<Animator>();
	    this.SetPath (this.objectManager.Pathfinding.Astar (this.onNode, this.objectManager.WaveManager.destinationNode));
	    this.animator.speed = this.speed;

	    this.maxHealth += (int)(this.maxHealth * ((float)this.objectManager.gameState.dificultyFactor * (float)(this.objectManager.gameState.waveCount)));
	    this.moneyValue += (int)(this.moneyValue * this.objectManager.gameState.enemyValueFactor * (this.objectManager.gameState.waveCount));
	    this.health = this.maxHealth;
	}

	


	/// <summary>
	/// Applies the debuffs to the <see cref="EnemyBase"/>
	/// </summary>
	public void ApplyDebuffs ()
	{
		List<IBuff> removeList = new List<IBuff> ();
		foreach (IBuff debuff in this.debuffs)
			if (debuff.Apply (Time.deltaTime * (float)this.objectManager.gameState.GameSpeed))
				removeList.Add (debuff);

		foreach (IBuff debuff in removeList) {
		    this.debuffs.Remove (debuff);
		}
	}

    /// <summary>
    /// Doesn't take armor into acount.
    /// </summary>
    /// <param name="damage">Damage to inclict.</param>
    public void DirectDamage(int damage)
    {
        if (damage == 0)
            damage = 1;

        this.Health -= damage;
    }

    /// <summary>
    /// Reduces the damage the enemy takes by it's armor.
    /// </summary>
    /// <param name="damage">Damage inflicted before armor reduction.</param>
    public void Damage (int damage)
	{
		if (damage == 0)
			damage = 1;

	    this.Health -= damage;
        
        // In case we want armor to be used again.
        int armorOffsetDamage = damage - armor;
        if (armorOffsetDamage > 0) {
            Health -= armorOffsetDamage;
        }
	}

    /// <summary>
    /// Damages the enemy over time.
    /// Updates a coroutine that flashes a color over the <see cref="EnemyBase"/> sprite.  
    /// </summary>
    /// <param name="damage">The total damage.</param>
    /// <param name="duration">Duration of effect.</param>
    /// <param name="interval">The interval to inflict damage.</param>
    /// <param name="status">The status of the enemy.</param>
    public void DamageOverTime(int damage, float duration, float interval, StatusEffects status)
    {
        if (status == StatusEffects.Poison)
        {
            IBuff buff = new DamageOverTime(this, duration, damage, interval);
            this.debuffs.Add(buff);
            this.StartOrUpdateCoroutine(status, duration);
        }
        else if (status == StatusEffects.Burn)
        {
            IBuff buff = new DamageOverTime(this, duration, damage, interval);
            this.debuffs.Add(buff);
            this.StartOrUpdateCoroutine(status, duration);
        }
    }

    /// <summary>
    /// Reduces the armor.
    /// </summary>
    /// <param name="amount">Amount to reduce.</param>
    /// <param name="duration">Duration of effect.</param>
    public void ReduceArmor (int amount, float duration)
	{

		IBuff buff = new ArmorReduce(this, duration, amount);
	    this.debuffs.Add (buff);
	}

    /// <summary>
    /// Makes the enemy move backwards along it's path. 
    /// </summary>
    /// <param name="duration">Duration of effect.</param>
    public void ReverseDirection (float duration)
	{
        if (duration <= 0)
            return;
        
		IBuff buff = new ReverseDirection(this, duration);
	    this.debuffs.Add (buff);
	}

    /// <summary>
    /// Slows the <see cref="EnemyBase"/> by a percentage.
    /// </summary>
    /// <param name="percentage">Percent to slow.</param>
    /// <param name="duration">Duration of effect.</param>
    /// <remarks>Percentage is 0-1 not 0-100</remarks>
    public void Slow (float percentage, float duration)
	{        
		IBuff buff = new Slow (this, duration, percentage);
	    this.debuffs.Add (buff);
	}

    /// <summary>
    /// A helper method that updates or starts the Flash coroutine.
    /// </summary>
    /// <param name="enemyState">The coroutine to update</param>
    /// <param name="duration">The duration of the coroutine</param>
    private void StartOrUpdateCoroutine(StatusEffects enemyState, float duration)
    {
        if (!this.activeStatusFlashCoroutineEndTimes.ContainsKey(enemyState) ||
            this.activeStatusFlashCoroutineEndTimes[enemyState] < Time.time)
        {
            this.activeStatusFlashCoroutineEndTimes[enemyState] = Time.time + duration;
            return;
        }
        this.activeStatusFlashCoroutineEndTimes[enemyState] = Time.time + duration;
    }

    /// <summary>
    /// Is called when the <see cref="Agnet"/> changes <see cref="Node"/>s.
    /// </summary>
    protected override void AgentChangedNodes()
    {
        if (this.onNode.enemie == this)
        {
            this.onNode.enemie = null;
        }
        this.onNode = this.path[this.currentPathIndex];
        this.onNode.enemie = this;
    }

    /// <summary>
    /// Is called when the <see cref="Agnet"/> reaches the destination.
    /// </summary>
    protected override void AgentReachedDestination()
    {
        if (this.movingBackwards > 0)
        {
            return;
        }
        StartCoroutine(this.DestroyThis());
    }

    /// <summary>
    /// Called when this objects time has come to an end.
    /// Spawns a death int with the money the user get, returns enemy to pool, and cleans up references.
    /// </summary>
    public virtual IEnumerator DestroyThis()
    {
        // Call callback
        if (this.Killed != null)
        {
            this.Killed();
        }

        TextMesh deathInt = this.objectManager.Map.enemyDeathInt.GetObjectFromPool<TextMesh>(this.objectManager.Map.enemyDeathInt.name, new Vector3(this.transform.position.x, 40, this.transform.position.z), Quaternion.Euler(new Vector3(90, 0, 0)));
        deathInt.gameObject.name = this.objectManager.Map.enemyDeathInt.name;

        if (this.onNode.enemie == this)
        {
            this.onNode.enemie = null;
        }

        this.objectManager.DeReference(this);

        if (this.onNode == this.objectManager.WaveManager.destinationNode)
        {
            this.objectManager.gameState.PlayerHealth -= this.damageValue;
            deathInt.text = "-" + this.damageValue;
        }
        else if (!this.objectManager.gameState.gameOver)
        {
            this.objectManager.gameState.playerMoney += this.moneyValue;
            this.objectManager.gameState.score += this.moneyValue;
            deathInt.text = "+" + this.moneyValue;
        }

        // remove debuffs
        foreach (var debuff in debuffs)
        {
            debuff.EndEffect();
        }
        debuffs.Clear();

        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(1.5f);

        this.gameObject.ReturnToPool(this.gameObject.name);
    }
}
