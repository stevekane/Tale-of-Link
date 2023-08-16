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
    var delta = hit.Attacker.transform.position - hit.Victim.transform.position;
    Run(-hit.KnockbackStrength * delta.XZ().normalized);
  }

  void OnHit(HitEvent hit) {
    var delta = hit.Attacker.transform.position - hit.Victim.transform.position;
    Run(hit.RecoilStrength * delta.XZ().normalized);
  }

  public void Run(Vector3 v) {
    Velocity = v;
    AbilityManager.Run(Main);
  }

  public override async Task MainAction(TaskScope scope) {
    try {
      Controller.DirectMove = true;
      while (true) {
        if (Velocity.sqrMagnitude < MIN_VELOCITY.Sqr()) break;
        Controller.ScriptVelocity += Velocity;
        Velocity = Velocity * Mathf.Exp(-Time.fixedDeltaTime * DRAG);
        await scope.Tick();
      }
    } finally {
      Controller.DirectMove = false;
    }
  }
}