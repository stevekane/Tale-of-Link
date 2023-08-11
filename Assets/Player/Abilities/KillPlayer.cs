using System;
using System.Threading.Tasks;
using UnityEngine;

public class KillPlayer : ClassicAbility {
  [SerializeField] bool HasFairy;
  [SerializeField] float FadeSpeed = 1;
  [SerializeField] Timeval DyingDuration = Timeval.FromSeconds(2);

  public override async Task MainAction(TaskScope scope) {
    try {
      var hearts = AbilityManager.GetComponent<Hearts>();
      var killable = AbilityManager.GetComponent<Killable>();
      var animator = AbilityManager.GetComponent<Animator>();
      killable.Dying = true;
      animator.SetTrigger("Dying");
      CameraManager.Instance.FadeOut(FadeSpeed);
      await scope.Ticks(DyingDuration.Ticks);
      if (HasFairy) {
        animator.SetTrigger("Reviving");
        killable.Spawning = true;
        hearts.ChangeCurrent(hearts.Total-hearts.Current);
        await scope.Ticks(DyingDuration.Ticks);
        killable.Alive = true;
        CameraManager.Instance.FadeIn(FadeSpeed);
      } else {
        killable.Dead = true;
      }
    } catch (Exception e){
      throw e;
    }
  }
}