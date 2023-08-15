using System.Threading.Tasks;
using UnityEngine;

// Ehh not sure if I like this.
public class TaskRunnerComponent : MonoBehaviour {
  TaskRunner Scheduler = new();

  protected virtual void OnDestroy() {
    Scheduler.Dispose();
  }

  protected virtual void FixedUpdate() {
    Scheduler.FixedUpdate();
  }

  public Task WaitForFixedUpdate() {
    return Scheduler.WaitForFixedUpdate();
  }

  public void StopAllTasks() {
    Scheduler.StopAllTasks();
  }

  public void StartTask(TaskFunc f) {
    Scheduler.StartTask(f);
  }
}