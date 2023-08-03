using UnityEngine;

public class ContactHitbox : MonoBehaviour {
  public enum Modes { Passive, Active };
  public Combatant Owner;
  public HitConfig HitConfig;

  void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out Hurtbox hb)) {
      hb.ProcessHit(Owner, HitConfig);
    }
  }
}