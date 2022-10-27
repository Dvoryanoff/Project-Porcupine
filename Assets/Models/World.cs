using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World {

    Tile[,] tiles;

    Dictionary<string, InstalledObject> installedObjectsPrototype;

    public int Width {
        get; protected set;
    }

    public int Height {
        get; protected set;
    }

    public World(int width = 100, int height = 100) {
        Width = width;
        Height = height;

        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                tiles[x, y] = new Tile(this, x, y);
            }
        }
        Debug.Log("World created with " + (Width * Height) + " tiles.");

        CreateInstalledObjectsPrototype();

    }

    protected void CreateInstalledObjectsPrototype() {
        installedObjectsPrototype = new Dictionary<string, InstalledObject>();

        installedObjectsPrototype.Add("Wall", InstalledObject.CreatePrototype("Wall", 0, 1, 1));
    }

    public void RandomizeTiles() {
        Debug.Log("Randomize");
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                if (Random.Range(0, 2) == 0) {
                    tiles[x, y].Type = Tile.TileType.Empty;
                } else {
                    tiles[x, y].Type = Tile.TileType.Floor;
                }

            }
        }
    }

    public Tile GetTileAt(int x, int y) {
        if (x > Width || x < 0 || y > Height || y < 0) {
            Debug.Log("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    internal void PlaceInstalledOblect(string objectType, Tile t) {
        // TODO: This function assumes 1x1 tile only ----- fix it later

        if (installedObjectsPrototype.ContainsKey(objectType) == false) {
            Debug.LogError($"installedObjectProrotybe doesn't contains key: {objectType}");
            return;
        }
        Debug.Log("PlaceInstalledOblect");

        InstalledObject.PlaceInstance(installedObjectsPrototype[objectType], t);

    }
}
