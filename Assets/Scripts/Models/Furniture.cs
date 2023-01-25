using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Debug = UnityEngine.Debug;

public class Furniture : IXmlSerializable {
    public Tile tile {
        get; protected set;
    }
    public string objectType {
        get; protected set;
    }
    public float movementCost { get; protected set; } = 1f;
    private int width;
    private int height;

    public bool linksToNeighbour {
        get; protected set;
    }

    // TODO: Implement larger objects
    // TODO: Implement object rotation

    Action<Furniture> cbOnChanged;

    private Func<Tile, bool> funcPositionValidation;

    public Furniture () {

    }

    static public Furniture CreatePrototype (string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linkToNeighbour = false) {
        Furniture obj = new Furniture ();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linkToNeighbour;

        obj.funcPositionValidation = obj.__IsValidPosition;

        return obj;
    }
    static public Furniture PlaceInstance (Furniture proto, Tile tile) {

        if (proto.funcPositionValidation (tile) == false) {
            Debug.LogError ($"PlaceInstance -- Position validity function returned FALSE!");
            return null;
        }

        Furniture obj = new Furniture ();

        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbour = proto.linksToNeighbour;

        obj.tile = tile;
        if (tile.PlaceFurniture (obj) == false) {
            return null;
        };
        if (obj.linksToNeighbour) {
            // This type of furniture links inself to its neighbours
            // so we should inform our neighbours that they have a new buudy/
            // Just trigger their callback/

            Tile t;

            int x = obj.tile.X;
            int y = obj.tile.Y;

            t = tile.world.GetTileAt (x, y + 1);
            if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType) {
                t.furniture.cbOnChanged (t.furniture);
            }

            t = tile.world.GetTileAt (x + 1, y);
            if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType) {
                t.furniture.cbOnChanged (t.furniture);
            }

            t = tile.world.GetTileAt (x, y - 1);
            if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType) {
                t.furniture.cbOnChanged (t.furniture);
            }

            t = tile.world.GetTileAt (x - 1, y);
            if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType) {
                t.furniture.cbOnChanged (t.furniture);
            }
        }

        return obj;
    }

    public void RegisterOnChangedCallback (Action<Furniture> callbackFunc) {
        cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback (Action<Furniture> callbackFunc) {
        cbOnChanged -= callbackFunc;
    }

    public bool IsValidPosition (Tile tile) {
        return funcPositionValidation (tile);
    }

    // FIXME: These functions shouldn't be public.

    public bool __IsValidPosition (Tile tile) {
        // Make sure tile is FLOOR.

        if (tile.Type != TileType.Floor) {
            return false;
        }

        // Make sure tile doesn't already have furniture.

        if (tile.furniture != null) {
            return false;
        }

        return true;

    }

    // FIXME: These functions shouldn't be public.

    public bool __IsValidPosition_Door (Tile tile) {
        // Make sure we have a pair of E/W walls or S/N walls.

        if (__IsValidPosition (tile) == false) {
            return false;
        }

        return true;
    }

    public XmlSchema GetSchema () {
        return null;
    }
    public void WriteXml (XmlWriter writer) {
        writer.WriteAttributeString ("X", tile.X.ToString ());
        writer.WriteAttributeString ("Y", tile.Y.ToString ());
        writer.WriteAttributeString ("objectType", objectType);
        writer.WriteAttributeString ("movementCost", movementCost.ToString ());
    }

    public void ReadXml (XmlReader reader) {
        // X, Y, and objectType have already been set, and we should already
        // be assigned to a tile.  So just read extra data.

        movementCost = int.Parse (reader.GetAttribute ("movementCost"));

    }

}

