using UnityEngine;

public class WallSpaceMove : SimpleAbility {
  [SerializeField] float Speed = 3;
  [SerializeField] WallSpaceController WallSpaceController;

  public AbilityAction<Vector3> Move;

  void Awake() {
    Move.Ability = this;
    Move.Listen(OnMove);
  }

  void OnMove(Vector3 v) {
    WallSpaceController.Move(Speed * v.x);
  }
}