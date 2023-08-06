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
  public bool IgnoreRotation = false;

  PathTraversal PathTraversal;
  float StartOffset = 0f;

  public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime) {
    goalPosition = PhysicsMover.TransientPosition;
    goalRotation = PhysicsMover.TransientRotation;
    if (IsActive) {
      var previousPosition = PhysicsMover.TransientPosition;
      var previousRotation = PhysicsMover.TransientRotation;
      if (PathTraversal != null)
        PathTraversal.Advance(ref goalPosition, ref goalRotation, MoveSpeed);
      if (IgnoreRotation)  // dumb hack to work with PhysicsRotator
        goalRotation = previousRotation;
      var nextPosition = goalPosition;
      if (MovingWall) {
        MovingWall.PreviousMotionDelta = MovingWall.MotionDelta;
        MovingWall.MotionDelta = nextPosition-previousPosition;
      }
    }
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
    var path = Waypoints.CreatePathTraversal(Mode);
    path.DrawGizmos();

    var pos = transform.position;
    var rotation = transform.rotation;
    path.WarpTo(ref pos, ref rotation, StartOffset);
    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(pos, GetComponent<BoxCollider>().size);
  }
}