using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {
  public GameObject Model;
  public Vector3 Extents = new Vector3(1, 1, 1);
  public Timeval SquashDuration = Timeval.FromSeconds(1.5f);
  public Vector3 SquashOffset = new Vector3(0, .15f, 0);
  public List<WorldSpaceController> Controllers = new();
  int TicksRemaining = -1;

  private void Awake() {
    GetComponent<Combatant>().OnHurt += OnHurt;
  }

  void OnHurt(HitEvent hit) {
    if (hit.HitConfig.HitType != HitConfig.Types.Hammer) return;
    if (TicksRemaining > 0) return;
    Squash();
  }

  void OnTriggerEnter(Collider c) {
    if (c.TryGetComponent(out WorldSpaceController controller)) {
      Controllers.Add(controller);
    }
  }

  void OnTriggerExit(Collider c) {
    if (c.TryGetComponent(out WorldSpaceController controller)) {
      Controllers.Remove(controller);
    }
  }

  void FixedUpdate() {
    if (TicksRemaining > 0 && --TicksRemaining <= 0) {
      Popup();
    }
  }

  void Squash() {
    TicksRemaining = SquashDuration.Ticks;
    Model.transform.localPosition -= SquashOffset;
  }

  void Popup() {
    foreach (var controller in Controllers)
      controller.Launch(700f * Vector3.up);
    Model.transform.localPosition += SquashOffset;
  }
}