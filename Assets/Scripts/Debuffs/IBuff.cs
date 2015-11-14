
public interface IBuff
{
    /// <summary>
    /// Applies the Buff.
    /// </summary>
    /// <param name="deltaTime">The time between the last application.</param>
    /// <returns>A boolean value. True if the Buff if finished else false.</returns>
    bool Apply(float deltaTime);

    /// <summary>
    /// Ends a Buff's effect.
    /// </summary>
    void EndEffect();
}

