// Inventory are things that are lying on the floor/stockpile, like a bunch of metal bars
// or potentially a non-installed copy of furniture (e.g. a cabinet still in the box from Ikea)

public class Inventory {

    public int stackSize = 1;
    public int maxStackSize = 50;
    public string objectType = "Steel Plate";

    public Tile tile;
    public Character character;

    public Inventory () {

    }

    protected Inventory ( Inventory other ) {
        objectType = other.objectType;
        stackSize = other.stackSize;
        maxStackSize = other.maxStackSize;
    }

    public Inventory ( string objectType, int maxStackSize, int stackSize ) {
        this.objectType = objectType;
        this.maxStackSize = maxStackSize;
        this.stackSize = stackSize;
    }

    public virtual Inventory Clone () {
        return new Inventory ( this );
    }
}
