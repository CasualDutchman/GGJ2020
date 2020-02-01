using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	public float Speed = 2;

	public AnimationCurve LeftRightCurve;
	public float UpDownHeight = 0.5f;
	public float UpDownFrequency = 1;

	[Header("Grabbing")]
	public float maxGrabTimer = 5;

	Vector2 velocity;
	Vector2 velocityRef;
	float velocityDamping = 0.05f;

	bool right;

	float patrol = 0.5f;

	Vector2 pos1, pos2;

	Player grabbedPlayer;
	float grabTimer;

	Room room;

	CapsuleCollider2D collider2d;

	void Start()
    {
		collider2d = GetComponent<CapsuleCollider2D>();
		right = Random.value < 0.5f;
	}

	public void SetPosition(Vector2 pos)
	{
		pos1 = new Vector2(pos.x - 2, pos.y);
		pos2 = new Vector2(pos.x + 2, pos.y);
	}

	public void SetRoom(Room r)
	{
		room = r;
	}

	void Update()
	{
		if(grabbedPlayer != null)
		{
			grabTimer += Time.deltaTime;

			if(grabTimer >= maxGrabTimer)
			{
				grabbedPlayer.transform.SetParent(null);
				grabbedPlayer.EnableMovement(true);
				room.RemoveGhost(this);

				var newRoom = House.instance.GetRandomGhostRoom();
				newRoom.AddGhost(this);
				SetRoom(newRoom);

				grabTimer = 0;
				grabbedPlayer = null;
				collider2d.isTrigger = true;
			}
		}
	}

	void FixedUpdate()
    {
		var pos = Vector3.Lerp(pos1, pos2, LeftRightCurve.Evaluate(patrol));
		pos.y += (Mathf.Sin(Time.time * Mathf.PI * UpDownFrequency) * 0.5f + 0.5f) * UpDownHeight;
		transform.position = pos;

		if (right)
			patrol += Time.fixedDeltaTime / 4 * Speed;
		else
			patrol -= Time.fixedDeltaTime / 4 * Speed;

		if (patrol >= 1)
			right = false;
		else if (patrol <= 0)
			right = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (grabbedPlayer == null && collision.TryGetComponent(out Player player))
		{
			if (player.Warding)
			{
				right = !right;
			}
			else
			{
				player.transform.SetParent(transform);
				player.transform.localPosition = Vector3.zero;
				grabbedPlayer = player;
				player.DropItem();
				player.EnableMovement(false);
				collider2d.isTrigger = false;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.transform.TryGetComponent(out Player player))
		{
			player.DropItem();
		}
	}
}
