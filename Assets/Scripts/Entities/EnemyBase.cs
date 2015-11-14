using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyBase : MonoBehaviour
{
	// Status colors
	public static Color normal = new Color (1,1,1,1);
    public static Color burn = new Color (1,0,0,1); // red
    public static Color poison = new Color (.5f,0,.5f,1); // purple
    private static float statusFlashInterval = .5f;

    private Dictionary<EnemyState, float> activeStatusFlashCoroutineEndTimes = new Dictionary<EnemyState, float>();

    public delegate void KilledAction();
	public delegate void NodeExitAction();
	public event KilledAction Killed;
	public event NodeExitAction NodeExit;

	public int damageValue;
	public int moneyValue;
	public Vector2 healthBarSize;
	public ObjectManager objectManager;
	public SpriteRenderer spriteRenderer;
	protected Animator animator;
	protected int health;
	public int maxHealth = 100;
	private int armor = 2;

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
	public int elementType; // 0-3; 0:earth, 1:fire, 2:storm, 3:voodoo

	// Variables used for pathing
	public Node onNode;
	protected float minWaypointDisplacement;
	protected int currentWayPoint = 0;
	protected List<Node> path = null;
	private float speed = 10;

    public float Speed
    {
        get { return this.speed; }
        set
        {
            if (value < 0)
            {
                this.speed = 0;
                return;
            }
            this.speed = value;
        }
    }
	public int mindControlled = 0; // 0 is walking forward, >0 are stacked mindControle commands.
	public Node spawnNode;

    public bool StopMindControlling { get; set; }

	// Debuffs
	protected List<IBuff> debuffs = new List<IBuff> ();

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
			    this.DestroyThisEntity ();
			} else if (value > this.maxHealth) {
			    this.health = this.maxHealth;
			} else {
			    this.health = value;
			}
		}
	}

	// Runs when entity is Instantiated
	void OnEnable()
	{
		this.objectManager = ObjectManager.GetInstance ();
		this.objectManager.AddEntity (this);
	    this.onNode = this.objectManager.NodeManager.GetClosestNode (this.transform.position);
	    this.spawnNode = this.onNode;
	    this.InitAttributes();
	}	
	// Update is called once per frame
	void Update ()
	{
	    this.CorrectPosition();
	    this.Move ();
	    this.ApplyDebuffs ();
	}

    private void CorrectPosition()
    {
        // perfect for non mind control
		float correctedY = -((this.onNode.listIndexY / this.objectManager.NodeManager.size_y) + (this.onNode.listIndexX / this.objectManager.NodeManager.size_x));
        this.transform.position = new Vector3(this.transform.position.x, correctedY, this.transform.position.z);
    }

	protected void InitAttributes(){
	    this.minWaypointDisplacement = this.objectManager.MapData.NodeSize.x / 10;
	    this.spriteRenderer = this.GetComponent<SpriteRenderer> ();
	    this.animator = this.GetComponent<Animator>();
	    this.SetPath (this.objectManager.Pathfinding.Astar (this.onNode, this.objectManager.Map.destinationNode));
	    this.animator.speed = this.speed;

	    this.maxHealth += (int)(this.maxHealth * ((float)this.objectManager.gameState.dificultyFactor * (float)(this.objectManager.gameState.waveCount)));
	    this.moneyValue += (int)(this.moneyValue * this.objectManager.gameState.enemyValueFactor * (this.objectManager.gameState.waveCount));
	    this.health = this.maxHealth;
	    this.speed = this.speed + Random.Range(-1f,1f);
	}

	/// <summary>
	/// Sets the path.
	/// </summary>
	/// <param name="path">Path.</param>
	public void SetPath (List<Node> path)
	{
		if (path == null || path.Count == 0)
			return;
		
		this.path = path;
	    this.currentWayPoint = path.Count - 1;
	    this.animator.SetInteger("walking", this.onNode.GetDirection(path[this.currentWayPoint]));
		// Start walk animation.
		
	}

	// called in update
	// move the unit closer to the next tile in it's path.
	public void Move ()
	{
		if(this.path == null)
			return;

	    this.animator.speed = 5 * (this.speed/20 + 1) * (float)this.objectManager.gameState.GameSpeed;
	    this.SetState((int)State.Walking);

		// don't move in the Y direction.
		Vector3 moveVector = new Vector3 (this.transform.position.x - this.path [this.currentWayPoint].UnityPosition.x,
		                                 0, this.transform.position.z - this.path [this.currentWayPoint].UnityPosition.z).normalized;
		
		// update the position
	    this.transform.position = new Vector3 (this.transform.position.x - moveVector.x * (this.speed * (float)this.objectManager.gameState.GameSpeed) * Time.deltaTime, this.transform.position.y, this.transform.position.z - moveVector.z * (this.speed * (float)this.objectManager.gameState.GameSpeed) * Time.deltaTime);
		
		// unit has reached the waypoint
		Vector3 position = this.transform.position;
		position.y = this.path [this.currentWayPoint].UnityPosition.y;
		//if (_ObjectManager._Map.GetNodeFromLocation(transform.position) == path[currentWayPoint]) {
		if(Vector3.Distance(position, this.path [this.currentWayPoint].UnityPosition) <= this.minWaypointDisplacement){

			if(this.onNode.enemie == this)
			{
			    this.onNode.enemie = null;
			}
		    this.onNode = this.path [this.currentWayPoint];
		    this.onNode.enemie = this;

			if(this.NodeExit != null)
			{
			    this.NodeExit();
			}
		    this.currentWayPoint--;

            //if (this.StopMindControlling)
            //{
            //    this.debuffs = this.debuffs.Where(d => d.enemyState != EnemyState.MindControl).ToList();
            //    this.mindControlled = 0;
            //    this.SetPath(this.objectManager.Pathfinding.Astar(this.onNode, this.objectManager.Map.destinationNode));
            //    this.StopMindControlling = false;
            //    return;
            //}

			// unit has reached the destination
			if (this.currentWayPoint < 0) {
				if(this.mindControlled > 0){
				    //this.debuffs = this.debuffs.Where(d => d.enemyState != EnemyState.MindControl).ToList();
				    //this.mindControlled = 0;
				    //this.SetPath(this.objectManager.Pathfinding.Astar(this.onNode, this.objectManager.Map.destinationNode));
					return;
				}
			    this.DestroyThisEntity ();
				return;
			}
		    this.animator.SetInteger("walking", this.onNode.GetDirection(this.path[this.currentWayPoint]));
		}
	}

	/// <summary>
	/// Applies the debuffs to the enemy
	/// </summary>
	public void ApplyDebuffs ()
	{
		List<IBuff> removeList = new List<IBuff> ();
		foreach (IBuff debuff in this.debuffs)
			if (debuff.Apply (Time.deltaTime * (float)this.objectManager.gameState.GameSpeed))
				removeList.Add (debuff);

		foreach (IBuff debuff in removeList) {
		    this.debuffs.Remove (debuff);
		    this.spriteRenderer.color = normal;
		}
	}

    /// <summary>
	/// Dosn't take armor into acount.
	/// </summary>
	public void DirectDamage (int damage, int towerElement)
	{
		if(towerElement == this.elementType)
			damage = (int) (damage * .8f); // reduces damage by 20%

		if (damage == 0)
			damage = 1;

	    this.Health -= damage;
	}

    /// <summary>
	/// Dosn't take armor into acount.
	/// </summary>
	public void DirectDamage(int damage)
    {
        if (damage == 0)
            damage = 1;

        this.Health -= damage;
    }

    /// <summary>
    /// Reduces the damage the enemy takes by it's armor.
    /// </summary>
    public void Damage (int damage, int towerElement)
	{
		if(towerElement == this.elementType)
			damage = (int) (damage * .8f);; // reduces damage by 20%

		if (damage == 0)
			damage = 1;

	    this.Health -= damage;
        
        // In case we want armor to be used again.
        //int armorOffsetDamage = damage - armor;
        //if (armorOffsetDamage > 0) {
        //    Health -= armorOffsetDamage;
        //}
	}

	/// <summary>
	/// Damages the enemy over time. Takes into acount the enemy's armor.
	/// </summary>
	public void DamageOverTime (int damage, float duration, float interval, EnemyState enemyState, int towerElement)
	{
	    if (towerElement == this.elementType)
	    {
	        return;
	    }

	    if (enemyState == EnemyState.Poison)
	    {
	        IBuff buff = new DamageOverTime(this, duration, damage, interval);
            this.debuffs.Add(buff);
            this.StartOrUpdateCoroutine(enemyState, duration);
	    }
        else if (enemyState == EnemyState.Burn)
        {
            IBuff buff = new DamageOverTime(this, duration, damage, interval);
            this.debuffs.Add(buff);
            this.StartOrUpdateCoroutine(enemyState, duration);
        }
	}

    /// <summary>
    /// A helper method that updates or starts the Flash coroutine.
    /// </summary>
    /// <param name="enemyState">The coroutine to update</param>
    /// <param name="duration">The duration of the coroutine</param>
    private void StartOrUpdateCoroutine(EnemyState enemyState, float duration)
    {
        if (!this.activeStatusFlashCoroutineEndTimes.ContainsKey(enemyState) ||
            this.activeStatusFlashCoroutineEndTimes[enemyState] < Time.time)
        {
            this.activeStatusFlashCoroutineEndTimes[enemyState] = Time.time + duration;
            this.StartCoroutine(this.FlashColor(EnemyBase.burn, enemyState));
            return;
        }
        this.activeStatusFlashCoroutineEndTimes[enemyState] = Time.time + duration;
    }

    /// <summary>
    /// Flashes the debuff color on the agent.
    /// </summary>
    /// <param name="color">The color to flash.</param>
    /// <param name="state">The state of the flash. 
    /// Used as the key into activeStatusFlashCoroutineEndTimes. </param>
    IEnumerator FlashColor(Color color, EnemyState state)
    {
        while (true)
        {
            this.spriteRenderer.color = color;
            this.Invoke("SetDefaultColor", .15f);

            float endTime;
            if (this.activeStatusFlashCoroutineEndTimes.TryGetValue(state, out endTime))
            {
                if (this.objectManager.gameState.GameSpeed == GameSpeed.Paused)
                {
                    this.activeStatusFlashCoroutineEndTimes[state] = endTime + EnemyBase.statusFlashInterval;
                }
                else if (this.objectManager.gameState.GameSpeed != GameSpeed.X1)
                {
                    float adjustedInterval = EnemyBase.statusFlashInterval /
                                             (float)this.objectManager.gameState.GameSpeed;
                    this.activeStatusFlashCoroutineEndTimes[state] = endTime - adjustedInterval;
                    yield return new WaitForSeconds(adjustedInterval);
                    continue;
                }

                if (Time.time > endTime)
                {
                    break;
                }
                yield return new WaitForSeconds(EnemyBase.statusFlashInterval);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Called by the Invoke method.
    /// Sets the color of the agent back to normal.
    /// </summary>
    private void SetDefaultColor()
    {
        if (this.spriteRenderer.color != EnemyBase.normal)
        {
            this.spriteRenderer.color = EnemyBase.normal;
        }
    }

    /// <summary>
    /// Reduces the armor.
    /// </summary>
    public void ReduceArmor (int amount, float duration, int towerElement)
	{
		if(towerElement == this.elementType)
			return;

		IBuff buff = new ArmorReduce(this, duration, amount);
	    this.debuffs.Add (buff);
	}

	/// <summary>
	/// Makes the enemy move backwards along it's path.
	/// </summary>
	public void MindControl (float duration, int towerElement)
	{
		if(towerElement == this.elementType)
			return;

        if (duration <= 0)
            return;
        
		IBuff buff = new ReverseDirection(this, duration);
	    this.debuffs.Add (buff);
	}

	//Note: percentage is 0-1 not 0-100
	public void Slow (float percentage, float duration, int towerElement)
	{
		if(towerElement == this.elementType)
			return;
        
		IBuff buff = new Slow (this, duration, percentage);
	    this.debuffs.Add (buff);
	}

	public virtual Vector2 GetPixelSize ()
	{
		Vector3 start = Camera.main.WorldToScreenPoint (new Vector3 (this.spriteRenderer.bounds.min.x, this.spriteRenderer.bounds.min.y, this.spriteRenderer.bounds.min.z));
		Vector3 end = Camera.main.WorldToScreenPoint (new Vector3 (this.spriteRenderer.bounds.max.x, this.spriteRenderer.bounds.max.y, this.spriteRenderer.bounds.max.z));
		
		int widthX = (int)(end.z - start.z);
		int widthY = (int)(end.y - start.y);
		
		return new Vector2 (widthX, widthY);
	}

	public virtual void DestroyThisEntity ()
	{
		if(this.Killed != null)
		{
		    this.Killed();
		}

		TextMesh deathInt = this.objectManager.Map.enemyDeathInt.GetObjectFromPool<TextMesh>(this.objectManager.Map.enemyDeathInt.name, new Vector3 (this.transform.position.x, 40, this.transform.position.z), Quaternion.Euler(new Vector3(90, 0, 0)));
		deathInt.gameObject.name = this.objectManager.Map.enemyDeathInt.name;

		if(this.onNode == this.objectManager.Map.destinationNode){
			this.objectManager.gameState.PlayerHealth -= this.damageValue;
			deathInt.text = "-"+ this.damageValue;
		}else if (!this.objectManager.gameState.gameOver){
			this.objectManager.gameState.playerMoney += this.moneyValue;
			this.objectManager.gameState.score += this.moneyValue;
			deathInt.text = "+"+ this.moneyValue;
		}

		if (this.onNode.enemie == this)
		{
		    this.onNode.enemie = null;
		}

		this.objectManager.DeReference (this);
		this.gameObject.ReturnToPool(this.gameObject.name);
	}

	public void SetState(int stateId){
		if(this.animator.GetInteger("currentState") != stateId){
		    this.animator.SetTrigger("resetState");
		    this.animator.SetInteger("currentState",stateId);
		}
	}
	
}
