using UnityEngine;

public class AIWander : MonoBehaviour {
  public AbilityManager AbilityManager;
  public WorldSpaceMove Move;
  public Timeval ChooseTargetCooldown;

  Vector3 TargetPos;
  int ChooseTicksRemaining = 0;
  void FixedUpdate() {
    if (--ChooseTicksRemaining < 0) {
      TargetPos = transform.position + 10 * UnityEngine.Random.onUnitSphere.XZ();
      ChooseTicksRemaining = ChooseTargetCooldown.Ticks;
    }
    if (AbilityManager.CanRun(Move.Move)) {
      var dir = (TargetPos - transform.position).normalized;
      AbilityManager.Run(Move.Move, dir);
    }
  }
}