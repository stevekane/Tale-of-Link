using System;
using System.Threading.Tasks;

public abstract class ClassicAbility : Ability {
  int RunningTaskCount;
  TaskScope Scope = new();
  public override bool IsRunning => RunningTaskCount > 0;
  public override void Stop() {
    Tags = default;
    AddedToOwner = default;
    Scope.Dispose();
    Scope = new();
  }
  public AbilityAction Main;

  protected virtual void Awake() {
    Main.Ability = this;
    Main.Listen(FireMain);
  }

  void OnDestroy() {
    Scope?.Dispose();
  }

  void FireMain() {
    Scope.Start(Runner(MainAction));
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