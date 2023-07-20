using UnityEngine;

public class Hitbox : MonoBehaviour {
  public Combatant Owner;
  Collider Collider;

  public bool EnableCollision {
    get => Collider.enabled;
    set => Collider.enabled = value;
  }

  void Awake() {
    Collider = GetComponent<Collider>();
    Owner = Owner ?? GetComponentInParent<Combatant>();
  }

  void OnTriggerEnter(Collider c) {
    if (c.TryGetComponent(out Hurtbox hurtee)) {
      var victim = hurtee.Owner;
      Owner.HandleHit(victim);
      victim.HandleHurt(Owner);
    }
  }
}