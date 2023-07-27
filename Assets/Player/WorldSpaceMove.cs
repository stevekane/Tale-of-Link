using UnityEngine;

public class WorldSpaceMove : Ability {
  [SerializeField] WorldSpaceController WorldSpaceController;
  [SerializeField] float Speed = 5;

  Vector3 Velocity;

  public AbilityAction<Vector3> Move;

  void Awake() {
    Move.Ability = this;
    Move.Listen(OnMove);
  }

  void OnMove(Vector3 stick) {
    WorldSpaceController.MaxMoveSpeed = Speed;
    WorldSpaceController.ScriptVelocity += Speed * stick.XZ();
    if (stick.sqrMagnitude > 0 && AbilityManager.HasTag(AbilityTag.CanRotate))
      WorldSpaceController.Forward = stick;
  }
}