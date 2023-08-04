using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(1)]
public class InputManager : MonoBehaviour {
  [SerializeField] AbilityManager AbilityManager;
  [SerializeField] EnterWallSpace EnterWallSpace;
  [SerializeField] ExitWallSpace ExitWallSpace;
  [SerializeField] WorldSpaceMove WorldSpaceMove;
  [SerializeField] WallSpaceMove WallSpaceMove;
  [SerializeField] OpenDoorAbility OpenDoor;

  public UnityAction<string> OnInteractChange;

  AbilityAction ItemAction;
  AbilityAction SwordAction;
  Inputs Inputs;
  AbilityAction[] InteractPriority;
  AbilityAction CurrentInteraction;

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
      OpenDoor.Main,
    };
    GetComponent<Inventory>().OnNewItemAbility += OnNewItemAbility;
  }

  void OnDestroy() {
    Inputs.Disable();
    Inputs.Dispose();
  }

  void FixedUpdate() {
    var currentInteraction = InteractPriority.FirstOrDefault(AbilityManager.CanRun);
    var interactMessage = currentInteraction == null ? "" : currentInteraction.Ability.Name;
    if (currentInteraction != CurrentInteraction) {
      CurrentInteraction = currentInteraction;
      OnInteractChange?.Invoke(interactMessage);
    }

    var interact = Inputs.Player.Interact.WasPerformedThisFrame();
    if (interact && currentInteraction != null && AbilityManager.CanRun(currentInteraction)) {
      AbilityManager.Run(currentInteraction);
    }

    var move = Inputs.Player.Move.ReadValue<Vector2>();
    if (AbilityManager.CanRun(WallSpaceMove.Move)) {
      AbilityManager.Run(WallSpaceMove.Move, move);
    } else if (AbilityManager.CanRun(WorldSpaceMove.Move)) {
      AbilityManager.Run(WorldSpaceMove.Move, new(move.x, 0, move.y));
    }

    if (SwordAction != null && Inputs.Player.Sword.WasPerformedThisFrame() && AbilityManager.CanRun(SwordAction)) {
      (SwordAction.Ability as Sword).Direction = new (move.x, 0, move.y);
      AbilityManager.Run(SwordAction);
    }
    if (ItemAction != null && Inputs.Player.Item1.WasPerformedThisFrame() && AbilityManager.CanRun(ItemAction))
      AbilityManager.Run(ItemAction);

    if (Inputs.Player.L1.WasPerformedThisFrame()) {
      GetComponent<Magic>().Consume(25);
    }
    if (Inputs.Player.L2.WasPerformedThisFrame()) {
      GetComponent<WorldSpaceController>().Launch(700f * Vector3.up);
    }
  }
}