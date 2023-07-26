using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitWallSpace: ClassicAbility {
  [SerializeField] Timeval WallTransitionDuration;
  [SerializeField] GameObject WallSpaceAvatar;
  [SerializeField] GameObject WorldSpaceAvatar;
  [SerializeField] WorldSpaceController WorldSpaceController;
  [SerializeField] WallSpaceController WallSpaceController;
  [SerializeField] CinemachineVirtualCamera WallSpaceCamera;
  [SerializeField] CinemachineVirtualCamera WorldSpaceCamera;
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
      WallSpaceAvatar.SetActive(false);
      WorldSpaceAvatar.SetActive(true);
      WorldSpaceCamera.Priority = 1;
      WallSpaceCamera.Priority = 0;
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