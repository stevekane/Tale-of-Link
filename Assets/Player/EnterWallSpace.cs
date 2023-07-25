using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnterWallSpace : ClassicAbility {
  [SerializeField] Timeval WallTransitionDuration;
  [SerializeField] GameObject WallSpaceAvatar;
  [SerializeField] GameObject WorldSpaceAvatar;
  [SerializeField] Controller Controller;
  [SerializeField] CinemachineVirtualCamera WallSpaceCamera;
  [SerializeField] CinemachineVirtualCamera WorldSpaceCamera;
  [SerializeField] LayerMask LayerMask;
  [SerializeField] CapsuleCollider CapsuleCollider;
  [SerializeField] Mesh CapsuleMesh;
  [SerializeField] float EnterDistance = 1;

  public override async Task MainAction(TaskScope scope) {
    var start = Controller.transform.position;
    var direction = Controller.transform.forward;
    var didHit = CapsuleCollider.CapsuleColliderCast(start, direction, EnterDistance, out var hit, LayerMask, QueryTriggerInteraction.Ignore);
    if (didHit && !hit.collider.CompareTag("Blocker")) {
      Controller.DirectMove = true;
      Controller.WorldSpace = false;
      // use our current height + the position of the point of contact
      // we add Vector3.up to it because wallspace positions are at the half-height of the worldspace actor
      // TODO: possibly change this for wallspace actors?
      Controller.Position = hit.point.XZ() + Controller.transform.position.y * Vector3.up + Vector3.up;
      Controller.Forward = hit.normal;
      WorldSpaceAvatar.SetActive(false);
      WallSpaceAvatar.SetActive(true);
      await scope.Ticks(WallTransitionDuration.Ticks);
      WallSpaceCamera.Priority = 1;
      WorldSpaceCamera.Priority = 0;
    }
  }

  void OnDrawGizmos() {
    if (!AbilityManager || !AbilityManager.CanRun(Main))
      return;
    var distance = EnterDistance;
    var start = Controller.transform.position + Vector3.up;
    var direction = Controller.transform.forward;
    var end = start + distance * direction;
    var didHit = CapsuleCollider.CapsuleColliderCast(start, direction, distance, out var hit, LayerMask, QueryTriggerInteraction.Ignore);
    var color = didHit
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