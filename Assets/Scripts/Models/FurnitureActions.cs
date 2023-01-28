using UnityEngine;

public class FurnitureActions : MonoBehaviour {
    public static void Door_UpdateAction (Furniture furn, float deltaTime) {
        // Debug.Log ($"FurnitureActions {furn.furnParameters["openness"]}");
        if (furn.furnParameters["is_opening"] >= 1) {
            furn.furnParameters["openness"] += Time.deltaTime;
            if (furn.furnParameters["openness"] >= 1) {
                furn.furnParameters["is_opening"] = 0;
            }
        } else {
            furn.furnParameters["openness"] -= Time.deltaTime;
        }
        furn.furnParameters["openness"] = Mathf.Clamp01 (furn.furnParameters["openness"]);
    }

    public static ENTERABILITY Door_IsEnterable (Furniture furn) {
        Debug.Log ("Door_IsEnterable");

        furn.furnParameters["is_opening"] = 1;

        if (furn.furnParameters["openness"] >= 1) {
            return ENTERABILITY.Yes;
        }
        return ENTERABILITY.Soon;
    }
}

