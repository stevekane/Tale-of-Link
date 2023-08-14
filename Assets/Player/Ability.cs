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

  protected virtual void Awake() {
    AbilityManager = GetComponentInParent<AbilityManager>();
    if (AbilityManager)  // Ability may be detached, e.g. part of an item
      AbilityManager.AddAbility(this);
  }

  protected virtual void OnDestroy() {
    AbilityManager = GetComponentInParent<AbilityManager>();
    if (AbilityManager)
      AbilityManager.RemoveAbility(this);
  }
}