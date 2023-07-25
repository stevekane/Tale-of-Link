using System;
using UnityEngine;

[DefaultExecutionOrder(-400)]
public class InputHandler : MonoBehaviour {
  Inputs Inputs;

  public Action<Vector3> OnMove;
  public Action OnSword;
  public Action OnWest;

  Action CurrentOnWest;

  public void BindWest(Action onWest) {
    OnWest -= CurrentOnWest;
    CurrentOnWest = onWest;
    OnWest += onWest;
  }

  void OnNewItemAbility(TmpAbility ability) {
    // TODO: UI for this
    // TODO: Only do it if CurrentOnWest is empty.
    BindWest(ability.TryStart);
  }

  void Awake() {
    Inputs = new();
    Inputs.Enable();
    GetComponent<Inventory>().OnNewItemAbility += OnNewItemAbility;
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
    if (Inputs.Player.West.WasPerformedThisFrame())
      OnWest?.Invoke();
  }
}