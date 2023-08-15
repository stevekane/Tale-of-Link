using System.Collections;
using UnityEngine;

public class VerticalDoor : MonoBehaviour {
  public GameObject Hinge;
  public Timeval AnimationDuration = Timeval.FromSeconds(.5f);
  [field:SerializeField]
  public bool IsOpen { get; private set; } = false;

  void Start() {
    if (IsOpen)
      Hinge.transform.localScale = new(1, 0, 1);
    else
      Hinge.transform.localScale = new(1, 1, 1);
  }

  [ContextMenu("Open")]
  public void Open() {
    IsOpen = true;
    StartCoroutine(Animate());
  }

  [ContextMenu("Close")]
  public void Close() {
    IsOpen = false;
    StartCoroutine(Animate());
  }

  IEnumerator Animate() {
    var fromScale = Hinge.transform.localScale.y;
    var toScale = IsOpen ? 0f : 1f;
    for (var ticks = 0; ticks <= AnimationDuration.Ticks; ticks++) {
      var scale = Mathf.Lerp(fromScale, toScale, (float)ticks / AnimationDuration.Ticks);
      Hinge.transform.localScale = new(1, scale, 1);
      yield return new WaitForFixedUpdate();
    }
  }
}
