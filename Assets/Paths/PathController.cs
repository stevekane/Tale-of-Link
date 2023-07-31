using KinematicCharacterController;
using UnityEngine;

public class PathController : MonoBehaviour, IMoverController {
  public PhysicsMover PhysicsMover;
  public Waypoints Waypoints;
  public float MoveSpeed = 10f;
  public PathTraversal.Modes Mode;
  public bool IsActive = true;

  PathTraversal PathTraversal;

  public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime) {
    goalPosition = PhysicsMover.TransientPosition;
    goalRotation = PhysicsMover.TransientRotation;
    if (IsActive)
      PathTraversal.Advance(ref goalPosition, ref goalRotation, MoveSpeed);
  }

  void Start() {
    PathTraversal = Waypoints.CreatePathTraversal(Mode);
    PhysicsMover.MoverController = this;
    PhysicsMover.SetPositionAndRotation(Waypoints.Nodes[0].transform.position, Waypoints.Nodes[0].transform.rotation);
  }

  void OnDrawGizmosSelected() {
    var path = Waypoints.CreatePathTraversal(Mode);
    path.DrawGizmos();
  }
}
