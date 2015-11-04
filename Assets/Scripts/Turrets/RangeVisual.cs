using UnityEngine;
using System.Collections;

public class RangeVisual : MonoBehaviour {
	public Sprite[] sprites;
	private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void ChangeSprite(int range)
	{
		if (range - 1 < sprites.Length && range > 0)
		{
			spriteRenderer.sprite = sprites[range - 1];
		}
		else
		{
			Debug.Log(@"******* No Image For Range " + range);
		}
	}
}
