using System;
using UnityEngine;

[DefaultExecutionOrder(-400)]
public class InputHandler : MonoBehaviour {
  Inputs Inputs;

  public Action<Vector3> OnMove;
  public Action OnSword;
  public Action OnHammer;

  void Awake() {
    Inputs = new();
    Inputs.Enable();
  }

  void OnDestroy() {
    Inputs.Dispose();
  }

  // TODO: Task.Start runs async, so you could start 2 abilities on the same frame.
  void FixedUpdate() {
    var move = Inputs.Player.Move.ReadValue<Vector2>();
    OnMove?.Invoke(move.XZ());
    if (Inputs.Player.Sword.WasPerformedThisFrame())  // TODO: charging
      OnSword?.Invoke();
    if (Inputs.Player.Hammer.WasPerformedThisFrame())
      OnHammer?.Invoke();
  }
}