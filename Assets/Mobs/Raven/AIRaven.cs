using System.Threading.Tasks;
using UnityEngine;

public class AIRaven : MonoBehaviour {
  public AbilityManager AbilityManager;
  public WorldSpaceMove Move;
  public LayerMask SeeMask;
  public Timeval WindupDuration;
  public Timeval ChargeDuration;
  public Animator Animator;
  public float TerritoryRadius;
  public float TurnSpeed;

  Vector3 ChargeDirection;
  Vector3 TerritoryCenter;

  TaskScope Scope = new();
  bool CanSee(Transform target) => target.IsVisibleFrom(transform.position, SeeMask);
  bool InTerritory(Transform target) => (target.position-transform.position).sqrMagnitude <= TerritoryRadius*TerritoryRadius;

  public void Start() {
    TerritoryCenter = transform.position;
    Scope.Start(Waiter.Repeat(Behavior));
  }

  void OnDestroy() {
    Scope.Dispose();
  }

  async Task ReturnHome(TaskScope scope) {

  }

  async Task Behavior(TaskScope scope) {
    await scope.Until(() => PlayerManager.Instance.MobTarget && CanSee(PlayerManager.Instance.MobTarget.transform));
    Animator.SetTrigger("Awake");
    GetComponent<WorldSpaceController>().Unground();
    await scope.Delay(WindupDuration);
    await scope.Repeat(ChargeAtPlayer);
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