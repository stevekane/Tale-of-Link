using System.Threading.Tasks;
using UnityEngine;

public class ThrowAbility : ClassicAbility {
  public Combatant Combatant => AbilityManager.GetComponent<Combatant>();
  public Projectile Projectile;
  public Vector3 Offset = new(0, 1f, 1f);
  public Timeval Windup;
  public Timeval Recovery;

  public override async Task MainAction(TaskScope scope) {
    await scope.Delay(Windup);
    var target = FindObjectOfType<Player>();
    var dir = (target.transform.position - transform.position).normalized;
    var rotation = Quaternion.LookRotation(dir, Vector3.up);
    Projectile.Fire(Projectile, Combatant, Combatant.transform.position + rotation*Offset, rotation);
    await scope.Delay(Recovery);
  }
}