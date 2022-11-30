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

    Tile currentTile;
    Tile destTile; // If we are not moving then destTile == currentTile
    float movementPercentage; // Gos from 0 to 1.

    public Character(Tile tile) {
        currentTile = destTile = tile;
    }

    public void SetDestination(Tile tile) {
        if (currentTile.IsNeighbour(tile) == false) {
            Debug.LogError("Character :: SetDestination -- Our destination tile isn't actually our neighbour.");
        }

    }
}
