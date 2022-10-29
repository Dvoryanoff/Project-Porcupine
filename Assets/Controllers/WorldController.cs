using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldController : MonoBehaviour {

    Dictionary<Tile, GameObject> tileGameobjectMap;
    Dictionary<InstalledObject, GameObject> installedObjectGameobjectMap;
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
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/InstalledObjects");
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
        World.RegisterInstalledObjectCreated(OnInstalledObjectCreated);

        tileGameobjectMap = new Dictionary<Tile, GameObject>();
        installedObjectGameobjectMap = new Dictionary<InstalledObject, GameObject>();

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

    public void OnInstalledObjectCreated(InstalledObject obj) {

        Debug.Log("OnInstalledObjectCreated");
        // Create a visual Game Object linked to this data.

        GameObject objGameObject = new GameObject();

        installedObjectGameobjectMap.Add(obj, objGameObject);

        objGameObject.name = obj.objectType + "_" + obj.tile.X + "_" + obj.tile.Y;
        objGameObject.transform.position = new Vector3(obj.tile.X, obj.tile.Y, 0);
        objGameObject.transform.SetParent(this.transform, true);
        // objGameObject.S

        objGameObject.AddComponent<SpriteRenderer>().sprite = installObjectsSprites["Wall_"];
        //objGameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GameObjects";
        objGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

        obj.RegisterOnChangedCallback(OnInstalledObjectChange);
    }

    private void OnInstalledObjectChange(InstalledObject obj) {
        Debug.LogError("OnInstalledObjectChange ---- NOT IMPEMENTED!");
    }
}

