using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AngryTileEncounter : TaskRunnerComponent {
  [SerializeField] VerticalDoor VerticalDoor;
  [SerializeField] AngryTileSet AngryTileSet;
  [SerializeField] GameObject[] AngryTiles;
  [SerializeField] PathController ElevatorController;
  [SerializeField] Timeval FocusDelay = Timeval.FromSeconds(2);

  bool IsDead(GameObject go) => go == null;
  bool AllTilesDead() => AngryTiles.All(IsDead);
  void SetEnabled(AbilityManager abilityManager, bool enabled) {
    abilityManager.Abilities.ForEach(abilityManager.Stop);
    abilityManager.SetTag(AbilityTag.CanAttack | AbilityTag.CanMove | AbilityTag.CanRotate | AbilityTag.CanUseItem, enabled);
  }
  void Enable(GameObject o) => SetEnabled(o.GetComponent<AbilityManager>(), true);
  void Disable(GameObject o) => SetEnabled(o.GetComponent<AbilityManager>(), false);

  public void Run() => RunTask(Encounter);

  async Task Encounter(TaskScope scope) {
    // Show the closed door
    TimeManager.Instance.Frozen = true;
    TimeManager.Instance.IgnoreFreeze.Add(LocalTime);
    TimeManager.Instance.IgnoreFreeze.Add(PlayerManager.Instance.Player.GetComponent<LocalTime>());
    TimeManager.Instance.IgnoreFreeze.Add(ElevatorController.GetComponent<LocalTime>());
    VerticalDoor.Close();
    PlayerManager.Instance.Player.GetComponent<Animator>().SetTrigger("Alert");
    Disable(PlayerManager.Instance.Player.gameObject);
    CameraManager.Instance.FocusOn(VerticalDoor.transform);
    await scope.Delay(FocusDelay);

    // Allow the encounter to occur
    AngryTileSet.Anger();
    TimeManager.Instance.Frozen = false;
    Enable(PlayerManager.Instance.Player.gameObject);
    CameraManager.Instance.UnFocus();
    await scope.Until(AllTilesDead);

    // Show the newly active elevator
    TimeManager.Instance.Frozen = true;
    Disable(PlayerManager.Instance.Player.gameObject);
    CameraManager.Instance.FocusOn(ElevatorController.transform);
    ElevatorController.Activate();
    await scope.Delay(FocusDelay);

    // Cleanup
    TimeManager.Instance.IgnoreFreeze.Clear();
    TimeManager.Instance.Frozen = false;
    Enable(PlayerManager.Instance.Player.gameObject);
    CameraManager.Instance.UnFocus();
    VerticalDoor.Open();
  }
}