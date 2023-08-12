using UnityEngine;
using KinematicCharacterController;

public class MoverTester : MonoBehaviour, IMoverController {
  public enum Mode { Constant, Speedup, Slowdown }

  public float Speed = 1;
  public float Acceleration = 1;
  public Mode MovementMode;
  public Transform Pusher;

  void Awake() {
    GetComponent<PhysicsMover>().MoverController = this;
  }

  void FixedUpdate() {
    if (!Pusher)
      return;
    LifeCycleTests.Print("Mover FixedUpdate");
    Debug.DrawLine(Pusher.position, transform.position);
  }

  public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime) {
    LifeCycleTests.Print("Mover UpdateMovement");
    Speed = MovementMode switch {
      Mode.Constant => Speed,
      Mode.Speedup => Speed + deltaTime * Acceleration,
      Mode.Slowdown => Speed - deltaTime * Acceleration
    };
    goalPosition = transform.position + deltaTime * Speed * transform.forward;
    goalRotation = transform.rotation;
  }
}