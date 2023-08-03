using UnityEngine;

public class Hurtbox : MonoBehaviour {
  public Combatant Owner;

  void Awake() {
    Owner = Owner ?? GetComponentInParent<Combatant>();
  }

  public void ProcessHit(Combatant attacker, HitConfig hitConfig) {
    var hit = new HitEvent { HitConfig = hitConfig, Attacker = attacker, Victim = Owner };
    hit.Attacker.HandleHit(hit);
    hit.Victim.HandleHurt(hit);
  }
}