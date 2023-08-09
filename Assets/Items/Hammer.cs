using System;
using System.Threading.Tasks;
using UnityEngine;

public class Hammer : ClassicAbility {
  public Hitbox Hitbox;
  public Vector3 Direction;
  public Timeval SwingDuration = Timeval.FromMillis(250);
  public Timeval RecoveryDuration = Timeval.FromMillis(250);
  public GameObject ImpactVFX;
  public float CameraShakeIntensity = 10;

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
      if (Direction.sqrMagnitude > 0)
        WorldSpaceController.Forward = Direction;
      Animator.SetTrigger("Hammer");
      EquipmentVisibility.AddCurrentObject(EquipmentVisibility.Hammer);
      await scope.Ticks(SwingDuration.Ticks);
      Hitbox.EnableCollision = true;
      var impactPosition = AbilityManager.transform.position + AbilityManager.transform.forward;
      var impactRotation = AbilityManager.transform.rotation;
      Instantiate(ImpactVFX, impactPosition, impactRotation);
      CameraShaker.Instance.Shake(CameraShakeIntensity);
      await scope.Ticks(SwingDuration.Ticks);
    } catch (Exception e) {
      throw e;
    } finally {
      Hitbox.EnableCollision = false;
      EquipmentVisibility.DisplayBaseObjects();
    }
  }
}