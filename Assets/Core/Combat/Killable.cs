using System;
using UnityEngine;

[RequireComponent(typeof(AbilityManager))]
public class Killable : MonoBehaviour {
  public Action OnSpawning;
  public Action OnAlive;
  public Action OnDying;
  public Action OnDead;

  AbilityManager AbilityManager;

  void Start() {
    this.InitComponent(out AbilityManager);
    Alive = true;
  }

  public bool Spawning {
    get => AbilityManager.HasTag(AbilityTag.Spawning);
    set {
      if (value) {
        Set(AbilityTag.Spawning);
        OnSpawning?.Invoke();
      }
    }
  }

  public bool Alive {
    get => AbilityManager.HasTag(AbilityTag.Alive);
    set {
      if (value) {
        Set(AbilityTag.Alive);
        OnAlive?.Invoke();
      }
    }
  }

  public bool Dying {
    get => AbilityManager.HasTag(AbilityTag.Dying);
    set {
      if (value) {
        Set(AbilityTag.Dying);
        OnDying?.Invoke();
      }
    }
  }

  public bool Dead {
    get => AbilityManager.HasTag(AbilityTag.Dead);
    set {
      if (value) {
        Set(AbilityTag.Dead);
        OnDead?.Invoke();
      }
    }
  }

  void Set(AbilityTag tag) {
    AbilityManager.SetTag(AbilityTag.Spawning, false);
    AbilityManager.SetTag(AbilityTag.Alive, false);
    AbilityManager.SetTag(AbilityTag.Dying, false);
    AbilityManager.SetTag(AbilityTag.Dead, false);
    AbilityManager.SetTag(tag, true);
  }
}