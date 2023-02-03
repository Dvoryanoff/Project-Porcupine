using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour {

    public static WorldController Instance {
        get; protected set;
    }

    public World world {
        get; protected set;
    }

    static bool loadWorld = false;

    private void OnEnable () {

        if ( Instance != null ) {
            Debug.Log ( $"There should never be two worlds!!!" );
        }
        Instance = this;

        if ( loadWorld ) {
            loadWorld = false;
            CreateWorldFromSaveFile ();
        } else {
            CreateEmptyWorld ();
        }
    }

    private void Update () {

        // TODO: Add pause/unpause, speed control, etc....

        world.Update ( Time.deltaTime );
    }

    // Called whenever tile's data get changed.

    public Tile GetTileAtWorldCoord ( Vector3 coord ) {
        int x = Mathf.FloorToInt (coord.x + 0.5f);
        int y = Mathf.FloorToInt (coord.y + 0.5f);

        return world.GetTileAt ( x, y );
    }

    public void NewWorld () {
        Debug.Log ( "NewWorld button was clicked!" );
        SceneManager.LoadScene ( SceneManager.GetActiveScene ().name );
    }

    public void SaveWorld () {
        Debug.Log ( "SaveWorld button was clicked!" );
        XmlSerializer serializer = new XmlSerializer (typeof (World));
        TextWriter writer = new StringWriter ();

        serializer.Serialize ( writer, world );
        writer.Close ();

        Debug.Log ( writer.ToString () );

        PlayerPrefs.SetString ( "SaveGame00", writer.ToString () );
    }

    public void LoadWorld () {

        // Reload the scene te resrt all data!
        Debug.Log ( "LoadWorld button was clicked!" );
        loadWorld = true;
        SceneManager.LoadScene ( SceneManager.GetActiveScene ().name );
    }

    private void CreateEmptyWorld () {

        // Create a world with empty tiles.
        world = new World ( 100, 100 );

        // Center the camera.

        Camera.main.transform.position = new Vector3 ( world.Width / 2, world.Height / 2, Camera.main.transform.position.z );

    }

    private void CreateWorldFromSaveFile () {
        Debug.Log ( "CreateWorldFromSaveFile!" );

        // Create a world from save file data.

        XmlSerializer serializer = new XmlSerializer (typeof (World));
        TextReader reader = new StringReader (PlayerPrefs.GetString ("SaveGame00"));

        world = (World) serializer.Deserialize ( reader );
        reader.Close ();

        // Center the camera.

        Camera.main.transform.position = new Vector3 ( world.Width / 2, world.Height / 2, Camera.main.transform.position.z );

    }

}