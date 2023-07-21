using System;
using UnityEngine;

public class JumpPad : MonoBehaviour {
  public Hurtbox Hurtbox;
  public GameObject Model;
  public Collider Collider;
  public LayerMask LayerMask;
  public Vector3 Extents = new Vector3(1, 1, 1);
  public Timeval SquashDuration = Timeval.FromSeconds(1.5f);
  public Vector3 SquashOffset = new Vector3(0, .15f, 0);
  int TicksRemaining = -1;

  private void Awake() {
    GetComponent<Combatant>().OnHurt += OnHurt;
    Collider.enabled = true;
  }

  void OnHurt(HitEvent hit) {
    if (hit.HitConfig.HitType != HitConfig.Types.Hammer) return;
    if (TicksRemaining > 0) return;
    Squash();
  }

  void Squash() {
    TicksRemaining = SquashDuration.Ticks;
    Model.transform.localPosition -= SquashOffset;
    Collider.enabled = false;
  }

  void Popup() {
    var hits = Physics.OverlapBox(transform.position, Extents, transform.rotation, LayerMask, QueryTriggerInteraction.Collide);
    hits.ForEach(c => {
      if (c.TryGetComponent(out Hurtbox hb))
        hb.Owner.GetComponent<Rigidbody>().AddForce(7f * Vector3.up, ForceMode.VelocityChange);
    });
    Model.transform.localPosition += SquashOffset;
    Collider.enabled = true;
    // Fling Link
  }

  void FixedUpdate() {
    if (TicksRemaining > 0 && --TicksRemaining <= 0) {
      Popup();
    }
  }
}
