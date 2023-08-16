using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AbilityManager))]
[RequireComponent(typeof(WorldSpaceController))]
public class AIRaven : TaskRunnerComponent {
  public DiveBombAbility DiveBombAbility;
  public LayerMask SeeMask;
  public Timeval WindupDuration = Timeval.FromSeconds(1);
  public Timeval FlyHomeDuration = Timeval.FromSeconds(1);
  public float TerritoryRadius = 20;
  public float TurnSpeed = 180;
  public float FlyingSpeed = 10;

  Animator Animator;
  AbilityManager AbilityManager;
  WorldSpaceController WorldSpaceController;
  Vector3 HomePosition;
  Vector3 HomeForward;
  Vector3 TowardsHome => (HomePosition-transform.position).normalized;
  Vector3 TowardsTarget => Target ? (Target.position-transform.position).normalized : transform.forward;
  Transform Target => PlayerManager.Instance.MobTarget ? PlayerManager.Instance.MobTarget.transform : null;
  bool TargetInTerritory => Target && (Target.position - HomePosition).sqrMagnitude <= TerritoryRadius*TerritoryRadius;
  bool CanSeeTarget => Target && Target.IsVisibleFrom(transform.position+Vector3.up, SeeMask);
  bool AtHome() => (HomePosition-transform.position).sqrMagnitude <= .5f;
  bool ShouldAggro() => CanSeeTarget && TargetInTerritory;
  void MoveForward() {
    WorldSpaceController.MaxMoveSpeed = FlyingSpeed;
    WorldSpaceController.DesiredVelocity += FlyingSpeed * WorldSpaceController.Forward;
  }
  void WindupOnTarget() {
    if (!Target)
      return;
    var currentRotation = transform.rotation;
    var targetRotation = Quaternion.LookRotation(TowardsTarget, Vector3.up);
    var nextRotation = Quaternion.RotateTowards(currentRotation, targetRotation, Time.fixedDeltaTime * TurnSpeed);
    WorldSpaceController.Rotation = nextRotation;
  }
  void TurnTowardsHome() {
    if (TowardsHome.sqrMagnitude <= 0)
      return;
    var currentRotation = transform.rotation;
    var targetRotation = Quaternion.LookRotation(TowardsHome, Vector3.up);
    var nextRotation = Quaternion.RotateTowards(currentRotation, targetRotation, Time.fixedDeltaTime * TurnSpeed);
    WorldSpaceController.Rotation = nextRotation;
  }

  void Start() {
    this.InitComponent(out Animator);
    this.InitComponent(out AbilityManager);
    this.InitComponent(out WorldSpaceController);
    HomePosition = transform.position;
    HomeForward = transform.forward;
    Run(Sleep);
  }

  // Use this to change Tasks while avoiding true mutual recursion which blows the stack
  // This gives behavior very similar to a BehaviorTree
  void Run(TaskFunc f) {
    Debug.Log($"{gameObject.name} Running {f.Method.Name}");
    //StopAllTasks();
    StartTask(f);
  }

  async Task Sleep(TaskScope scope) {
    Animator.SetBool("Awake", false);
    await scope.Until(ShouldAggro);
    Animator.SetBool("Awake", true);
    WorldSpaceController.Unground();
    Run(Charge);
  }

  async Task ReturnHome(TaskScope scope) {
    await scope.Repeat(WindupDuration.Ticks, TurnTowardsHome);
    await scope.Any(
      Waiter.Until(AtHome),
      Waiter.Repeat(FlyHomeDuration.Ticks, MoveForward)
    );
    WorldSpaceController.Position = HomePosition;
    WorldSpaceController.Forward = HomeForward;
    Run(Sleep);
  }

  async Task Windup(TaskScope scope) {
    try {
      WorldSpaceController.DirectMove = true;
      await scope.Repeat(WindupDuration.Ticks, WindupOnTarget);
    } finally {
      WorldSpaceController.DirectMove = false;
    }
  }

  async Task Charge(TaskScope scope) {
    await scope.Run(Windup);
    AbilityManager.TryRun(DiveBombAbility.Main);
    await scope.Until(() => !DiveBombAbility.IsRunning);
    Run(ShouldAggro() ? Charge : ReturnHome);
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(Application.isPlaying ? HomePosition : transform.position, TerritoryRadius);
  }
}