using System.Threading.Tasks;
using UnityEngine;

public class AISkeleton : TaskRunnerComponent {
  public AbilityManager AbilityManager;
  public DodgeAbility Dodge;
  public ThrowAbility Throw;
  public float WanderSpeed = 1f;
  public float AggroSpeed = 3f;
  public float PlayerAggroMinDistance = 6f;
  public float PlayerAggroMaxDistance = 10f;
  public float PlayerDodgeDistance = 3f;
  public float PlayerFacingThreshold = 0f;
  public Timeval ThrowCooldown = Timeval.FromSeconds(5);

  int ThrowTicksRemaining = 60*3;

  AIWander AIWander;
  AIChasePlayer AIChasePlayer;

  public void Start() {
    //base.Start();
    //NavMeshAgent.updatePosition = false;
    //NavMeshAgent.updateRotation = false;
    //NavMeshAgent.updateUpAxis = false;
    this.InitComponent(out AIWander);
    this.InitComponent(out AIChasePlayer);
    AIWander.enabled = true;
    AIChasePlayer.enabled = false;
    AIWander.Move.Speed = WanderSpeed;
    StartTask(Waiter.Repeat(Behavior));
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
    return IsAggro && --ThrowTicksRemaining <= 0;
  }

  bool ShouldDodge() {
    var target = PlayerManager.Instance.MobTarget;
    if (!target) return false;
    var sword = target.GetComponentInChildren<Sword>();
    return (sword && sword.IsRunning && IsInDodgeRange(target.transform));
  }

  bool IsInDodgeRange(Transform player) {
    var delta = transform.position - player.position;
    return delta.sqrMagnitude < PlayerDodgeDistance.Sqr() && Vector3.Dot(delta, player.forward) > PlayerFacingThreshold;
  }

  bool IsAggro = false;
  protected override void FixedUpdate() {
    base.FixedUpdate();
    var target = PlayerManager.Instance.MobTarget;
    if (target) {
      if (!IsAggro && (transform.position - target.transform.position).sqrMagnitude < PlayerAggroMinDistance.Sqr()) {
        IsAggro = true;
        AIWander.enabled = false;
        AIChasePlayer.enabled = true;
        AIWander.Move.Speed = AggroSpeed;
      } else if (IsAggro && (transform.position - target.transform.position).sqrMagnitude > PlayerAggroMaxDistance.Sqr()) {
        IsAggro = false;
        AIWander.enabled = true;
        AIChasePlayer.enabled = false;
        AIWander.Move.Speed = WanderSpeed;
      }
    }
  }
}