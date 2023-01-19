using Priority_Queue;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Path_AStar {

    float diagDist = Mathf.Sqrt (2);

    Queue<Tile> path;
    public Path_AStar (World world, Tile tileStart, Tile tileEnd) {

        // Check to see if we have a valid tileGraph.
        if (world.tileGraph == null) {
            world.tileGraph = new Path_TileGraph (world);
        }

        Dictionary<Tile, Path_Node<Tile>> nodes = world.tileGraph.nodes;

        // Dictionary of all valid, walckable nodes.
        if (nodes.ContainsKey (tileStart) == false) {
            Debug.LogError ($"Path_AStar: The starting tile isn't in the list of nodes!");
            return;
        }

        // Make sure that our start/end tiles are in the list of nodes.
        if (nodes.ContainsKey (tileEnd) == false) {
            Debug.LogError ($"Path_AStar: The ending tile isn't in the list of nodes!");
            return;
        }

        Path_Node<Tile> start = nodes[tileStart];
        Path_Node<Tile> goal = nodes[tileEnd];
        // Mostly following this pseudocode:
        // https://en.wikipedia.org/wiki/A*_search_algorithm

        List<Path_Node<Tile>> ClosedSet = new List<Path_Node<Tile>> ();

        //List<Path_Node<Tile>> OpenSet = new List<Path_Node<Tile>> ();

        //OpenSet.Add (start);
        SimplePriorityQueue<Path_Node<Tile>> OpenSet = new SimplePriorityQueue<Path_Node<Tile>> ();
        OpenSet.Enqueue (start, 0);

        Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From = new Dictionary<Path_Node<Tile>, Path_Node<Tile>> ();

        Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float> ();
        foreach (Path_Node<Tile> n in nodes.Values) {
            g_score[n] = Mathf.Infinity;
            g_score[start] = 0;
        }

        Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float> ();
        foreach (Path_Node<Tile> n in nodes.Values) {
            f_score[n] = Mathf.Infinity;
            f_score[start] = HeuristicCostEstimate (start, goal);
        }

        while (OpenSet.Count > 0) {
            Path_Node<Tile> current = OpenSet.Dequeue ();

            if (current == goal) {

                // We have reached our goal!
                // Let's convert this into an actual sequene of
                // tiles to walk on, then end this constructor function!

                ReconstructPath (Came_From, current);
                return;
            }

            ClosedSet.Add (current);

            foreach (Path_Edge<Tile> edge_neighbour in current.edges) {

                Path_Node<Tile> neighbour = edge_neighbour.node;

                if (ClosedSet.Contains (neighbour) == true) {
                    continue; // ignore this already complrtrd neighbour.
                }

                float tentative_g_score = g_score[current] + DistBetween (current, edge_neighbour.node);

                if (OpenSet.Contains (neighbour) && tentative_g_score >= g_score[neighbour])
                    continue;

                Came_From[neighbour] = current;
                g_score[neighbour] = tentative_g_score;
                f_score[neighbour] = g_score[neighbour] + HeuristicCostEstimate (neighbour, goal);

                if (OpenSet.Contains (neighbour) == false) {
                    OpenSet.Enqueue (neighbour, f_score[neighbour]);
                }
            } // for each neighbour
        }// while 

        // If we reached here, it means that we've burned through the entire
        // OpenSet without ever reaching a point where current == goal.
        // This happens when there is no path from start to goal
        // (so there's a wall or missing floor or something).

        // We don't have a failure state, maybe? It's just that the
        // path list will be null.
    }

    float HeuristicCostEstimate (Path_Node<Tile> start, Path_Node<Tile> goal) {
        return Mathf.Sqrt (
            Mathf.Pow (start.data.X - goal.data.X, 2) +
            Mathf.Pow (start.data.Y - goal.data.Y, 2)
            );
    }

    float DistBetween (Path_Node<Tile> a, Path_Node<Tile> b) {

        if (Mathf.Abs (a.data.X - b.data.X) + Mathf.Abs (a.data.Y - b.data.Y) == 1) {
            return 1f;
        }

        if (Mathf.Abs (a.data.X - b.data.X) == 1 && Mathf.Abs (a.data.Y - b.data.Y) == 1) {
            return diagDist;
        }

        return Mathf.Sqrt (
            Mathf.Pow (a.data.X - b.data.X, 2) +
            Mathf.Pow (a.data.Y - b.data.Y, 2)
            );

    }

    void ReconstructPath (
    Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From,
    Path_Node<Tile> current
) {
        // So at this point, current IS the goal.
        // So what we want to do is walk backwards through the Came_From
        // map, until we reach the "end" of that map...which will be
        // our starting node!
        Queue<Tile> total_path = new ();
        total_path.Enqueue (current.data); // This "final" step is the path is the goal!

        while (Came_From.ContainsKey (current)) {
            // Came_From is a map, where the
            //    key => value relation is real saying
            //    some_node => we_got_there_from_this_node

            current = Came_From[current];
            total_path.Enqueue (current.data);
        }

        // At this point, total_path is a queue that is running
        // backwards from the END tile to the START tile, so let's reverse it.

        path = new Queue<Tile> (total_path.Reverse ());

    }

    public Tile Dequeue () {
        return path.Dequeue ();
    }

    public int Length () {
        if (path == null) {
            return 0;
        }
        return path.Count;
    }
}
