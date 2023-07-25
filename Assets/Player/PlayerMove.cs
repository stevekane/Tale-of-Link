using UnityEngine;

public class PlayerMove : MonoBehaviour {
  public Player Player { get; internal set; }
  public bool CanMove => true; // TODO: Any ability IsRunning
  //public bool CanMove => !Player.Sword.IsRunning && !Player.Hammer.IsRunning;

  public float Speed = 10f;
  public float RotationSpeed = 180f;

  Vector3 Velocity;
  Quaternion Rotation;

  void Awake() {
    GetComponent<InputHandler>().OnMove += OnMove;
  }

  void FixedUpdate() {
    if (!CanMove)
      return;
    transform.position += Time.fixedDeltaTime * Velocity;
    transform.rotation = Quaternion.RotateTowards(transform.rotation, Rotation, Time.fixedDeltaTime * RotationSpeed);
  }

  void OnMove(Vector3 v) {
    Velocity = Speed * v;
    Rotation = Quaternion.LookRotation(v.sqrMagnitude > 0 ? v.normalized : transform.forward);
  }
}