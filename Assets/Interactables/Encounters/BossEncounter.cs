using System.Threading.Tasks;
using UnityEngine;

public class BossEncounter : TaskRunnerComponent {
  [SerializeField] GameObject Boss;
  [SerializeField] Timeval TauntDelay = Timeval.FromSeconds(2);
  [SerializeField] Timeval DeadDelay = Timeval.FromSeconds(2);
  [SerializeField] GameObject FireworksVFX;
  [SerializeField] float FireworksRadius = 5;
  [SerializeField] float FireworksPeriodMin = .1f;
  [SerializeField] float FireworksPeriodMax = .9f;
  [SerializeField] AudioClip BossMusic;
  [SerializeField] AudioClip VictoryMusic;

  void SetEnabled(AbilityManager abilityManager, bool enabled) {
    abilityManager.Abilities.ForEach(abilityManager.Stop);
    abilityManager.SetTag(AbilityTag.CanAttack | AbilityTag.CanMove | AbilityTag.CanRotate | AbilityTag.CanUseItem, enabled);
  }
  void Enable(GameObject o) => SetEnabled(o.GetComponent<AbilityManager>(), true);
  void Disable(GameObject o) => SetEnabled(o.GetComponent<AbilityManager>(), false);
  bool IsDying(GameObject o) => o.GetComponent<Killable>().Dying;
  bool IsDead(GameObject o) => o == null;

  public void Run() => RunTask(Encounter);

  async Task Encounter(TaskScope scope) {
    var player = PlayerManager.Instance.Player;
    await scope.Until(() => player.GetComponent<WorldSpaceController>().IsGrounded);
    TimeManager.Instance.Frozen = true;
    TimeManager.Instance.IgnoreFreeze.Add(LocalTime);
    TimeManager.Instance.IgnoreFreeze.Add(player.GetComponent<LocalTime>());
    TimeManager.Instance.IgnoreFreeze.Add(Boss.GetComponent<LocalTime>());
    Disable(player.gameObject);
    Disable(Boss);
    AudioManager.Instance.MusicSource.Play(BossMusic);
    player.GetComponent<Animator>().SetTrigger("Alert");
    CameraManager.Instance.FocusOn(Boss.transform);
    Boss.GetComponent<Animator>().SetTrigger("Taunt");
    await scope.Delay(TauntDelay);
    // TODO: Play encounter music
    TimeManager.Instance.Frozen = false;
    CameraManager.Instance.UnFocus();
    Enable(Boss);
    Enable(player.gameObject);
    await scope.Until(() => IsDying(Boss));
    AudioManager.Instance.MusicSource.Stop();

    CameraManager.Instance.FocusOn(Boss.transform);
    Disable(player.gameObject);
    await scope.Until(() => IsDead(Boss));
    AudioManager.Instance.MusicSource.Play(VictoryMusic);
    CameraManager.Instance.FocusOn(player.transform);
    player.GetComponent<Animator>().SetBool("Collecting", true);
    await scope.Delay(DeadDelay);
    await scope.Any(
      Waiter.Seconds(10),
      Waiter.Repeat(Fireworks));
  }

  async Task Fireworks(TaskScope scope) {
    var pos = PlayerManager.Instance.Player.transform.position + FireworksRadius * UnityEngine.Random.onUnitSphere;
    //pos.y = Mathf.Abs(pos.y);
    Destroy(Instantiate(FireworksVFX, pos, Quaternion.identity), 3f);
    await scope.Seconds(UnityEngine.Random.Range(FireworksPeriodMin, FireworksPeriodMax));
  }
}