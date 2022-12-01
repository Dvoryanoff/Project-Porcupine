using System;
using UnityEngine;

public class Character {

    public float X {
        get {
            return Mathf.Lerp(currentTile.X, destTile.X, movementPercentage);
        }
    }
    public float Y {
        get {
            return Mathf.Lerp(currentTile.Y, destTile.Y, movementPercentage);
        }
    }

    public Tile currentTile {
        get; protected set;
    }
    Tile destTile;            // If we are not moving then destTile == currentTile
    float movementPercentage; // Gos from 0 to 1.
    float speed = 2f;         //  TileSpriteController per second;

    Job myJob;

    Action<Character> cbCharacterChanged;
    public Character(Tile tile) {
        currentTile = destTile = tile;
    }

    public void Update(float deltaTime) {

        // Debug.Log("Character Update");

        // Do i have a job?
        if (myJob == null) {
            // Grab a new job
            myJob = currentTile.world.jobQueue.Dequeue();

            if (myJob != null) {

                // We have a job
                destTile = myJob.tile;

                myJob.RegisterJobCompleteCallBack(OnJobEnded);
                myJob.RegisterJobCancelCallBack(OnJobEnded);
            }
        }

        // Are we there yet?
        if (currentTile == destTile) {

            if (myJob != null) {
                myJob.DoWork(deltaTime);
            }
            return;
        }

        // Whats the total distance from A to B?
        float distToTravel = Mathf.Sqrt(Mathf.Pow(currentTile.X - destTile.X, 2) + Mathf.Pow(currentTile.Y - destTile.Y, 2));

        // How much distance can travel this Update?
        float distThisFrame = speed * deltaTime;

        //How much is that in terms of percantage to our destination?
        float percThisFrame = distThisFrame / distToTravel;

        //Ad this to overall percentage travelled.
        movementPercentage += percThisFrame;

        if (movementPercentage >= 1) {

            // We have reached our destination.
            currentTile = destTile;
            movementPercentage = 0;

            // FIXME: Do we really want to retain any overshot movement?
        }

        if (cbCharacterChanged != null) {
            cbCharacterChanged(this);
        }
    }
    public void SetDestination(Tile tile) {
        if (currentTile.IsNeighbour(tile) == false) {
            Debug.Log("Character :: SetDestination -- Our destination tile isn't actually our neighbour.");
        }
        destTile = tile;
    }

    public void RegisteOnChangedCallback(Action<Character> cb) {
        cbCharacterChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<Character> cb) {
        cbCharacterChanged -= cb;
    }

    void OnJobEnded(Job j) {
        if (j != myJob) {
            Debug.LogError("Character being told about job that isn't his. You forgot to Unregister smth.");
            return;
        }

        myJob = null;
    }
}
