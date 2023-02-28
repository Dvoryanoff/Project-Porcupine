using UnityEngine;

public class BuildModeController : MonoBehaviour {

    TileType buildModelTile = TileType.Floor;
    bool buildModeIsObjects = false;

    string buildModeObjectType;

    public void SetMode_BuildFloor () {
        buildModeIsObjects = false;
        buildModelTile = TileType.Floor;
    }

    public void SetMode_Bulldoze () {
        buildModeIsObjects = false;
        buildModelTile = TileType.Empty;
    }

    public void SetMode_BuildFurniture (string objectType) {
        buildModeIsObjects = true;
        buildModeObjectType = objectType;
    }

    public void DoPathfindingTest () {
        WorldController.Instance.world.SetupPathfindingExample ();
    }

    public void DoBuild (Tile tile) {
        // Tile t = WorldController.Instance.world.GetTileAt(x, y);
        if (buildModeIsObjects == true) {

            // FIXME: This instantly build the furniture.

            // Can we build the furnityre in the selected tile?
            // Run the ValidPlacement function.

            string furnitureType = buildModeObjectType;
            if (WorldController.Instance.world.IsFurniturePlacementValid (furnitureType, tile) &&
                tile.pendingFurnitureJob == null) {
                // This tile position is valid for this furniture!

                // Create a job for it to be build.

                Job j;

                if (WorldController.Instance.world.furnitureJobPrototypes.ContainsKey (furnitureType)) {

                    // Make a clone of a jobProrotype
                    j = WorldController.Instance.world.furnitureJobPrototypes[furnitureType].Clone ();

                    //Assign a correct tile.
                    j.tile = tile;
                } else {
                    Debug.LogError ($"There is no furniture job prototype for '{furnitureType}'.");
                    j = new (tile, furnitureType, FurnitureActions.JobComplete_FurnitureBuilding, 0.1f, null);
                }

                // Job to queue

                WorldController.Instance.world.jobQueue.Enqueue (j);

                // FIXME
                tile.pendingFurnitureJob = j;
                j.RegisterJobCancelCallback ((theJob) => { theJob.tile.pendingFurnitureJob = null; });

            }

        } else {
            tile.Type = buildModelTile;
        }
    }

}
