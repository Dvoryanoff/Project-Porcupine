public class InstalledObject {
    Tile tile;
    string objectType;
    float movementCost = 1f;
    int width;
    int height;
    public InstalledObject(string objectType, float movementCost, int width = 1, int height = 1) {
        this.objectType = objectType;
        this.movementCost = movementCost;
        this.width = width;
        this.height = height;
    }
    public InstalledObject(InstalledObject proto, Tile tile) {
        this.objectType = proto.objectType;
        this.movementCost = proto.movementCost;
        this.width = proto.width;
        this.height = proto.height;
        this.tile = tile;

        tile.installedObject = this;
    }
}
