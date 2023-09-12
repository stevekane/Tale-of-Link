using System;
using System.Collections;
using UnityEngine;


public class JumpPad : MonoBehaviour {
  public GameObject Model;
  public Collider Collider;
  public float LaunchHeight = 2;
  public float LaunchSpeed => Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * Mathf.Abs(LaunchHeight));
  public float LaunchAngleDeg = 80f;
  public Timeval SquashDuration = Timeval.FromSeconds(1.5f);
  public Vector3 SquashOffset = new Vector3(0, .15f, 0);
  int TicksRemaining = -1;

  public Action<JumpPad> OnPopup;
  public bool IsSquashed => TicksRemaining > 0;

  private void Awake() {
    GetComponent<Combatant>().OnHurt += OnHurt;
    Collider.enabled = true;
  }

  void OnHurt(HitEvent hit) {
    if (hit.HitConfig.HitType != HitConfig.Types.Hammer) return;
    if (IsSquashed) return;
    Squash();
  }

  void Squash() {
    TicksRemaining = SquashDuration.Ticks;
    Model.transform.localPosition -= SquashOffset;
    Collider.enabled = false;
  }

  public void Popup() {
    if (!IsSquashed)
      return;
    OnPopup?.Invoke(this);
    StopAllCoroutines();
    StartCoroutine(Raise());
    TicksRemaining = -1;
  }

  IEnumerator Raise() {
    const int RaiseTicks = 5;
    for (int i = 0; i < RaiseTicks; i++) {
      Model.transform.localPosition += SquashOffset/RaiseTicks;
      yield return new WaitForFixedUpdate();
    }
    Collider.enabled = true;
  }

  void FixedUpdate() {
    if (IsSquashed && TicksRemaining <= 1)
      Popup();
    if (IsSquashed)
      TicksRemaining--;
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.magenta;
    Gizmos.DrawRay(transform.position, LaunchHeight * Vector3.up);
  }
}
