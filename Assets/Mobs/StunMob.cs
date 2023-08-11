using System.Threading.Tasks;
using UnityEngine;

public class StunMob : ClassicAbility {
  [SerializeField] ParticleSystem StunVFX;

  public Timeval StunDuration;

  public override async Task MainAction(TaskScope scope) {
    var animator = AbilityManager.GetComponent<Animator>();
    try {
      animator.SetBool("Stunned", true);
      StunVFX.Play();
      await scope.Delay(StunDuration);
    } finally {
      StunVFX.Clear();
      StunVFX.Stop();
      animator.SetBool("Stunned", false);
    }
  }
}