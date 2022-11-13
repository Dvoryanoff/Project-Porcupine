using UnityEngine;

public class WorldController : MonoBehaviour {

    public static WorldController Instance {
        get; protected set;
    }

    public World World {
        get; protected set;
    }
    private void OnEnable() {

        if (Instance != null) {
            Debug.Log($"There should never be two worlds!!!");
        }
        Instance = this;

        World = new World();

        // Center the camera.

        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
    }

    // Called whenever tile's data get changed.

    public Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return World.GetTileAt(x, y);
    }
}

