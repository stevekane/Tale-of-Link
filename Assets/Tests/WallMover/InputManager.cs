using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-700)]
public class InputManager : MonoBehaviour {
  [SerializeField] AbilityManager AbilityManager;
  [SerializeField] EnterWallSpace EnterWallSpace;
  [SerializeField] ExitWallSpace ExitWallSpace;
  [SerializeField] WorldSpaceMove WorldSpaceMove;
  [SerializeField] WallSpaceMove WallSpaceMove;

  public UnityAction<string> OnInteractChange;

  AbilityAction WestAction;
  AbilityAction SouthAction;

  Inputs Inputs;
  AbilityAction[] InteractPriority;

  public void BindWest(AbilityAction action) {
    WestAction = action;
  }

  public void BindSouth(AbilityAction action) {
    SouthAction = action;
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
    // Steve - Done in code for now because missing editor support for ability action references
    InteractPriority = new[] {
      EnterWallSpace.Main,
      ExitWallSpace.Main,
    };
    GetComponent<Inventory>().OnNewItemAbility += OnNewItemAbility;
  }

  void OnDestroy() {
    Inputs.Disable();
    Inputs.Dispose();
  }

  void FixedUpdate() {
    var interactAbilityAction = InteractPriority.FirstOrDefault(ability => AbilityManager.CanRun(ability));
    var interactMessage = interactAbilityAction == null ? "" : interactAbilityAction.Ability.Name;
    // TODO: Only fire this when something changes... kinda overkill
    OnInteractChange?.Invoke(interactMessage);

    var interact = Inputs.Player.Interact.WasPerformedThisFrame();
    if (interact && interactAbilityAction != null && AbilityManager.CanRun(interactAbilityAction)) {
      AbilityManager.Run(interactAbilityAction);
    }

    var move = Inputs.Player.Move.ReadValue<Vector2>();
    if (AbilityManager.CanRun(WallSpaceMove.Move)) {
      AbilityManager.Run(WallSpaceMove.Move, move);
    } else if (AbilityManager.CanRun(WorldSpaceMove.Move)) {
      AbilityManager.Run(WorldSpaceMove.Move, new(move.x, 0, move.y));
    }

    if (Inputs.Player.South.WasPerformedThisFrame() && AbilityManager.CanRun(SouthAction))  // TODO: charging
      AbilityManager.Run(SouthAction);
    if (Inputs.Player.West.WasPerformedThisFrame() && AbilityManager.CanRun(WestAction))
      AbilityManager.Run(WestAction);

    if (Inputs.Player.L1.WasPerformedThisFrame()) {
      GetComponent<Magic>().Consume(25);
    }
    if (Inputs.Player.L2.WasPerformedThisFrame()) {
      GetComponent<Magic>().Restore(101);
    }
  }
}