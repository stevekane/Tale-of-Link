using System;
using System.Threading.Tasks;
using UnityEngine;

public class StunMob : ClassicAbility {
  public Animator Animator;
  public Timeval StunDuration;

  public override async Task MainAction(TaskScope scope) {
    try {
      Debug.Log($"Stunned");
      //Animator.SetTrigger("Stunned");
      await scope.Delay(StunDuration);
    } finally {
      //Animator.ResetTrigger("Stunned");
    }
  }
}