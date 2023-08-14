using UnityEngine;

public class BallisticWhileAirborne : MonoBehaviour {
  [SerializeField] AbilityManager AbilityManager;
  [SerializeField] ContactHitbox ContactHitbox;

  void Start() {
    this.InitComponent(out ContactHitbox);
  }

  void FixedUpdate() {
    ContactHitbox.IsActive = AbilityManager.HasTags(AbilityTag.Airborne);
  }
}