using UnityEngine;

public class AIChasePlayer : MonoBehaviour {
  public AbilityManager AbilityManager;
  public WorldSpaceMove Move;

  void FixedUpdate() {
    var player = PlayerManager.Instance.MobTarget;
    if (player && AbilityManager.CanRun(Move.Move)) {
      var dir = (player.transform.position - transform.position).XZ().normalized;
      AbilityManager.Run(Move.Move, dir);
    }
  }
}