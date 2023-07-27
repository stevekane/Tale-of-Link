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

  AbilityAction ItemAction;
  AbilityAction SwordAction;
  Inputs Inputs;
  AbilityAction[] InteractPriority;

  void OnNewItemAbility((AbilityAction action, bool isSword) arg) {
    // TODO: UI for this
    // TODO: Only do it if Current is empty.
    if (arg.isSword) {
      SwordAction = arg.action;
    } else {
      ItemAction = arg.action;
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

    if (SwordAction != null && Inputs.Player.Sword.WasPerformedThisFrame() && AbilityManager.CanRun(SwordAction))  // TODO: charging
      AbilityManager.Run(SwordAction);
    if (ItemAction != null && Inputs.Player.Item1.WasPerformedThisFrame() && AbilityManager.CanRun(ItemAction))
      AbilityManager.Run(ItemAction);

    if (Inputs.Player.L1.WasPerformedThisFrame()) {
      GetComponent<Magic>().Consume(25);
    }
    if (Inputs.Player.L2.WasPerformedThisFrame()) {
      GetComponent<Magic>().Restore(101);
    }
  }
}