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
  [SerializeField] float SkinWidth = .1f;

  protected override void Awake() {
    base.Awake();
    Main.CanRun = CanRun;
  }

  bool CanRun() {
    const int X_SAMPLES = 3;
    const int Y_SAMPLES = 3;
    var canRun = true;
    var dx = (CapsuleCollider.radius-SkinWidth)*2/(X_SAMPLES-1);
    var dy = (CapsuleCollider.height-SkinWidth*2)/(Y_SAMPLES-1);
    var direction = WorldSpaceController.transform.forward;
    var x0 = -CapsuleCollider.radius+SkinWidth;
    var y0 = SkinWidth;
    var z = 0;
    for (var i = 0; i < X_SAMPLES; i++) {
      var x = x0 + dx * i;
      for (var j = 0; j < Y_SAMPLES; j++) {
        var y = y0 + dy * j;
        var origin = AbilityManager.transform.TransformPoint(new Vector3(x,y,z));
        var rayHit = Physics.Raycast(origin, direction, out var hit, EnterDistance, LayerMask, QueryTriggerInteraction.Ignore);
        var validHit = rayHit && !hit.collider.GetComponent<Blocker>();
        canRun = canRun && validHit;
        Debug.DrawRay(origin, direction, validHit ? Color.green : Color.red);
      }
    }
    return canRun;
  }

  public override async Task MainAction(TaskScope scope) {
    var start = WorldSpaceController.transform.position;
    var direction = WorldSpaceController.transform.forward;
    var rayHit = Physics.Raycast(start, direction, out var hit, EnterDistance, LayerMask, QueryTriggerInteraction.Ignore);
    if (rayHit && !hit.collider.GetComponent<Blocker>()) {
      WorldSpaceController.enabled = false;
      WallSpaceController.enabled = true;
      WallSpaceController.MovingWall = hit.collider.GetComponent<MovingWall>();
      WallSpaceController.Merge(hit.point.XZ() + WorldSpaceController.transform.position.y * Vector3.up + Vector3.up, hit.normal);
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
      ? hit.collider.GetComponent<Blocker>()
        ? Color.yellow
        : Color.white
      : Color.red;
    color.a = .2f;
    Gizmos.color = color;
    // TODO: This isn't right. The mesh position/size should be drawn from the Collider
    Gizmos.DrawWireMesh(CapsuleMesh, submeshIndex: -1, end, Quaternion.identity, Vector3.one);
    Handles.Label(end + Vector3.up, $"{(hit.collider ? hit.collider.name : default)}");
  }
}