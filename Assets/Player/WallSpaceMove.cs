using UnityEngine;

public class WallSpaceMove : SimpleAbility {
  [SerializeField] float Speed = 3;
  [SerializeField] WallMover WallMover;

  public AbilityAction<Vector3> Move;

  void Awake() {
    Move.Ability = this;
    Move.Listen(OnMove);
  }

  void OnMove(Vector3 v) {
    WallMover.Move(Speed * v.x);
  }
}