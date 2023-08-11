using UnityEngine;

public class WorldSpaceMove : Ability {
  [SerializeField] float Speed = 5;
  [SerializeField] bool StopAtLedges = false;

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
    var dir = stick.XZ();
    if (StopAtLedges && WorldSpaceController.IsOnLedge && Vector3.Dot(dir, WorldSpaceController.LedgeDirection) > 0) {
      var towardsLedge = Vector3.Project(dir, WorldSpaceController.LedgeDirection.XZ());
      dir -= towardsLedge.XZ();
    }
    WorldSpaceController.MaxMoveSpeed = Speed;
    WorldSpaceController.DesiredVelocity += Speed * dir;
    if (stick.sqrMagnitude > 0 && AbilityManager.HasTag(AbilityTag.CanRotate))
      WorldSpaceController.Forward = stick;
    if (Animator)
      Animator.SetFloat("Normalized Move Speed", stick.XZ().magnitude);
  }
}