using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

public class World : IXmlSerializable {

    Tile[,] tiles;

    public List<Character> characters;
    public List<Furniture> furnitures;
    public List<Room> rooms;

    // The pathfinfing graph used to navigate our world map.

    public Path_TileGraph tileGraph;

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

    public World (int width, int height) {

        // Create an empty world.
        SetupWorld (width, height);

        //Create one character.
        Character c = CreateCharacter (GetTileAt (Width / 2, Height / 2));
    }

    public Room GetOutsideRoom () {
        return rooms[0];
    }

    public void DeleteRoom (Room room) {
        if (room == GetOutsideRoom ()) {
            Debug.LogError ("Tried to delete the outside room!");
        }

        room.UnAssignAllTiles ();
        rooms.Remove (room);
    }

    private void SetupWorld (int width, int height) {
        jobQueue = new JobQueue ();

        Width = width;
        Height = height;

        tiles = new Tile[this.Width, Height];

        rooms = new List<Room> ();
        rooms.Add (new Room ());

        for (int x = 0; x < this.Width; x++) {
            for (int y = 0; y < Height; y++) {
                tiles[x, y] = new Tile (this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback (OnTileChanged);
                tiles[x, y].room = GetOutsideRoom (); // Rooms 0 is alwways going to ba outside, and its our default room.
            }
        }

        Debug.Log ("World created with " + (Width * Height) + " tiles.");

        CreateFurniturePrototypes ();

        characters = new List<Character> ();
        furnitures = new List<Furniture> ();
    }

    public void Update (float deltaTime) {

        foreach (Character c in characters) {
            c.Update (deltaTime);
        }

        foreach (Furniture f in furnitures) {
            f.Update (deltaTime);
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

    protected void CreateFurniturePrototypes () {
        furniturePrototypes = new Dictionary<string, Furniture> ();

        furniturePrototypes.Add ("Wall", new Furniture ("Wall", 0, 1, 1, true, true));
        furniturePrototypes.Add ("Door", new Furniture ("Door", 1, 1, 1, false, true));

        furniturePrototypes["Door"].furnParameters["openness"] = 0;
        furniturePrototypes["Door"].furnParameters["is_opening"] = 0;
        furniturePrototypes["Door"].updateActions += FurnitureActions.Door_UpdateAction;
        furniturePrototypes["Door"].IsEnterable = FurnitureActions.Door_IsEnterable;
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
        if (x >= Width || x < 0 || y >= Height || y < 0) {
            // Debug.Log ("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    public Furniture PlaceFurniture (string objectType, Tile t) {
        // TODO: This function assumes 1x1 tile only ----- fix it later

        if (furniturePrototypes.ContainsKey (objectType) == false) {
            Debug.LogError ($"installedObjectProrotybe doesn't contains key: {objectType}");
            return null;
        }
        // Debug.Log ("PlaceInstalledOblect");

        Furniture furn = Furniture.PlaceInstance (furniturePrototypes[objectType], t);

        if (furn == null) {
            // Failed to place object -- most likely there was already something there.
            return null;
        }

        furnitures.Add (furn);

        // Do we need recalculate our rooms?
        if (furn.roomEnclosure) {
            Room.DoRoomFloodFill (furn);
        }

        if (cbFurnitureCreated != null) {
            cbFurnitureCreated (furn);
            if (furn.movementCost != 1) {
                // Since tiles return movement cost as their base cost multiplied
                // buy the furniture's movement cost, a furniture movement cost
                // of exactly 1 doesn't impact our pathfinding system, so we can
                // occasionally avoid invalidating pathfinding graphs
                InvalidateTileGraph (); // Reset the pathfinding system.
            }
        }

        return furn;
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

    public void OnTileChanged (Tile tile) { //Gets Called whenever ANY tile changed.
        if (cbTileChanged == null)
            return;

        cbTileChanged (tile);
        InvalidateTileGraph ();
    }

    // This should be called whenever a change to the world
    // means that our old pathfinding into is invalid.

    public void InvalidateTileGraph () {
        tileGraph = null;
        // Path_TileGraph tileGraph = new Path_TileGraph (WorldController.Instance.world);

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

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///
    ///                             SAVING AND LOADING!!!
    ///
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public World () {

    }
    public XmlSchema GetSchema () {
        return null;
    }
    public void WriteXml (XmlWriter writer) {
        // Save info here!   

        writer.WriteAttributeString ("Width", Width.ToString ());
        writer.WriteAttributeString ("Height", Height.ToString ());

        writer.WriteStartElement ("Tiles");
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (tiles[x, y].Type != TileType.Empty) {
                    writer.WriteStartElement ("Tile");
                    tiles[x, y].WriteXml (writer);
                    writer.WriteEndElement ();
                    //break;
                }
            }
            //break;
        }
        writer.WriteEndElement ();

        writer.WriteStartElement ("Furnitures");
        foreach (Furniture furn in furnitures) {
            writer.WriteStartElement ("Furniture");
            furn.WriteXml (writer);
            writer.WriteEndElement ();
        }

        writer.WriteEndElement ();

        writer.WriteStartElement ("Characters");
        foreach (Character character in characters) {
            writer.WriteStartElement ("Character");
            character.WriteXml (writer);
            writer.WriteEndElement ();
        }

        writer.WriteEndElement ();
    }

    public void ReadXml (XmlReader reader) {
        // Load info here!
        Debug.Log ("World::ReadXml");

        Width = int.Parse (reader.GetAttribute ("Width"));
        Height = int.Parse (reader.GetAttribute ("Width"));

        SetupWorld (Width, Height);

        while (reader.Read ()) {
            switch (reader.Name) {
                case "Tiles":
                    ReadXml_Tiles (reader);
                    break;

                case "Furnitures":
                    ReadXml_Furnitures (reader);
                    break;

                case "Characters":
                    ReadXml_Characters (reader);
                    break;
            }
        }
    }

    private void ReadXml_Characters (XmlReader reader) {
        Debug.Log ("ReadXml_Characters");

        // We are in the "Characters" element, so read elements until
        // we run out of "Character" nodes.

        if (reader.ReadToDescendant ("Character"))
            do {
                int x = int.Parse (reader.GetAttribute ("X"));
                int y = int.Parse (reader.GetAttribute ("Y"));

                Character c = CreateCharacter (tiles[x, y]);
                c.ReadXml (reader);
            } while (reader.ReadToNextSibling ("Character"));
    }

    private void ReadXml_Furnitures (XmlReader reader) {
        Debug.Log ("ReadXml_Furnitures");
        // We are in the "Furnitures" element, so read elements until
        // we run out of "Furniture".

        if (reader.ReadToDescendant ("Furniture") == true) {
            do {
                int x = int.Parse (reader.GetAttribute ("X"));
                int y = int.Parse (reader.GetAttribute ("Y"));

                Furniture furn = PlaceFurniture (reader.GetAttribute ("objectType"), tiles[x, y]);
                furn.ReadXml (reader);
            } while (reader.ReadToNextSibling ("Furniture"));
        }
    }

    void ReadXml_Tiles (XmlReader reader) {
        Debug.Log ("ReadXml_Tiles");
        // We are in the "Tiles" element, so read elements until
        // we run out of "Tile" nodes.

        if (reader.ReadToDescendant ("Tile")) {
            // We have at least one tile, so do something with it.

            do {
                int x = int.Parse (reader.GetAttribute ("X"));
                int y = int.Parse (reader.GetAttribute ("Y"));
                tiles[x, y].ReadXml (reader);
            } while (reader.ReadToNextSibling ("Tile"));
        }
    }
}
