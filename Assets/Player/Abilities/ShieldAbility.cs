using UnityEngine;

public class ShieldAbility : Ability {
  [SerializeField] Animator Animator;
  [SerializeField] int ShieldAnimatorLayer;
  [SerializeField] float TransitionDuration = .25f;

  public AbilityAction Raise;
  public AbilityAction Lower;
  [field:SerializeField]
  public bool Raised { get; private set; } = false;
  public override bool IsRunning => Raised;

  public override void Stop() {
    base.Stop();
    LowerShield();
  }

  public bool Blocks(Transform attacker) {
    var toAttacker = (attacker.position - transform.position).normalized;
    var attackerInFront = Vector3.Dot(transform.forward, toAttacker) >= 0;
    return Raised && attackerInFront;
  }

  bool IsRaised() => Raised;
  bool IsLowered() => !Raised;
  void RaiseShield() => Raised = true;
  void LowerShield() => Raised = false;

  void Start() {
    Raise.Listen(RaiseShield);
    Raise.CanRun = IsLowered;
    Lower.Listen(LowerShield);
    Lower.CanRun = IsRaised;
  }

  void Update() {
    var speed = 1 / TransitionDuration;
    var currentWeight = Animator.GetLayerWeight(ShieldAnimatorLayer);
    var targetWeight = Raised ? 1 : 0;
    var nextWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime * speed);
    Animator.SetLayerWeight(ShieldAnimatorLayer, nextWeight);
  }
}