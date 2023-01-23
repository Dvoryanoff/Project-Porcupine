using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour {

    public static WorldController Instance {
        get; protected set;
    }

    public World world {
        get; protected set;
    }
    private void OnEnable () {

        if (Instance != null) {
            Debug.Log ($"There should never be two worlds!!!");
        }
        Instance = this;

        CreateEmptyWorld ();
    }

    private void Update () {

        // TODO: Add pause/unpause, speed control, etc....

        world.Update (Time.deltaTime);
    }

    // Called whenever tile's data get changed.

    public Tile GetTileAtWorldCoord (Vector3 coord) {
        int x = Mathf.FloorToInt (coord.x);
        int y = Mathf.FloorToInt (coord.y);

        return world.GetTileAt (x, y);
    }

    public void NewWorld () {
        Debug.Log ("NewWorld button was clicked!");
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }

    public void SaveWorld () {

    }

    public void LoadWorld () {

    }

    private void CreateEmptyWorld () {
        world = new World ();

        // Center the camera.

        Camera.main.transform.position = new Vector3 (world.Width / 2, world.Height / 2, Camera.main.transform.position.z);

    }
}

