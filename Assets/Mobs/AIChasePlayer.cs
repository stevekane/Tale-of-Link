using UnityEngine;

public class AIChasePlayer : MonoBehaviour {
  public AbilityManager AbilityManager;
  public WorldSpaceMove Move;
  //public LayerMask SeeMask;

  // Target is visible if he's on the same floor. Ish.
  Transform Target => PlayerManager.Instance.MobTarget ? PlayerManager.Instance.MobTarget.transform : null;
  bool CanSeeTarget => Target && Mathf.Abs(Target.position.y - transform.position.y) < 2f;

  void FixedUpdate() {
    if (CanSeeTarget && AbilityManager.CanRun(Move.Move)) {
      var dir = (Target.position - transform.position).XZ().normalized;
      AbilityManager.Run(Move.Move, dir);
    } else {
      AbilityManager.Run(Move.Move, Vector3.zero);
    }
  }
}