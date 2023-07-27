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

  public override async Task MainAction(TaskScope scope) {
    var start = WorldSpaceController.transform.position;
    var direction = WorldSpaceController.transform.forward;
    var invalidExit = CapsuleCollider.CapsuleColliderCast(start, direction, ExitDistance, out var hit, LayerMask, QueryTriggerInteraction.Ignore);
    var rayHit = Physics.Raycast(start, direction, out hit, ExitDistance, LayerMask, QueryTriggerInteraction.Ignore);
    if (!invalidExit && !rayHit) {
      WallSpaceController.enabled = false;
      WorldSpaceController.enabled = true;
      WorldSpaceController.Position = start + Vector3.down + ExitDistance * direction;
      WorldSpaceController.Forward = direction;
      await scope.Ticks(WallTransitionDuration.Ticks);
    }
  }

  void OnDrawGizmos() {
    if (!AbilityManager || !AbilityManager.CanRun(Main))
      return;
    var distance = ExitDistance;
    var start = WorldSpaceController.transform.position;
    var direction = WorldSpaceController.transform.forward;
    var end = start + distance * direction;
    var didHit = CapsuleCollider.CapsuleColliderCast(start, direction, distance, out var hit, LayerMask, QueryTriggerInteraction.Ignore);
    var rayHit = Physics.Raycast(start, direction, out hit, ExitDistance, LayerMask, QueryTriggerInteraction.Ignore);
    var color = didHit || rayHit ? Color.red : Color.white;
    color.a = .2f;
    Gizmos.color = color;
    Gizmos.DrawWireMesh(CapsuleMesh, submeshIndex: -1, end, Quaternion.identity, Vector3.one);
    Handles.Label(end + Vector3.up, $"{(hit.collider ? hit.collider.name : default)}");
  }
}