using System.Collections;
using UnityEngine;

public class SwitchBlock : MonoBehaviour {
  public int UpState = 0;
  public int State = 0;
  public Timeval AnimationDuration = Timeval.FromSeconds(.2f);
  Vector3 TargetScale = Vector3.one;

  void Awake() {
    ValidateState(false);
  }

  public void SetSwitchState(int state, bool animate) {
    State = state;
    ValidateState(animate);
  }

  void ValidateState(bool animate) {
    if (State == UpState) {
      TargetScale = Vector3.one;
    } else {
      TargetScale = new(1, .01f, 1);
    }
    if (animate) {
      StopAllCoroutines();
      StartCoroutine(Animate());
    } else {
      transform.localScale = TargetScale;
    }
  }

  IEnumerator Animate() {
    var startScale = transform.localScale;
    for (var ticks = 0; ticks < AnimationDuration.Ticks; ticks++) {
      transform.localScale = Vector3.Lerp(startScale, TargetScale, (float)ticks / AnimationDuration.Ticks);
      yield return new WaitForFixedUpdate();
    }
    transform.localScale = TargetScale;
  }
}
