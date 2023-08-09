using UnityEngine;

public class HammerFlip : MonoBehaviour {
  public AbilityManager AbilityManager;
  public Combatant Combatant;
  public GameObject Model;
  public Hearts Hearts;
  public bool IsFlipped = false;
  public float Height = 1f;

  void Awake() {
    this.InitComponent(out Hearts);
    Combatant.OnHurt += OnHurt;
    Hearts.IsInvulnerable = true;
  }

  void OnHurt(HitEvent hit) {
    if (hit.HitConfig.HitType != HitConfig.Types.Hammer) return;
    IsFlipped = !IsFlipped;
    if (IsFlipped) {
      Model.transform.Rotate(0, 0, 180);
      Model.transform.position += new Vector3(0, Height, 0);
      AbilityManager.SetTag(AbilityTag.CanMove, false);
      AbilityManager.SetTag(AbilityTag.CanRotate, false);
      Hearts.IsInvulnerable = false;
    } else {
      Model.transform.Rotate(0, 0, 180);
      Model.transform.position -= new Vector3(0, Height, 0);
      AbilityManager.SetTag(AbilityTag.CanMove, true);
      AbilityManager.SetTag(AbilityTag.CanRotate, true);
      Hearts.IsInvulnerable = true;
    }
  }
}