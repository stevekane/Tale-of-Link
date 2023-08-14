using UnityEngine;

public class ContactHitbox : MonoBehaviour {
  public Combatant Owner;
  public HitConfig HitConfig;
  public bool IsActive = true;

  void OnTriggerEnter(Collider other) {
    if (IsActive && other.gameObject.TryGetComponent(out Hurtbox hb)) {
      hb.ProcessHit(Owner, HitConfig);
    }
  }
}