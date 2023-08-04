using System;
using System.Threading.Tasks;
using UnityEngine;

public enum MoveStyle {
  Stationary,
  Wander,
  Chase
}

public class AI : MonoBehaviour {
  //public WallSpaceController Controller;
  public AbilityManager AbilityManager;
  public WorldSpaceMove Move;
  public MoveStyle MoveStyle;
  public Timeval MoveDuration;
  public ClassicAbility Ability;
  //[SerializeField] NavMeshAgent NavMeshAgent;
  //[SerializeField] AbilityActionFieldReference AbilityActionRef;
  //[SerializeField] int MoveTicks = 60;
  //[SerializeField] string AreaName = "Walkable";

  TaskScope Scope;

  public void Start() {
    //base.Start();
    //NavMeshAgent.updatePosition = false;
    //NavMeshAgent.updateRotation = false;
    //NavMeshAgent.updateUpAxis = false;
    Scope = new();
    Scope.Start(Behavior);
  }

  void OnDestroy() {
    Scope.Dispose();
  }

  void Idle() { }

  Vector3 TargetPos;
  int NextChooseTick = 0;
  void Wander() {
    if (--NextChooseTick <= 0) {
      TargetPos = transform.position + 10 * UnityEngine.Random.onUnitSphere.XZ();
      NextChooseTick = MoveDuration.Ticks;
    }
    var dir = (TargetPos - transform.position).normalized;
    AbilityManager.Run(Move.Move, dir);
  }

  void Chase() {
    var player = FindObjectOfType<Player>().transform;
    var dir = (player.position - transform.position).normalized;
    AbilityManager.Run(Move.Move, dir);
  }

  public virtual async Task MaybeMove(TaskScope scope) {
    Action moveBehavior = MoveStyle switch {
      MoveStyle.Wander => Wander,
      MoveStyle.Chase => Chase,
      _ => Idle,
    };
    while (true) {
      if (AbilityManager.CanRun(Move.Move))
        moveBehavior();
      await scope.Tick();
    }
  }

  public virtual async Task MaybeUseAbility(TaskScope scope) {
    if (Ability && AbilityManager.CanRun(Ability.Main)) {
      AbilityManager.Run(Ability.Main);
      await scope.Until(() => !Ability.IsRunning);
    }
  }

  public virtual async Task Behavior(TaskScope scope) {
    await scope.All(
      Waiter.Repeat(MaybeMove),
      Waiter.Repeat(MaybeUseAbility)
    );
  }
}