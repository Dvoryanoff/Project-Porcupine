using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum TileType { Empty, Floor };
public class Tile {

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
            (diagOkay && (Mathf.Abs (this.X - tile.X) == 1 && Mathf.Abs (this.Y - tile.Y) == 1)) // Check diag adjacency
            ;

        /*		// First, are we on the same X column?  If so, see if we differ
				// in our Y by exactly one.
				if(this.X == tile.X && ( Mathf.Abs( this.Y - tile.Y ) == 1 ) )
					return true;

				// Now check on the same Y row...
				if(this.Y == tile.Y && ( Mathf.Abs( this.X - tile.X ) == 1 ) )
					return true;

				if(diagOkay) {
					if(this.X == tile.X+1 && (this.Y == tile.Y+1 || this.Y == tile.Y-1 ))
						return true;
					if(this.X == tile.X-1 && (this.Y == tile.Y+1 || this.Y == tile.Y-1 ))
						return true;
				}

				return false;
		*/
    }

}
