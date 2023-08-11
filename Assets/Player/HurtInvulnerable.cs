using System.Threading.Tasks;
using UnityEngine;

public class HurtInvulnerable : MonoBehaviour {
  public Timeval InvulnerabilityDuration = Timeval.FromSeconds(2);
  public GameObject Model;
  public Hurtbox Hurtbox;
  AbilityManager AbilityManager;
  Combatant Combatant;
  TaskScope Scope = new();

  void Awake() {
    this.InitComponent(out AbilityManager);
    this.InitComponent(out Combatant);
    Combatant.OnHurt += OnHurt;
  }
  void OnDestroy() {
    Scope.Dispose();
  }

  void OnHurt(HitEvent hitEvent) {
    Hurtbox.EnableCollision = false;
    Scope.Start(InvulnEffect);
  }

  async Task InvulnEffect(TaskScope scope) {
    try {
      await scope.Any(
        Waiter.While(() => AbilityManager.HasTags(AbilityTag.Alive)),
        Waiter.Delay(InvulnerabilityDuration),
        Waiter.Repeat(async s => {
          Model.SetActive(false);
          await s.Seconds(.1f);
          Model.SetActive(true);
          await s.Seconds(.2f);
        }));
    } finally {
      Model.SetActive(true);
      Hurtbox.EnableCollision = true;
    }
  }
}