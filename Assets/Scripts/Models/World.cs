using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World {

    Tile[,] tiles;

    List<Character> characters;

    Dictionary<string, Furniture> furniturePrototypes;

    public int Width {
        get; protected set;
    }

    public int Height {
        get; protected set;
    }
    Action<Furniture> cbFurnitureCreated;
    Action<Character> cbCharacterCreated;
    Action<Tile> cbTileChanged;

    public JobQueue jobQueue;

    public World (int width = 100, int height = 100) {
        jobQueue = new JobQueue ();

        Width = width;
        Height = height;

        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                tiles[x, y] = new Tile (this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback (OnTileChanged);
            }
        }

        Debug.Log ("World created with " + (Width * Height) + " tiles.");

        CreateInstalledObjectsPrototype ();
        characters = new List<Character> ();

    }

    public void Update (float deltaTime) {
        foreach (Character c in characters) {
            c.Update (deltaTime);
        }
    }
    public Character CreateCharacter (Tile t) {

        Character c = new Character (t);

        characters.Add (c);

        if (cbCharacterCreated != null) {
            cbCharacterCreated (c);
        }
        return c;
    }

    protected void CreateInstalledObjectsPrototype () {
        furniturePrototypes = new Dictionary<string, Furniture> ();

        furniturePrototypes.Add ("Wall", Furniture.CreatePrototype ("Wall", 0, 1, 1, true));
    }

    public void RandomizeTiles () {
        Debug.Log ("Randomize");
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                if (Random.Range (0, 2) == 0) {
                    tiles[x, y].Type = TileType.Empty;
                } else {
                    tiles[x, y].Type = TileType.Floor;
                }
            }
        }
    }

    public void SetupPathfindingExample () {
        Debug.Log ("SetupPathfindingExample");

        // Make a set of floors/walls to test pathfinding with.

        int l = Width / 2 - 5;
        int b = Height / 2 - 5;

        for (int x = l - 5; x < l + 15; x++) {
            for (int y = b - 5; y < b + 15; y++) {
                tiles[x, y].Type = TileType.Floor;

                if (x == l || x == (l + 9) || y == b || y == (b + 9)) {
                    if (x != (l + 9) && y != (b + 4)) {
                        PlaceFurniture ("Wall", tiles[x, y]);
                    }
                }

            }
        }

    }

    public Tile GetTileAt (int x, int y) {
        if (x > Width || x < 0 || y > Height || y < 0) {
            Debug.Log ("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    internal void PlaceFurniture (string objectType, Tile t) {
        // TODO: This function assumes 1x1 tile only ----- fix it later

        if (furniturePrototypes.ContainsKey (objectType) == false) {
            Debug.LogError ($"installedObjectProrotybe doesn't contains key: {objectType}");
            return;
        }
        Debug.Log ("PlaceInstalledOblect");

        Furniture obj = Furniture.PlaceInstance (furniturePrototypes[objectType], t);

        if (obj == null) {
            // Failed to place object -- most likely there was already something there.
            return;
        }

        if (cbFurnitureCreated != null) {
            cbFurnitureCreated (obj);
        }

    }

    public void RegisterFurnitureCreated (Action<Furniture> callbackFunc) {
        cbFurnitureCreated += callbackFunc;
    }

    public void UnRegisterFurnitureCreated (Action<Furniture> callbackFunc) {
        cbFurnitureCreated -= callbackFunc;
    }

    public void RegisterCharacterCreated (Action<Character> callbackFunc) {
        cbCharacterCreated += callbackFunc;
    }

    public void UnRegisterCharacterCreated (Action<Character> callbackFunc) {
        cbCharacterCreated -= callbackFunc;
    }

    public void RegisterTileChanged (Action<Tile> callbackFunc) {
        cbTileChanged += callbackFunc;
    }

    public void UnRegisterTileChanged (Action<Tile> callbackFunc) {
        cbTileChanged -= callbackFunc;
    }

    public void OnTileChanged (Tile tile) {
        if (tile == null) {
            return;
        }
        cbTileChanged (tile);
    }

    public bool IsFurniturePlacementValid (string furnitureType, Tile tile) {
        return furniturePrototypes[furnitureType].IsValidPosition (tile);
    }

    public Furniture GetFurniturePrototype (string objecType) {
        if (furniturePrototypes.ContainsKey (objecType) == false) {
            Debug.LogError ($"No furniture with type {objecType}");
        }
        return furniturePrototypes[objecType];
    }
}