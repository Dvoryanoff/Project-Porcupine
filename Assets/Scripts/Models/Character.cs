using System;
using UnityEngine;

public class Character {

    public float X {
        get {
            return Mathf.Lerp (currTile.X, nextTile.X, movementPercentage);
        }
    }
    public float Y {
        get {
            return Mathf.Lerp (currTile.Y, nextTile.Y, movementPercentage);
        }
    }

    public Tile currTile {
        get; protected set;
    }
    Tile destTile;            // If we are not moving then destTile == currentTile.
    Tile nextTile;            // Next tile in the pathfinding sequence.
    Path_AStar pathAStar;

    float movementPercentage; // Gos from 0 to 1.
    float speed = 5f;         //  TileSpriteController per second;

    Action<Character> cbCharacterChanged;
    Job myJob;
    public Character (Tile tile) {
        currTile = destTile = nextTile = tile;
    }

    public void Update_DoJob (float deltaTime) {
        // Debug.Log("Character Update");

        // Do i have a job?
        if (myJob == null) {
            // Grab a new job
            myJob = currTile.world.jobQueue.Dequeue ();

            if (myJob != null) {

                // We have a job

                // TODO: Check to see if the job is reachable? 
                destTile = myJob.tile;

                myJob.RegisterJobCompleteCallback (OnJobEnded);
                myJob.RegisterJobCancelCallback (OnJobEnded);
            }
        }

        // Are we there yet?
        if (myJob != null && currTile == myJob.tile) {

            if (myJob != null) {
                myJob.DoWork (deltaTime);
            }
            // Tell the parent Update function that it should stop.
        }
        // Tell the parent Update function that it should continue to execute.
    }

    public void AbandonJob () {
        nextTile = destTile = currTile;
        pathAStar = null;
        currTile.world.jobQueue.Enqueue (myJob);
        myJob = null;
    }

    public void Update_DoMovement (float deltaTime) {

        if (currTile == destTile) {
            return; // We're already where we want to be.
        }

        if (nextTile == null || nextTile == currTile) {

            // Get the next tile from the pathfinding system.

            if (pathAStar == null || pathAStar.Length () == 0) {
                pathAStar = new Path_AStar (currTile.world, currTile, destTile); // This will calculate path from curr to dest.
                if (pathAStar.Length () == 0) {
                    Debug.LogError ($"Path_AStar returned no path to destination!");
                    // FIXME: Job should maybe be re-enqued instead?
                    AbandonJob ();
                    pathAStar = null;
                    return;
                }
            }
            // Grab the next waypoint from the pathing system!

            nextTile = pathAStar.Dequeue ();

            if (nextTile == currTile) {
                Debug.LogError ("Update_DoMovement - nextTile is currTile?");
            }
        }

        // Whats the total distance from A to B?
        float distToTravel = Mathf.Sqrt (
            Mathf.Pow (currTile.X - nextTile.X, 2) +
            Mathf.Pow (currTile.Y - nextTile.Y, 2));

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

            currTile = nextTile;
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
        if (currTile.IsNeighbour (tile, true) == false) {
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
