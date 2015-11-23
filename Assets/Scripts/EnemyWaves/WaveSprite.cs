using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(RectTransform))]
public class WaveSprite : MonoBehaviour
{

    private RawImage rawImage;
    public RectTransform rectTransform;

    // Use this for initialization
    void Awake()
    {
        rawImage = this.GetComponent<RawImage>();
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void SetTexture(Texture tex)
    {
        this.rawImage.texture = tex;
    }
}
