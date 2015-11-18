using System;
public class Wave
{

    public WaveId waveId;

	public float nextEnemySpawnEvent = 0;
	public float spawnDelay;
	public int numberOfEnemies;
    public int waveNumber;

	public Wave (WaveId waveId, float spawnDelay, int numberOfEnemies, int waveNumber)
	{
		this.waveId = waveId;
		this.spawnDelay = spawnDelay;
		this.numberOfEnemies = numberOfEnemies;
	    this.waveNumber = waveNumber;
	}
}

