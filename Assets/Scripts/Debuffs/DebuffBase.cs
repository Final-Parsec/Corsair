
public abstract class DebuffBase : IBuff
{
    /// <summary>
    /// The <see cref="EnemyBase"/> being Debuffed.
    /// </summary>
	protected EnemyBase owner;

    /// <summary>
    /// Manages and maps class instances.
    /// </summary>
    protected ObjectManager objectManager;
    
    /// <summary>
    /// The length of time the <see cref="DebuffBase"/> is active.
    /// </summary>
    protected float duration;

    /// <summary>
    /// The sum of every deltaTime.
    /// </summary>
    protected float elapsedTime;

    /// <summary>
    /// Applies the Buff.
    /// </summary>
    /// <param name="deltaTime">The time between the last application.</param>
    /// <returns>A boolean value. True if the Buff if finished else false.</returns>
    public abstract bool Apply(float deltaTime);

    /// <summary>
    /// Ends a Buff's effect.
    /// </summary>
    public abstract void EndEffect();
}


