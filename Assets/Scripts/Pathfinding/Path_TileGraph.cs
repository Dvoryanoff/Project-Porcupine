using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
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

                //if (t.movementCost > 0) { // Tiles with movementCost 0 are unwalkable.
                Path_Node<Tile> n = new Path_Node<Tile> ();
                n.data = t;
                nodes.Add (t, n);
                //}
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
                if (neighbours[i] != null && neighbours[i].movementCost > 0 && IsClippingCorner (t, neighbours[i]) == false) {
                    // This neighbour exists, is walkable, and doesn't requiring clipping a corner --> so create an edge.

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

    bool IsClippingCorner (Tile curr, Tile neigh) {
        // If the movement from curr to neigh is diagonal (e.g. N-E)
        // Then check to make sure we aren't clipping (e.g. N and E are both walkable)

        int dX = curr.X - neigh.X;
        int dY = curr.Y - neigh.Y;

        if (Mathf.Abs (dX) + Mathf.Abs (dY) == 2) {
            // We are diagonal

            if (curr.world.GetTileAt (curr.X - dX, curr.Y).movementCost == 0) {
                // East or West is unwalkable, therefore this would be a clipped movement.
                return true;
            }

            if (curr.world.GetTileAt (curr.X, curr.Y - dY).movementCost == 0) {
                // North or South is unwalkable, therefore this would be a clipped movement.
                return true;
            }

            // If we reach here, we are diagonal, but not clipping
        }

        // If we are here, we are either not clipping, or not diagonal
        return false;
    }
}

