using System;
using UnityEngine;

[DefaultExecutionOrder(-400)]
public class InputHandler : MonoBehaviour {
  Inputs Inputs;

  public AbilityManager AbilityManager;
  public Action<Vector3> OnMove;
  public Action OnSouth;
  public Action OnWest;

  Action CurrentOnSouth;
  Action CurrentOnWest;

  public void BindWest(AbilityAction action) {
    OnWest -= CurrentOnWest;
    CurrentOnWest = () => AbilityManager.Run(action);
    OnWest += CurrentOnWest;
  }

  public void BindSouth(AbilityAction action) {
    OnSouth -= CurrentOnSouth;
    CurrentOnSouth = () => AbilityManager.Run(action);
    OnSouth += CurrentOnSouth;
  }

  void OnNewItemAbility(IItemAbility ability) {
    // TODO: UI for this
    // TODO: Only do it if Current is empty.
    switch (ability.DefaultButtonAssignment) {
    case IItemAbility.Buttons.South: BindSouth(ability.Action); break;
    case IItemAbility.Buttons.West: BindWest(ability.Action); break;
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
    if (Inputs.Player.South.WasPerformedThisFrame())  // TODO: charging
      OnSouth?.Invoke();
    if (Inputs.Player.West.WasPerformedThisFrame())
      OnWest?.Invoke();
  }
}