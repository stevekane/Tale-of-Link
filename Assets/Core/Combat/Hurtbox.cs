using UnityEngine;

/*
Steve

Both ShieldAbility and InstantDeathFromHammer are shoved in here for convenience of this game.
In general, these things should be handed off to some kind of damage processing systems which
then decide the outcome of this attack given the Attack, Attacker and Victim.

These could be subclasses of HurtBox or they could be a general system that is paramaterized
by a stack of modifiers which act on the incoming HitConfig and return the processed hitconfig
before passing it along to attacker and victim.
*/
public class Hurtbox : MonoBehaviour {
  public Combatant Owner;
  public ShieldAbility ShieldAbility;
  public bool CanBeHitBySword = true;
  public bool CanBeHitByHammer = true;
  public bool InstantDeathFromHammer;
  Collider Collider;

  public bool EnableCollision {
    get => Collider.enabled;
    set => Collider.enabled = value;
  }

  void Awake() {
    Collider = GetComponent<Collider>();
    Owner = Owner ?? GetComponentInParent<Combatant>();
  }

  public void ProcessHit(Combatant attacker, HitConfig hitConfig) {
    var hit = new HitEvent { HitConfig = hitConfig, Attacker = attacker, Victim = Owner };

    if (!CanBeHitBySword && hitConfig.HitType == HitConfig.Types.Sword)
      return;
    if (!CanBeHitByHammer && hitConfig.HitType == HitConfig.Types.Hammer)
      return;

    if (InstantDeathFromHammer && hitConfig.HitType == HitConfig.Types.Hammer) {
      hit.HitConfig.Damage = 1000;
    }
    if (ShieldAbility && ShieldAbility.Blocks(attacker.transform)) {
      hit.HitConfig.Damage = 0;
      hit.Blocked = true;
    }
    hit.Attacker.HandleHit(hit);
    hit.Victim.HandleHurt(hit);
  }
}