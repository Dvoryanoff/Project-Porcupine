using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldController : MonoBehaviour {

    Dictionary<Tile, GameObject> tileGameobjectMap;
    Dictionary<Furniture, GameObject> furnitureGameobjectMap;
    Dictionary<string, Sprite> installObjectsSprites;

    [SerializeField] private Sprite floorSprite; // FIXME!

    public static WorldController Instance {
        get; protected set;
    }

    public World World {
        get; protected set;
    }
    private void Start() {

        installObjectsSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture");
        Debug.Log("LOADED RESOURCES:");

        foreach (Sprite s in sprites) {
            Debug.Log(s);
            installObjectsSprites[s.name] = s;
        }
        if (Instance != null) {
            Debug.Log($"There should never be two worlds!!!");
        }
        Instance = this;

        World = new World();
        World.RegisterInstalledObjectCreated(OnFurnitureCreated);

        tileGameobjectMap = new Dictionary<Tile, GameObject>();
        furnitureGameobjectMap = new Dictionary<Furniture, GameObject>();

        for (int x = 0; x < World.Width; x++) {
            for (int y = 0; y < World.Height; y++) {

                Tile tile_data = World.GetTileAt(x, y);

                GameObject tileGameObject = new GameObject();

                tileGameobjectMap.Add(tile_data, tileGameObject);

                tileGameObject.name = "Tile_" + x + "_" + y;
                tileGameObject.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tileGameObject.transform.SetParent(this.transform, true);

                tileGameObject.AddComponent<SpriteRenderer>();

                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

        World.RandomizeTiles();
    }

    private void DestroyAllTileGameobjects() {
        while (tileGameobjectMap.Count > 0) {
            Tile tile_data = tileGameobjectMap.Keys.First();
            GameObject tileGameObject = tileGameobjectMap[tile_data];

            tileGameobjectMap.Remove(tile_data);

            tile_data.UnregisterTileTypeChangedCallback(OnTileTypeChanged);

            Destroy(tileGameObject);

        }
    }

    public void OnTileTypeChanged(Tile tile_data) {

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
        //float randomizeTileTimer = 2f;

        //void Update() {
        //    randomizeTileTimer -= Time.deltaTime;
        //
        //    if (randomizeTileTimer < 0) {
        //        world.RandomizeTiles();
        //        randomizeTileTimer = 2f;
        //
        //    }
        //}

    }

    public Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return World.GetTileAt(x, y);
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
            // Debug.LogError("OnFurnitureChanged -- trying to change visuals for furniture not in our map");
            return;
        }

        GameObject furn_go = furnitureGameobjectMap[furn];
        furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);

    }

    Sprite GetSpriteForFurniture(Furniture obj) {
        if (obj.linksToNeighbour == false) {
            return installObjectsSprites[obj.objectType];
        }

        string spriteName = $"{obj.objectType}_";

        // Check neighbours for North, East, South, West!

        Tile t;

        int x = obj.tile.X;
        int y = obj.tile.Y;

        t = World.GetTileAt(x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "N";
        }

        t = World.GetTileAt(x + 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "E";
        }

        t = World.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "S";
        }

        t = World.GetTileAt(x - 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "W";
        }

        //if (furnitureSprites.ContainsKey(spriteName) == false) {
        //    Debug.LogError($"GetSpritesForInstalledObjects: -- No sprites with name: {spriteName}");
        //    return null;
        //}

        // Debug.Log($" Need sprite {spriteName}");
        return installObjectsSprites[spriteName];

    }

}

