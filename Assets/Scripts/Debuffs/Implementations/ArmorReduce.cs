
public class ArmorReduce : DebuffBase
{
    /// <summary>
    /// Tells if the effect has been applied.
    /// </summary>
    private bool appliedEffect;

    /// <summary>
    /// The ammount to reduce the armor by.
    /// </summary>
    private int reductionAmmount;

    /// <summary>
    /// Initializes an instance of <see cref="DebuffBase"/>.
    /// </summary>
    /// <param name="owner">The agent under the effect.</param>
    /// <param name="duration">The length of time the buff is active.</param>
    /// <param name="reductionAmmount">The ammount to reduce the armor by.</param>
    public ArmorReduce(EnemyBase owner, float duration, int reductionAmmount)
	{
        this.objectManager = ObjectManager.GetInstance();
        this.owner = owner;
		this.duration = duration;
        this.reductionAmmount = reductionAmmount;
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

        if (this.owner.Armor < this.reductionAmmount)
	    {
            this.reductionAmmount = this.owner.Armor;
	    }
        this.owner.Armor -= this.reductionAmmount;
        this.appliedEffect = true;

        return false;
	}

    /// <summary>
    /// Ends a Buff's effect.
    /// </summary>
	public override void EndEffect ()
	{
        this.owner.Armor += this.reductionAmmount;
    }
}

