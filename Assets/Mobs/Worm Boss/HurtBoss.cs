using System;
using System.Threading.Tasks;
using UnityEngine;

public class HurtBoss : ClassicAbility {
  [SerializeField] Timeval HurtDuration = Timeval.FromSeconds(2);
  [SerializeField] WorldSpaceMove Move;
  [SerializeField] Color StartColor = Color.yellow;
  [SerializeField] Color AngryColor = Color.red;
  [SerializeField] float[] SpeedsBeforeHit;
  [SerializeField] Renderer[] Renderers;
  [SerializeField] Hurtbox Hurtbox;
  int NumHits = 0;

  MoveTail MoveTail;
  WorldSpaceController Controller;

  private void Start() {
    AbilityManager.GetComponent<Combatant>().OnHurt += OnHurt;
    AbilityManager.InitComponent(out Controller);
    AbilityManager.InitComponent(out MoveTail);
    Move.Speed = SpeedsBeforeHit[0];
    SetColor();
  }

  void SetColor() {
    var color = Color.Lerp(StartColor, AngryColor, (float)NumHits / SpeedsBeforeHit.Length);
    foreach (var r in Renderers)
      r.material.color = color;
  }

  void OnHurt(HitEvent hit) {
    if (hit.HitConfig.Damage > 0)
      AbilityManager.Run(Main);
  }

  public override async Task MainAction(TaskScope scope) {
    try {
      NumHits++;
      Hurtbox.EnableCollision = false;
      if (NumHits >= SpeedsBeforeHit.Length)
        return;
      SetColor();
      Move.Speed = SpeedsBeforeHit[NumHits];
      await TailShrink(scope);
      await scope.Any(
        Waiter.Delay(HurtDuration),
        Waiter.Repeat(Spin));
    } finally {
      Hurtbox.EnableCollision = true;
    }
  }

  const float ShrinkSpeed = 5f;
  async Task TailShrink(TaskScope scope) {
    var shrinking = true;
    while (shrinking) {
      shrinking = false;
      foreach (var t in MoveTail.TailBones) {
        var delta = (MoveTail.Head.position - t.position);
        if (delta.sqrMagnitude > .1f.Sqr()) {
          shrinking = true;
          t.position += ShrinkSpeed * Time.fixedDeltaTime * delta.normalized;
        }
      }
      await scope.Tick();
    }
  }

  const float SpinSpeed = 10f;
  void Spin() {
    Controller.Forward = Quaternion.Euler(0, SpinSpeed, 0) * Controller.Forward;
  }
}