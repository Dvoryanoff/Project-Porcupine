using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Debug = UnityEngine.Debug;

public class Furniture : IXmlSerializable {

    // Custom parameter for this particular piece of furniture.  We are
    // using a dictionary because later, custom LUA function will be
    // able to use whatever parameters the user/modder would like.
    // Basically, the LUA code will bind to this dictionary.
    protected Dictionary<string, float> furnParameters;

    // These actions are called every update. They get passed the furniture
    // they belong to, plus a deltaTime.
    protected Action<Furniture, float> updateActions;
    public Func<Furniture, ENTERABILITY> IsEnterable;

    public void Update ( float deltaTime ) {
        if ( updateActions != null ) {
            updateActions ( this, deltaTime );
        }
    }

    public Tile tile { get; protected set; }
    public string objectType { get; protected set; }
    public float movementCost { get; protected set; } = 1f;
    public bool roomEnclosure { get; protected set; }
    public bool linksToNeighbour { get; protected set; }

    // TODO: Implement larger objects
    // TODO: Implement object rotation
    private int width;
    private int height;

    public Action<Furniture> cbOnChanged;

    private Func<Tile, bool> funcPositionValidation;
    private Furniture proto;

    // Empty constructor is used for Serialization.
    public Furniture () {
        furnParameters = new Dictionary<string, float> ();
    }

    // Copy Constructor -- don't call this directly, unless we never
    // do ANY sub-classing. Instead use Clone(), which is more virtual.
    protected Furniture ( Furniture other ) {
        this.objectType = other.objectType;
        this.movementCost = other.movementCost;
        this.roomEnclosure = other.roomEnclosure;
        this.width = other.width;
        this.height = other.height;
        this.linksToNeighbour = other.linksToNeighbour;

        this.furnParameters = new Dictionary<string, float> ( other.furnParameters );

        if ( other.updateActions != null ) {
            this.updateActions = (Action<Furniture, float>) other.updateActions.Clone ();
        }

        this.IsEnterable = other.IsEnterable;
    }

    // Make a copy of the current furniture.  Sub-classed should
    // override this Clone() if a different (sub-classed) copy
    // constructor should be run.
    virtual public Furniture Clone () {
        return new Furniture ( this );
    }

    // Create furniture from parameters -- this will probably ONLY ever be used for prototypes.
    public Furniture ( string objectType,
        float movementCost = 1f,
        int width = 1,
        int height = 1,
        bool linkToNeighbour = false,
        bool roomEnclosure = false ) {
        this.objectType = objectType;
        this.movementCost = movementCost;
        this.roomEnclosure = roomEnclosure;
        this.width = width;
        this.height = height;
        this.linksToNeighbour = linkToNeighbour;
        this.funcPositionValidation = this.DEFAULT_IsValidPosition;
        furnParameters = new Dictionary<string, float> ();
    }
    static public Furniture PlaceInstance ( Furniture proto, Tile tile ) {

        if ( proto.funcPositionValidation ( tile ) == false ) {
            Debug.LogError ( $"PlaceInstance -- Position validity function returned FALSE!" );
            return null;
        }

        Furniture obj = proto.Clone ();

        obj.tile = tile;

        if ( tile.PlaceFurniture ( obj ) == false ) {
            return null;
        };
        if ( obj.linksToNeighbour ) {
            // This type of furniture links inself to its neighbours
            // so we should inform our neighbours that they have a new buudy/
            // Just trigger their callback/

            Tile t;

            int x = obj.tile.X;
            int y = obj.tile.Y;

            t = tile.world.GetTileAt ( x, y + 1 );
            if ( t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType ) {
                t.furniture.cbOnChanged ( t.furniture );
            }

            t = tile.world.GetTileAt ( x + 1, y );
            if ( t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType ) {
                t.furniture.cbOnChanged ( t.furniture );
            }

            t = tile.world.GetTileAt ( x, y - 1 );
            if ( t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType ) {
                t.furniture.cbOnChanged ( t.furniture );
            }

            t = tile.world.GetTileAt ( x - 1, y );
            if ( t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType ) {
                t.furniture.cbOnChanged ( t.furniture );
            }
        }

        return obj;
    }

    public void RegisterOnChangedCallback ( Action<Furniture> callbackFunc ) {
        cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback ( Action<Furniture> callbackFunc ) {
        cbOnChanged -= callbackFunc;
    }

    public bool IsValidPosition ( Tile tile ) {
        return funcPositionValidation ( tile );
    }

    // FIXME: These functions should never be called directly,
    // so they probably shouldn't be public functions of Furniture
    // This will be replaced by validation checks fed to use from 
    // LUA files that will be customizable for each piece of furniture.
    // For example, a door might specific that it needs two walls to
    // connect to.
    protected bool DEFAULT_IsValidPosition ( Tile tile ) {
        // Make sure tile is FLOOR.

        if ( tile.Type != TileType.Floor ) {
            return false;
        }

        // Make sure tile doesn't already have furniture.

        if ( tile.furniture != null ) {
            return false;
        }

        return true;

    }

    // FIXME: These functions shouldn't be public.

    public XmlSchema GetSchema () {
        return null;
    }
    public void WriteXml ( XmlWriter writer ) {
        writer.WriteAttributeString ( "X", tile.X.ToString () );
        writer.WriteAttributeString ( "Y", tile.Y.ToString () );
        writer.WriteAttributeString ( "objectType", objectType );
        //writer.WriteAttributeString ("movementCost", movementCost.ToString ());

        foreach ( string k in furnParameters.Keys ) {
            writer.WriteStartElement ( "Params" );
            writer.WriteAttributeString ( "name", k );
            writer.WriteAttributeString ( "value", furnParameters[k].ToString () );
            writer.WriteEndElement ();
        }
    }

    public void ReadXml ( XmlReader reader ) {
        // X, Y, and objectType have already been set, and we should already
        // be assigned to a tile.  So just read extra data.

        //movementCost = int.Parse (reader.GetAttribute ("movementCost"));

        if ( reader.ReadToDescendant ( "Param" ) ) {
            do {
                string k = reader.GetAttribute ("name");
                float v = float.Parse (reader.GetAttribute ("value"));
                furnParameters[k] = v;
            } while ( reader.ReadToNextSibling ( "Param" ) );
        }
    }
    public float GetParameter ( string key, float default_value = 0 ) {
        if ( furnParameters.ContainsKey ( key ) == false ) {
            return default_value;
        }
        return furnParameters[key];
    }

    public void SetParameter ( string key, float value ) {
        furnParameters[key] = value;
    }

    public void ChangeParameter ( string key, float value ) {
        if ( furnParameters.ContainsKey ( key ) == false ) {
            furnParameters[key] = value;
        }
        furnParameters[key] += value;
    }

    // Registers a function that will be called every Update.
    // (Later this implementation might change a bit as we support LUA.)
    public void RegisterUpdateAction ( Action<Furniture, float> a ) {
        updateActions += a;
    }

    public void UnregisterUpdateActions ( Action<Furniture, float> a ) {
        updateActions -= a;
    }
}

