using UnityEngine;

public class WorldController : MonoBehaviour {

    public Sprite floorSprite;

    World world;
    void Start() {

        world = new World();

        for (int x = 0; x < world.Width; x++) {
            for (int y = 0; y < world.Height; y++) {

                Tile tile_data = world.GetTileAt(x, y);

                GameObject tileGameObject = new GameObject();
                tileGameObject.name = "Tile_" + x + "_" + y;
                tileGameObject.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tileGameObject.transform.SetParent(this.transform, true);

                tileGameObject.AddComponent<SpriteRenderer>();

                tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tileGameObject); });
            }
        }

        world.RandomizeTiles();
    }

    float randomizeTileTimer = 2f;

    //void Update() {
    //    randomizeTileTimer -= Time.deltaTime;
    //
    //    if (randomizeTileTimer < 0) {
    //        world.RandomizeTiles();
    //        randomizeTileTimer = 2f;
    //
    //    }
    //}

    void OnTileTypeChanged(Tile tile_data, GameObject tile_go) {

        if (tile_data.Type == Tile.TileType.Floor) {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        } else if (tile_data.Type == Tile.TileType.Empty) {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        } else {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }

    }
}

