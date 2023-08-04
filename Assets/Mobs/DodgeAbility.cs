using System.Threading.Tasks;
using UnityEngine;

public class DodgeAbility : ClassicAbility {
  public WorldSpaceController Controller;
  public Timeval Windup;
  public Timeval Recovery;
  public float JumpForce = 20f;

  public override async Task MainAction(TaskScope scope) {
    await scope.Delay(Windup);
    var player = PlayerManager.Instance.Player;
    var dir = (player.transform.position - transform.position).normalized;
    Controller.PhysicsVelocity += -JumpForce * dir;
    await scope.Delay(Recovery);
  }
}