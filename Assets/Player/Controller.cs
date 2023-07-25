using UnityEngine;
using KinematicCharacterController;

[RequireComponent(typeof(KinematicCharacterMotor))]
public class Controller : MonoBehaviour, ICharacterController {
  [SerializeField] SimpleAbilityManager AbilityManager;
  [SerializeField] KinematicCharacterMotor Motor;

  public Vector3 Velocity;
  public bool WorldSpace {
    get => Motor.enabled;
    set => Motor.enabled = value;
  }

  public Vector3 Position {
    get => transform.position;
    set {
      if (WorldSpace) {
        Motor.SetPosition(value);
      } else {
        transform.position = value;
      }
    }
  }

  public Vector3 Forward {
    get => transform.forward;
    set {
      if (WorldSpace) {
        Motor.SetRotation(Quaternion.LookRotation(value, Vector3.up));
      } else {
        transform.forward = value;
      }
    }
  }

  public bool DirectMove;

  void Start() {
    Motor.CharacterController = this;
  }

  void FixedUpdate() {
    AbilityManager.SetTag(AbilityTag.Grounded, WorldSpace && Motor.GroundingStatus.FoundAnyGround);
    AbilityManager.SetTag(AbilityTag.Airborne, WorldSpace && !Motor.GroundingStatus.FoundAnyGround);
    AbilityManager.SetTag(AbilityTag.WorldSpace, WorldSpace);
    AbilityManager.SetTag(AbilityTag.WallSpace, !WorldSpace);
  }

  void OnDestroy() {
    Motor.CharacterController = null;
  }

  public void BeforeCharacterUpdate(float deltaTime) {
  }

  public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
    // Could handle turn speed here
  }

  public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
    if (DirectMove) {
      currentVelocity = Vector3.zero;
    } else {
      currentVelocity = Velocity + deltaTime * Physics.gravity;
    }
  }

  public void AfterCharacterUpdate(float deltaTime) {
  }

  public bool IsColliderValidForCollisions(Collider coll) {
    return true;
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