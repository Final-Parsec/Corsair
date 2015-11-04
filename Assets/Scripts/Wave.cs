using System;
public class Wave
{

	public EnemyType enemyType;
	public BossType bossType;

	public float nextEnemySpawnEvent = 0;
	public float spawnDelay;
	public int numberOfEnemies;

	public Wave (EnemyType enemyType, float spawnDelay, int numberOfEnemies)
	{
		bossType = BossType.Max;
		this.enemyType = enemyType;
		this.spawnDelay = spawnDelay;
		this.numberOfEnemies = numberOfEnemies;

	}

	public Wave (BossType bossType, float spawnDelay, int numberOfEnemies)
	{
		enemyType = EnemyType.Max;
		this.bossType = bossType;
		this.spawnDelay = spawnDelay;
		this.numberOfEnemies = numberOfEnemies;
		
	}
}

