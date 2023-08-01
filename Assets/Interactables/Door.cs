using System;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour {
  public GameObject Left;
  public GameObject Right;
  public ItemProto RequiredKey;
  public Timeval AnimationDuration = Timeval.FromSeconds(.5f);
  public bool IsOpen { get; private set; } = false;

  [ContextMenu("Open")]
  public void Open() {
    Debug.Assert(!IsOpen);
    IsOpen = true;
    StartCoroutine(Animate());
  }

  IEnumerator Animate() {
    var angle = 0f;
    for (var ticks = 0; ticks <= AnimationDuration.Ticks; ticks++) {
      angle = Mathf.Lerp(0f, 90f, (float)ticks / AnimationDuration.Ticks);
      Left.transform.localEulerAngles = new(0, -angle, 0);
      Right.transform.localEulerAngles = new(0, angle, 0);
      yield return new WaitForFixedUpdate();
    }
  }
}
