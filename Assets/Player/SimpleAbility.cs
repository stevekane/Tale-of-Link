using UnityEngine;

[DefaultExecutionOrder(ScriptExecutionGroups.Ability)]
public abstract class SimpleAbility : MonoBehaviour {
  public string Name;
  public AbilityTag BlockActionsWith;
  public AbilityTag Tags;
  public AbilityTag AddedToOwner;
  public AbilityTag RemovedFromOwner;
  public virtual bool IsRunning { get; set; }
  public virtual void Stop() {
    Tags = default;
    AddedToOwner = default;
    RemovedFromOwner = default;
    IsRunning = false;
  }

  protected SimpleAbilityManager AbilityManager;

  void OnEnable() {
    AbilityManager = GetComponentInParent<SimpleAbilityManager>();
    AbilityManager.AddAbility(this);
  }

  void OnDisable() {
    AbilityManager = GetComponentInParent<SimpleAbilityManager>();
    if (AbilityManager)
      AbilityManager.RemoveAbility(this);
  }
}