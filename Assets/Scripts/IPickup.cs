using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickup
{
	void Pickup(Player player);
	void LetGo();

	GameObject GetGameObject();
}
