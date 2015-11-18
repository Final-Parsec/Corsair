using System;

public class RandomWaveGenerator : IWaveGenerator
{
    /// <summary>
    /// A Random Number Generator used to create <see cref="WaveId"/>s.
    /// </summary>
    private readonly Random rng;

    /// <summary>
    /// Creats a <see cref="RandomWaveGenerator"/> Instance
    /// </summary>
    /// <param name="seed">The seed used by <see cref="rng"/></param>
    public RandomWaveGenerator(int seed)
    {
        this.rng = new Random(seed);
    }

    /// <summary>
    /// Calls <see cref="IWaveGenerator.MakeWave"/> and returns the generated <see cref="Wave"/>.
    /// </summary>
    /// <returns>The generated <see cref="Wave"/></returns>
    public override Wave GetWave()
    {
        return this.MakeWave((WaveId) this.rng.Next((int)WaveId.Start + 1, (int)WaveId.Max));
    }
}
