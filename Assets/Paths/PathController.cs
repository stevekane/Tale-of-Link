using KinematicCharacterController;
using UnityEngine;

[RequireComponent(typeof(LocalTime))]
public class PathController : MonoBehaviour, IMoverController {
  public PhysicsMover PhysicsMover;
  public MovingWall MovingWall;
  public Waypoints Waypoints;
  public float MoveSpeed = 10f;
  public PathTraversal.Modes Mode;
  public bool IsActive = true;
  public bool IgnoreRotation = false;
  public float StartOffset = 0f;

  LocalTime LocalTime;
  PathTraversal PathTraversal;

  public void Activate() => IsActive = true;
  public void Deactivate() => IsActive = false;

  public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime) {
    goalPosition = PhysicsMover.TransientPosition;
    goalRotation = PhysicsMover.TransientRotation;
    if (IsActive) {
      var previousPosition = PhysicsMover.TransientPosition;
      var previousRotation = PhysicsMover.TransientRotation;
      if (PathTraversal != null)
        PathTraversal.Advance(ref goalPosition, ref goalRotation, MoveSpeed, LocalTime.TimeScale * deltaTime);
      if (IgnoreRotation)  // dumb hack to work with PhysicsRotator
        goalRotation = previousRotation;
      var nextPosition = goalPosition;
      if (MovingWall) {
        MovingWall.PreviousMotionDelta = MovingWall.MotionDelta;
        MovingWall.MotionDelta = nextPosition-previousPosition;
      }
    }
  }

  void Awake() {
    this.InitComponent(out LocalTime);
  }

  void Start() {
    PhysicsMover.MoverController = this;
    var pos = PhysicsMover.TransientPosition;
    var rotation = PhysicsMover.TransientRotation;
    if (Waypoints) {
      PathTraversal = Waypoints.CreatePathTraversal(Mode);
      PathTraversal.WarpTo(ref pos, ref rotation, StartOffset);
    }
    PhysicsMover.SetPositionAndRotation(pos, rotation);
  }

  // Must be called before Start.
  public void SetStartOffset(float startOffset) => StartOffset = startOffset;

  public void OnDrawGizmosSelected() {
    if (!Waypoints) return;
    Gizmos.color = Color.green;
    var boxCollider = GetComponent<BoxCollider>();
    var size = boxCollider ? boxCollider.size : Vector3.one;
    var center = boxCollider ? boxCollider.center : Vector3.zero;
    foreach (var waypoint in Waypoints.Nodes) {
      Gizmos.DrawWireCube(waypoint.transform.TransformPoint(center), size);
    }
  }
}