using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour {

    FurnitureSpriteController fsc;
    Dictionary<Job, GameObject> jobGameobjectMap;

    void Start () {

        jobGameobjectMap = new Dictionary<Job, GameObject> ();
        fsc = GameObject.FindObjectOfType<FurnitureSpriteController> ();
        WorldController.Instance.world.jobQueue.RegisterJobCreationCallback (OnJobCreated);
    }

    void OnJobCreated (Job job) {

        // FIXME: We can only do furniture-bulding jobs.

        // TODO: Sprite

        if (jobGameobjectMap.ContainsKey (job)) {
            Debug.LogError ($"OnJobCreated for a jobGo that already exists -- most likely a job being RE-QUEUED, as opposed to created");

            return;
        }
        GameObject job_go = new GameObject ();

        jobGameobjectMap.Add (job, job_go);

        job_go.name = $"JOB_{job.jobObjectType}_{job.tile.X}_{job.tile.Y}";
        job_go.transform.position = new Vector3 (job.tile.X, job.tile.Y, 0);
        job_go.transform.SetParent (this.transform, true);

        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer> ();
        sr.sprite = fsc.GetSpriteForFurniture (job.jobObjectType);
        sr.color = new Color (0.5f, 1f, 0.5f, 0.25f);
        sr.sortingLayerName = "Jobs";

        if (job.jobObjectType == "Door") {

            // By default, the door graphic is meant for walls to the east & west
            // Check to see if we actually have a wall north/south, and if so
            // then rotate this GO by 90 degrees
            // 
            Tile northTile = job.tile.world.GetTileAt (job.tile.X, job.tile.Y + 1);
            Tile southTile = job.tile.world.GetTileAt (job.tile.X, job.tile.Y - 1);

            if (northTile != null &&
                southTile != null &&
                northTile.furniture != null &&
                southTile.furniture != null &&
                northTile.furniture.objectType == "Wall" &&
                southTile.furniture.objectType == "Wall") {
                job_go.transform.rotation = Quaternion.Euler (0, 0, 90);
            }
        }

        job_go.GetComponent<SpriteRenderer> ().sortingOrder = 1;

        job.RegisterJobCancelCallback (OnJobEnded);
        job.RegisterJobCompleteCallback (OnJobEnded);
    }

    void OnJobEnded (Job job) {

        // FIXME: We can only do furniture-bulding jobs.

        // TODO: Delete sprite!

        GameObject job_go = jobGameobjectMap[job];

        job.UnRegisterJobCancelCallBack (OnJobEnded);
        job.UnRegisterJobCompleteCallBack (OnJobEnded);

        Destroy (job_go);
    }
}
