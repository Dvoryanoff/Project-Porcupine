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
    void Start() {
        LoadSprites();

        characterGameobjectMap = new Dictionary<Character, GameObject>();

        world.RegisterCharacterCreated(OnCharacterCreated);

        // DEBUG
        world.CreateCharacter(world.GetTileAt(world.Width / 2, world.Height / 2));

    }

    private void LoadSprites() {
        characterSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters");
        Debug.Log("LOADED RESOURCES:");

        foreach (Sprite s in sprites) {
            Debug.Log(s);
            characterSprites[s.name] = s;
        }
    }

    public void OnCharacterCreated(Character character) {

        Debug.Log("OnCharactertCreated");

        // Create a visual Game Object linked to this data.

        GameObject char_go = new GameObject();

        characterGameobjectMap.Add(character, char_go);

        char_go.name = "Character";
        char_go.transform.position = new Vector3(character.currentTile.X, character.currentTile.Y, 0);
        char_go.transform.SetParent(this.transform, true);

        SpriteRenderer sr = char_go.AddComponent<SpriteRenderer>();
        sr.sprite = characterSprites["p1_front"];
        sr.sortingLayerName = "Characters";
        char_go.GetComponent<SpriteRenderer>().sortingOrder = 1;

        // character.RegisterOnChangedCallback(OnFurnitureChange);
    }

    // private void OnFurnitureChange(Furniture furn) {
    // 
    //     // Make sure that furnityre graphics are corrrect.
    // 
    //     if (characterGameobjectMap.ContainsKey(furn) == false) {
    //         return;
    //     }
    // 
    //     GameObject furn_go = characterGameobjectMap[furn];
    //     furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
    // 
    // }

}
