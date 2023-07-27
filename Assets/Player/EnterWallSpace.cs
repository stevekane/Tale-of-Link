using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnterWallSpace : ClassicAbility {
  [SerializeField] Timeval WallTransitionDuration;
  [SerializeField] WorldSpaceController WorldSpaceController;
  [SerializeField] WallSpaceController WallSpaceController;
  [SerializeField] LayerMask LayerMask;
  [SerializeField] CapsuleCollider CapsuleCollider;
  [SerializeField] Mesh CapsuleMesh;
  [SerializeField] float EnterDistance = 1;

  public override async Task MainAction(TaskScope scope) {
    var start = WorldSpaceController.transform.position;
    var direction = WorldSpaceController.transform.forward;
    var capsuleHit = CapsuleCollider.CapsuleColliderCast(start, direction, EnterDistance, out var hit, LayerMask, QueryTriggerInteraction.Ignore);
    var rayHit = Physics.Raycast(start, direction, out hit, EnterDistance, LayerMask, QueryTriggerInteraction.Ignore);
    if (capsuleHit && rayHit && !hit.collider.CompareTag("Blocker")) {
      WorldSpaceController.enabled = false;
      WallSpaceController.enabled = true;
      WallSpaceController.transform.position = hit.point.XZ() + WorldSpaceController.transform.position.y * Vector3.up + Vector3.up;
      WallSpaceController.transform.forward = hit.normal; //TODO: Is this normal ever suspect? Maybe it is sometimes?
      await scope.Ticks(WallTransitionDuration.Ticks);
    }
  }

  void OnDrawGizmos() {
    if (!AbilityManager || !AbilityManager.CanRun(Main))
      return;
    var distance = EnterDistance;
    var start = WorldSpaceController.transform.position + Vector3.up;
    var direction = WorldSpaceController.transform.forward;
    var end = start + distance * direction;
    var didHit = CapsuleCollider.CapsuleColliderCast(start, direction, distance, out var hit, LayerMask, QueryTriggerInteraction.Ignore);
    var rayHit = Physics.Raycast(start, direction, out hit, EnterDistance, LayerMask, QueryTriggerInteraction.Ignore);
    var color = didHit && rayHit
      ? hit.collider.CompareTag("Blocker")
        ? Color.yellow
        : Color.white
      : Color.red;
    color.a = .2f;
    Gizmos.color = color;
    Gizmos.DrawWireMesh(CapsuleMesh, submeshIndex: -1, end, Quaternion.identity, Vector3.one);
    Handles.Label(end + Vector3.up, $"{(hit.collider ? hit.collider.name : default)}");
  }
}