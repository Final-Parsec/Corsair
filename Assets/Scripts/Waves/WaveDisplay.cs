using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Canvas))]
public class WaveDisplay : MonoBehaviour {
    public static float screenHeightPercent = .2f;

    private readonly List<WaveSprite> sprites = new List<WaveSprite>();
    private readonly IDictionary<WaveId, Texture> waveTextures = new Dictionary<WaveId, Texture>();
    public WaveSprite waveSprite;

    [HideInInspector]
    public float topYcordinate;

    private ObjectManager objectManager;
    private Vector2 size;
    private float speed;
    private float turboSpeed = 350;
    private int turboCount = 0;
    private float xScreenEdge;


    // Use this for initialization
    void Awake()
    {
        objectManager = ObjectManager.GetInstance();
        size.y = Screen.height * screenHeightPercent;
        size.x = size.y * 2f;

        Texture def = Resources.Load("GUI/Wave Images/Default") as Texture;
        // Load textures
        for (int x = 0; x < (int)WaveId.Max; x++)
        {
            WaveId wave = ((WaveId)x);
            Texture tex = Resources.Load("GUI/Wave Images/" + wave.ToString()) as Texture;
            if (tex != null)
            {
                waveTextures.Add(wave, tex);
                Debug.Log("Loaded Wave Image " + wave.ToString());
            }
            else if (def != null)
            {
                waveTextures.Add(wave, def);
                Debug.Log("Loaded Default Wave Image.");
            }
        }
        
        LinkedListNode<Wave> node = objectManager.WaveManager.upcomingWaves.First;
        for (int x = 0; x < objectManager.WaveManager.upcomingWaves.Count; x++)
        {
            sprites.Add(Instantiate(waveSprite, new Vector3(0, 0, 0), Quaternion.Euler(Vector3.zero)) as WaveSprite);

            // TODO: set wave number

            if(x == 0)
            {
                sprites[x].rectTransform.SetSize(size);
                sprites[x].rectTransform.SetLeftBottomPosition(new Vector2(0, 0));
                xScreenEdge = sprites[x].rectTransform.position.x;
            }
            else
            {
                sprites[x].rectTransform.SetSize(size);
                var position = new Vector2(size.x * x, 0);
                sprites[x].rectTransform.SetLeftBottomPosition(position);
            }

            sprites[x].SetTexture(waveTextures[node.Value.waveId]);
            sprites[x].rectTransform.pivot = new Vector2(0.5f, 0.5f);
            sprites[x].rectTransform.SetAnchorBotLeft();
            sprites[x].rectTransform.SetParent(this.transform);

            if (node.Next != null)
            {
                node = node.Next;
            }
        }
        topYcordinate = sprites[0].rectTransform.anchoredPosition.y / sprites[0].rectTransform.pivot.y;
        speed = Mathf.Abs(sprites[0].rectTransform.position.x - sprites[1].rectTransform.position.x) / objectManager.WaveManager.waveSpawnDelay;

        objectManager.WaveManager.SendWave += UpdateWaveSprites;

        Texture activeWaveTex = Resources.Load("GUI/Wave Images/ActiveWave") as Texture;
        if (activeWaveTex != null)
        {
            WaveSprite activeWave = Instantiate(waveSprite, new Vector3(0, 0, 1), Quaternion.Euler(Vector3.zero)) as WaveSprite;
            activeWave.rectTransform.SetSize(size);
            activeWave.rectTransform.SetLeftBottomPosition(new Vector2(0, 0));
            activeWave.SetTexture(activeWaveTex);
            activeWave.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            activeWave.rectTransform.SetAnchorBotLeft();
            activeWave.rectTransform.SetParent(this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (objectManager.gameState.gameSpeed == GameSpeed.Paused || !objectManager.gameState.gameStarted)
        {
            return;
        }
        
        foreach (var sprite in sprites)
        {
            float adjustedSpeed = (speed + (turboCount != 0 ? turboSpeed : 0)) * (float)this.objectManager.gameState.GameSpeed * Time.deltaTime;
            sprite.rectTransform.localPosition = new Vector3(sprite.rectTransform.localPosition.x - adjustedSpeed,
                                                  sprite.rectTransform.localPosition.y,
                                                  sprite.rectTransform.localPosition.z);
        }
        
        if (sprites[0].rectTransform.position.x < xScreenEdge - size.x - 5)
        {
            this.BackOfTheLine(sprites[0]);
            if (turboCount > 0)
            {
                turboCount--;
            }
        }
    }

    public void DeReference()
    {
        objectManager.WaveManager.SendWave -= UpdateWaveSprites;
    }

    private void UpdateWaveSprites()
    {
        if (objectManager.gameState.waveCount == 1)
        {
            return;
        }

        if (objectManager.WaveManager.playerTriggeredWave)
        {
            turboCount++;
        }
    }

    private void BackOfTheLine(WaveSprite sprite)
    {
        if (objectManager.WaveManager.upcomingWaves.Last.Value.waveNumber > objectManager.gameState.numberOfWaves)
        {
            sprite.gameObject.SetActive(false);
        }

        sprites.RemoveAt(0);
        sprite.SetTexture(waveTextures[objectManager.WaveManager.upcomingWaves.Last.Value.waveId]);
        sprite.rectTransform.position = new Vector3(sprite.rectTransform.position.x + size.x * (sprites.Count + 1),
                                                    sprite.rectTransform.position.y,
                                                    sprite.rectTransform.position.z);
        sprites.Add(sprite);
    }
}
