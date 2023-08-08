using System.Threading.Tasks;
using UnityEngine;

public class OpenAbility : ClassicAbility {
  public float InteractDistance = 1f;
  public LayerMask OpenableMask;
  Openable Openable;

  bool CanRun() => Openable && !Openable.IsOpen;

  /*
  When you open a chest, you should be handed an Item.
  This item should be collectable.
  Collecting the item is done by the "Collect" Ability.
  Collect ability
    displays the item over your head
    displays shiny particles around the item
    displays UI text explaining what the item is
    waits for you to push a button to complete the collecting process
    adds the item to your inventory
  */

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