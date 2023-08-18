using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BossEncounter : TaskRunnerComponent {
  [SerializeField] GameObject Boss;
  [SerializeField] Timeval TauntDelay = Timeval.FromSeconds(2);
  [SerializeField] Timeval DeadDelay = Timeval.FromSeconds(2);

  void SetEnabled(AbilityManager abilityManager, bool enabled) {
    abilityManager.Abilities.ForEach(abilityManager.Stop);
    abilityManager.SetTag(AbilityTag.CanAttack | AbilityTag.CanMove | AbilityTag.CanRotate | AbilityTag.CanUseItem, enabled);
  }
  void Enable(GameObject o) => SetEnabled(o.GetComponent<AbilityManager>(), true);
  void Disable(GameObject o) => SetEnabled(o.GetComponent<AbilityManager>(), false);
  bool IsDead(GameObject o) => o == null;

  public void Run() => StartTask(Encounter);

  async Task Encounter(TaskScope scope) {
    await scope.Until(() => PlayerManager.Instance.Player.GetComponent<WorldSpaceController>().IsGrounded);
    TimeManager.Instance.Frozen = true;
    TimeManager.Instance.IgnoreFreeze.Add(LocalTime);
    TimeManager.Instance.IgnoreFreeze.Add(PlayerManager.Instance.Player.GetComponent<LocalTime>());
    TimeManager.Instance.IgnoreFreeze.Add(Boss.GetComponent<LocalTime>());
    Disable(PlayerManager.Instance.Player.gameObject);
    Disable(Boss);
    PlayerManager.Instance.Player.GetComponent<Animator>().SetTrigger("Alert");
    CameraManager.Instance.FocusOn(Boss.transform);
    Boss.GetComponent<Animator>().SetTrigger("Taunt");
    await scope.Delay(TauntDelay);
    // TODO: Play encounter music
    TimeManager.Instance.Frozen = false;
    CameraManager.Instance.UnFocus();
    Enable(Boss);
    Enable(PlayerManager.Instance.Player.gameObject);
    await scope.Until(() => IsDead(Boss));
    await scope.Delay(DeadDelay);

    TimeManager.Instance.Frozen = true;
    Disable(PlayerManager.Instance.Player.gameObject);
    // TODO: fireworks
    Enable(PlayerManager.Instance.Player.gameObject);
    TimeManager.Instance.IgnoreFreeze.Clear();
    TimeManager.Instance.Frozen = false;
  }
}