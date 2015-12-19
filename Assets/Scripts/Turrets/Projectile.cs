using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	// Configurable
	public float range;
	public float speed;
	public float Speed { get{return speed * (float)objectManager.gameState.GameSpeed;} set{speed = value;} }
	public EnemyBase target;
	public Vector3 targetPosition;

    public int AoeDamage { get; set; }

    public float AoeRange { get; set; }

	public int Damage { get; set; }

    public int DamageOverTime { get; set; }

    public float Slow { get; set; }
    public float SlowDuration { get; set; }

    public Turret Owner { get; set; }
	
	// Internal
	private float distance;
    private ObjectManager objectManager;
	
	// Runs when entity is Instantiated
	void Awake ()
	{
        objectManager = ObjectManager.GetInstance();
        this.Initialize();
	}

    void OnEnable()
    {
        this.Initialize();
    }

    /// <summary>
    ///     Sets up the projectile!
    /// </summary>
    private void Initialize()
    {
        distance = 0;
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (target != null)
        {
            targetPosition = target.transform.position;
        }

		Vector3 moveVector = new Vector3 (transform.position.x - targetPosition.x,
		                                 transform.position.y - targetPosition.y,
		                                 transform.position.z - targetPosition.z).normalized;
		
		// update the position
		transform.position = new Vector3 (transform.position.x - moveVector.x * Speed * Time.deltaTime,
		                                 transform.position.y - moveVector.y * Speed * Time.deltaTime,
		                                 transform.position.z - moveVector.z * Speed * Time.deltaTime);
		                                 
		distance += Time.deltaTime * Speed;
		
		if (distance > range ||
			Vector3.Distance (transform.position, new Vector3 (targetPosition.x, targetPosition.y, targetPosition.z)) < 1) 
        {
            ////Destroy (gameObject);
            transform.gameObject.ReturnToPool(this.gameObject.name);
			if (target != null) 
            {

                if (Damage > 0)
                {
                    target.Damage(Damage);
                }

                if (Slow > 0)
                {
					target.Slow(Slow, SlowDuration);
                }

				if(DamageOverTime > 0)
				{
					target.DamageOverTime(DamageOverTime, 3.0f, .5f, StatusEffects.Burn);
				}

				if(Owner.turretModel.MindControlDuration > 0)
				{
					target.ReverseDirection(Owner.turretModel.MindControlDuration);
				}
              
                if (AoeDamage > 0 && AoeRange > 0)
                {
                    foreach (EnemyBase enemy in objectManager.enemies)
                    {
                        Vector3 temp1 = new Vector3(targetPosition.x, enemy.transform.position.y, targetPosition.z);
                        if (Vector3.Distance(temp1, enemy.transform.position) < AoeRange)
                        {
                            var enemyTargetPosition = enemy.transform.position;
                            ////GameObject projectileObject = Instantiate(Owner.projectileType, targetPosition, Quaternion.LookRotation(enemyTargetPosition)) as GameObject;
                            ////Projectile projectile = projectileObject.GetComponent<Projectile>();
                            var projectile = Owner.projectileType.GetObjectFromPool<Projectile>(
                                ObjectPools.ProjectilePool,
                                targetPosition,
                                Quaternion.LookRotation(enemyTargetPosition));
                            projectile.Damage = AoeDamage;
                            projectile.target = enemy;
                            projectile.targetPosition = enemyTargetPosition;
                            projectile.Owner = this.Owner;
							projectile.DamageOverTime = DamageOverTime;
                        }
                    }
                }
			}
		}
	}
}