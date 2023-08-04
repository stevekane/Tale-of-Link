using System.Threading.Tasks;
using UnityEngine;

public class AISkeleton : MonoBehaviour {
  public AbilityManager AbilityManager;
  public DodgeAbility Dodge;
  public ThrowAbility Throw;
  public float PlayerMaxDistance = 3f;
  public float PlayerFacingThreshold = 0f;
  public Timeval ThrowCooldown = Timeval.FromSeconds(5);

  int ThrowTicksRemaining = 60*3;

  TaskScope Scope;

  public void Start() {
    //base.Start();
    //NavMeshAgent.updatePosition = false;
    //NavMeshAgent.updateRotation = false;
    //NavMeshAgent.updateUpAxis = false;
    Scope = new();
    Scope.Start(Waiter.Repeat(Behavior));
  }

  void OnDestroy() {
    Scope.Dispose();
  }

  async Task Behavior(TaskScope scope) {
    if (ShouldDodge() && AbilityManager.CanRun(Dodge.Main)) {
      await AbilityManager.RunUntilDone(Dodge.Main)(scope);
      await scope.Until(() => AbilityManager.CanRun(Throw.Main));
      await AbilityManager.RunUntilDone(Throw.Main)(scope);
    } else if (ShouldThrow() && AbilityManager.CanRun(Throw.Main)) {
      await AbilityManager.RunUntilDone(Throw.Main)(scope);
      ThrowTicksRemaining = ThrowCooldown.Ticks;
    }
  }

  bool ShouldThrow() {
    return --ThrowTicksRemaining <= 0;
  }

  bool ShouldDodge() {
    var player = PlayerManager.Instance.Player.GetComponent<AbilityManager>();
    var sword = player.GetComponentInChildren<Sword>();
    return (sword && sword.IsRunning && IsInRange(player.transform));
  }

  bool IsInRange(Transform player) {
    var delta = transform.position - player.position;
    return delta.sqrMagnitude < PlayerMaxDistance.Sqr() && Vector3.Dot(delta, player.forward) > PlayerFacingThreshold;
  }
}