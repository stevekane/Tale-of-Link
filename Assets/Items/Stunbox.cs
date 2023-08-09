using System;
using UnityEngine;

public class Stunbox : MonoBehaviour {
  Collider Collider;

  public bool EnableCollision {
    get => Collider.enabled;
    set => Collider.enabled = value;
  }

  void Awake() {
    Collider = GetComponent<Collider>();
  }

  void OnTriggerEnter(Collider c) {
    if (c.TryGetComponent(out Hurtbox hurtee) &&
      hurtee.Owner.TryGetComponent(out AbilityManager abilityManager) &&
      abilityManager.Abilities.Find(a => a is StunMob) is StunMob stunAbility && stunAbility != null) {
      abilityManager.Run(stunAbility.Main);
    }
  }
}