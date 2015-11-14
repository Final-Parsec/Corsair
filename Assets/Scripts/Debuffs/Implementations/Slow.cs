public class Slow : DebuffBase
{
    /// <summary>
    /// Tells if the effect has been applied.
    /// </summary>
    private bool appliedEffect;

    /// <summary>
    /// The ammount to slow the owner by.
    /// </summary>
    private float slowAmmount;

    /// <summary>
    /// Initializes an instance of <see cref="DebuffBase"/>.
    /// </summary>
    /// <param name="owner">The agent under the effect.</param>
    /// <param name="duration">The length of time the buff is active.</param>
    /// <param name="slowPercent">The percentage of the agent's speed to subtract.</param>
    public Slow(EnemyBase owner, float duration, float slowPercent)
	{
        this.objectManager = ObjectManager.GetInstance();
        this.owner = owner;
		this.duration = duration;
        this.slowAmmount = this.owner.Speed * slowPercent;
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
		    this.EndEffect ();
			return true;
		}

	    if (this.appliedEffect)
	    {
	        return false;
        }

        if (this.owner.Speed < this.slowAmmount)
	    {
            this.slowAmmount = this.owner.Speed;
	    }
        this.owner.Speed -= this.slowAmmount;
        this.appliedEffect = true;

        return false;
	}

    /// <summary>
    /// Ends a Buff's effect.
    /// </summary>
	public override void EndEffect ()
	{
        this.owner.Speed += this.slowAmmount;
    }
}

