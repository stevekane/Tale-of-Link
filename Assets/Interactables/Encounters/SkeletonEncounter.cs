using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SkeletonEncounter : TaskRunnerComponent {
  [SerializeField] VerticalDoor VerticalDoor;
  [SerializeField] GameObject[] Skeletons;
  [SerializeField] TeleporterPad TeleporterPad;
  [SerializeField] PathController ElevatorController;
  [SerializeField] Transform SkeletonFocus;
  [SerializeField] Transform ElevatorFocus;
  [SerializeField] Transform PortalFocus;
  [SerializeField] Timeval SkeletonTauntDelay = Timeval.FromSeconds(2);
  [SerializeField] Timeval SkeletonResponseDelay = Timeval.FromSeconds(2);
  [SerializeField] Timeval SkeletonsDeadDelay = Timeval.FromSeconds(2);
  [SerializeField] Timeval ShowPortalDelay = Timeval.FromSeconds(2);
  [SerializeField] Timeval ShowElevatorDelay = Timeval.FromSeconds(2);

  void SetEnabled(AbilityManager abilityManager, bool enabled) {
    abilityManager.Abilities.ForEach(abilityManager.Stop);
    abilityManager.SetTag(AbilityTag.CanAttack | AbilityTag.CanMove | AbilityTag.CanRotate | AbilityTag.CanUseItem, enabled);
  }
  void Enable(GameObject o) => SetEnabled(o.GetComponent<AbilityManager>(), true);
  void Disable(GameObject o) => SetEnabled(o.GetComponent<AbilityManager>(), false);
  bool IsDead(GameObject o) => o == null;
  bool AllSkeletonsDead() => Skeletons.All(IsDead);

  public void Run() => StartTask(Encounter);

  async Task Encounter(TaskScope scope) {
    TimeManager.Instance.Frozen = true;
    TimeManager.Instance.IgnoreFreeze.Add(LocalTime);
    TimeManager.Instance.IgnoreFreeze.Add(PlayerManager.Instance.Player.GetComponent<LocalTime>());
    Skeletons.ForEach(skeleton => TimeManager.Instance.IgnoreFreeze.Add(skeleton.GetComponent<LocalTime>()));
    Disable(PlayerManager.Instance.Player.gameObject);
    VerticalDoor.Close();
    PlayerManager.Instance.Player.GetComponent<Animator>().SetTrigger("Alert");
    CameraManager.Instance.FocusOn(SkeletonFocus);
    Skeletons[0].GetComponent<Animator>().SetTrigger("Taunt");
    await scope.Delay(SkeletonTauntDelay);
    Skeletons[1..].ForEach(skeleton => skeleton.GetComponent<Animator>().SetTrigger("Taunt"));
    await scope.Delay(SkeletonResponseDelay);
    // TODO: Play encounter music
    TimeManager.Instance.Frozen = false;
    CameraManager.Instance.UnFocus();
    Skeletons.ForEach(Enable);
    Enable(PlayerManager.Instance.Player.gameObject);
    await scope.Until(AllSkeletonsDead);
    await scope.Delay(SkeletonsDeadDelay);
    TimeManager.Instance.Frozen = true;
    Disable(PlayerManager.Instance.Player.gameObject);
    CameraManager.Instance.FocusOn(PortalFocus);
    TeleporterPad.gameObject.SetActive(true);
    TeleporterPad.Exit.gameObject.SetActive(true);
    TeleporterPad.Show();
    TeleporterPad.Exit.Show();
    await scope.Delay(ShowPortalDelay);
    CameraManager.Instance.FocusOn(ElevatorFocus);
    TimeManager.Instance.IgnoreFreeze.Add(ElevatorController.GetComponent<LocalTime>());
    ElevatorController.Activate();
    // TODO: Play unlock Jingle
    await scope.Delay(ShowElevatorDelay);
    CameraManager.Instance.UnFocus();
    VerticalDoor.Open();
    Enable(PlayerManager.Instance.Player.gameObject);
    TimeManager.Instance.IgnoreFreeze.Clear();
    TimeManager.Instance.Frozen = false;
    TeleporterPad.Activate();
    TeleporterPad.Exit.Activate();
  }
}