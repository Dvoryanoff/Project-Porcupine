using System;
using System.Collections.Generic;

public class Job {

    // This class holds info for a queuned up job, which can include
    // things like placing furniture, moving stored inventory, 
    // working at a desk and maybe even fighting enemies.

    public Tile tile {
        get; protected set;
    }

    private float jobTime;

    // FIXME:

    public string jobObjectType {
        get; protected set;
    }

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    public Queue<Job> jobQueue;

    public Job (Tile tile, string jobObjectType, Action<Job> cbJobComplete, float jobTime = 0.1f) {
        this.tile = tile;
        this.jobObjectType = jobObjectType;
        this.cbJobComplete += cbJobComplete;
        this.jobTime = jobTime;
    }

    public void RegisterJobCompleteCallback (Action<Job> cb) {

        this.cbJobComplete += cb;
    }
    public void RegisterJobCancelCallback (Action<Job> cb) {

        this.cbJobCancel += cb;
    }

    public void UnRegisterJobCompleteCallBack (Action<Job> cb) {

        this.cbJobComplete -= cb;
    }
    public void UnRegisterJobCancelCallBack (Action<Job> cb) {

        this.cbJobCancel -= cb;
    }

    public void DoWork (float workTime) {
        jobTime -= workTime;

        if (jobTime <= 0) {
            cbJobComplete?.Invoke (this);
        }
    }

    public void CancelJob () {

        cbJobCancel?.Invoke (this);
    }
}

