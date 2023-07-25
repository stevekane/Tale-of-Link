using System;
using UnityEngine;

[DefaultExecutionOrder(-400)]
public class InputHandler : MonoBehaviour {
  Inputs Inputs;

  public Action<Vector3> OnMove;
  public Action OnSouth;
  public Action OnWest;

  Action CurrentOnSouth;
  Action CurrentOnWest;

  public void BindWest(Action onWest) {
    OnWest -= CurrentOnWest;
    CurrentOnWest = onWest;
    OnWest += onWest;
  }

  public void BindSouth(Action onSouth) {
    OnSouth -= CurrentOnSouth;
    CurrentOnSouth = onSouth;
    OnSouth += onSouth;
  }

  void OnNewItemAbility(TmpAbility ability) {
    // TODO: UI for this
    // TODO: Only do it if Current is empty.
    switch (ability.DefaultButtonAssignment) {
    case TmpAbility.Buttons.South: BindSouth(ability.TryStart); break;
    case TmpAbility.Buttons.West: BindWest(ability.TryStart); break;
    default: Debug.Assert(false, "Not impl"); break;
    }
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
      OnSouth?.Invoke();
    if (Inputs.Player.West.WasPerformedThisFrame())
      OnWest?.Invoke();
  }
}