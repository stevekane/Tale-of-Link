using UnityEngine;

public class MobOnHurt : MonoBehaviour {
  public Combatant Combatant;
  public Hearts Hearts;
  public int SwordDamage = 1;
  public int HammerDamage = 1;

  void Awake() {
    Combatant.OnHurt += OnHurt;
  }

  void OnHurt(HitEvent hit) {
    int damage = hit.HitConfig.HitType switch {
      HitConfig.Types.Sword => SwordDamage,
      HitConfig.Types.Hammer => HammerDamage,
      _ => SwordDamage,
    };
    if (damage > 0)
      Hearts.ChangeCurrent(-damage);
  }
}