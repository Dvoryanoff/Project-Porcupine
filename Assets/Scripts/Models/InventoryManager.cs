using System.Collections.Generic;
using UnityEngine;

public class InventoryManager {

    // This is a list of all "live" inventories.
    // Later on this will likely be organized by rooms instead
    // of a single master list. (Or in addition to.)
    public Dictionary<string, List<Inventory>> inventories;

    public InventoryManager () {
        inventories = new Dictionary<string, List<Inventory>> ();
    }

    public bool PlaceInventory (Tile tile, Inventory inv) {

        bool tileWasEmpty = tile.inventory == null; if (tile.PlaceInventory (inv) == false) {
            // The tile did not eccept the inventory for whatever reason, therefore stop!
            return false;
        }

        // At this point, "inv" might be an empty stack if it was merged to another stack.
        if (inv.stackSize == 0) {
            if (inventories.ContainsKey (tile.inventory.objectType)) {
                inventories[inv.objectType].Remove (inv);
            }
        }

        // We may also created a new stack on the tile, if the tile was previously empty.
        if (tileWasEmpty) {
            if (inventories.ContainsKey (tile.inventory.objectType) == false) {
                inventories[tile.inventory.objectType] = new List<Inventory> ();
            }
            inventories[tile.inventory.objectType].Add (tile.inventory);
        }
        return true;
    }

    public bool PlaceInventory (Job job, Inventory inv) {

        if (job.inventoryRequirements.ContainsKey (inv.objectType) == false) {
            Debug.LogError ($"Trying to add invenroty to a job that it doesn't want!");
            return false;
        }

        job.inventoryRequirements[inv.objectType].stackSize += inv.stackSize;

        if (job.inventoryRequirements[inv.objectType].maxStackSize > job.inventoryRequirements[inv.objectType].stackSize) {
            inv.stackSize = job.inventoryRequirements[inv.objectType].stackSize - job.inventoryRequirements[inv.objectType].maxStackSize;
            job.inventoryRequirements[inv.objectType].stackSize = job.inventoryRequirements[inv.objectType].maxStackSize;
        } else {
            inv.stackSize = 0;
        }

        // At this point, "inv" might be an empty stack if it was merged to another stack.
        if (inv.stackSize == 0) {
            if (inventories.ContainsKey (inv.objectType)) {
                inventories[inv.objectType].Remove (inv);
            }
        }

        return true;
    }
}
