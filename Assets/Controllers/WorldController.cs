using UnityEngine;

public class WorldController : MonoBehaviour {

    public static WorldController Instance {
        get; protected set;
    }

    public Sprite floorSprite;

    public World World {
        get; protected set;
    }
    private void Start() {

        if (Instance != null) {
            Debug.Log($"There should never be two worlds!!!");
        }
        Instance = this;

        World = new World();

        for (int x = 0; x < World.Width; x++) {
            for (int y = 0; y < World.Height; y++) {

                Tile tile_data = World.GetTileAt(x, y);

                GameObject tileGameObject = new GameObject();
                tileGameObject.name = "Tile_" + x + "_" + y;
                tileGameObject.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tileGameObject.transform.SetParent(this.transform, true);

                tileGameObject.AddComponent<SpriteRenderer>();

                tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tileGameObject); });
            }
        }

        World.RandomizeTiles();
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

    private void OnTileTypeChanged(Tile tile_data, GameObject tile_go) {

        if (tile_data.Type == Tile.TileType.Floor) {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        } else if (tile_data.Type == Tile.TileType.Empty) {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        } else {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }

    }

    public Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return World.GetTileAt(x, y);
    }
}

