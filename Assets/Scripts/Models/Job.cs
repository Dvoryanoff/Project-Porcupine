using System;
using System.Collections.Generic;

public class Job {

    // This class holds info for a queuned up job, which can include
    // things like placing furniture, moving stored inventory, 
    // working at a desk and maybe even fighting enemies.

    public Tile tile;

    private float jobTime;

    // FIXME:

    public string jobObjectType {
        get; protected set;
    }

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    Dictionary<string, Inventory> inventoryRequirements;

    public Job (Tile tile, string jobObjectType, Action<Job> cbJobComplete, float jobTime, Inventory[] inventoryRequirements) {
        this.tile = tile;
        this.jobObjectType = jobObjectType;
        this.cbJobComplete += cbJobComplete;
        this.jobTime = jobTime;

        this.inventoryRequirements = new Dictionary<string, Inventory> ();
        if (inventoryRequirements != null) {
            foreach (Inventory inventory in inventoryRequirements) {
                this.inventoryRequirements[inventory.objectType] = inventory.Clone ();
            }
        }
    }

    protected Job (Job other) {
        this.tile = other.tile;
        this.jobObjectType = other.jobObjectType;
        this.cbJobComplete = other.cbJobComplete;
        this.jobTime = other.jobTime;

        this.inventoryRequirements = new Dictionary<string, Inventory> ();
        if (inventoryRequirements != null) {
            foreach (Inventory inventory in other.inventoryRequirements.Values) {
                this.inventoryRequirements[inventory.objectType] = inventory.Clone ();
            }
        }
    }

    virtual public Job Clone () {
        return new Job (this);
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

