using UnityEngine;

public class WorldSpaceMove : Ability {
  public float Speed = 5;
  public bool StopAtLedges = false;

  public AbilityAction<Vector3> Move;

  Animator Animator;
  LocalTime LocalTime;
  WorldSpaceController WorldSpaceController;

  void Start() {
    Move.Ability = this;
    Move.Listen(OnMove);
    AbilityManager.InitComponent(out Animator, true);
    AbilityManager.InitComponent(out LocalTime);
    AbilityManager.InitComponent(out WorldSpaceController);
  }

  void OnMove(Vector3 stick) {
    var dir = stick.XZ();
    if (StopAtLedges && WorldSpaceController.IsOnLedge && Vector3.Dot(dir, WorldSpaceController.LedgeDirection) > 0) {
      var towardsLedge = Vector3.Project(dir, WorldSpaceController.LedgeDirection.XZ());
      dir -= towardsLedge.XZ();
    }
    WorldSpaceController.MaxMoveSpeed = Speed;
    if (LocalTime.TimeScale > 0) {
      WorldSpaceController.DesiredVelocity += LocalTime.TimeScale * Speed * dir;
      if (stick.sqrMagnitude > 0 && AbilityManager.HasTags(AbilityTag.CanRotate))
        WorldSpaceController.Forward = stick;
      if (Animator)
        Animator.SetFloat("Normalized Move Speed", stick.XZ().magnitude);
    }
  }
}