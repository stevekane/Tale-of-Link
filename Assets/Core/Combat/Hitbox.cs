using System;
using UnityEngine;

[Serializable]
public class HitConfig {
  public enum Types { Sword, Hammer, OtherThingy };
  public Types HitType;
  public int MaybeDamageOrSomething;
}

public class HitEvent {
  public HitConfig HitConfig;
  public Combatant Attacker;
  public Combatant Victim;
}

public class Hitbox : MonoBehaviour {
  public Combatant Owner;
  public HitConfig HitConfig;
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
      var hit = new HitEvent { HitConfig = HitConfig, Attacker = Owner, Victim = hurtee.Owner };
      hit.Attacker.HandleHit(hit);
      hit.Victim.HandleHurt(hit);
    }
  }
}