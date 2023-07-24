using UnityEngine;
using UnityEngine.Events;

public class Coins : MonoBehaviour {
  const int INITIAL_CURRENT = 0;

  int Total = 9999;
  int Current;

  public UnityAction<int> OnSetCurrent;
  public UnityAction<int> OnChangeCurrent;
  public UnityAction<int> OnSetTotal;
  public UnityAction<int> OnChangeTotal;

  public bool IsFull => Current >= Total;

  void Start() {
    SetCurrent(INITIAL_CURRENT);
  }

  public void SetCurrent(int current) {
    Current = current;
    Current = Mathf.Min(Current, Total);
    OnSetCurrent?.Invoke(Current);
  }

  public void ChangeCurrent(int delta) {
    Current += delta;
    Current = Mathf.Min(Current, Total);
    OnChangeCurrent?.Invoke(Current);
  }
}