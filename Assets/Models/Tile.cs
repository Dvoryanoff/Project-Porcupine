public class Tile {
    public enum TileType {
        Empty,
        Floor
    }

    public TileType type = TileType.Empty;

    public TileType Type {
        get {
            return type;
        }
        set {
            type = value;
        }
    }

    LooseObject LooseObject;
    InstalledObject InstalledObject;

    World world;

    int x;
    int y;

    public Tile(World world, int x, int y) {
        this.world = world;
        this.x = x;
        this.y = y;
    }
}
