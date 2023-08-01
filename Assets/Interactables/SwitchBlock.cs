using System.Collections;
using UnityEngine;

public class SwitchBlock : MonoBehaviour {
  [SerializeField] Timeval AnimationDuration = Timeval.FromSeconds(.2f);
  [SerializeField] Collider Collider;

  bool Raised;
  bool TargetColliderEnabled;
  Vector3 TargetScale = Vector3.one;

  public void SetSwitchState(bool raised, bool animate) {
    Raised = raised;
    if (Raised) {
      TargetScale = Vector3.one;
      TargetColliderEnabled = true;
    } else {
      TargetScale = new(1, .01f, 1);
      TargetColliderEnabled = false;
    }
    if (animate) {
      StopAllCoroutines();
      StartCoroutine(Animate());
    } else {
      transform.localScale = TargetScale;
      Collider.enabled = TargetColliderEnabled;
    }
  }

  IEnumerator Animate() {
    var startScale = transform.localScale;
    if (TargetColliderEnabled)
      Collider.enabled = true;
    for (var ticks = 0; ticks < AnimationDuration.Ticks; ticks++) {
      transform.localScale = Vector3.Lerp(startScale, TargetScale, (float)ticks / AnimationDuration.Ticks);
      yield return new WaitForFixedUpdate();
    }
    if (!TargetColliderEnabled)
      Collider.enabled = false;
    transform.localScale = TargetScale;
  }
}
