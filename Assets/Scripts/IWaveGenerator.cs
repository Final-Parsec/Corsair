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
            case WaveId.Zombie:
                return new Wave(waveId, .25f, 21, this.nextWaveNumber);

            case WaveId.Horde:
                return new Wave(waveId, .1f, 21, this.nextWaveNumber);

            case WaveId.PregerWench:
                return new Wave(waveId, .35f, 13, this.nextWaveNumber);

            case WaveId.SeaDog:
                return new Wave(waveId, .3f, 18, this.nextWaveNumber);

            case WaveId.Dog:
                return new Wave(waveId, .3f, 21, this.nextWaveNumber);

            case WaveId.Seagull:
                return new Wave(waveId, .35f, 21, this.nextWaveNumber);
                
            case WaveId.Boss3:
            case WaveId.Boss2:
            case WaveId.Boss1:
                return new Wave(waveId, .0f, 1, this.nextWaveNumber);

            default:
                throw new NotImplementedException();
        }
    }
}
