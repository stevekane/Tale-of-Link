using UnityEngine;

public class Knockbackable : MonoBehaviour {
  public float SwordRecoilScale = 1;
  public float HammerRecoilScale = 1;
  public float SwordKnockbackScale = 1;
  public float HammerKnockbackScale = 1;
  public float RecoilScale(HitConfig.Types type) => type switch {
    HitConfig.Types.Sword => SwordRecoilScale,
    HitConfig.Types.Hammer => HammerRecoilScale,
    _ => 0
  };
  public float KnockbackScale(HitConfig.Types type) => type switch {
    HitConfig.Types.Sword => SwordKnockbackScale,
    HitConfig.Types.Hammer => HammerKnockbackScale,
    _ => 0
  };
}