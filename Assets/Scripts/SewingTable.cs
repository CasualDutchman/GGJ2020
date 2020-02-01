using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingTable : MonoBehaviour
{
	public bool HasSewingKit;

    public void AddSewingKit()
	{
		HasSewingKit = true;
		GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
	}
}
