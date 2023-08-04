using UnityEngine;

public class AIChasePlayer : MonoBehaviour {
  public AbilityManager AbilityManager;
  public WorldSpaceMove Move;

  void FixedUpdate() {
    var player = PlayerManager.Instance.Player;
    if (player && AbilityManager.CanRun(Move.Move)) {
      var dir = (player.transform.position - transform.position).normalized;
      AbilityManager.Run(Move.Move, dir);
    }
  }
}