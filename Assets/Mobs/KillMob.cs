using System;
using System.Threading.Tasks;
using UnityEngine;

public class KillMob : ClassicAbility {
  [SerializeField] Timeval DyingDuration = Timeval.FromSeconds(2);
  [SerializeField] GameObject Model;
  [SerializeField] ParticleSystem DeathVFX;

  public override async Task MainAction(TaskScope scope) {
    var hearts = AbilityManager.GetComponent<Hearts>();
    var killable = AbilityManager.GetComponent<Killable>();
    var animator = AbilityManager.GetComponent<Animator>();
    var squish = hearts.LastHitType == HitConfig.Types.Hammer;
    try {
      killable.Dying = true;
      AbilityManager.SetTag(AbilityTag.CanMove, false);
      AbilityManager.SetTag(AbilityTag.CanAttack, false);
      AbilityManager.SetTag(AbilityTag.CanRotate, false);
      AbilityManager.SetTag(AbilityTag.CanUseItem, false);
      if (squish) {
        animator.enabled = false;
        scope.Start(Squish);
      } else if (animator) {
        animator.SetTrigger("Kill");
      }
      await scope.Ticks(DyingDuration.Ticks);
      Destroy(Instantiate(DeathVFX, transform.position+Vector3.up, transform.rotation), 2);
      Destroy(killable.gameObject);
    } catch (Exception e){
      throw e;
    } finally {
      killable.Dead = true;
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