using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class TaskManager : SingletonBehavior<TaskManager> {
  public static TaskRunner Scheduler => Instance._Scheduler;
  TaskRunner _Scheduler = new();

  void FixedUpdate() {
    Scheduler.FixedUpdate();
  }
}

public class TaskRunner : TaskScheduler, IDisposable {
  ConcurrentQueue<Task> Tasks = new ConcurrentQueue<Task>();
  TaskCompletionSource<bool> NextTick = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
  bool ProcessingItems;
  TaskScope MainScope = new();

  public void Dispose() {
    MainScope.Dispose();
  }

  public void FixedUpdate() {
    var old = SynchronizationContext.Current;
    ProcessingItems = true;
    try {
      // This dumb hack is necessary because .NET will ignore the current TaskScheduler if there's a SynchronizationContext
      // (which Unity controls). If it's null, it'll use our current TaskScheduler when awaiting.
      SynchronizationContext.SetSynchronizationContext(null);
      while (Tasks.TryDequeue(out var task)) {
        try {
          TryExecuteTask(task);
        } catch (Exception e) {
          Debug.LogException(e);
        }
      }

      NextTick.TrySetResult(true);
    } finally {
      ProcessingItems = false;
      SynchronizationContext.SetSynchronizationContext(old);
    }
    NextTick = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
  }

  public Task WaitForFixedUpdate() {
    return NextTick.Task;
  }

  public void StartTask(TaskFunc f) {
    MainScope.Start(f, this);
  }
  public void StopAllTasks() {
    MainScope?.Dispose();
    MainScope = new();
  }

  protected override void QueueTask(Task task) {
    Tasks.Enqueue(task);
  }

  protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) {
    // If this thread isn't already processing a task, we don't support inlining
    if (!ProcessingItems) return false;

    // If the task was previously queued, remove it from the queue.
    if (taskWasPreviouslyQueued) {
      Debug.Assert(false, "Matt needs to handle this case apparently");
      //if (Tasks.Remove(task))
      //  return TryExecuteTask(task);
      //else
      return false;
    } else {
      return TryExecuteTask(task);
    }
  }

  protected override IEnumerable<Task> GetScheduledTasks() {
    return Tasks.ToArray();
  }
}