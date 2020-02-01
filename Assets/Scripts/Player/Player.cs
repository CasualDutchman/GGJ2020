using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Framework;

public class Player : MonoBehaviour
{
	[NonSerialized] public bool OnLadder;

	public LayerMask GroundMask;
	public LayerMask PlatformMask;
	public LayerMask InteractableMask;
	public LayerMask SewingTableMask;

	public float Speed;

	Rigidbody2D rigid2d;
	Vector2 movement;

	Vector2 velocity;
	float velocityDamping = 0.05f;

	CapsuleCollider2D collider2d;

	bool fallThrough = false;
	bool falling = false;

	bool CanMove = true;

	public bool press = false;
	Interactable interactable;

	public bool Warding;

	IPickup holdingItem;

	void Start()
	{
		rigid2d = GetComponent<Rigidbody2D>();
		collider2d = GetComponent<CapsuleCollider2D>();
	}

	void OnMovement(InputValue value)
	{
		var move = value.Get<Vector2>();
		if(!Warding)
			movement = move;
	}

	void OnInteract(InputValue value)
	{
		var v = value.Get<float>();

		var newpress = v > 0.1f;

		if(newpress && newpress != press)
		{
			if (holdingItem != null)
			{
				var sewingtablehit = Physics2D.OverlapCircle(transform.position + new Vector3(0, 0.5f), 0.5f, SewingTableMask);

				if (sewingtablehit != null)
				{
					var sewingTable = sewingtablehit.GetComponent<SewingTable>();
					sewingTable.AddSewingKit();
					Destroy(holdingItem.GetGameObject());
					holdingItem = null;
				}
				else
				{
					holdingItem.LetGo();
					holdingItem = null;
					goto finish;
				}
			}
		}

		var hitcol = Physics2D.OverlapCircle(transform.position + new Vector3(0, 0.5f), 0.5f, InteractableMask);

		if (hitcol != null && hitcol.TryGetComponent(out Interactable s))
		{
			if (newpress != press)
			{
				if(newpress)
				{
					if (s is IPickup pickup)
					{
						pickup.Pickup(this);
						holdingItem = pickup;
					}
					else
					{
						s.PressDown();
						interactable = s;
					}
				}
				else
				{
					s.PressUp();
					interactable = null;
				}
			}
		}

		finish:

		press = newpress;
	}

	bool grounded = false;

	void OnJump()
	{
		if(!Warding && grounded && holdingItem == null)
			rigid2d.AddForce(new Vector2(0, 300f));
	}

	void OnWard(InputValue value)
	{
		var v = value.Get<float>();

		if (!grounded)
			return;

		var newpress = v > 0.1f;

		Warding = newpress;

		if (Warding)
			movement = Vector2.zero;
	}

	public void DropItem()
	{
		if (holdingItem != null)
		{
			holdingItem.LetGo();
			holdingItem = null;
		}
	}

	void Update()
	{
		var hitcol = Physics2D.OverlapCircle(transform.position + new Vector3(0, 0.5f), 0.5f, InteractableMask);

		if (hitcol != null && hitcol.TryGetComponent(out Interactable s))
		{
			if(press)
			{
				if (s.pressed)
					s.Hold();
				else
				{
					s.PressDown();
					interactable = s;
				}
			}
			else
			{
				if (s.pressed)
					s.Hold();
				else
				{
					s.PressUp();
					interactable = null;
				}
			}
		}
		else
		{
			if(interactable != null)
			{
				interactable.PressUp();
				interactable = null;
			}
		}
	}

	void FixedUpdate()
	{
		if (movement.y < -0.9f)
			fallThrough = true;
		else 
			fallThrough = false;

		var groundedHit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, GroundMask);
		grounded = groundedHit.collider != null;

		var hit = Physics2D.Raycast(transform.position.XY() + new Vector2(0, 2f), Vector2.down, 2.4f, PlatformMask);
		Debug.DrawRay(transform.position + new Vector3(0, 2f), Vector3.down * 2.4f);

		if(hit.collider != null && hit.collider.TryGetComponent(out PlatformEffector2D pe))
		{
			if(fallThrough)
			{
				gameObject.layer = LayerMask.NameToLayer("Faller");
				falling = true;
			}
		}
		else if(falling)
		{
			gameObject.layer = LayerMask.NameToLayer("Player");
			falling = false;
		}

		if(CanMove)
			rigid2d.velocity = Vector2.SmoothDamp(rigid2d.velocity, new Vector2(movement.x * 3, rigid2d.velocity.y), ref velocity, velocityDamping);
	}

	public void EnableMovement(bool b)
	{
		CanMove = b;
		rigid2d.bodyType = b ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
	}
}
