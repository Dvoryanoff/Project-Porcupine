using System.Collections.Generic;

public class Path_TileGraph {

    // This class constructs a asimple path-finding compatible graph 
    // on our World.
    // Each Tile is a Node.  Each WACKABLE neighbour from a Tile is Linkedvia an Edge connection.

    Dictionary<Tile, Path_Node<Tile>> nodes;
    public Path_TileGraph (World world) {

        // Loop through all tiles of the world.

        // Create a node for each tile.

        // Do we create nodes for non-floor tiles? NO!
        // Do we create nodes for the tiles that are completely unwalkable (i.e. walls) ? NO!

        nodes = new Dictionary<Tile, Path_Node<Tile>> ();

        for (int x = 0; x < world.Width; x++) {
            for (int y = 0; y < world.Height; y++) {

                Tile t = world.GetTileAt (x, y);

                if (t.movementCost > 0) { // Tiles with movementCost 0 are unwalkable.
                    Path_Node<Tile> n = new Path_Node<Tile> ();
                    n.data = t;
                    nodes.Add (t, n);
                }
            }
        }

        foreach (Tile t in nodes.Keys) {

            // Get a list of neighbours for the tile.
            // if neighbours is walkable, create an edge to the relevant node.

        }

        // Now loop though all nodes again.
        // Create enges for neighbours.

    }
}
