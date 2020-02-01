using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningRoom : Room
{
	public SewingTable table;

	float timer = 0;

	private void Update()
	{
		if(table.HasSewingKit && players.Count == 4)
		{
			var count = 0;

			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].Warding)
					count++;
			}

			if (count == 4)
				timer = Mathf.Clamp(timer + Time.deltaTime, 0, 10);
			else
				timer = Mathf.Clamp(timer - Time.deltaTime, 0, 10);
		}
	}
}
