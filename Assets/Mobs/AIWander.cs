using KinematicCharacterController;
using UnityEngine;

public class AIWander : TaskRunnerComponent {
  public WorldSpaceController Controller;
  public AbilityManager AbilityManager;
  public WorldSpaceMove Move;
  public Timeval ChooseTargetCooldown;

  void Start() {
    Controller.OnCollision += OnCollision;
    Controller.OnLedge += OnCollision;
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    Controller.OnCollision -= OnCollision;
    Controller.OnLedge -= OnCollision;
  }

  Vector3 TargetDir;
  int ChooseTicksRemaining = 0;
  protected override void FixedUpdate() {
    base.FixedUpdate();
    ChooseTicksRemaining -= (int)LocalTime.TimeScale;
    if (ChooseTicksRemaining < 0) {
      TargetDir = Random.onUnitSphere.XZ().normalized;
      ChooseTicksRemaining = ChooseTargetCooldown.Ticks;
    }
    if (AbilityManager.CanRun(Move.Move)) {
      AbilityManager.Run(Move.Move, TargetDir);
    }
  }

  void OnCollision(HitStabilityReport obj) {
    ChooseTicksRemaining = 0;
  }
}