using System;
using UnityEngine.Serialization;

[Serializable]
public class AbilityAction : IEventSource {
  EventSource Source = new();
  public AbilityTag Tags;
  [FormerlySerializedAs("CancelAbilitiesWith")]
  public AbilityTag CancelAbilitiesWithAll;
  public AbilityTag CancelAbilitiesWithAny;
  public AbilityTag AddToOwner;
  public AbilityTag AddToAbility;
  public AbilityTag OwnerActivationRequired;
  public AbilityTag OwnerActivationBlocked;
  public bool CanRun;
  public SimpleAbility Ability;
  public void Listen(Action handler) => Source.Listen(handler);
  public void Unlisten(Action handler) => Source.Unlisten(handler);
  public void Set(Action handler) => Source.Set(handler);
  public void Clear() => Source.Clear();
  public void Fire() {
    Ability.AddedToOwner.AddFlags(AddToOwner);
    Ability.Tags.AddFlags(AddToAbility);
    Source.Fire();
  }
}

[Serializable]
public class AbilityAction<T> : IEventSource<T> {
  EventSource<T> Source = new();
  public AbilityTag Tags;
  [FormerlySerializedAs("CancelAbilitiesWith")]
  public AbilityTag CancelAbilitiesWithAll;
  public AbilityTag CancelAbilitiesWithAny;
  public AbilityTag AddToOwner;
  public AbilityTag AddToAbility;
  public AbilityTag OwnerActivationRequired;
  public AbilityTag OwnerActivationBlocked;
  public bool CanRun;
  public SimpleAbility Ability;
  public void Listen(Action<T> handler) => Source.Listen(handler);
  public void Unlisten(Action<T> handler) => Source.Unlisten(handler);
  public void Set(Action<T> handler) => Source.Set(handler);
  public void Clear() => Source.Clear();
  public void Fire(T t) {
    Ability.AddedToOwner.AddFlags(AddToOwner);
    Ability.Tags.AddFlags(AddToAbility);
    Source.Fire(t);
  }
}