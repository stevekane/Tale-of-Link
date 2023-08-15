using System;
using System.Threading.Tasks;
using UnityEngine;

public class KillBoss : ClassicAbility {
  [SerializeField] Timeval ExplodeDuration = Timeval.FromSeconds(2);
  [SerializeField] ParticleSystem DeathVFX;

  MoveTail MoveTail;
  private void Start() {
    AbilityManager.InitComponent(out MoveTail);
  }

  public override async Task MainAction(TaskScope scope) {
    var hearts = AbilityManager.GetComponent<Hearts>();
    var killable = AbilityManager.GetComponent<Killable>();
    var animator = AbilityManager.GetComponent<Animator>();
    try {
      killable.Dying = true;
      AbilityManager.SetTag(AbilityTag.CanMove, false);
      AbilityManager.SetTag(AbilityTag.CanAttack, false);
      AbilityManager.SetTag(AbilityTag.CanRotate, false);
      AbilityManager.SetTag(AbilityTag.CanUseItem, false);
      animator.SetTrigger("Kill");

      for (var i = MoveTail.TailBones.Length-1; i >= 0; i--) {
        var t = MoveTail.TailBones[i];
        scope.Start(s => Shake(s, t));
      }
      scope.Start(s => Shake(s, MoveTail.Head));

      for (var i = MoveTail.TailBones.Length-1; i >= 0; i--)
        await Explode(scope, MoveTail.TailBones[i]);
      await Explode(scope, MoveTail.Head);

      Destroy(killable.gameObject);
    } finally {
      if (killable)
        killable.Dead = true;
    }
  }

  async Task Shake(TaskScope scope, Transform transform) {
    var frequency = 80f;
    var amplitude = .02f;
    var t = UnityEngine.Random.Range(0f, 1f);
    var localScale = transform.localScale;
    var localPos = transform.localPosition;
    while (true) {
      t += Time.fixedDeltaTime*frequency;
      transform.localScale = localScale * (1f + amplitude*Mathf.Sin(t));
      transform.localPosition = localPos + amplitude * new Vector3(Mathf.Sin(t), 0, Mathf.Sin(t*.91f));
      await scope.Tick();
    }
  }

  async Task Explode(TaskScope scope, Transform transform) {
    await scope.Ticks(ExplodeDuration.Ticks);
    Destroy(Instantiate(DeathVFX, transform.position+Vector3.up, transform.rotation), ExplodeDuration.Seconds);
    transform.gameObject.SetActive(false);
  }
}