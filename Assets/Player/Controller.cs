using UnityEngine;
using KinematicCharacterController;

/*
Character state
  WorldSpace
    Spawning
      Immune
      NoTurn
      NoItem
      NoAttack
      NoBlock
    Teleport
      Immune
      NoTurn
      NoMove
      NoItem
      NoAttack
      NoBlock
    Blocking
      NoTurn
      NoItem
      NoAttack
      NoBlock
    Attacking
      NoTurn
      NoAttack
      NoBlock
    Using Item
      NoAttack
      NoItem
      NoBlock
    Grabbing
      NoTurn
      NoMove
      NoAttack
      NoItem
      NoBlock
    Holding
      NoAttack
      NoItem
      NoBlock
    Throwing
      NoTurn
      NoAttack
      NoItem
      NoBlock
    Falling
      NoMove
      NoTurn
      NoAttack
      NoItem
      NoBlock
    Jumping
      NoMove
      NoTurn
      NoAttack
      NoItem
      NoBlock
    Dazed
      NoMove
      NoTurn
      NoAttack
      NoItem
      NoBlock
    Knockdown
      NoMove
      NoTurn
      NoAttack
      NoItem
      NoBlock
    OnGround
      NoMove
      NoTurn
      NoAttack
      NoItem
      NoBlock
    GetUp
      NoMove
      NoTurn
      NoAttack
      NoItem
      NoBlock
    Dying
      Immune
      NoTurn
      NoMove
      NoItem
      NoAttack
      NoBlock
    Dead
      Immune
      NoTurn
      NoMove
      NoItem
      NoAttack
      NoBlock
*/

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