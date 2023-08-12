using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AbilityManager))]
[RequireComponent(typeof(WorldSpaceController))]
public class AirborneAnimator : MonoBehaviour {
  Animator Animator;
  AbilityManager AbilityManager;
  WorldSpaceController WorldSpaceController;

  void Awake() {
    this.InitComponent(out Animator);
    this.InitComponent(out AbilityManager);
    this.InitComponent(out WorldSpaceController);
  }

  void LateUpdate() {
    var airborne = !AbilityManager.HasTags(AbilityTag.Grounded);
    Animator.SetBool("Airborne", airborne);
  }
}