using System;
using UnityEngine;

public class Character {

    public float X {
        get {
            return Mathf.Lerp (currentTile.X, nextTile.X, movementPercentage);
        }
    }
    public float Y {
        get {
            return Mathf.Lerp (currentTile.Y, nextTile.Y, movementPercentage);
        }
    }

    public Tile currentTile {
        get; protected set;
    }
    Tile destTile;            // If we are not moving then destTile == currentTile.
    Tile nextTile;            // Next tile in the pathfinding sequence.
    Path_AStar pathAStar;

    float movementPercentage; // Gos from 0 to 1.
    float speed = 2f;         //  TileSpriteController per second;

    Action<Character> cbCharacterChanged;
    Job myJob;
    public Character (Tile tile) {
        currentTile = destTile = nextTile = tile;
    }

    public void Update_DoJob (float deltaTime) {
        // Debug.Log("Character Update");

        // Do i have a job?
        if (myJob == null) {
            // Grab a new job
            myJob = currentTile.world.jobQueue.Dequeue ();

            if (myJob != null) {

                // We have a job
                destTile = myJob.tile;

                myJob.RegisterJobCompleteCallback (OnJobEnded);
                myJob.RegisterJobCancelCallback (OnJobEnded);
            }
        }

        // Are we there yet?
        if (currentTile == destTile) {

            if (myJob != null) {
                myJob.DoWork (deltaTime);
            }
            // Tell the parent Update function that it should stop.
        }
        // Tell the parent Update function that it should continue to execute.
    }

    public void Update_DoMovement (float deltaTime) {

        if (currentTile == destTile) {
            return; // We're already where we want to be.
        }

        if (nextTile == null || nextTile == currentTile) {

            // Get the next tile from the pathfinding system.

            if (pathAStar == null || pathAStar.Length () == 0) {
                pathAStar = new Path_AStar (currentTile.world, currentTile, destTile); // This will calculate path from curr to dest.
                if (pathAStar.Length () == 0) {
                    Debug.LogError ($"Path_AStar returned no path to destination!");
                    // FIXME: Job should maybe be re-enqued instead?
                    myJob.CancelJob ();
                    pathAStar = null;
                    return;
                }
            }
            // Grab the next waypoint from the pathing system!

            nextTile = pathAStar.Dequeue ();

            // if (nextTile == currentTile) {
            //     Debug.LogError ("Update_DoMovement - nextTile is currTile?");
            // }
        }

        // Whats the total distance from A to B?
        float distToTravel = Mathf.Sqrt (
            Mathf.Pow (currentTile.X - destTile.X, 2) +
            Mathf.Pow (currentTile.Y - destTile.Y, 2));

        // How much distance can travel this Update?
        float distThisFrame = speed * deltaTime;

        //How much is that in terms of percantage to our destination?
        float percThisFrame = distThisFrame / distToTravel;

        //Ad this to overall percentage travelled.
        movementPercentage += percThisFrame;

        if (movementPercentage >= 1) {

            // We have reached our destination.

            // TODO: Get the next tile from the pathfinding system.
            //       If the are no more tiles, we have TRULY reached the destination.

            currentTile = nextTile;
            movementPercentage = 0;

            // FIXME: Do we really want to retain any overshot movement?

        }

    }

    public void Update (float deltaTime) {

        Update_DoJob (deltaTime);
        Update_DoMovement (deltaTime);

        if (cbCharacterChanged != null) {
            cbCharacterChanged (this);
        }

    }
    public void SetDestination (Tile tile) {
        if (currentTile.IsNeighbour (tile) == false) {
            Debug.Log ("Character :: SetDestination -- Our destination tile isn't actually our neighbour.");
        }
        destTile = tile;
    }

    public void RegisterOnChangedCallback (Action<Character> cb) {
        cbCharacterChanged += cb;
    }

    public void UnregisterOnChangedCallback (Action<Character> cb) {
        cbCharacterChanged -= cb;
    }

    void OnJobEnded (Job j) {
        if (j != myJob) {
            Debug.LogError ("Character being told about job that isn't his. You forgot to Unregister smth.");
            return;
        }

        myJob = null;
    }

}
