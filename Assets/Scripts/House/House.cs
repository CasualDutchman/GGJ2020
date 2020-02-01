using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System;
using Random = UnityEngine.Random;

public class House : MonoBehaviour
{
	public static House instance;

	public int Width = 19;
	public int Height = 9;
	Vector2 offset;

	public int MaxRoomSizeX = 2;

	public int MaxGhostCount = 1;
	public Ghost ghost;

	public Room Stairs;
	Dictionary<int, List<Room>> rooms = new Dictionary<int, List<Room>>();
	RoomData[] roomData;

	List<Vector2> GhostLocations = new List<Vector2>();

	public Sprite Square;

	void Awake()
	{
		instance = this;
	}

	void Start()
    {
		for (int i = 1; i <= MaxRoomSizeX; i++)
		{
			rooms.Add(i, new List<Room>());
		}

		var allRooms = Resources.LoadAll<Room>("Rooms");

		for (int i = 0; i < allRooms.Length; i++)
		{
			if (allRooms[i].ExcludeInResources)
				continue;

			var size = allRooms[i].Size.x;
			rooms[size].Add(allRooms[i]);
		}

		roomData = new RoomData[Width * Height];
		GenerateHouse();
		VisualizeHouse();
		SpawnGhosts();
	}

	void GenerateHouse()
	{
		int prevStairsX = -1;

		for (int y = 0; y < Height - 1; y++)
		{
			var x = GetRandomExcluding(1, Width - 1, prevStairsX, 1);

			var r = roomData[y * Width + x] = new RoomData()
			{
				Position = new Vector2(x, y),
				Room = Stairs
			};

			roomData[(y + 1) * Width + x] = new RoomData()
			{
				Attached = r
			};

			prevStairsX = x;
		}

		for (int y = 0; y < Height; y++)
		{
			var xPos = 0;
			var i = Width;

			while(i > 0)
			{
				var maxX = 0;

				for (int x = xPos; x < Width; x++)
				{
					if (roomData[y * Width + x] == null)
						maxX++;
					else
						break;
				}

				if (maxX == 0)
				{
					xPos++;
					i--;
					continue;
				}

				var sizeX = Random.Range(0, Mathf.Min(maxX, MaxRoomSizeX)) + 1;

				if (sizeX > 1)
					GhostLocations.Add(new Vector2(xPos + (sizeX * 0.5f), y + 0.25f));

				var r = roomData[y * Width + xPos] = new RoomData()
				{
					Position = new Vector2(xPos, y),
					Room = GetRandomRoom(sizeX)
				};

				for (int j = xPos + 1; j < xPos + sizeX; j++)
				{
					roomData[y * Width + j] = new RoomData()
					{
						Attached = r
					};
				}

				i -= sizeX;
				xPos += sizeX;
			}
		}
	}

	void VisualizeHouse()
	{
		offset = new Vector3(Width, Height) * -2f + new Vector3(2, 0);

		for (int i = 0; i < roomData.Length; i++)
		{
			var roomdata = roomData[i];

			if (roomdata.Attached != null)
				continue;

			var go = Instantiate(roomdata.Room, transform);
			go.transform.position = offset + roomdata.Position * 4;
			go.AddBlack(Square);
		}
	}

	void SpawnGhosts()
	{
		for (int i = 0; i < MaxGhostCount; i++)
		{
			var g = Instantiate(ghost);
			g.SetPosition(GetRandomGhostLocation());
		}
	}

	Room GetRandomRoom(int size)
	{
		var list = rooms[size];
		return list[Random.Range(0, list.Count)];
	}

	int GetRandomExcluding(int min, int max, int exclude, int range)
	{
		var test = Random.Range(min, max);

		if (exclude < 0)
			return test;

		while(test >= exclude - range && test <= exclude + range)
		{
			test = Random.Range(min, max);
		}

		return test;
	}

	public Vector3 GetRandomGhostLocation()
	{
		return offset + GhostLocations[Random.Range(0, GhostLocations.Count)] * 4 - new Vector2(2, 0);
	}

	private void OnDrawGizmos()
	{
		if (roomData != null && roomData.Length > 0)
		{
			for (int i = 0; i < roomData.Length; i++)
			{
				var r = roomData[i];

				if(r != null && r.Attached == null)
					Gizmos.DrawWireCube(r.Position + new Vector2(r.Room.Size.x * 0.5f, r.Room.Size.y * 0.5f), new Vector3(r.Room.Size.x * 0.9f, r.Room.Size.y * 0.9f, 0.1f));
			}
		}

		Gizmos.color = Color.red;

		for (int i = 0; i < GhostLocations.Count; i++)
		{
			Gizmos.DrawWireSphere(offset + GhostLocations[i] * 4 - new Vector2(2, 0), 0.1f);
		}
	}
}

public class RoomData
{
	public Vector2 Position;
	public Room Room;
	public RoomData Attached;

	public void Debug()
	{
		Gizmos.DrawWireSphere(Position, 0.1f);
	}
}

