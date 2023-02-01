using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MouseOverFurnitureTypeText : MonoBehaviour {
    // Every frame, this script checks to see which tile
    // is under the mouse and then updates the GetComponent<Text>.text
    // parameter of the object it is attached to.

    TMP_Text myText;
    MouseController mouseController;

    void Start () {
        myText = GetComponent<TMP_Text> ();
        Debug.Log ($"{myText.text} : {myText.text}");
        if (myText == null) {
            Debug.LogError ("MouseOverTileTypeText: No 'Text' UI component on this object.");
            this.enabled = false;
            return;
        }
        mouseController = GameObject.FindObjectOfType<MouseController> ();
        if (mouseController == null) {
            Debug.LogError ("How do we not have an instance of mouse controller?");
            return;
        }
    }

    void Update () {
        Tile t = mouseController.GetMouseOverTile ();
        string s = "NULL";

        if (t.furniture != null) {
            s = t.furniture.objectType;
        }
        myText.text = $"Furniture: {s}";
    }
}
