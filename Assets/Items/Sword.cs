using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class Sword : ClassicAbility {
  public VisualEffect VisualEffect;
  public AudioClip WooshSFX;
  public Hitbox Hitbox;
  public Vector3 Direction;
  public Timeval Duration = Timeval.FromMillis(250);

  Animator Animator;
  WorldSpaceController WorldSpaceController;
  EquipmentVisibility EquipmentVisibility;

  void Start() {
    AbilityManager.InitComponent(out Animator);
    AbilityManager.InitComponent(out WorldSpaceController);
    AbilityManager.InitComponent(out EquipmentVisibility);
  }

  public override async Task MainAction(TaskScope scope) {
    try {
      AudioManager.Instance.SoundSource.PlayOneShot(WooshSFX);
      if (Direction.sqrMagnitude > 0)
        WorldSpaceController.Forward = Direction;
      Hitbox.EnableCollision = true;
      Animator.SetTrigger("Attack");
      VisualEffect.Play();
      EquipmentVisibility.DisplayBaseObjects();
      await scope.Ticks(Duration.Ticks);
    } finally {
      Hitbox.EnableCollision = false;
    }
  }
}