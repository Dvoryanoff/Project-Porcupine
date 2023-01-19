using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class Path_TileGraph {

    // This class constructs a asimple path-finding compatible graph 
    // on our World.
    // Each Tile is a Node.  Each WACKABLE neighbour from a Tile is Linkedvia an Edge connection.

    public Dictionary<Tile, Path_Node<Tile>> nodes;
    public Path_TileGraph (World world) {

        Debug.Log ("PathTileGraph");

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

        Debug.Log ($"PathTileGraph: created {nodes.Count} nodes!");

        int edgeCount = 0;

        foreach (Tile t in nodes.Keys) {

            // Get a list of neighbours for the tile.
            Path_Node<Tile> n = nodes[t];

            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>> ();

            Tile[] neighbours = t.GetNeighbours (true);
            // if neighbours is walkable, create an edge to the relevant node.

            for (int i = 0; i < neighbours.Length; i++) {
                if (neighbours[i] != null && neighbours[i].movementCost > 0) {
                    // This neighbour exist and is walkable, co create an edge.

                    Path_Edge<Tile> e = new Path_Edge<Tile> ();
                    e.cost = neighbours[i].movementCost;
                    e.node = nodes[key: neighbours[i]];

                    edges.Add (e);
                    edgeCount++;

                }

                n.edges = edges.ToArray ();

            }

            // Now loop though all nodes again.
            // Create enges for neighbours.

        }
        Debug.Log ($"PathTileGraph: created {edgeCount} edges!");
    }
}

