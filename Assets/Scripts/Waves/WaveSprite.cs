using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class WaveSprite : MonoBehaviour
{

    private Image image;
    public RectTransform rectTransform;
    public int waveNumber;

    // Use this for initialization
    void Awake()
    {
        image = this.GetComponent<Image>();
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void SetSprite(Sprite sprite)
    {
        this.image.sprite = sprite;
    }
}
