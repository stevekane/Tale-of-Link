using UnityEngine;
using UnityEngine.Events;

public class Hearts : MonoBehaviour {
  [SerializeField] AbilityManager AbilityManager;
  [SerializeField] KillPlayer KillPlayer;

  public int Current = 2;
  public int Total = 12;

  public UnityAction<int> OnSetCurrent;
  public UnityAction<int> OnChangeCurrent;
  public UnityAction<int> OnSetTotal;
  public UnityAction<int> OnChangeTotal;

  public bool IsFull => Current >= Total;

  void Start() {
    SetTotal(Total);
    SetCurrent(Current);
  }

  void CheckForDeath() {
    if (Current <= 0 && AbilityManager.HasTag(AbilityTag.Alive)) {
      AbilityManager.Abilities.ForEach(AbilityManager.Stop);
      AbilityManager.Run(KillPlayer.Main);
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