using UnityEngine;
using UnityEngine.Events;

public class Hearts : MonoBehaviour {
  const int INITIAL_CURRENT = 2;
  const int INITIAL_TOTAL = 12;

  int Current;
  int Total;

  public UnityEvent<int> OnSetCurrent;
  public UnityEvent<int> OnChangeCurrent;
  public UnityEvent OnDeath;
  public UnityEvent<int> OnSetTotal;
  public UnityEvent<int> OnChangeTotal;

  void Start() {
    Current = INITIAL_CURRENT;
    Total = INITIAL_TOTAL;
    OnSetCurrent?.Invoke(Current);
    OnSetTotal?.Invoke(Total);
  }

  public void Change(int delta) {
    Current += delta;
    Current = Mathf.Min(Current, Total);
    OnChangeCurrent?.Invoke(Current);
    Debug.Log($"{Current}/{Total} Hearts");
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
    guiStyle.normal.textColor = Color.red;
    Rect rect = new Rect(10, 10, 300, 50);
    var total = Current / 4;
    var partial = Current % 4;
    var str = "";
    for (var i = 0; i < total; i++) {
      str += "||||  ";
    }
    for (var i = 0; i < partial; i++) {
      str += "|";
    }
    GUI.Label(rect, str, guiStyle);
  }
}