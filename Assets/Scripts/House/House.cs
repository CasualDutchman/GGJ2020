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

	public GameObject WallPrefab;
	public Switch SwitchPrefab;
	public Door DoorPrefab;
	public Room Stairs;
	public Room priceRoom;
	public Room TableRoom;
	public Room FuseBoxRoom;
	public Room Entrance;

	Dictionary<int, List<Room>> rooms = new Dictionary<int, List<Room>>();
	RoomData[] roomData;

	List<Room> GhostLocations = new List<Room>();
	List<Switch> switches = new List<Switch>();

	List<RoomData> switchRooms = new List<RoomData>();

	public Sprite Square;

	public Transform parent;

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

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.G))
		{
			DestroyHouse();
			roomData = new RoomData[Width * Height];
			GenerateHouse();
			VisualizeHouse();
			SpawnGhosts();
		}
	}

	void DestroyHouse()
	{
		switchRooms.Clear();
		GhostLocations.Clear();
		switches.Clear();
		DestroyImmediate(parent.gameObject);
	}

	void GenerateHouse()
	{
		var priceRoomData = new RoomData()
		{
			Position = new Vector2(Width - 2, Height - 1),
			Room = priceRoom,
			HasDoorLeft = true
		};

		roomData[(Height - 1) * Width + Width - 2] = priceRoomData;
		roomData[(Height - 1) * Width + Width - 1] = new RoomData() { Attached = priceRoomData };

		var TableRoomData = new RoomData()
		{
			Position = new Vector2(0, 0),
			Room = TableRoom
		};

		roomData[0] = TableRoomData;
		roomData[1] = new RoomData() { Attached = TableRoomData };

		var FuseRoomData = new RoomData()
		{
			Position = new Vector2(Width - 2, 0),
			Room = FuseBoxRoom
		};

		roomData[Width - 2] = FuseRoomData;
		roomData[Width - 1] = new RoomData() { Attached = FuseRoomData };

		var center = Mathf.FloorToInt(Width * 0.5f) - 1;

		var entranceRoomData = new RoomData()
		{
			Position = new Vector2(center, 0),
			Room = Entrance
		};

		roomData[center] = entranceRoomData;
		roomData[center + 1] = new RoomData() { Attached = entranceRoomData };

		List<int> possible = new List<int>();

		for (int y = 0; y < Height - 1; y++)
		{
			possible.Clear();

			for (int i = 2; i < Width - 3; i++)
			{
				if (roomData[y * Height + i] == null)
					possible.Add(i);
			}

			for (int i = 0; i < 2; i++)
			{
				var x = -1;

				while(x == -1)
				{
					var index = Random.Range(0, possible.Count);
					var xx = possible[index];
					possible.RemoveAt(index);

					if ((roomData[y * Width + (xx - 1)] == null || roomData[y * Width + (xx - 1)].Room != Stairs) 
						&& (roomData[y * Width + (xx + 1)] == null || roomData[y * Width + (xx + 1)].Room != Stairs))
						x = xx;
				}

				var r = roomData[y * Width + x] = new RoomData()
				{
					Position = new Vector2(x, y),
					Room = Stairs
				};

				roomData[(y + 1) * Width + x] = new RoomData()
				{
					Attached = r
				};
			}
		}

		int[] hasWall = new int[Height];

		for (int i = 1; i < Height; i++)
		{
			var has = Random.value < 0.1f;
			if (has)
			{
				var index = Random.Range(2, Width - 2);
				hasWall[i] = index;
			}
			else
				hasWall[i] = -1;
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

				var sizeX = Mathf.Min(maxX, 2);

				if (maxX % 2 == 1)
					sizeX = Random.Range(0, sizeX) + 1;

				var addSwitch = false;

				if (y > 0 && xPos < Mathf.FloorToInt(Width * 0.6f) && roomData[y * Width + xPos] == null)
					addSwitch = true;
				
				var r = roomData[y * Width + xPos] = new RoomData()
				{
					Position = new Vector2(xPos, y),
					Room = GetRandomRoom(sizeX),
					HasWallLeft = hasWall[y] > 0 && hasWall[y] >= xPos && hasWall[y] < xPos + sizeX
				};

				if(addSwitch)
					switchRooms.Add(r);

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

		for (int i = 0; i < 2; i++)
		{
			var index = Random.Range(0, switchRooms.Count);
			var data = switchRooms[index];
			data.HasSwitch = true;

			switchRooms.RemoveAt(index);
		}
	}

	void VisualizeHouse()
	{
		parent = new GameObject().transform;
		parent.SetParent(transform);

		offset = new Vector3(Width, Height) * -2f + new Vector3(2, 0);

		for (int i = 0; i < roomData.Length; i++)
		{
			var roomdata = roomData[i];

			if (roomdata.Attached != null)
				continue;

			var go = Instantiate(roomdata.Room, parent);
			go.transform.position = offset + roomdata.Position * 4;
			go.AddBlack(Square);

			if (roomdata.HasSwitch)
				AddSwitch(go);

			if (roomdata.HasDoorLeft)
				AddDoor(go);

			//if (roomdata.HasWallLeft)
			//	AddWall(go);

			if (go.Size.x > 1)
				GhostLocations.Add(go);
		}
	}

	void SpawnGhosts()
	{
		for (int i = 0; i < MaxGhostCount; i++)
		{
			var g = Instantiate(ghost, parent);
			var room = GetRandomGhostRoom();
			room.AddGhost(g);
			g.SetRoom(room);
		}
	}

	void AddSwitch(Room room)
	{
		var s = Instantiate(SwitchPrefab, parent);
		s.transform.position = room.transform.position;
		switches.Add(s);
	}

	void AddDoor(Room room)
	{
		var d = Instantiate(DoorPrefab, parent);
		d.transform.position = room.transform.position + new Vector3(-2, 2, 0);
		d.switches = switches.ToArray();
	}

	void AddWall(Room room)
	{
		var d = Instantiate(WallPrefab, parent);
		d.transform.position = room.transform.position + new Vector3(-2, 2, 0);
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

	public Room GetRandomGhostRoom()
	{
		return GhostLocations[Random.Range(0, GhostLocations.Count)];
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
	}
}

public class RoomData
{
	public Vector2 Position;
	public Room Room;
	public RoomData Attached;
	public bool HasSwitch;
	public bool HasDoorLeft;
	public bool HasWallLeft;
}

