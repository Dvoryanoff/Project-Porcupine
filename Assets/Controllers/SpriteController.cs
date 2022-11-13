using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteController : MonoBehaviour {

    Dictionary<Tile, GameObject> tileGameobjectMap;
    Dictionary<Furniture, GameObject> furnitureGameobjectMap;
    Dictionary<string, Sprite> furnitureSprites;

    [SerializeField] private Sprite floorSprite; // FIXME!
    [SerializeField] private Sprite emptySprite; // FIXME!

    World world {
        get {
            return WorldController.Instance.World;
        }
    }

    public static WorldController Instance {
        get; protected set;
    }

    private void Start() {

        LoadSprites();

        tileGameobjectMap = new Dictionary<Tile, GameObject>();
        furnitureGameobjectMap = new Dictionary<Furniture, GameObject>();

        world.RegisterFurnitureCreated(OnFurnitureCreated);
        world.RegisterTileChanged(OnTileChanged);

    }

    private void LoadSprites() {
        furnitureSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture");
        Debug.Log("LOADED RESOURCES:");

        foreach (Sprite s in sprites) {
            Debug.Log(s);
            furnitureSprites[s.name] = s;
        }
    }

    private void DestroyAllTileGameobjects() {
        while (tileGameobjectMap.Count > 0) {
            Tile tile_data = tileGameobjectMap.Keys.First();
            GameObject tileGameObject = tileGameobjectMap[tile_data];

            tileGameobjectMap.Remove(tile_data);

            tile_data.UnregisterTileTypeChangedCallback(OnTileChanged);

            Destroy(tileGameObject);
        }
    }

    // Called whenever tile's data get changed.

    public void OnTileChanged(Tile tile_data) {

        if (tileGameobjectMap.ContainsKey(tile_data) == false) {
            Debug.LogError($"tileGameobjectMap doesn/t contain tole data!");
            return;
        }

        GameObject tile_go = tileGameobjectMap[tile_data];

        if (tile_go == null) {
            Debug.LogError($"tileGameobjectMap doesn/t contain tole data!");
            return;
        }

        if (tile_data.Type == TileType.Floor) {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        } else if (tile_data.Type == TileType.Empty) {
            tile_go.GetComponent<SpriteRenderer>().sprite = null; // FIXME!
        } else {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }

    public void OnFurnitureCreated(Furniture furn) {

        Debug.Log("OnInstalledObjectCreated");

        // Create a visual Game Object linked to this data.

        GameObject furn_go = new GameObject();

        furnitureGameobjectMap.Add(furn, furn_go);

        furn_go.name = furn.objectType + "_" + furn.tile.X + "_" + furn.tile.Y;
        furn_go.transform.position = new Vector3(furn.tile.X, furn.tile.Y, 0);
        furn_go.transform.SetParent(this.transform, true);

        furn_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
        furn_go.GetComponent<SpriteRenderer>().sortingOrder = 1;

        furn.RegisterOnChangedCallback(OnFurnitureChange);
    }

    private void OnFurnitureChange(Furniture furn) {

        // Make sure that furnityre graphics are corrrect.

        if (furnitureGameobjectMap.ContainsKey(furn) == false) {
            return;
        }

        GameObject furn_go = furnitureGameobjectMap[furn];
        furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);

    }

    Sprite GetSpriteForFurniture(Furniture obj) {
        if (obj.linksToNeighbour == false) {
            return furnitureSprites[obj.objectType];
        }

        string spriteName = $"{obj.objectType}_";

        // Check neighbours for North, East, South, West!

        Tile t;

        int x = obj.tile.X;
        int y = obj.tile.Y;

        t = world.GetTileAt(x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "N";
        }

        t = world.GetTileAt(x + 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "E";
        }

        t = world.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "S";
        }

        t = world.GetTileAt(x - 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "W";
        }

        //if (furnitureSprites.ContainsKey(spriteName) == false) {
        //    Debug.LogError($"GetSpritesForInstalledObjects: -- No sprites with name: {spriteName}");
        //    return null;
        //}

        return furnitureSprites[spriteName];

    }

}

