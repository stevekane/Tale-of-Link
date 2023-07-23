using UnityEngine;
using UnityEngine.Events;

public class Coins : MonoBehaviour {
  const int INITIAL_CURRENT = 0;
  const int INITIAL_TOTAL = 99;

  int Current;
  int Total;

  public UnityAction<int> OnSetCurrent;
  public UnityAction<int> OnChangeCurrent;
  public UnityAction OnDeath;
  public UnityAction<int> OnSetTotal;
  public UnityAction<int> OnChangeTotal;

  void Start() {
    Current = INITIAL_CURRENT;
    Total = INITIAL_TOTAL;
    OnSetCurrent?.Invoke(Current);
    OnSetTotal?.Invoke(Total);
  }

  public bool IsFull => Current >= Total;

  public void Change(int delta) {
    Current += delta;
    Current = Mathf.Min(Current, Total);
    OnChangeCurrent?.Invoke(Current);
    if (Current <= 0)
      OnDeath?.Invoke();
  }

  public void ChangeTotal(int delta) {
    Total += delta;
    Current = Mathf.Min(Current, Total);
    OnChangeTotal?.Invoke(Total);
    if (Current <= 0)
      OnDeath?.Invoke();
  }

  void OnGUI() {
    GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
    guiStyle.fontSize = 30;
    guiStyle.normal.textColor = Color.white;
    Rect rect = new Rect(350, 10, 300, 50);
    var total = Current / 4;
    var partial = Current % 4;
    GUI.Label(rect, $"{Current} Rupees", guiStyle);
  }
}