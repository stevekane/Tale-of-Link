using System.Threading.Tasks;
using UnityEngine;

public class DiveBombAbility : ClassicAbility {
  public float ChargeHeight = 2;
  public float ChargeSpeed = 15;
  public Timeval ChargeDuration = Timeval.FromSeconds(1);

  WorldSpaceController WorldSpaceController;

  void Start() {
    WorldSpaceController = AbilityManager.GetComponent<WorldSpaceController>();
  }

  public override async Task MainAction(TaskScope scope) {
    try {
      WorldSpaceController.DirectMove = true;
      for (var i = 0; i < ChargeDuration.Ticks; i++) {
        WorldSpaceController.ScriptVelocity = ChargeSpeed * WorldSpaceController.Forward;
        await scope.Tick();
      }
    } finally {
      WorldSpaceController.DirectMove = false;
    }
  }
}