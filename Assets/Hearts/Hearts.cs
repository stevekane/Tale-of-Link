using UnityEngine;
using UnityEngine.Events;

public class Hearts : MonoBehaviour {
  const int INITIAL_CURRENT = 2;
  const int INITIAL_TOTAL = 12;

  int Current;
  int Total;

  public UnityAction<int> OnSetCurrent;
  public UnityAction<int> OnChangeCurrent;
  public UnityAction OnDeath;
  public UnityAction<int> OnSetTotal;
  public UnityAction<int> OnChangeTotal;

  public bool IsFull => Current >= Total;

  void Start() {
    SetTotal(INITIAL_TOTAL);
    SetCurrent(INITIAL_CURRENT);
  }

  public void SetCurrent(int current) {
    Current = current;
    Current = Mathf.Min(Current, Total);
    OnSetCurrent?.Invoke(Current);
    if (Current <= 0)
      OnDeath?.Invoke();
  }

  public void ChangeCurrent(int delta) {
    Current += delta;
    Current = Mathf.Min(Current, Total);
    OnChangeCurrent?.Invoke(Current);
    if (Current <= 0)
      OnDeath?.Invoke();
  }

  public void SetTotal(int total) {
    Total = total;
    OnSetTotal?.Invoke(Total);
  }

  public void ChangeTotal(int delta) {
    Total += delta;
    Current += delta;
    Current = Mathf.Min(Current, Total);
    OnChangeCurrent?.Invoke(Current);
    OnChangeTotal?.Invoke(Total);
    if (Current <= 0)
      OnDeath?.Invoke();
  }
}