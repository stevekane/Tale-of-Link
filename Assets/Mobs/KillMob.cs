using System;
using System.Threading.Tasks;
using UnityEngine;

public class KillMob : ClassicAbility {
  [SerializeField] bool HasFairy;
  //[SerializeField] float FadeSpeed = 1;
  [SerializeField] Timeval DyingDuration = Timeval.FromSeconds(2);
  public GameObject Model;

  public override async Task MainAction(TaskScope scope) {
    try {
      var hearts = AbilityManager.GetComponent<Hearts>();
      var killable = AbilityManager.GetComponent<Killable>();
      var animator = AbilityManager.GetComponent<Animator>();
      var squish = hearts.LastHitType == HitConfig.Types.Hammer;
      killable.Dying = true;
      AbilityManager.SetTag(AbilityTag.CanMove, false);
      AbilityManager.SetTag(AbilityTag.CanAttack, false);
      AbilityManager.SetTag(AbilityTag.CanRotate, false);
      //CameraManager.Instance.FadeOut(FadeSpeed);
      if (squish) {
        scope.Start(Squish);
      } else if (animator) {
        animator.SetTrigger("Dying");
      }
      await scope.Ticks(DyingDuration.Ticks);
      if (HasFairy) {
        animator.SetTrigger("Reviving");
        killable.Spawning = true;
        hearts.ChangeCurrent(hearts.Total-hearts.Current);
        await scope.Ticks(DyingDuration.Ticks);
        killable.Alive = true;
        //CameraManager.Instance.FadeIn(FadeSpeed);
      } else {
        killable.Dead = true;
        Destroy(killable.gameObject);
      }
    } catch (Exception e){
      throw e;
    }
  }

  async Task Squish(TaskScope scope) {
    const int SquishTicks = 15;
    for (int i = 0; i < SquishTicks; i++) {
      var t = (float)i/SquishTicks;
      Model.transform.localScale = new Vector3(1 + t*.5f, 1 - t, 1 + t*.5f);
      await scope.Tick();
    }
  }
}