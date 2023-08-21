using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitWallSpace: ClassicAbility {
  [SerializeField] Timeval WallTransitionDuration;
  [SerializeField] WorldSpaceController WorldSpaceController;
  [SerializeField] WallSpaceController WallSpaceController;
  [SerializeField] LayerMask LayerMask;
  [SerializeField] CapsuleCollider CapsuleCollider;
  [SerializeField] Mesh CapsuleMesh;
  [SerializeField] float ExitDistance = 1f;

  protected override void Awake() {
    base.Awake();
    Main.CanRun = CanRun;
  }

  bool CanRun() {
    var start = WorldSpaceController.transform.position;
    var direction = WorldSpaceController.transform.forward;
    var invalidExit = CapsuleCollider.CapsuleColliderCast(start, direction, ExitDistance, out var hit, LayerMask, QueryTriggerInteraction.Ignore);
    var rayHit = Physics.Raycast(start, direction, out hit, ExitDistance, LayerMask, QueryTriggerInteraction.Ignore);
    return !invalidExit && !rayHit;
  }

  public override async Task MainAction(TaskScope scope) {
    var start = WorldSpaceController.transform.position;
    var direction = WorldSpaceController.transform.forward;
    var invalidExit = CapsuleCollider.CapsuleColliderCast(start, direction, ExitDistance, out var hit, LayerMask, QueryTriggerInteraction.Ignore);
    var rayHit = Physics.Raycast(start, direction, out hit, ExitDistance, LayerMask, QueryTriggerInteraction.Ignore);
    if (!invalidExit && !rayHit) {
      WallSpaceController.MovingWall = null;
      WallSpaceController.enabled = false;
      WorldSpaceController.enabled = true;
      WorldSpaceController.Position = start + Vector3.down + ExitDistance * direction;
      WorldSpaceController.Forward = direction;
      await scope.Ticks(WallTransitionDuration.Ticks);
    }
  }

#if UNITY_EDITOR
  void OnDrawGizmos() {
    if (!AbilityManager || !AbilityManager.CanRun(Main))
      return;
    var start = WorldSpaceController.transform.position;
    var direction = WorldSpaceController.transform.forward;
    var end = start + ExitDistance * direction;
    var invalidExit = CapsuleCollider.CapsuleColliderCast(start, direction, ExitDistance, out var hit, LayerMask, QueryTriggerInteraction.Ignore);
    var rayHit = Physics.Raycast(start, direction, out hit, ExitDistance, LayerMask, QueryTriggerInteraction.Ignore);
    var color = invalidExit || rayHit ? Color.red : Color.white;
    color.a = .2f;
    Gizmos.color = color;
    Gizmos.DrawWireMesh(CapsuleMesh, submeshIndex: -1, end, Quaternion.identity, Vector3.one);
    Handles.Label(end + Vector3.up, $"{(hit.collider ? hit.collider.name : default)}");
  }
#endif
}