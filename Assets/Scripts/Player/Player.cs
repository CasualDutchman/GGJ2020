using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Framework;

public class Player : MonoBehaviour
{
	[NonSerialized] public bool OnLadder;

	public LayerMask PlatformMask;
	public LayerMask PlayerMask;

	public float Speed;

	Rigidbody2D rigid2d;
	Vector2 movement;

	Vector2 velocity;
	float velocityDamping = 0.05f;

	CapsuleCollider2D collider2d;

	void Start()
	{
		rigid2d = GetComponent<Rigidbody2D>();
		collider2d = GetComponent<CapsuleCollider2D>();
	}

	void OnMovement(InputValue value)
	{
		var move = value.Get<Vector2>();
		movement = move;
	}

	void OnAction()
	{

	}

	bool fallThrough = false;
	bool falling = false;

	void FixedUpdate()
	{
		var y = OnLadder ? movement.y * 3 : rigid2d.velocity.y;

		if (movement.y < -0.2f)
			fallThrough = true;
		else 
			fallThrough = false;

		var hit = Physics2D.Raycast(transform.position.XY() + new Vector2(0, 2f), Vector2.down, 2.1f, PlatformMask);
		Debug.DrawRay(transform.position + new Vector3(0, 2f), Vector3.down * 2.1f);

		if(hit.collider != null && hit.collider.TryGetComponent(out PlatformEffector2D pe))
		{
			if(fallThrough)
			{
				gameObject.layer = LayerMask.NameToLayer("Player");
				falling = true;
			}
		}
		else if(falling)
		{
			gameObject.layer = 0;
			falling = false;
		}

		rigid2d.velocity = Vector2.SmoothDamp(rigid2d.velocity, new Vector2(movement.x * 3, y), ref velocity, velocityDamping);
	}
}
