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

	void Start()
    {
		right = Random.value < 0.5f;
	}

	public void SetPosition(Vector2 pos)
	{
		pos1 = new Vector2(pos.x - 2, pos.y);
		pos2 = new Vector2(pos.x + 2, pos.y);
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
				SetPosition(House.instance.GetRandomGhostLocation());

				grabTimer = 0;
				grabbedPlayer = null;
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
			player.transform.SetParent(transform);
			player.transform.localPosition = Vector3.zero;
			grabbedPlayer = player;
			player.EnableMovement(false);
		}
	}
}
