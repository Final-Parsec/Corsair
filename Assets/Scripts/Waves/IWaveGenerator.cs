using System;
public abstract class IWaveGenerator
{

    protected int nextWaveNumber = 0;
    
    /// <summary>
    /// Calls <see cref="MakeWave"/> and returns the generated <see cref="Wave"/>.
    /// </summary>
    /// <returns>The generated <see cref="Wave"/></returns>
    public abstract Wave GetWave();

    /// <summary>
    /// Makes a <see cref="Wave"/> when given a <see cref="WaveId"/>
    /// </summary>
    /// <param name="waveId">The Id of the <see cref="Wave"/></param>
    /// <returns>The generated <see cref="Wave"/></returns>
    protected Wave MakeWave(WaveId waveId)
    {
        this.nextWaveNumber++;

        switch (waveId)
        {
            case WaveId.StrongMuslimMen:
                return new Wave(waveId, .55f, 16, this.nextWaveNumber);

            case WaveId.FastMexicanMen:
                return new Wave(waveId, .45f, 12, this.nextWaveNumber);

            case WaveId.GroupChineseMen:
                return new Wave(waveId, .15f, 24, this.nextWaveNumber);

            case WaveId.AnchorMuslimWomen:
            case WaveId.AnchorMexicanWomen:
            case WaveId.AnchorChineseWomen:
                return new Wave(waveId, .55f, 12, this.nextWaveNumber);

            case WaveId.Hillary:
            case WaveId.Bernie:
            case WaveId.Obama:
                return new Wave(waveId, .0f, 1, this.nextWaveNumber);

            default:
                throw new NotImplementedException();
        }
    }
}
