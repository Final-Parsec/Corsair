using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Mask))]
[RequireComponent(typeof(RectTransform))]
public class WaveDisplay : MonoBehaviour
{
    private const float TurboSpeed = 350;
    public static float screenWidthPercent = .5f;

    private readonly List<WaveSprite> sprites = new List<WaveSprite>();
    private int numberOfDisplayWaves;

    private ObjectManager objectManager;
    private Vector2 size;
    private float speed;
    private int turboCount;
    private IDictionary<string, Sprite> waveImages = new Dictionary<string, Sprite>();
    public WaveSprite waveSprite;
    private float xRemoveLimit;

    /// <summary>
    ///     Use this for initialization.
    /// </summary>
    private void Awake()
    {
        objectManager = ObjectManager.GetInstance();
        var rectTransform = GetComponent<RectTransform>();

        numberOfDisplayWaves = WaveManager.numberOfWavesInMemory;
        size.x = 75;
        size.y = 75;

        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.SetSize(new Vector2(size.x * (numberOfDisplayWaves - 1), size.y));
        //rectTransform.SetAnchorTopRight();
        //rectTransform.anchoredPosition = new Vector2(0, 0);

        waveImages = ObjectManager.LoadResources<Sprite>("GUI/Wave Images/", Enum.GetNames(typeof(WaveId)));

        StartCoroutine(EventualSetup());

        objectManager.WaveManager.SendWave += UpdateWaveSprites;
    }

    private IEnumerator EventualSetup()
    {
        var node = objectManager.WaveManager.upcomingWaves.First;
        while (node == null)
        {
            yield return new WaitForSeconds(.01f);
            node = objectManager.WaveManager.upcomingWaves.First;
        }

        for (var x = 0; x < numberOfDisplayWaves; x++)
        {
            sprites.Add(Instantiate(waveSprite, new Vector3(0, 0, 0), Quaternion.Euler(Vector3.zero)) as WaveSprite);

            sprites[x].rectTransform.pivot = new Vector2(1f, 1f);
            sprites[x].rectTransform.SetParent(this.transform, false);
            sprites[x].rectTransform.SetAnchorTopRight();
            sprites[x].waveNumber = node.Value.waveNumber;
            if (x == 0)
            {
                sprites[x].rectTransform.SetSize(size);
                sprites[x].rectTransform.anchoredPosition = new Vector2(0, 0);
                sprites[x].SetSprite(waveImages[node.Value.waveId.ToString()]);
                xRemoveLimit = sprites[x].rectTransform.anchoredPosition.x;
            }
            else
            {
                sprites[x].rectTransform.SetSize(size);
                sprites[x].rectTransform.anchoredPosition = new Vector2(size.x * -x, 0);
                sprites[x].SetSprite(waveImages[node.Value.waveId.ToString()]);
            }

            if (node.Next != null)
            {
                node = node.Next;
            }
        }

        speed = Mathf.Abs(sprites[0].rectTransform.anchoredPosition.x - sprites[1].rectTransform.anchoredPosition.x) /
                objectManager.WaveManager.waveSpawnDelay;
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
            var adjustedSpeed = -(speed + (turboCount != 0 ? WaveDisplay.TurboSpeed : 0)) *
                                (float)this.objectManager.gameState.GameSpeed * Time.deltaTime;
            sprite.rectTransform.anchoredPosition = new Vector2(sprite.rectTransform.anchoredPosition.x - adjustedSpeed,
                sprite.rectTransform.anchoredPosition.y);
        }

        if (sprites[0].rectTransform.anchoredPosition.x > xRemoveLimit + size.x)
        {
            this.BackOfTheLine();

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

    private void BackOfTheLine()
    {
        WaveSprite sprite = sprites[0];
        sprites.RemoveAt(0);
        if (objectManager.WaveManager.upcomingWaves.Count < WaveManager.numberOfWavesInMemory)
        {
            sprite.gameObject.SetActive(false);
            sprites.Add(sprite);
            return;
        }

        Wave nextWave = objectManager.WaveManager.upcomingWaves.Where(w => w.waveNumber == sprites[sprites.Count - 1].waveNumber + 1).FirstOrDefault();

        if (nextWave == null)
        {
            throw new InvalidOperationException("Next wave does not exist.");
        }

        sprite.SetSprite(waveImages[nextWave.waveId.ToString()]);
        sprite.rectTransform.anchoredPosition =
            new Vector2(sprite.rectTransform.anchoredPosition.x - size.x * (sprites.Count + 1),
                sprite.rectTransform.anchoredPosition.y);
        sprite.waveNumber = nextWave.waveNumber;
        sprites.Add(sprite);
    }
}