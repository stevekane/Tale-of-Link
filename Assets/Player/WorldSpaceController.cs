using UnityEngine;
using KinematicCharacterController;

[RequireComponent(typeof(KinematicCharacterMotor))]
public class WorldSpaceController : MonoBehaviour, ICharacterController {
  [SerializeField] AbilityManager AbilityManager;
  [SerializeField] KinematicCharacterMotor Motor;

  public Vector3 Velocity;

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
      Velocity.y = Motor.GroundingStatus.FoundAnyGround ? 0 : Velocity.y + deltaTime * Physics.gravity.y;
      currentVelocity = Velocity;
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