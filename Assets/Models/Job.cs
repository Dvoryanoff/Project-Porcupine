using System;

public class Job {

    // This class holds info for a queuned up job, which can include
    // things like placing furniture, moving stored inventory, 
    // working at a desk and maybe even fighting enemies.

    private Tile tile;
    private float jobTime = 1f;

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;
    public Job(Tile tile, Action<Job> cbJobComplete, Action<Job> cbJobCancel, float jobTime = 1f) {
        this.tile = tile;
        this.cbJobComplete += cbJobComplete;
    }

    public void RegisterJobCompleteCallBack(Action<Job> cb) {

        this.cbJobCancel += cb;
    }
    public void RegisterJobCancelCallBack(Action<Job> cb) {

        this.cbJobCancel += cb;
    }

    public void DoWork(float workTime) {
        jobTime -= workTime;

        if (jobTime <= 0) {
            if (cbJobComplete != null) {
                cbJobComplete(this);
            }
        }
    }

    public void CancelJob() {

        if (cbJobCancel != null) {
            cbJobCancel(this);
        }
    }
}

