using UnityEngine;
using KinematicCharacterController;

[RequireComponent(typeof(KinematicCharacterMotor))]
[DefaultExecutionOrder(100)]
public class WorldSpaceController : MonoBehaviour, ICharacterController {
  [SerializeField] AbilityManager AbilityManager;
  [SerializeField] KinematicCharacterMotor Motor;

  public float MaxMoveSpeed;
  public Vector3 PhysicsAcceleration;
  public Vector3 PhysicsVelocity;
  public Vector3 ScriptVelocity;

  public void Unground() {
    Motor.ForceUnground();
  }

  public void Launch(Vector3 acceleration) {
    Unground();
    PhysicsVelocity.y = 0;
    PhysicsAcceleration += acceleration;
  }

  public Vector3 Position {
    get => transform.position;
    set => Motor.SetPosition(value);
  }

  public Vector3 Forward {
    get => transform.forward;
    set => Motor.SetRotation(Quaternion.LookRotation(value, Vector3.up));
  }

  public bool DirectMove;

  void Start() {
    Motor.CharacterController = this;
  }

  void OnEnable() {
    AbilityManager.AddTag(AbilityTag.WorldSpace);
    Motor.enabled = true;
  }

  void OnDisable() {
    AbilityManager.RemoveTag(AbilityTag.WorldSpace);
    Motor.enabled = false;
  }

  // TODO: Should this be here or in GroundedUpdate?
  void FixedUpdate() {
    AbilityManager.SetTag(AbilityTag.Grounded, Motor.GroundingStatus.FoundAnyGround);
    AbilityManager.SetTag(AbilityTag.Airborne, !Motor.GroundingStatus.FoundAnyGround);
  }

  void OnDestroy() {
    Motor.CharacterController = null;
  }

  public void BeforeCharacterUpdate(float deltaTime) {
  }

  public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
  }

  public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
    if (DirectMove) {
      currentVelocity = Vector3.zero;
    } else {
      var steeringVector = (ScriptVelocity - PhysicsVelocity).XZ();
      var desiredMagnitude = steeringVector.magnitude;
      var maxSteeringMagnitude = 2f * MaxMoveSpeed;
      var boundedSteeringVelocity = Mathf.Min(desiredMagnitude, maxSteeringMagnitude) * steeringVector.normalized;
      var grounded = Motor.GroundingStatus.FoundAnyGround;
      // TODO: maybe move this out of here to own gravity component?
      PhysicsAcceleration += grounded ? Vector3.zero : Physics.gravity;
      PhysicsVelocity += boundedSteeringVelocity;
      PhysicsVelocity += deltaTime * PhysicsAcceleration;
      PhysicsVelocity.y = grounded ? 0 : PhysicsVelocity.y;
      currentVelocity = PhysicsVelocity;
    }
    PhysicsAcceleration = Vector3.zero;
    ScriptVelocity = Vector3.zero;
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