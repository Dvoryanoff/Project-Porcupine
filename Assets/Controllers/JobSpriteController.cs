using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour {

    FurnitureSpriteController fsc;
    Dictionary<Job, GameObject> jobGameobjectMap;

    void Start() {

        jobGameobjectMap = new Dictionary<Job, GameObject>();
        fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();
        WorldController.Instance.world.jobQueue.RegisterJobCreationCallback(OnJobCreated);
    }

    // Update is called once per frame
    void Update() {

    }

    void OnJobCreated(Job job) {

        // FIXME: We can only do furniture-bulding jobs.

        // TODO: Sprite

        GameObject job_go = new GameObject();

        jobGameobjectMap.Add(job, job_go);

        job_go.name = $"JOB_{job.jobObjectType}_{job.tile.X}_{job.tile.Y}";
        job_go.transform.position = new Vector3(job.tile.X, job.tile.Y, 0);
        job_go.transform.SetParent(this.transform, true);

        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
        sr.sprite = fsc.GetSpriteForFurniture(job.jobObjectType);
        sr.color = new Color(0.5f, 1f, 0.5f, 0.25f);

        job_go.GetComponent<SpriteRenderer>().sortingOrder = 1;

        job.RegisterJobCancelCallBack(OnJobEnded);
        job.RegisterJobCompleteCallBack(OnJobEnded);
    }

    void OnJobEnded(Job job) {

        // FIXME: We can only do furniture-bulding jobs.

        // TODO: Delete sprite!

        GameObject job_go = jobGameobjectMap[job];

        job.UnRegisterJobCancelCallBack(OnJobEnded);
        job.UnRegisterJobCompleteCallBack(OnJobEnded);

        Destroy(job_go);
    }
}
