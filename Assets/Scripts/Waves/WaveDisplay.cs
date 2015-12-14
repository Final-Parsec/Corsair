using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class WaveDisplay : MonoBehaviour {
    public static float screenHeightPercent = .15f;

    private readonly List<WaveSprite> sprites = new List<WaveSprite>();
    private IDictionary<string, Sprite> waveImages = new Dictionary<string, Sprite>();
    public WaveSprite waveSprite;
    public GameObject sendWave;

    [HideInInspector]
    public float topYcordinate;

    private ObjectManager objectManager;
    private Vector2 size;
    private float speed;
    private float turboSpeed = 350;
    private int turboCount = 0;
    private float xRemoveLimit;
    private float xSpawnLimit;
    private int numberOfDisplayWaves;


    // Use this for initialization
    void Awake()
    {
        numberOfDisplayWaves = WaveManager.numberOfWavesInMemory;
        Canvas canvas = this.gameObject.GetComponent<Canvas>();
        objectManager = ObjectManager.GetInstance();
        size.y = Screen.height * screenHeightPercent * .5f; // Scaleing issue so X.5
        size.x = size.y;

        waveImages = ObjectManager.LoadResources<Sprite>("GUI/Wave Images/", Enum.GetNames(typeof(WaveId)));
        
        LinkedListNode<Wave> node = objectManager.WaveManager.upcomingWaves.First;
        for (int x = 0; x < numberOfDisplayWaves; x++)
        {
            sprites.Add(Instantiate(waveSprite, new Vector3(0, 0, 0), Quaternion.Euler(Vector3.zero)) as WaveSprite);

            sprites[x].rectTransform.pivot = new Vector2(1f, 1f);
            sprites[x].rectTransform.SetParent(this.transform, false);
            sprites[x].rectTransform.SetAnchorTopRight();
            // TODO: set wave number
            if (x == 0)
            {
                sprites[x].rectTransform.SetSize(size);
                sprites[x].rectTransform.anchoredPosition = new Vector2(0, 0);
                xRemoveLimit = sprites[x].rectTransform.anchoredPosition.x;
                sprites[x].gameObject.SetActive(false);
            }
            else if (x == 1)
            {
                sprites[x].rectTransform.SetSize(size);
                //var position = sprites[x-1].rectTransform.position;
                //position.x = position.x - size.x;
                //sprites[x].rectTransform.position = position;
                sprites[x].rectTransform.anchoredPosition = new Vector2(size.x * -x, 0);
                sprites[x].SetSprite(waveImages[node.Value.waveId.ToString()]);
                xSpawnLimit = sprites[x].rectTransform.anchoredPosition.x;
            }
            else
            {
                sprites[x].rectTransform.SetSize(size);
                //var position = sprites[x - 1].rectTransform.position;
                //position.x = position.x - size.x;
                //sprites[x].rectTransform.position = position;

                sprites[x].rectTransform.anchoredPosition = new Vector2(size.x * -x, 0);
                sprites[x].SetSprite(waveImages[node.Value.waveId.ToString()]);
            }
            
            if (x != 0 && node.Next != null)
            {
                sprites[x].waveNumber = node.Value.waveNumber;
                node = node.Next;
            }
        }

        speed = Mathf.Abs(sprites[0].rectTransform.anchoredPosition.x - sprites[1].rectTransform.anchoredPosition.x) / objectManager.WaveManager.waveSpawnDelay;

        objectManager.WaveManager.SendWave += UpdateWaveSprites;

        //Texture activeWaveTex = Resources.Load("GUI/Wave Images/ActiveWave") as Texture;
        GameObject sendWaveObj = Instantiate(sendWave, new Vector3(0, 0, 1), Quaternion.Euler(Vector3.zero)) as GameObject;
        sendWaveObj.name = sendWave.name;
        RectTransform rt = sendWaveObj.GetComponent<RectTransform>();
        rt.SetSize(new Vector2(size.x, size.y + 5)); // add a bit of padding to wave button
        rt.pivot = new Vector2(1f, 1f);
        rt.SetParent(this.transform, false);
        rt.SetAnchorTopRight();
        rt.anchoredPosition = new Vector2(0, 0);
        UnityAction action = () => { objectManager.GuiButtonMethods.SendWavePressed(); };
        sendWaveObj.GetComponent<Button>().onClick.AddListener(action);

        topYcordinate = rt.anchoredPosition.y / rt.pivot.y;
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
            float adjustedSpeed = -(speed + (turboCount != 0 ? turboSpeed : 0)) * (float)this.objectManager.gameState.GameSpeed * Time.deltaTime;
            sprite.rectTransform.anchoredPosition = new Vector2(sprite.rectTransform.anchoredPosition.x - adjustedSpeed,
                                                                sprite.rectTransform.anchoredPosition.y);
        }

        if (sprites[1].rectTransform.anchoredPosition.x > xSpawnLimit + size.x)
        {
            if (turboCount > 0)
            {
                turboCount--;
            }
        }

        if (sprites[0].rectTransform.anchoredPosition.x > xRemoveLimit + size.x)
        {
            this.BackOfTheLine(sprites[0]);
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
        sprite.gameObject.SetActive(true); // to enable the first sprite
        if (objectManager.WaveManager.upcomingWaves.Count < WaveManager.numberOfWavesInMemory)
        {
            sprite.gameObject.SetActive(false);
        }

        sprites.RemoveAt(0);
        sprite.SetSprite(waveImages[objectManager.WaveManager.upcomingWaves.Last.Value.waveId.ToString()]);
        sprite.rectTransform.anchoredPosition = new Vector2(sprite.rectTransform.anchoredPosition.x - size.x * (sprites.Count + 1),
                                                            sprite.rectTransform.anchoredPosition.y);
        sprite.waveNumber = objectManager.WaveManager.upcomingWaves.Last.Value.waveNumber;
        sprites.Add(sprite);
    }
}
