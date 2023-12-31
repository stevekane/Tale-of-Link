using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(ScriptExecutionGroups.Late)]
public class AbilityManager : MonoBehaviour {
  AbilityTag NextSystemTags;
  AbilityTag SystemTags;

  [HideInInspector, NonSerialized]
  public List<Ability> Abilities = new();

  [field:SerializeField]
  public AbilityTag Tags { get; private set; }
  public AbilityTag InitialTags = AbilityTag.CanAttack | AbilityTag.CanMove | AbilityTag.CanRotate | AbilityTag.CanUseItem;

  public bool HasTags(AbilityTag tags) => (Tags & tags) == tags;
  public void AddTag(AbilityTag tag) => NextSystemTags |= tag;
  public void RemoveTag(AbilityTag tag) => NextSystemTags.ClearFlags(tag);
  public void SetTag(AbilityTag tag, bool active) {
    if (active) AddTag(tag);
    else        RemoveTag(tag);
  }

  public void AddAbility(Ability ability) => Abilities.Add(ability);
  public void RemoveAbility(Ability ability) {
    ability.Stop();
    Abilities.Remove(ability);
  }

  public bool TryRun(AbilityAction action) {
    if (CanRun(action)) {
      Run(action);
      return true;
    }
    return false;
  }

  public bool TryRun<T>(AbilityAction<T> action, T t) {
    if (CanRun(action)) {
      Run(action, t);
      return true;
    }
    return false;
  }

  public bool CanRun(AbilityAction action) {
    var predicateSatisfied = action.CanRun();
    var ownerTagsAfterCancelations = AbilityOwnerTagsWhere(a => a.IsRunning && !IsCancellable(action, a), SystemTags);
    var ownerAllowed = ownerTagsAfterCancelations.HasAllFlags(action.OwnerActivationRequired);
    var ownerBlocked = ownerTagsAfterCancelations.HasAnyFlags(action.OwnerActivationBlocked);
    var abilityBlocked = Abilities.Any(a => a.IsRunning && !IsCancellable(action, a) && IsBlocked(action, a));
    return predicateSatisfied && ownerAllowed && !ownerBlocked && !abilityBlocked;
  }

  public bool CanRun<T>(AbilityAction<T> action) {
    var predicateSatisfied = action.CanRun();
    var ownerTagsAfterCancelations = AbilityOwnerTagsWhere(a => a.IsRunning && !IsCancellable(action, a), SystemTags);
    var ownerAllowed = ownerTagsAfterCancelations.HasAllFlags(action.OwnerActivationRequired);
    var ownerBlocked = ownerTagsAfterCancelations.HasAnyFlags(action.OwnerActivationBlocked);
    var abilityBlocked = Abilities.Any(a => a.IsRunning && !IsCancellable(action, a) && IsBlocked(action, a));
    return predicateSatisfied && ownerAllowed && !ownerBlocked && !abilityBlocked;
  }

  public void Stop(Ability ability) {
    if (ability.IsRunning)
      ability.Stop();
  }

  public void Run(AbilityAction action) {
    foreach (var ability in Abilities)
      if (ability.IsRunning && IsCancellable(action, ability))
        ability.Stop();
    action.Fire();
  }

  public TaskFunc RunUntilDone(AbilityAction action) => async (s) => {
    Run(action);
    await s.Until(() => !action.Ability.IsRunning);
  };

  public void Run<T>(AbilityAction<T> action, T t) {
    foreach (var ability in Abilities)
      if (ability.IsRunning && IsCancellable(action, ability))
        ability.Stop();
    action.Fire(t);
  }

  void Awake() {
    AddTag(InitialTags);
  }

  void OnDestroy() => Abilities.ForEach(a => a.Stop());

  void FixedUpdate() {
    SystemTags = NextSystemTags;
    Tags = AbilityOwnerTagsWhere(a => a.IsRunning, SystemTags);
  }

  bool IsCancellable(AbilityAction action, Ability ability) {
    var hasAll = action.CancelAbilitiesWithAll != default && ability.Tags.HasAllFlags(action.CancelAbilitiesWithAll);
    var hasAny = ability.Tags.HasAnyFlags(action.CancelAbilitiesWithAny);
    return hasAll || hasAny;
  }

  bool IsCancellable<T>(AbilityAction<T> action, Ability ability) {
    var hasAll = action.CancelAbilitiesWithAll != default && ability.Tags.HasAllFlags(action.CancelAbilitiesWithAll);
    var hasAny = ability.Tags.HasAnyFlags(action.CancelAbilitiesWithAny);
    return hasAll || hasAny;
  }

  bool IsBlocked(AbilityAction action, Ability ability) {
    return ability.BlockActionsWith.HasAnyFlags(action.Tags);
  }

  bool IsBlocked<T>(AbilityAction<T> action, Ability ability) {
    return ability.BlockActionsWith.HasAnyFlags(action.Tags);
  }

  AbilityTag AbilityOwnerTagsWhere(Predicate<Ability> predicate, AbilityTag tag = default) {
    var addedTags = Abilities.Aggregate(tag, (tags, ability) => tags | (predicate(ability) ? ability.AddedToOwner : default));
    var removedTags = Abilities.Aggregate(default(AbilityTag), (tags, ability) => tags | (predicate(ability) ? ability.RemovedFromOwner : default));
    return addedTags & ~removedTags;
  }
}