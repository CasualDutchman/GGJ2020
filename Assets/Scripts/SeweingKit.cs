using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeweingKit : Interactable, IPickup
{
	Rigidbody2D rigid2d;

	Vector3 originalPosition;

    void Start()
    {
		rigid2d = GetComponent<Rigidbody2D>();

		originalPosition = transform.position;
		TeleportBack();
	}

    void TeleportBack()
	{
		transform.position = originalPosition;
		rigid2d.bodyType = RigidbodyType2D.Static;
	}

	public void Pickup(Player player)
	{
		transform.SetParent(player.transform);
		rigid2d.simulated = false;
		transform.localPosition = new Vector3(0, 1, 0);
	}

	public void LetGo()
	{
		transform.SetParent(null);
		rigid2d.simulated = true;
		rigid2d.bodyType = RigidbodyType2D.Dynamic;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		TeleportBack();
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}
}
