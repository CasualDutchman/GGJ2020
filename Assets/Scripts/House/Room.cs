using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class Room : MonoBehaviour
{
	public bool ExcludeInResources;

	public Vector2Int Size;

	public int playerCount = 0;
	public bool visible = false;

	SpriteRenderer overlay;

	public void AddBlack(Sprite sprite)
	{
		var black = new GameObject("Overlay");
		black.transform.SetParent(transform);
		black.transform.localPosition = ((Size - new Vector2Int(1, 0)) * 2).ToVector2();
		black.transform.localScale = (Size * 4).XYO();
		overlay = black.AddComponent<SpriteRenderer>();
		overlay.sprite = sprite;
		overlay.color = Color.black;
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Player p))
			playerCount++;

		if(!visible && playerCount > 0)
			Visualize(true);
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Player p))
			playerCount--;

		if (visible && playerCount <= 0)
			Visualize(false);
	}

	void Visualize(bool b)
	{
		visible = b;

		overlay.color = new Color(0, 0, 0, b ? 0 : 1);
	}
}
