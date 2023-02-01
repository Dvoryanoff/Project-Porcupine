using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpriteController : MonoBehaviour {

    Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<string, Sprite> furnitureSprites;

    World world {
        get {
            return WorldController.Instance.world;
        }
    }

    public static WorldController Instance {
        get; protected set;
    }

    private void Start () {

        LoadSprites ();

        // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
        furnitureGameObjectMap = new Dictionary<Furniture, GameObject> ();

        // Register our callback so that our GameObject gets updated whenever
        // the tile's type changes.
        world.RegisterFurnitureCreated (OnFurnitureCreated);

        // Go through any EXISTING furniture (i.e. from a save that was loaded OnEnable) and call the OnCreated event manually
        foreach (Furniture furn in world.furnitures) {
            OnFurnitureCreated (furn);
        }
    }

    private void LoadSprites () {
        furnitureSprites = new Dictionary<string, Sprite> ();
        Sprite[] sprites = Resources.LoadAll<Sprite> ("Images/Furniture");
        // Debug.Log ("LOADED RESOURCES:");

        foreach (Sprite s in sprites) {
            // Debug.Log (s);
            furnitureSprites[s.name] = s;
        }
    }

    public void OnFurnitureCreated (Furniture furn) {

        // Debug.Log ("OnInstalledObjectCreated");

        // Create a visual Game Object linked to this data.

        GameObject furn_go = new GameObject ();

        // FIXME: This hardcoding is not ideal.

        furnitureGameObjectMap.Add (furn, furn_go);

        furn_go.name = furn.objectType + "_" + furn.tile.X + "_" + furn.tile.Y;
        furn_go.transform.position = new Vector3 (furn.tile.X, furn.tile.Y, 0);
        furn_go.transform.SetParent (this.transform, true);

        if (furn.objectType == "Door") {

            // By default, the door graphic is meant for walls to the east & west
            // Check to see if we actually have a wall north/south, and if so
            // then rotate this GO by 90 degrees
            Tile northTile = world.GetTileAt (furn.tile.X, furn.tile.Y + 1);
            Tile southTile = world.GetTileAt (furn.tile.X, furn.tile.Y - 1);
            if (northTile != null &&
            southTile != null &&
            northTile.furniture != null &&
                southTile.furniture != null &&
                northTile.furniture.objectType == "Wall" &&
                southTile.furniture.objectType == "Wall") {
                furn_go.transform.rotation = Quaternion.Euler (0, 0, 90);
                furn_go.transform.Translate (1f, 0, 0, Space.World); // UGLY HACK TO COPMENSATE FOR BOTTOM_LAFT ANCHOR POINT!
            }
        }

        SpriteRenderer sr = furn_go.AddComponent<SpriteRenderer> ();
        sr.sprite = GetSpriteForFurniture (furn);
        sr.sortingLayerName = "Furniture";

        furn.RegisterOnChangedCallback (OnFurnitureChange);
    }

    private void OnFurnitureChange (Furniture furn) {
        // Debug.Log ("OnFurnitureChange");

        // Make sure that furnityre graphics are corrrect.

        if (furnitureGameObjectMap.ContainsKey (furn) == false) {
            Debug.LogError ("OnFurnitureChanged -- trying to change visuals for furniture not in our map.");
            return;
        }

        GameObject furn_go = furnitureGameObjectMap[furn];
        furn_go.GetComponent<SpriteRenderer> ().sprite = GetSpriteForFurniture (furn);
    }

    public Sprite GetSpriteForFurniture (Furniture furn) {

        string spriteName = furn.objectType;

        if (furn.linksToNeighbour == false) {

            // If this is a DOOR, lets chack OPENNESS and update the sprite.
            // FIXME: All this hardcoding needs to be generilized later.

            if (furn.objectType == "Door") {
                if (furn.GetParameter ("openness") < 0.1f) {
                    // Door is closed.
                    spriteName = "Door";
                } else if (furn.GetParameter ("openness") < 0.5f) {
                    // Door is a bit open.
                    spriteName = "Door_openness_1";
                } else if (furn.GetParameter ("openness") < 0.9f) {
                    // Door is a lot open.
                    spriteName = "Door_openness_2";
                } else {
                    // Door is fully open.
                    spriteName = "Door_openness_3";
                }
                // Debug.Log (spriteName);
            }
            return furnitureSprites[spriteName];
        }

        spriteName = $"{furn.objectType}_";

        // Check neighbours for North, East, South, West!

        Tile t;

        int x = furn.tile.X;
        int y = furn.tile.Y;

        t = world.GetTileAt (x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
            spriteName += "N";
        }

        t = world.GetTileAt (x + 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
            spriteName += "E";
        }

        t = world.GetTileAt (x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
            spriteName += "S";
        }

        t = world.GetTileAt (x - 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
            spriteName += "W";
        }

        if (furnitureSprites.ContainsKey (spriteName) == false) {
            Debug.LogError ($"GetSpritesForInstalledObjects: -- No sprites with name: {spriteName}");
            return null;
        }

        return furnitureSprites[spriteName];

    }

    public Sprite GetSpriteForFurniture (string objectType) {
        if (furnitureSprites.ContainsKey (objectType)) {
            return furnitureSprites[objectType];
        }
        if (furnitureSprites.ContainsKey ($"{objectType}_")) {
            return furnitureSprites[$"{objectType}_"];
        }

        Debug.LogError ($"GetSpritesForInstalledObjects: -- No sprites with name: {objectType}");
        return null;
    }
}

