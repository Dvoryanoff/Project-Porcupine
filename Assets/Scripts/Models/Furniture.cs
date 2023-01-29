using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Debug = UnityEngine.Debug;

public class Furniture : IXmlSerializable {

    public Dictionary<string, float> furnParameters;
    public Action<Furniture, float> updateActions;
    public Func<Furniture, ENTERABILITY> IsEnterable;

    public void Update (float deltaTime) {
        if (updateActions != null) {
            updateActions (this, deltaTime);
        }
    }

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

    public Action<Furniture> cbOnChanged;

    private Func<Tile, bool> funcPositionValidation;
    private Furniture proto;

    // Empty constructor is used for Serialization.
    public Furniture () {
        furnParameters = new Dictionary<string, float> ();
    }
    // Copy constructor.
    protected Furniture (Furniture other) {
        this.objectType = other.objectType;
        this.movementCost = other.movementCost;
        this.width = other.width;
        this.height = other.height;
        this.linksToNeighbour = other.linksToNeighbour;

        this.furnParameters = new Dictionary<string, float> (other.furnParameters);

        if (other.updateActions != null) {
            this.updateActions = (Action<Furniture, float>)other.updateActions.Clone ();
        }

        this.IsEnterable = other.IsEnterable;
    }

    virtual public Furniture Clone () {
        return new Furniture (this);
    }

    // Create furniture from parameters -- this will probably ONLY ever be used for prototypes.
    public Furniture (string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linkToNeighbour = false) {
        this.objectType = objectType;
        this.movementCost = movementCost;
        this.width = width;
        this.height = height;
        this.linksToNeighbour = linkToNeighbour;
        this.funcPositionValidation = this.__IsValidPosition;
        furnParameters = new Dictionary<string, float> ();
    }
    static public Furniture PlaceInstance (Furniture proto, Tile tile) {

        if (proto.funcPositionValidation (tile) == false) {
            Debug.LogError ($"PlaceInstance -- Position validity function returned FALSE!");
            return null;
        }

        Furniture obj = proto.Clone ();

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
        //writer.WriteAttributeString ("movementCost", movementCost.ToString ());

        foreach (string k in furnParameters.Keys) {
            writer.WriteStartElement ("Params");
            writer.WriteAttributeString ("name", k);
            writer.WriteAttributeString ("value", furnParameters[k].ToString ());
            writer.WriteEndElement ();
        }
    }

    public void ReadXml (XmlReader reader) {
        // X, Y, and objectType have already been set, and we should already
        // be assigned to a tile.  So just read extra data.

        //movementCost = int.Parse (reader.GetAttribute ("movementCost"));

        if (reader.ReadToDescendant ("Param")) {
            do {
                string k = reader.GetAttribute ("name");
                float v = float.Parse (reader.GetAttribute ("value"));
                furnParameters[k] = v;
            } while (reader.ReadToNextSibling ("Param"));
        }
    }
}

