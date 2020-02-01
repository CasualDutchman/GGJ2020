using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureGenerator : MonoBehaviour
{
    public enum RoomType {
        Sitting
    }

    public const RoomType ROOM_TYPE = RoomType.Sitting;

    private int[] roomColors = new int[] {
        0xFF0000,
        0x00FF00,
        0x0000FF,
        0xF0330F
    };

    private string[] wallpapers = {
        "Checkers",
        "Flowers",
        "Stripes",
        "Tile"
    };

    private string[] sittingRoomFurniture = {
        "bed",
        "bookcase",
        "box",
        "table",
        "chair",
        "clock",
        "kitchen-cabinet",
        "bathtub"
    };

    private string[] wallFurniture = {
        "portrait1",
        "portrait2",
        "portrait3",
        "window",
        "candle1",
        "candle2",
        "candle3",
        // FIXME: Add decals
    };

    private string[] smallItems = {
        "book1",
        "book2",
        "book3",
        "glass",
        "mug",
        "vase",
        "wine_glass"
    };

    // Start is called before the first frame update
    void Start()
    {
        ChooseWallpaper();
        switch(ROOM_TYPE) {
            case RoomType.Sitting:
            default:
                GenerateSitting();
                break;
        }
   }

    void ChooseWallpaper()
    {
        var spriteRenderer = gameObject.transform.Find("Background").GetComponent<SpriteRenderer>();
        Sprite wallpaper = null;

        switch(ROOM_TYPE) {
            // case RoomType.Bath:
            //     wallpaper = Resources.Load<Sprite>("Rooms/Backgrounds/BGTile");
            //     spriteRenderer.color = GetColor("Tile");
            //     break;
            //
            default:
                var randomWallpaperName = wallpapers[Random.Range(0, wallpapers.Length)];
                spriteRenderer.color = GetColor(randomWallpaperName);
                wallpaper = Resources.Load<Sprite>("Rooms/Backgrounds/BG" + randomWallpaperName); 
                break;
        }

        spriteRenderer.sprite = wallpaper;
    }

    Color32 GetColor(string name) {
        int color = 0xFFFFFF;

        switch(name) {
            case "Stripes":
                color = new int[]{0x63889C, 0x63B59F, 0x804960}[Random.Range(0, 3)];
                break;
            case "Tile":
                color = new int[]{0xFFBD86, 0xDAFFE3}[Random.Range(0, 2)];
                break;
            case "Flowers":
                color = new int[]{0x5EA9D4, 0xCCA9D4, 0xD4CBA9}[Random.Range(0, 3)];
                break;
            case "Checkers":
                color = new int[]{0xD2FFD1, 0xCBCBCB, 0xE5BD9D}[Random.Range(0, 3)];
                break;
            default:
                color = 0xFFFFFF;
                break;
        }

        Color32 unityColor = new Color32();

        unityColor.b = (byte)((color) & 0xFF);
        unityColor.g = (byte)((color >> 8) & 0xFF);
        unityColor.r = (byte)((color >> 16) & 0xFF);
        unityColor.a = 255;

        return unityColor;
    }

    void GenerateSitting()
    {
        var amountOfFurniture = Random.Range(2, 5);
        var size = GetComponent<BoxCollider2D>().bounds.size;
        var roomWidth = size.x - 2;

        Debug.Log("roomWidth");
        Debug.Log(roomWidth);

        for (int i = 0; i < amountOfFurniture; i++) {
            var randomFurnitureName = sittingRoomFurniture[Random.Range(0, sittingRoomFurniture.Length)];
            var furniture = GenerateFurniture(randomFurnitureName);

            furniture.transform.position = new Vector2((roomWidth / amountOfFurniture) * i - roomWidth / 4, 0);
        }
    }

    GameObject GenerateFurniture(string asset)
    {
        GameObject furniture = new GameObject("Furniture_" + asset);

        Debug.Log(asset);

        SpriteRenderer sprite = furniture.AddComponent<SpriteRenderer>();
        Sprite spriteResource = Resources.Load<Sprite>("Rooms/Furniture/" + asset);
        sprite.sprite = spriteResource;

        Rigidbody2D rigidBody = furniture.AddComponent<Rigidbody2D>();
        BoxCollider2D collider = furniture.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(spriteResource.rect.width / spriteResource.pixelsPerUnit, spriteResource.rect.height / spriteResource.pixelsPerUnit);

        furniture.transform.parent = gameObject.transform;

        return furniture;
    }

    void GenerateItem(string asset)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
