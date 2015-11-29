using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    /// <summary>
    /// Manages and maps class instances.
    /// </summary>
    private ObjectManager objectManager;

    private readonly IWaveGenerator waveGene = new RandomWaveGenerator(8008);

    /// <summary>
    /// Executes event when the <see cref="Wave"/> is sent.
    /// </summary>
    public delegate void SendWaveAction();
    public event SendWaveAction SendWave;

    [HideInInspector]
    public LinkedList<Wave> upcomingWaves = new LinkedList<Wave>();
    private float nextWaveSpawnEvent;
    [HideInInspector]
    public bool playerTriggeredWave;
    [HideInInspector]
    public Node[] enemySpawnNodes;
    [HideInInspector]
    public Node destinationNode;

    public List<GameObject> enemyPrefabs;
    public float waveSpawnDelay = 25;

    void Awake()
    {
        this.objectManager = ObjectManager.GetInstance();

        this.PopulateUpcomingWaves();

        this.destinationNode = this.objectManager.NodeManager.GetDestinationNode();
        this.enemySpawnNodes = this.objectManager.NodeManager.GetSpawnNodes();
    }

    public void Update()
    {
        if (this.objectManager.gameState.gameStarted && !this.objectManager.gameState.gameOver)
        {
            if (this.objectManager.gameState.GameSpeed == GameSpeed.Paused)
            {
                this.nextWaveSpawnEvent += Time.deltaTime;
            }
            else
            {
                this.nextWaveSpawnEvent -= (Time.deltaTime * (float)this.objectManager.gameState.GameSpeed) - Time.deltaTime;
            }

            this.objectManager.gameState.nextWaveCountDown = (int)(this.nextWaveSpawnEvent - Time.time);

            if ((Time.time >= this.nextWaveSpawnEvent || this.playerTriggeredWave) &&
                (this.objectManager.gameState.waveCount < this.objectManager.gameState.numberOfWaves))
            {
                this.nextWaveSpawnEvent = Time.time + this.waveSpawnDelay;
                this.SpawnWave();

                // Clear flag at the end. WaveDisplay depends on it.
                this.playerTriggeredWave = false;
            }
        }
    }

    /// <summary>
    /// Initializes the waves in <see cref="upcomingWaves"/>
    /// </summary>
    private void PopulateUpcomingWaves()
    {
        for (int x = 0; x < 15; x++)
        {
            this.upcomingWaves.AddLast(this.waveGene.GetWave());
        }
    }

    /// <summary>
    /// Starts a wave from the upcomming Waves list.
    /// Ends the game when all the waves have been ran, all the enemies are dead, and the player didn't lose.
    /// </summary>
    public void SpawnWave()
    {
        Wave wave = this.DequeueWave();
        if (wave != null)
        {
            this.objectManager.gameState.waveCount++;
            this.StartCoroutine(this.CreateEnemies(wave, this.enemySpawnNodes));

            if (SendWave != null)
            {
                SendWave();
            }
        }
    }

    /// <summary>
    /// Removes a <see cref="Wave"/> from <see cref="upcomingWaves"/> and will Enqueue a new <see cref="Wave"/>
    /// if required.
    /// </summary>
    /// <returns>The next <see cref="Wave"/> in the List</returns>
    private Wave DequeueWave()
    {
        if (this.upcomingWaves.Count > 0)
        {
            Wave wave = this.upcomingWaves.First.Value;
            this.upcomingWaves.RemoveFirst();
            if (this.objectManager.gameState.numberOfWaves > this.upcomingWaves.Count + this.objectManager.gameState.waveCount)
            {
                this.upcomingWaves.AddLast(this.waveGene.GetWave());
            }
            return wave;
        }
        return null;
    }

    /// <summary>
    /// Creates <see cref="EnemyBase"/>s at the given spawn points.
    /// </summary>
    /// <param name="wave">The <see cref="Wave"/> to spawn from.</param>
    /// <param name="spawns">The locations to spawn.</param>
    private IEnumerator CreateEnemies(Wave wave, Node[] spawns)
    {
        if (wave != null || spawns != null || spawns.Length != 0)
        {

            float lastTime = Time.time;
            int spawnIndex = 0;
            while (wave.numberOfEnemies > 0)
            {
                float localDeltaTime = Time.time - lastTime;
                lastTime = Time.time;

                if (this.objectManager.gameState.GameSpeed == GameSpeed.Paused)
                {
                    wave.nextEnemySpawnEvent += localDeltaTime;
                }
                else
                {
                    wave.nextEnemySpawnEvent -= (localDeltaTime * ((float)this.objectManager.gameState.GameSpeed - 1f));
                }

                if (Time.time >= wave.nextEnemySpawnEvent)
                {
                    GameObject enemy = this.enemyPrefabs[(int)wave.waveId];
                    GameObject madeEnemy = enemy.GetObjectFromPool(
                        enemy.gameObject.name,
                        spawns[spawnIndex].UnityPosition,
                        Quaternion.Euler(new Vector3(90, 0, 0)));
                    madeEnemy.name = enemy.name;

                    wave.nextEnemySpawnEvent = Time.time + wave.spawnDelay;
                    wave.numberOfEnemies--;

                    spawnIndex++;
                    if (spawnIndex >= spawns.Length)
                    {
                        spawnIndex = 0;
                    }
                }
                yield return new WaitForSeconds(wave.spawnDelay / ((float)GameSpeed.X3));
            }
        }
    } 
}
