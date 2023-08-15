using UnityEngine;

public enum UpdateMode {
  FixedUpdate,
  Update,
  LateUpdate
}

[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LocalTime))]
public class LocalTimeAnimator : MonoBehaviour {
  [SerializeField] UpdateMode UpdateMode;

  Animator Animator;
  LocalTime LocalTime;

  void Awake() {
    this.InitComponent(out Animator);
    this.InitComponent(out LocalTime);
    Animator.enabled = false;
  }

  void Update() {
    if (UpdateMode == UpdateMode.Update)
      Animator.Update(LocalTime.DeltaTime);
  }

  void LateUpdate() {
    if (UpdateMode == UpdateMode.LateUpdate)
      Animator.Update(LocalTime.DeltaTime);
  }

  void FixedUpdate() {
    if (UpdateMode == UpdateMode.FixedUpdate)
      Animator.Update(LocalTime.FixedDeltaTime);
  }
}