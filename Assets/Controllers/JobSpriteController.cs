using UnityEngine;

public class JobSpriteController : MonoBehaviour {

    FurnitureSpriteController fsc;
    void Start() {
        fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();
        WorldController.Instance.world.jobQueue.RegisterJobCreationCallback(OnJobCreated);
    }

    // Update is called once per frame
    void Update() {

    }

    void OnJobCreated(Job job) {

        // FIXME: We can only do furniture-bulding jobs.

        // TODO: Sprite

        Sprite theSprite = fsc.GetSpriteForFurniture();

        job.RegisterJobCancelCallBack(OnJobEnded);
        job.RegisterJobCompleteCallBack(OnJobEnded);
    }

    void OnJobEnded(Job job) {

        // FIXME: We can only do furniture-bulding jobs.

        // TODO: Delete sprite!
    }
}
