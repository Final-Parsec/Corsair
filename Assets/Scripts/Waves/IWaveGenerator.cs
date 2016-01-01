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
            case WaveId.Muslim:
                return new Wave(waveId, .1f, 21, this.nextWaveNumber);

            case WaveId.Mexican:
                return new Wave(waveId, .35f, 13, this.nextWaveNumber);

            case WaveId.Chinaman:
                return new Wave(waveId, .3f, 21, this.nextWaveNumber);
                
            case WaveId.Hillary:
                return new Wave(waveId, .0f, 1, this.nextWaveNumber);

            default:
                throw new NotImplementedException();
        }
    }
}
