using UnityEngine;

public class ContactHitbox : MonoBehaviour {
  public enum Modes { Passive, Active };
  public Combatant Owner;
  public HitConfig HitConfig;

  void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out Hurtbox hb)) {
      var hitEvent = new HitEvent { Attacker = Owner, Victim = hb.Owner, HitConfig = HitConfig };
      Owner.HandleHit(hitEvent);
      hb.Owner.HandleHurt(hitEvent);
    }
  }
}