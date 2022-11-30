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

    public Character(Tile tile) {
        currentTile = destTile = tile;
    }

    private void Update(float deltaTime) {

        // Are we there yet?
        if (currentTile == destTile) {
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

    }
    public void SetDestination(Tile tile) {
        if (currentTile.IsNeighbour(tile) == false) {
            Debug.Log("Character :: SetDestination -- Our destination tile isn't actually our neighbour.");
        }
        destTile = tile;

    }
}
