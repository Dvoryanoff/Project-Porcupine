
using System;

public class InstalledObject {
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

    Action<InstalledObject> cbOnChanged;

    protected InstalledObject() {

    }
    static public InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linkToNeighbour = false) {
        InstalledObject obj = new InstalledObject();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linkToNeighbour;

        return obj;
    }
    static public InstalledObject PlaceInstance(InstalledObject proto, Tile tile) {
        InstalledObject obj = new InstalledObject();

        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbour = proto.linksToNeighbour;

        obj.tile = tile;
        if (tile.PlaceObject(obj) == false) {
            return null;
        };

        return obj;
    }

    public void RegisterOnChangedCallback(Action<InstalledObject> callbackFunc) {
        cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback(Action<InstalledObject> callbackFunc) {
        cbOnChanged -= callbackFunc;
    }
}

