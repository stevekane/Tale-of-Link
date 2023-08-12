using System.Threading.Tasks;
using UnityEngine;

public class AIRaven : MonoBehaviour {
  public AbilityManager AbilityManager;
  public WorldSpaceMove Move;
  public LayerMask SeeMask;
  public Timeval WindupDuration;
  public Timeval ChargeDuration;
  public Animator Animator;

  TaskScope Scope = new();

  public void Start() {
    Scope.Start(Waiter.Repeat(Behavior));
  }

  void OnDestroy() {
    Scope.Dispose();
  }

  async Task Behavior(TaskScope scope) {
    await scope.Until(() => PlayerManager.Instance.MobTarget && CanSee(PlayerManager.Instance.MobTarget.transform));
    Animator.SetTrigger("Awake");
    GetComponent<WorldSpaceController>().Unground();
    await scope.Delay(WindupDuration);
    await scope.Repeat(ChargeAtPlayer);
  }

  bool CanSee(Transform target) {
    return target.IsVisibleFrom(transform.position, SeeMask);
  }

  async Task ChargeAtPlayer(TaskScope scope) {
    await scope.Until(() => AbilityManager.CanRun(Move.Move));
    await scope.Any(
      Waiter.Delay(ChargeDuration),
      async s => {
        var player = PlayerManager.Instance.Player;
        var dir = (player.transform.position - transform.position).normalized;
        while (true) {
          AbilityManager.Run(Move.Move, dir);
          await scope.Tick();
        }
      });
  }
}