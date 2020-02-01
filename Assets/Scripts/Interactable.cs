using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
	public bool pressed;

	public virtual void Hold()
	{

	}

	public virtual void PressDown()
	{
		//pressed = true;
	}

	public virtual void PressUp()
	{
		//pressed = false;
	}
}
