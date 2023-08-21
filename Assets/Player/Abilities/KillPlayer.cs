using System;
using System.Threading.Tasks;
using UnityEngine;

public class KillPlayer : ClassicAbility {
  [SerializeField] bool HasFairy;
  [SerializeField] float FadeSpeed = 1;
  [SerializeField] Timeval DyingDuration = Timeval.FromSeconds(2);
  [SerializeField] HUD HUD;

  public override async Task MainAction(TaskScope scope) {
    try {
      var hearts = AbilityManager.GetComponent<Hearts>();
      var killable = AbilityManager.GetComponent<Killable>();
      var animator = AbilityManager.GetComponent<Animator>();
      killable.Dying = true;
      animator.SetTrigger("Dying");
      CameraManager.Instance.FadeOut(FadeSpeed);
      TimeManager.Instance.IgnoreFreeze.Add(LocalTime);
      TimeManager.Instance.Frozen = true;
      Debug.Log(LocalTime.TimeScale);
      Debug.Log("Predying");
      if (!HasFairy) {
        HUD.ShowGameOver();
      }
      await scope.Ticks(DyingDuration.Ticks);
      Debug.Log("Postdying");
      if (HasFairy) {
        animator.SetTrigger("Reviving");
        killable.Spawning = true;
        hearts.ChangeCurrent(hearts.Total-hearts.Current);
        Debug.Log("Previving");
        await scope.Ticks(DyingDuration.Ticks);
        Debug.Log("Postviving");
        killable.Alive = true;
        CameraManager.Instance.FadeIn(FadeSpeed);
        HasFairy = false;
      } else {
        killable.Dead = true;
      }
    } catch (Exception e){
      throw e;
    } finally {
      TimeManager.Instance.Frozen = false;
      TimeManager.Instance.IgnoreFreeze.Remove(LocalTime);
    }
  }
}