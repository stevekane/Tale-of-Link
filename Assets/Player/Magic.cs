using UnityEngine;
using UnityEngine.Events;

public class Magic : MonoBehaviour {
  const float INITIAL_CURRENT = 101;
  const float INITIAL_TOTAL = 101;

  [SerializeField] float UseDelay = .5f;
  [SerializeField] float DrainSpeed = 50;
  [SerializeField] float RestoreSpeed = 25;

  float Total;
  float Current;
  float Recent;
  float UseCooldownRemaining;

  public UnityAction<float> OnSet;
  public UnityAction<float> OnChangeCurrent;
  public UnityAction<float> OnChangeRecent;
  public UnityAction<float> OnSetTotal;

  public bool IsFull => Current >= Total;
  public bool IsEmpty => Current <= 0;

  void Start() {
    SetTotal(INITIAL_TOTAL);
    Set(INITIAL_CURRENT);
  }

  void FixedUpdate() {
    UseCooldownRemaining = Mathf.Clamp(UseCooldownRemaining-Time.fixedDeltaTime, 0, UseDelay);
    if (UseCooldownRemaining <= 0) {
      Recent = Mathf.MoveTowards(Recent, Current, DrainSpeed * Time.fixedDeltaTime);
      OnChangeRecent?.Invoke(Recent);
      if (Recent <= Current) {
        Restore(RestoreSpeed * Time.fixedDeltaTime);
      }
    }
  }

  public void Set(float current) {
    Current = Mathf.Clamp(current, 0, Total);
    OnSet?.Invoke(Current);
  }

  public void Drain(float delta) {
    Current = Mathf.Clamp(Current-delta, 0, Total);
    Recent = Current;
    UseCooldownRemaining = UseDelay;
    OnChangeCurrent?.Invoke(Current);
  }

  public void Consume(float delta) {
    Recent = Current;
    OnChangeRecent?.Invoke(Recent);
    Current = Mathf.Clamp(Current-delta, 0, Total);
    UseCooldownRemaining = UseDelay;
    OnChangeCurrent?.Invoke(Current);
  }

  public void Restore(float delta) {
    Current = Mathf.Clamp(Current+delta, 0, Total);
    Recent = Current;
    OnChangeCurrent?.Invoke(Current);
  }

  public void SetTotal(float total) {
    Total = total;
    OnSetTotal?.Invoke(Total);
  }
}