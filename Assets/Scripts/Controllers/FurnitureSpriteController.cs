using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpriteController : MonoBehaviour {

    Dictionary<Furniture, GameObject> furnitureGameobjectMap;
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

        furnitureGameobjectMap = new Dictionary<Furniture, GameObject> ();

        world.RegisterFurnitureCreated (OnFurnitureCreated);
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

        furnitureGameobjectMap.Add (furn, furn_go);

        furn_go.name = furn.objectType + "_" + furn.tile.X + "_" + furn.tile.Y;
        furn_go.transform.position = new Vector3 (furn.tile.X, furn.tile.Y, 0);
        furn_go.transform.SetParent (this.transform, true);

        SpriteRenderer sr = furn_go.AddComponent<SpriteRenderer> ();
        sr.sprite = GetSpriteForFurniture (furn);
        sr.sortingLayerName = "Furniture";

        furn.RegisterOnChangedCallback (OnFurnitureChange);
    }

    private void OnFurnitureChange (Furniture furn) {

        // Make sure that furnityre graphics are corrrect.

        if (furnitureGameobjectMap.ContainsKey (furn) == false) {
            Debug.LogError ("OnFurnitureChanged -- trying to change visuals for furniture not in our map.");
            return;
        }

        GameObject furn_go = furnitureGameobjectMap[furn];
        furn_go.GetComponent<SpriteRenderer> ().sprite = GetSpriteForFurniture (furn);

    }

    public Sprite GetSpriteForFurniture (Furniture obj) {
        if (obj.linksToNeighbour == false) {
            return furnitureSprites[obj.objectType];
        }

        string spriteName = $"{obj.objectType}_";

        // Check neighbours for North, East, South, West!

        Tile t;

        int x = obj.tile.X;
        int y = obj.tile.Y;

        t = world.GetTileAt (x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "N";
        }

        t = world.GetTileAt (x + 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "E";
        }

        t = world.GetTileAt (x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "S";
        }

        t = world.GetTileAt (x - 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
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

