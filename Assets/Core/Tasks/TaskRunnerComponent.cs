using System.Threading.Tasks;
using UnityEngine;

// Ehh not sure if I like this.
[RequireComponent(typeof(LocalTime))]
public class TaskRunnerComponent : MonoBehaviour {
  protected LocalTime LocalTime;
  TaskRunner Scheduler;

  protected virtual void Awake() {
    this.InitComponent(out LocalTime);
    Scheduler = new(LocalTime);
  }

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