using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class Room : MonoBehaviour
{
	public bool ExcludeInResources;

	public Vector2Int Size;

	public bool visible = false;

	SpriteRenderer overlay;

	protected List<Player> players = new List<Player>();
	List<Renderer> renderers = new List<Renderer>();
	List<Ghost> ghosts = new List<Ghost>();

	public void AddBlack(Sprite sprite)
	{
		var black = new GameObject("Overlay");
		black.transform.SetParent(transform);
		var pos = (Size - new Vector2Int(1, 0)) * 2;
		black.transform.localPosition = new Vector3(pos.x, pos.y, -1);
		black.transform.localScale = (Size * 4).XYO();
		overlay = black.AddComponent<SpriteRenderer>();
		overlay.sprite = sprite;
		overlay.color = new Color(0, 0, 0, 0.8f);
	}

	public void RemoveGhost(Ghost ghost)
	{
		var r = ghost.GetComponentInChildren<Renderer>();
		if (renderers.Contains(r))
			renderers.Remove(r);
	}

	public void AddGhost(Ghost ghost)
	{
		ghost.SetPosition(transform.position + new Vector3(2, 1, 0));

		var r = ghost.GetComponentInChildren<Renderer>();
		AddStuff(r);
	}

	public void AddStuff(Renderer r)
	{
		renderers.Add(r);

		r.enabled = visible;
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Player p))
			players.Add(p);

		if(!visible && players.Count > 0)
			Visualize(true);
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Player p))
			players.Remove(p);

		if (visible && players.Count <= 0)
			Visualize(false);
	}

	void Visualize(bool b)
	{
		visible = b;

		overlay.color = new Color(0, 0, 0, b ? 0 : 0.8f);

		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].enabled = b;
		}
	}
}
