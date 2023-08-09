using System.Threading.Tasks;
using UnityEngine;

public class Knockback : ClassicAbility {
  const float MIN_VELOCITY = 2f;
  const float DRAG = 10f;
  Combatant Combatant;
  WorldSpaceController Controller;

  Vector3 Velocity;

  void Start() {
    AbilityManager.InitComponent(out Combatant);
    AbilityManager.InitComponent(out Controller);
    Combatant.OnHit += OnHit;
    Combatant.OnHurt += OnHurt;
  }

  void OnHurt(HitEvent hit) {
    if (!hit.Attacker.GetComponent<AbilityManager>()) return;
    var delta = hit.Attacker.transform.position - hit.Victim.transform.position;
    Velocity = -hit.HitConfig.KnockbackStrength * delta.XZ().normalized;
    AbilityManager.Run(Main);
  }

  void OnHit(HitEvent hit) {
    // Hack so that recoil doesn't happen for switches.
    if (!hit.Victim.GetComponent<AbilityManager>()) return;
    var delta = hit.Attacker.transform.position - hit.Victim.transform.position;
    Velocity = hit.HitConfig.RecoilStrength * delta.XZ().normalized;
    AbilityManager.Run(Main);
  }

  public override async Task MainAction(TaskScope scope) {
    try {
      Controller.DirectMove = true;
      while (true) {
        if (Velocity.sqrMagnitude < MIN_VELOCITY.Sqr()) break;
        //Controller.PhysicsVelocity += Velocity;
        Controller.Position += Time.fixedDeltaTime * Velocity;
        Velocity = Velocity * Mathf.Exp(-Time.fixedDeltaTime * DRAG);
        await scope.Tick();
      }
    } finally {
      Controller.DirectMove = false;
    }
  }
}