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

  async Task Idle(TaskScope scope) {
    await scope.Ticks(MoveDuration.Ticks);
  }

  async Task Wander(TaskScope scope) {
    var targetPosition = transform.position + 10 * Random.onUnitSphere.XZ();
    for (var i = 0; i < MoveDuration.Ticks; i++) {
      if (AbilityManager.CanRun(Move.Move)) {
        var dir = (targetPosition - transform.position).normalized;
        AbilityManager.Run(Move.Move, dir);
      }
      await scope.Tick();
    }
  }

  async Task Chase(TaskScope scope) {
    var player = FindObjectOfType<Player>().transform;
    for (var i = 0; i < MoveDuration.Ticks; i++) {
      if (AbilityManager.CanRun(Move.Move)) {
        var dir = (player.position - transform.position).normalized;
        AbilityManager.Run(Move.Move, dir);
      }
      await scope.Tick();
    }
  }

  public virtual async Task Behavior(TaskScope scope) {
    while (true) {
      var moveBehavior = MoveStyle switch {
        MoveStyle.Wander => Wander(scope),
        MoveStyle.Chase => Chase(scope),
        _ => Idle(scope),
      };
      await moveBehavior;
      if (Ability && AbilityManager.CanRun(Ability.Main)) {
        AbilityManager.Run(Ability.Main);
        await scope.Until(() => !Ability.IsRunning);
      }
      await scope.Tick();
    }
  }

  //public override void BeforeCharacterUpdate(float deltaTime) {
  //  base.BeforeCharacterUpdate(deltaTime);
  //  NavMeshAgent.nextPosition = transform.position;
  //}

  //public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
  //  base.UpdateRotation(ref currentRotation, deltaTime);
  //  if (Velocity.XZ().sqrMagnitude > 0) {
  //    currentRotation = Quaternion.LookRotation(Velocity.XZ());
  //  }
  //}

  //public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
  //  base.UpdateVelocity(ref currentVelocity, deltaTime);
  //  if (ScriptedVelocity.sqrMagnitude > 0) {
  //    currentVelocity = ScriptedVelocity;
  //  } else {
  //    currentVelocity = Velocity;
  //  }
  //  ScriptedVelocity = Vector3.zero;
  //}
}