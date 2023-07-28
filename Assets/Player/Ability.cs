using UnityEngine;

[DefaultExecutionOrder(ScriptExecutionGroups.Ability)]
public abstract class Ability : MonoBehaviour {
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

  protected AbilityManager AbilityManager;

  void OnEnable() {
    AbilityManager = GetComponentInParent<AbilityManager>();
    AbilityManager.AddAbility(this);
  }

  void OnDisable() {
    AbilityManager = GetComponentInParent<AbilityManager>();
    if (AbilityManager)
      AbilityManager.RemoveAbility(this);
  }
}