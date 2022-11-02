
using System;

public class Furniture {
    public Tile tile {
        get; protected set;
    }
    public string objectType {
        get; protected set;
    }
    private float movementCost = 1f;
    private int width;
    private int height;

    public bool linksToNeighbour {
        get; protected set;
    }

    // TODO: Implement larger objects
    // TODO: Implement object rotation

    Action<Furniture> cbOnChanged;

    protected Furniture() {

    }

    static public Furniture CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linkToNeighbour = false) {
        Furniture obj = new Furniture();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linkToNeighbour;

        return obj;
    }
    static public Furniture PlaceInstance(Furniture proto, Tile tile) {
        Furniture obj = new Furniture();

        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbour = proto.linksToNeighbour;

        obj.tile = tile;
        if (tile.PlaceFurniture(obj) == false) {
            return null;
        };
        if (obj.linksToNeighbour) {
            // This type of furniture links inself to its neighbours
            // so we should inform our neighbours that they have a new buudy/
            // Just trigger their callback/

            Tile t;

            int x = obj.tile.X;
            int y = obj.tile.Y;

            t = tile.world.GetTileAt(x, y + 1);
            if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
                t.furniture.cbOnChanged(t.furniture);
            }

            t = tile.world.GetTileAt(x + 1, y);
            if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
                t.furniture.cbOnChanged(t.furniture);
            }

            t = tile.world.GetTileAt(x, y - 1);
            if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
                t.furniture.cbOnChanged(t.furniture);
            }

            t = tile.world.GetTileAt(x - 1, y);
            if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
                t.furniture.cbOnChanged(t.furniture);
            }
        }

        return obj;
    }

    public void RegisterOnChangedCallback(Action<Furniture> callbackFunc) {
        cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback(Action<Furniture> callbackFunc) {
        cbOnChanged -= callbackFunc;
    }
}

