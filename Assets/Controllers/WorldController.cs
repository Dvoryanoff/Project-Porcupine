using UnityEngine;

public class WorldController : MonoBehaviour {
    public Sprite floorSprite;

    World world;

    void Start() {
        world = new World();
        world.RandomizeTiles();

        for (int x = 0; x < world.width; x++) {
            for (int y = 0; y < world.height; y++) {
                GameObject tileGameObject = new GameObject();
                tileGameObject.name = $"Tile_{x}_{y}";

                //SpriteRenderer tileSpriteRenderer = tileGameObject.AddComponent<SpriteRenderer>();

                //Tile tileData = world.GetTileAt(x, y);

                //if (tileData.Type == Tile.TileType.Floor) {
                //    tileSpriteRenderer.sprite = floorSprite;
                //}

            }
        }
    }
}
