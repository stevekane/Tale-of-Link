using System;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ClassicAbility : Ability {
  int RunningTaskCount;
  protected TaskRunner TaskRunner;
  protected LocalTime LocalTime;
  public override bool IsRunning => RunningTaskCount > 0;
  public override void Stop() {
    Tags = default;
    AddedToOwner = default;
    TaskRunner.StopAllTasks();
  }
  public AbilityAction Main;

  protected override void Awake() {
    base.Awake();
    AbilityManager.InitComponent(out LocalTime);
    TaskRunner = new(LocalTime);
    Main.Ability = this;
    Main.Listen(FireMain);
  }
  protected override void OnDestroy() {
    base.OnDestroy();
    TaskRunner?.Dispose();
  }
  protected virtual void FixedUpdate() {
    TaskRunner.FixedUpdate();
  }

  void FireMain() {
    TaskRunner.StartTask(Runner(MainAction));
    RunningTaskCount++;
  }

  protected TaskFunc Runner(Func<TaskScope, Task> f) => async scope => {
    try {
      await f(scope);
    } finally {
      RunningTaskCount--;
      if (RunningTaskCount == 0) {
        Stop();
      }
    }
  };

  public abstract Task MainAction(TaskScope scope);
}