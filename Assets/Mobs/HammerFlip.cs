using UnityEngine;

public class HammerFlip : MonoBehaviour {
  public AbilityManager AbilityManager;
  public Combatant Combatant;
  public GameObject Model;
  public bool IsFlipped = false;
  public float Height = 1f;

  void Awake() {
    Combatant.OnHurt += OnHurt;
  }

  void OnHurt(HitEvent obj) {
    IsFlipped = !IsFlipped;
    if (IsFlipped) {
      Model.transform.Rotate(0, 0, 180);
      Model.transform.position += new Vector3(0, Height, 0);
      AbilityManager.SetTag(AbilityTag.CanMove, false);
      AbilityManager.SetTag(AbilityTag.CanRotate, false);
    } else {
      Model.transform.Rotate(0, 0, 180);
      Model.transform.position -= new Vector3(0, Height, 0);
      AbilityManager.SetTag(AbilityTag.CanMove, true);
      AbilityManager.SetTag(AbilityTag.CanRotate, true);
    }
  }
}