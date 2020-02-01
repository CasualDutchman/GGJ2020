using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
	public Switch[] switches;

	bool open = false;

	Collider2D col;

    void Start()
    {
		col = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
		open = true;

		for (int i = 0; i < switches.Length; i++)
		{
			if(!switches[i].switched)
			{
				open = false;
				break;
			}
		}

		if(open)
		{
			col.enabled = false;
			GetComponentInChildren<SpriteRenderer>().color = Color.green;
		}
		else
		{
			col.enabled = true;
			GetComponentInChildren<SpriteRenderer>().color = Color.red;
		}
    }
}
