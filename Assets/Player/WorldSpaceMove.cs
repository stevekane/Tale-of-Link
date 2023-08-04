using UnityEngine;

public class WorldSpaceMove : Ability {
  [SerializeField] float Speed = 5;

  public AbilityAction<Vector3> Move;

  Animator Animator;
  WorldSpaceController WorldSpaceController;

  void Start() {
    Move.Ability = this;
    Move.Listen(OnMove);
    AbilityManager.InitComponent(out Animator, true);
    AbilityManager.InitComponent(out WorldSpaceController);
  }

  void OnMove(Vector3 stick) {
    WorldSpaceController.MaxMoveSpeed = Speed;
    WorldSpaceController.ScriptVelocity += Speed * stick.XZ();
    if (stick.sqrMagnitude > 0 && AbilityManager.HasTag(AbilityTag.CanRotate))
      WorldSpaceController.Forward = stick;
    if (Animator)
      Animator.SetFloat("Normalized Move Speed", stick.XZ().magnitude);
  }
}