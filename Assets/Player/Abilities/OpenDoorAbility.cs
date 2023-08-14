using System.Threading.Tasks;
using UnityEngine;

public class OpenDoorAbility : ClassicAbility {
  public float InteractDistance = 1f;
  public LayerMask DoorMask;
  Inventory Inventory => AbilityManager.GetComponent<Inventory>();
  Door Door;

  bool CanRun() => Door && !Door.IsOpen && Inventory.Count(Door.RequiredKey) > 0;

  public override Task MainAction(TaskScope scope) {
    Inventory.Remove(Door.RequiredKey);
    Door.Open();
    return null;
  }

  protected override void FixedUpdate() {
    base.FixedUpdate();
    var rayHit = Physics.Raycast(transform.position + Vector3.up, transform.forward, out var hit, InteractDistance, DoorMask, QueryTriggerInteraction.Collide);
    if (rayHit && hit.collider.TryGetComponent(out Door door) && !door.IsOpen) {
      Door = door;
    } else {
      Door = null;
    }
  }

  protected override void Awake() {
    base.Awake();
    Main.CanRun = CanRun;
  }
}