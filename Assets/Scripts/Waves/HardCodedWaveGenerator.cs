using System;

public class HardCodedWaveGenerator : IWaveGenerator
{
    private int count = 0;
    
    public HardCodedWaveGenerator()
    {
    }

    /// <summary>
    /// Calls <see cref="IWaveGenerator.MakeWave"/> and returns the generated <see cref="Wave"/>.
    /// </summary>
    /// <returns>The generated <see cref="Wave"/></returns>
    public override Wave GetWave()
    {
        return this.MakeWave((WaveId) (count++ % Enum.GetNames(typeof(WaveId)).Length));
    }
}
