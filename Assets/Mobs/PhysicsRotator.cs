using KinematicCharacterController;
using UnityEngine;

public class PhysicsRotator : MonoBehaviour {
  public PhysicsMover PhysicsMover;
  public Vector3 Axis = Vector3.up;
  public float DegreesPerSecond;

  void FixedUpdate() {
    PhysicsMover.SetRotation(Quaternion.AngleAxis(DegreesPerSecond*Time.fixedDeltaTime, Axis) * PhysicsMover.TransientRotation);
  }
}