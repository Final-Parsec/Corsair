using UnityEngine;

public class DamageOverTime : DebuffBase
{
    /// <summary>
    /// The ammount of damage to inflict at each interval.
    /// </summary>
    private readonly int damageSegment;

    /// <summary>
    /// The interval that events occur, in seconds.
    /// </summary>
    private readonly float interval;

    /// <summary>
    /// Stores the relative time in seconds that the next event will take place.
    /// </summary>
    private float nextEvent;
    
    /// <summary>
    /// Initializes an instance of <see cref="DebuffBase"/>.
    /// </summary>
    /// <param name="owner">The agent under the effect.</param>
    /// <param name="duration">The length of time the buff is active.</param>
    /// <param name="totalDamage">The total damage done.</param>
    /// <param name="interval">The interval to apply damage.</param>
    public DamageOverTime(EnemyBase owner, float duration, int totalDamage, float interval)
	{
        this.objectManager = ObjectManager.GetInstance();
        this.owner = owner;
		this.duration = duration;
        this.interval = interval;
        this.damageSegment = totalDamage / (int)(duration / this.interval);
	}

    /// <summary>
    /// Applies the Buff.
    /// </summary>
    /// <param name="deltaTime">The time between the last application.</param>
    /// <returns>A boolean value. True if the Buff if finished else false.</returns>
	public override bool Apply (float deltaTime)
	{
	    this.elapsedTime += deltaTime;
        if (this.elapsedTime > this.duration) {
		    //this.EndEffect ();
			return true;
		}

        if (this.elapsedTime >= this.nextEvent)
        {
            this.owner.DirectDamage(this.damageSegment);
            this.nextEvent = this.elapsedTime + this.interval;
        }

        return false;
	}

    /// <summary>
    /// Ends a Buff's effect.
    /// </summary>
	public override void EndEffect ()
    {
    }
}

