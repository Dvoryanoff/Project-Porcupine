public class Tile {
    enum TileType {
        Empty,
        Floor
    }

    TileType type = TileType.Empty;

    LooseObject LooseObject;
    InstalledObject InstalledObject;

    World world;

    int x;
    int y;

    public Tile(World world, int x, int y) {
        this.world = world;
        this.x = x;
        this.y = y;

        //rgfdgsdvdsfhtts
    }
}
