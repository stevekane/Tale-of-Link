using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AbilityManager))]
public class Hearts : MonoBehaviour {
  [SerializeField] ClassicAbility Kill;

  public int Current = 2;
  public int Total = 12;

  public UnityAction<int> OnSetCurrent;
  public UnityAction<int> OnChangeCurrent;
  public UnityAction<int> OnSetTotal;
  public UnityAction<int> OnChangeTotal;

  public bool IsFull => Current >= Total;
  public bool IsInvulnerable = false;
  public HitConfig.Types LastHitType;

  AbilityManager AbilityManager;
  Combatant Combatant;

  void Start() {
    this.InitComponent(out AbilityManager);
    this.InitComponent(out Combatant);
    Combatant.OnHurt += OnHurt;
    SetTotal(Total);
    SetCurrent(Current);
  }

  void OnHurt(HitEvent hit) {
    if (!IsInvulnerable) {
      LastHitType = hit.HitConfig.HitType;
      ChangeCurrent(-hit.HitConfig.Damage);
    }
  }

  void CheckForDeath() {
    if (Current <= 0 && AbilityManager.HasTags(AbilityTag.Alive)) {
      AbilityManager.Abilities.ForEach(AbilityManager.Stop);
      AbilityManager.Run(Kill.Main);
    }
  }

  public void SetCurrent(int current) {
    Current = Mathf.Max(0, current);
    Current = Mathf.Min(Current, Total);
    OnSetCurrent?.Invoke(Current);
    CheckForDeath();
  }

  public void ChangeCurrent(int delta) {
    Current = Mathf.Max(0, Current+delta);
    Current = Mathf.Min(Current, Total);
    OnChangeCurrent?.Invoke(Current);
    CheckForDeath();
  }

  public void SetTotal(int total) {
    Total = total;
    OnSetTotal?.Invoke(Total);
    CheckForDeath();
  }

  public void ChangeTotal(int delta) {
    Total = Mathf.Max(0, Total+delta);
    Current = Mathf.Max(0, Current+delta);
    Current = Mathf.Min(Current, Total);
    OnChangeCurrent?.Invoke(Current);
    OnChangeTotal?.Invoke(Total);
    CheckForDeath();
  }
}