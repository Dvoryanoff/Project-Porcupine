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

        GameObject job_go = new GameObject ();

        if (jobGameobjectMap.ContainsKey (job)) {
            // Debug.LogError ($"OnJobCreated for a jobGo that already exists -- most likely a job being RE-QUEUED, as opposed to created");

            return;
        }

        jobGameobjectMap.Add (job, job_go);

        job_go.name = $"JOB_{job.jobObjectType}_{job.tile.X}_{job.tile.Y}";
        job_go.transform.position = new Vector3 (job.tile.X, job.tile.Y, 0);
        job_go.transform.SetParent (this.transform, true);

        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer> ();
        sr.sprite = fsc.GetSpriteForFurniture (job.jobObjectType);
        sr.color = new Color (0.5f, 1f, 0.5f, 0.25f);
        sr.sortingLayerName = "Jobs";

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
