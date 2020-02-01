using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Interactable
{
	public bool switched = false;

	public override void PressDown()
	{
		switched = true;
		GetComponentInChildren<SpriteRenderer>().color = Color.red;
	}

	public override void PressUp()
	{
		switched = false;
		GetComponentInChildren<SpriteRenderer>().color = Color.white;
	}
}
