using System;
using System.Collections.Generic;

public class JobQueue {

    protected Queue<Job> jobQueue;

    Action<Job> cbJobCreated;

    public JobQueue() {
        jobQueue = new Queue<Job>();
    }

    public void Enqueue(Job job) {
        jobQueue.Enqueue(job);

        if (cbJobCreated != null) {
            cbJobCreated(job);
        }

    }

    public void RegisterJobCreationCallback(Action<Job> callback) {
        cbJobCreated += callback;
    }

    public void UnRegisterJobCreationCallback(Action<Job> callback) {
        cbJobCreated -= callback;
    }

}
