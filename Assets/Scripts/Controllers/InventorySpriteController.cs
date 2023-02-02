using System.Collections.Generic;
using UnityEngine;

public class InventorySpriteController : MonoBehaviour {

    Dictionary<Inventory, GameObject> inventoryGameobjectMap;
    Dictionary<string, Sprite> inventorySprites;

    World world {
        get {
            return WorldController.Instance.world;
        }
    }
    void Start () {
        LoadSprites ();

        inventoryGameobjectMap = new Dictionary<Inventory, GameObject> ();

        world.RegisterInventoryCreated ( OnInventoryCreated );

        // Check for pre-existing characters, which won't do the callback.
        foreach ( string objectType in world.inventoryManager.inventories.Keys ) {
            foreach ( Inventory inv in world.inventoryManager.inventories[objectType] ) {
                OnInventoryCreated ( inv );
            }
        }

        // c.SetDestination(world.GetTileAt(world.Width / 2 + 5, world.Height / 2));

    }

    private void LoadSprites () {
        inventorySprites = new Dictionary<string, Sprite> ();
        Sprite[] sprites = Resources.LoadAll<Sprite> ("Images/Inventory");
        // Debug.Log ("LOADED RESOURCES:");

        foreach ( Sprite s in sprites ) {
            // Debug.Log (s);
            inventorySprites[s.name] = s;
        }
    }

    public void OnInventoryCreated ( Inventory inv ) {

        Debug.Log ( "OnCharactertCreated" );

        // Create a visual Game Object linked to this data.

        GameObject inv_go = new GameObject ();

        inventoryGameobjectMap.Add ( inv, inv_go );

        inv_go.name = inv.objectType;
        inv_go.transform.position = new Vector3 ( inv.tile.X, inv.tile.Y, 0 );
        inv_go.transform.SetParent ( this.transform, true );

        SpriteRenderer sr = inv_go.AddComponent<SpriteRenderer> ();
        sr.sprite = inventorySprites[inv.objectType];
        sr.sortingLayerName = "Inventory";
        inv_go.GetComponent<SpriteRenderer> ().sortingOrder = 1;

        // FIXME: Add onChange callbacks.
        // character.RegisterOnChangedCallback ( OnCharacterChange );
    }

    private void OnInventoryChanged ( Inventory inv ) {

        // FIXME: Still needs to work!!! End get called!

        // Make sure that furnityre graphics are corrrect.

        if ( inventoryGameobjectMap.ContainsKey ( inv ) == false ) {
            Debug.LogError ( "OnCharacterChanged -- trying to change visuals for character not in our map." );
            return;
        }

        GameObject char_go = inventoryGameobjectMap[inv];

        char_go.transform.position = new Vector3 ( inv.tile.X, inv.tile.Y, 0 );

    }

}
