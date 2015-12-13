using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class WaveDisplay : MonoBehaviour {
    public static float screenHeightPercent = .1f;

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


    // Use this for initialization
    void Awake()
    {
        objectManager = ObjectManager.GetInstance();
        size.y = Screen.height * screenHeightPercent;
        size.x = size.y * 2f;

        waveImages = ObjectManager.LoadResources<Sprite>("GUI/Wave Images/", Enum.GetNames(typeof(WaveId)));
        
        LinkedListNode<Wave> node = objectManager.WaveManager.upcomingWaves.First;
        for (int x = 0; x < objectManager.WaveManager.upcomingWaves.Count; x++)
        {
            sprites.Add(Instantiate(waveSprite, new Vector3(0, 0, 0), Quaternion.Euler(Vector3.zero)) as WaveSprite);

            // TODO: set wave number
            if(x == 0)
            {
                sprites[x].rectTransform.SetSize(size);
                sprites[x].rectTransform.SetLeftBottomPosition(new Vector2(0, 0));
                xRemoveLimit = sprites[x].rectTransform.position.x;
                sprites[x].gameObject.SetActive(false);
            }
            else if (x == 1)
            {
                sprites[x].rectTransform.SetSize(size);
                var position = new Vector2(size.x * x, 0);
                sprites[x].rectTransform.SetLeftBottomPosition(position);
                sprites[x].SetSprite(waveImages[node.Value.waveId.ToString()]);
                xSpawnLimit = sprites[x].rectTransform.position.x;
            }
            else
            {
                sprites[x].rectTransform.SetSize(size);
                var position = new Vector2(size.x * x, 0);
                sprites[x].rectTransform.SetLeftBottomPosition(position);
                sprites[x].SetSprite(waveImages[node.Value.waveId.ToString()]);
            }

            sprites[x].rectTransform.pivot = new Vector2(0.5f, 0.5f);
            sprites[x].rectTransform.SetAnchorBotLeft();
            sprites[x].rectTransform.SetParent(this.transform);

            if (x != 0 && node.Next != null)
            {
                node = node.Next;
            }
        }

        speed = Mathf.Abs(sprites[0].rectTransform.position.x - sprites[1].rectTransform.position.x) / objectManager.WaveManager.waveSpawnDelay;

        objectManager.WaveManager.SendWave += UpdateWaveSprites;

        //Texture activeWaveTex = Resources.Load("GUI/Wave Images/ActiveWave") as Texture;
        GameObject sendWaveObj = Instantiate(sendWave, new Vector3(0, 0, 1), Quaternion.Euler(Vector3.zero)) as GameObject;
        sendWaveObj.name = sendWave.name;
        RectTransform rt = sendWaveObj.GetComponent<RectTransform>();
        rt.SetSize(new Vector2(size.x, size.y + 5)); // add a bit of padding to wave button
        rt.SetLeftBottomPosition(new Vector2(0, 0));
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.SetAnchorBotLeft();
        rt.SetParent(this.transform);
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
            float adjustedSpeed = (speed + (turboCount != 0 ? turboSpeed : 0)) * (float)this.objectManager.gameState.GameSpeed * Time.deltaTime;
            sprite.rectTransform.localPosition = new Vector3(sprite.rectTransform.localPosition.x - adjustedSpeed,
                                                  sprite.rectTransform.localPosition.y,
                                                  sprite.rectTransform.localPosition.z);
        }

        if (sprites[1].rectTransform.position.x < xSpawnLimit - size.x)
        {
            if (turboCount > 0)
            {
                turboCount--;
            }
        }

        if (sprites[0].rectTransform.position.x < xRemoveLimit - size.x)
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
        if (objectManager.WaveManager.upcomingWaves.Last.Value.waveNumber > objectManager.gameState.numberOfWaves)
        {
            sprite.gameObject.SetActive(false);
        }

        sprites.RemoveAt(0);
        sprite.SetSprite(waveImages[objectManager.WaveManager.upcomingWaves.Last.Value.waveId.ToString()]);
        sprite.rectTransform.position = new Vector3(sprite.rectTransform.position.x + size.x * (sprites.Count + 1),
                                                    sprite.rectTransform.position.y,
                                                    sprite.rectTransform.position.z);
        sprites.Add(sprite);
    }
}
