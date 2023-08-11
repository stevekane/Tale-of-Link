using System.Threading.Tasks;
using UnityEngine;

public class OpenAbility : ClassicAbility {
  public float InteractDistance = 1f;
  public LayerMask OpenableMask;
  Openable Openable;

  bool CanRun() => Openable && !Openable.IsOpen;

  public override Task MainAction(TaskScope scope) {
    Openable.Open(AbilityManager.gameObject);
    return null;
  }

  void FixedUpdate() {
    var origin = transform.position + Vector3.up;
    var direction = transform.forward;
    var rayHit = Physics.Raycast(origin, direction, out var hit, InteractDistance, OpenableMask, QueryTriggerInteraction.Collide);
    if (!rayHit || hit.collider.TryGetComponent(out Openable) && Openable.IsOpen) {
      Openable = null;
    }
  }

  protected override void Awake() {
    base.Awake();
    Main.CanRun = CanRun;
  }
}