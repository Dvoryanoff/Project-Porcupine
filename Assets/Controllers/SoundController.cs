using UnityEngine;

public class SoundController : MonoBehaviour {

    float soundCoolDown = 0f;

    private void Start() {
        WorldController.Instance.World.RegisterFurnitureCreated(OnFurnitureCreated);
        WorldController.Instance.World.RegisterTileChanged(OnTileChanged);

    }

    private void Update() {
        soundCoolDown -= Time.deltaTime;
    }

    public void OnTileChanged(Tile tile_data) {

        if (soundCoolDown > 0) {
            return;
        }

        // FIXME
        AudioClip ac = Resources.Load<AudioClip>("Sounds/Floor_OnCreated");
        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);

        soundCoolDown = 0.1f;
    }

    public void OnFurnitureCreated(Furniture furn) {
        if (soundCoolDown > 0) {
            return;
        }
        // FIXME
        AudioClip ac = Resources.Load<AudioClip>("Sounds/Wall_OnCreated");
        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        soundCoolDown = 0.1f;
    }
}
