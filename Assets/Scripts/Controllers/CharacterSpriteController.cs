using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour {

    Dictionary<Character, GameObject> characterGameobjectMap;
    Dictionary<string, Sprite> characterSprites;

    World world {
        get {
            return WorldController.Instance.world;
        }
    }
    void Start () {
        LoadSprites ();

        characterGameobjectMap = new Dictionary<Character, GameObject> ();

        world.RegisterCharacterCreated (OnCharacterCreated);

        // Check for pre-existing characters, which won't do the callback.
        foreach (Character c in world.characters) {
            OnCharacterCreated (c);

        }

        // c.SetDestination(world.GetTileAt(world.Width / 2 + 5, world.Height / 2));

    }

    private void LoadSprites () {
        characterSprites = new Dictionary<string, Sprite> ();
        Sprite[] sprites = Resources.LoadAll<Sprite> ("Images/Characters");
        // Debug.Log ("LOADED RESOURCES:");

        foreach (Sprite s in sprites) {
            // Debug.Log (s);
            characterSprites[s.name] = s;
        }
    }

    public void OnCharacterCreated (Character character) {

        Debug.Log ("OnCharactertCreated");

        // Create a visual Game Object linked to this data.

        GameObject char_go = new GameObject ();

        characterGameobjectMap.Add (character, char_go);

        char_go.name = "Character";
        char_go.transform.position = new Vector3 (character.X, character.Y, 0);
        char_go.transform.SetParent (this.transform, true);

        SpriteRenderer sr = char_go.AddComponent<SpriteRenderer> ();
        sr.sprite = characterSprites["p1_front"];
        sr.sortingLayerName = "Characters";
        char_go.GetComponent<SpriteRenderer> ().sortingOrder = 1;

        character.RegisterOnChangedCallback (OnCharacterChange);
    }

    private void OnCharacterChange (Character character) {

        // Make sure that furnityre graphics are corrrect.

        if (characterGameobjectMap.ContainsKey (character) == false) {
            Debug.LogError ("OnCharacterChanged -- trying to change visuals for character not in our map.");
            return;
        }

        GameObject char_go = characterGameobjectMap[character];

        char_go.transform.position = new Vector3 (character.X, character.Y, 0);

    }

}
