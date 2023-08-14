using UnityEngine;

public class ContactHitbox : MonoBehaviour {
  public Combatant Owner;
  public HitConfig HitConfig;
  public bool IsActive = true;

  void OnTriggerEnter(Collider other) {
    Debug.Log($"Ran into {other.name}");
    if (IsActive && other.gameObject.TryGetComponent(out Hurtbox hb)) {
      Debug.Log($"Ran into Hurtbox");
      hb.ProcessHit(Owner, HitConfig);
    }
  }
}