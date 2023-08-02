using KinematicCharacterController;
using System;
using UnityEngine;

public class PathController : MonoBehaviour, IMoverController {
  public PhysicsMover PhysicsMover;
  public MovingWall MovingWall;
  public Waypoints Waypoints;
  public float MoveSpeed = 10f;
  public PathTraversal.Modes Mode;
  public bool IsActive = true;

  PathTraversal PathTraversal;
  float StartOffset = 0f;

  public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime) {
    goalPosition = PhysicsMover.TransientPosition;
    goalRotation = PhysicsMover.TransientRotation;
    if (IsActive) {
      var previousPosition = PhysicsMover.TransientPosition;
      PathTraversal.Advance(ref goalPosition, ref goalRotation, MoveSpeed);
      var nextPosition = goalPosition;
      if (MovingWall) {
        MovingWall.PreviousMotionDelta = MovingWall.MotionDelta;
        MovingWall.MotionDelta = nextPosition-previousPosition;
      }
    }
  }

  void Start() {
    PhysicsMover.MoverController = this;
    PathTraversal = Waypoints.CreatePathTraversal(Mode);
    var pos = PhysicsMover.TransientPosition;
    var rotation = PhysicsMover.TransientRotation;
    PathTraversal.WarpTo(ref pos, ref rotation, StartOffset);
    PhysicsMover.SetPositionAndRotation(pos, rotation);
  }

  // Must be called before Start.
  public void SetStartOffset(float startOffset) => StartOffset = startOffset;

  public void OnDrawGizmosSelected() {
    var path = Waypoints.CreatePathTraversal(Mode);
    path.DrawGizmos();

    var pos = transform.position;
    var rotation = transform.rotation;
    path.WarpTo(ref pos, ref rotation, StartOffset);
    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(pos, Vector3.one);
  }

}
