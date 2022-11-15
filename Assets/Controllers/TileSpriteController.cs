using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSpriteController : MonoBehaviour {

    [SerializeField] private Sprite floorSprite; // FIXME!
    [SerializeField] private Sprite emptySprite; // FIXME!

    Dictionary<Tile, GameObject> tileGameobjectMap;

    World world {
        get {
            return WorldController.Instance.world;
        }
    }

    void Start() {
        // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
        tileGameobjectMap = new Dictionary<Tile, GameObject>();

        // Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
        for (int x = 0; x < world.Width; x++) {
            for (int y = 0; y < world.Height; y++) {
                // Get the tile data
                Tile tile_data = world.GetTileAt(x, y);

                // This creates a new GameObject and adds it to our scene.
                GameObject tile_go = new GameObject();

                // Add our tile/GO pair to the dictionary.
                tileGameobjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);

                // Add a Sprite Renderer
                // Add a default sprite for empty tiles.
                tile_go.AddComponent<SpriteRenderer>().sprite = emptySprite;

            }
        }
        world.RegisterTileChanged(OnTileChanged);
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
}

