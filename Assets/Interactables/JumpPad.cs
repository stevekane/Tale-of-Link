using System;
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

  public Action OnPopup;
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
    OnPopup?.Invoke();
    Model.transform.localPosition += SquashOffset;
    Collider.enabled = true;
    TicksRemaining = -1;
  }

  void EnableCollider() {
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
