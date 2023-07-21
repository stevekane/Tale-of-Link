using System;
using UnityEngine;

public class JumpPad : MonoBehaviour {
  public Hurtbox Hurtbox;
  public GameObject Model;
  public Timeval SquashDuration = Timeval.FromSeconds(1.5f);
  public Vector3 SquashOffset = new Vector3(0, .15f, 0);
  int TicksRemaining = -1;

  private void Awake() {
    GetComponent<Combatant>().OnHurt += OnHurt;
  }

  void OnHurt(HitEvent hit) {
    if (hit.HitConfig.HitType != HitConfig.Types.Hammer) return;
    if (TicksRemaining > 0) return;
    Squash();
  }

  void Squash() {
    TicksRemaining = SquashDuration.Ticks;
    Model.transform.localPosition -= SquashOffset;
  }

  void Popup() {
    Model.transform.localPosition += SquashOffset;
    // Fling Link
  }

  void FixedUpdate() {
    if (TicksRemaining > 0 && --TicksRemaining <= 0) {
      Popup();
    }
  }
}
