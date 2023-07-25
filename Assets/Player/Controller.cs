using UnityEngine;
using KinematicCharacterController;

[RequireComponent(typeof(KinematicCharacterMotor))]
public class Controller : MonoBehaviour, ICharacterController {
  KinematicCharacterMotor Motor;

  public Vector3 Velocity;
  public Vector3 Forward;
  public Vector3 Position {
    get => transform.position;
    set => Motor.SetPosition(value);
  }
  public bool DirectMove;
  public bool IgnoreCollision;

  void Start() {
    Motor = GetComponent<KinematicCharacterMotor>();
    Motor.CharacterController = this;
  }

  void OnDestroy() {
    Motor.CharacterController = null;
  }

  public void AfterCharacterUpdate(float deltaTime) {
  }

  public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
    if (Forward.sqrMagnitude > 0)
      currentRotation = Quaternion.LookRotation(Forward, Vector3.up);
  }

  public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
    if (DirectMove) {
      currentVelocity = Vector3.zero;
    } else {
      currentVelocity = Velocity;
    }
  }

  public void BeforeCharacterUpdate(float deltaTime) {
  }

  public bool IsColliderValidForCollisions(Collider coll) {
    return !IgnoreCollision;
  }

  public void OnDiscreteCollisionDetected(Collider hitCollider) {
  }

  public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {
  }

  public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {
  }

  public void PostGroundingUpdate(float deltaTime) {
  }

  public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) {
  }
}