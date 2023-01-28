using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum TileType { Empty, Floor };

public enum ENTERABILITY { Yes, Never, Soon };
public class Tile : IXmlSerializable {

    private TileType _type = TileType.Empty;

    Action<Tile> cbTileChanged;

    public TileType Type {
        get {
            return _type;
        }
        set {
            TileType oldType = _type;
            _type = value;

            if (cbTileChanged != null && oldType != _type)
                cbTileChanged (this);
        }
    }

    Inventory inventory;

    public Job pendingFurnitureJob;

    public Furniture furniture {
        get; protected set;
    }

    public World world {
        get; protected set;
    }
    public int X {
        get; protected set;
    }
    public int Y {
        get; protected set;
    }

    public float movementCost {
        get {
            if (Type == TileType.Empty)
                return 0; // 0 is Unwalkable.
            if (furniture == null)
                return 1; // Normal cost.

            return 1 * furniture.movementCost;

        }
    }

    public Tile (World world, int x, int y) {
        this.world = world;
        this.X = x;
        this.Y = y;
    }

    public void RegisterTileTypeChangedCallback (Action<Tile> callback) {
        cbTileChanged += callback;
    }

    public void UnregisterTileTypeChangedCallback (Action<Tile> callback) {
        cbTileChanged -= callback;
    }

    public bool PlaceFurniture (Furniture objInstance) {
        if (objInstance == null) {
            furniture = null;
            return true;
        }

        if (furniture != null) {
            Debug.LogError ("Trying to assing an furniture object to a tile that already have one!");
            return false;
        }

        furniture = objInstance;
        return true;

    }

    public bool IsNeighbour (Tile tile, bool diagOkay = false) {

        // Check to see if we have a difference of exactly ONE between the two
        // tile coordinates.  Is so, then we are vertical or horizontal neighbours.
        return
            Mathf.Abs (this.X - tile.X) + Mathf.Abs (this.Y - tile.Y) == 1 ||  // Check hori/vert adjacency
            (diagOkay && (Mathf.Abs (this.X - tile.X) == 1 && Mathf.Abs (this.Y - tile.Y) == 1)); // Check diag adjacency
    }

    public Tile[] GetNeighbours (bool diagOkay = false) {

        Tile[] neighbours;

        if (diagOkay == false) {
            neighbours = new Tile[4]; // Tile order: N E S W 
        } else {
            neighbours = new Tile[8]; // Tile order: N E S W NE SE SW NW

        }

        Tile n;

        n = world.GetTileAt (X, Y + 1);// Can be null but thats okay.
        neighbours[0] = n;
        n = world.GetTileAt (X + 1, Y);// Can be null but thats okay.
        neighbours[1] = n;
        n = world.GetTileAt (X, Y - 1);// Can be null but thats okay.
        neighbours[2] = n;
        n = world.GetTileAt (X - 1, Y);// Can be null but thats okay.
        neighbours[3] = n;

        if (diagOkay) {
            n = world.GetTileAt (X + 1, Y + 1);// Can be null but thats okay.
            neighbours[4] = n;
            n = world.GetTileAt (X + 1, Y - 1);// Can be null but thats okay.
            neighbours[5] = n;
            n = world.GetTileAt (X - 1, Y - 1);// Can be null but thats okay.
            neighbours[6] = n;
            n = world.GetTileAt (X + 1, Y + 1);// Can be null but thats okay.
            neighbours[7] = n;
        }

        return neighbours;

    }

    public XmlSchema GetSchema () {
        return null;
    }

    public void ReadXml (XmlReader reader) {

        Type = (TileType)int.Parse (reader.GetAttribute ("Type"));

    }

    public void WriteXml (XmlWriter writer) {
        writer.WriteAttributeString ("X", X.ToString ());
        writer.WriteAttributeString ("Y", Y.ToString ());
        writer.WriteAttributeString ("Type", ((int)Type).ToString ());
    }

    public ENTERABILITY IsEnterable () {

        // This returns true if you can enter this tile right this moment.
        if (movementCost == 0)
            return ENTERABILITY.Never;

        // Check out furniture to see if it has a special block on enterability
        if (furniture != null && furniture.IsEnterable != null) {
            return furniture.IsEnterable (furniture);
        }

        return ENTERABILITY.Yes;
    }
}
