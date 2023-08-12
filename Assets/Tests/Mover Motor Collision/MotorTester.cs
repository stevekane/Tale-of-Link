using UnityEngine;
using KinematicCharacterController;

public class MotorTester : MonoBehaviour, ICharacterController {
  public bool AddToVelocity;
  void Awake() {
    GetComponent<KinematicCharacterMotor>().CharacterController = this;
  }

  void ICharacterController.AfterCharacterUpdate(float deltaTime)
  {
  }

  void ICharacterController.BeforeCharacterUpdate(float deltaTime)
  {
  }

  bool ICharacterController.IsColliderValidForCollisions(Collider coll)
  {
    return true;
  }

  void ICharacterController.OnDiscreteCollisionDetected(Collider hitCollider)
  {
  }

  void ICharacterController.OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
  {
  }

  void ICharacterController.OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
  {
  }

  void ICharacterController.PostGroundingUpdate(float deltaTime)
  {
  }

  void ICharacterController.ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
  {
  }

  void ICharacterController.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
  {
  }

  void ICharacterController.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
  {
    LifeCycleTests.Print("Motor UpdateVelocity");
    currentVelocity = Vector3.zero;
    return;
    if (AddToVelocity)
      currentVelocity += deltaTime * Physics.gravity;
    else
      currentVelocity = deltaTime * Physics.gravity;
  }
}