using System.Threading.Tasks;
using UnityEngine;

public class AngryTile : TaskRunnerComponent {
  [SerializeField] Timeval WindupDuration = Timeval.FromSeconds(1);
  [SerializeField] Rigidbody Rigidbody;
  [SerializeField] Collider DormantCollider;
  [SerializeField] ContactHitbox ContactHitbox;
  [SerializeField] Rotator Rotator;
  [SerializeField] float KamikazeSpeed = 15;
  [SerializeField] float KamikazeHeight = 1;
  [SerializeField] float KamikazeSpinRate = 1080;

  Vector3 TargetDirection => PlayerManager.Instance.MobTarget
    ? (PlayerManager.Instance.MobTarget.transform.position-transform.position).XZ().normalized
    : transform.forward.XZ();

  public bool Angry { get; private set; } = false;

  bool IsAngry() => Angry;

  [ContextMenu("Anger")]
  public void Anger() {
    Angry = true;
  }

  void Start() {
    StartTask(LieInWaiting);
  }

  async Task LieInWaiting(TaskScope scope) {
    DormantCollider.enabled = true;
    ContactHitbox.IsActive = false;
    await scope.Until(IsAngry);
    StartTask(Kamikaze);
  }

  async Task Kamikaze(TaskScope scope) {
    DormantCollider.enabled = false;
    ContactHitbox.IsActive = true;
    Rigidbody.isKinematic = true;
    Rotator.DegreesPerSecond = 0;
    var dy = KamikazeHeight / (float)WindupDuration.Ticks;
    var dtheta = KamikazeSpinRate / (float)WindupDuration.Ticks;
    for (var i = 0; i < WindupDuration.Ticks; i++) {
      if (PlayerManager.Instance.MobTarget) {
        transform.rotation = Quaternion.LookRotation(TargetDirection, Vector3.up);
      }
      transform.position += new Vector3(0,dy,0);
      Rotator.DegreesPerSecond += dtheta;
      await scope.Tick();
    }
    transform.forward = TargetDirection;
    Rigidbody.useGravity = false;
    Rigidbody.isKinematic = false;
    Rigidbody.AddForce(KamikazeSpeed * TargetDirection, ForceMode.VelocityChange);
    await scope.Seconds(3);
    Destroy(gameObject);
  }
}