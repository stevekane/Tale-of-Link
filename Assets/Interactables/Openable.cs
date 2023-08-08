using UnityEngine;
using UnityEngine.Events;

public class Openable : MonoBehaviour {
  [SerializeField] UnityEvent OnOpen;
  [SerializeField] UnityEvent OnClose;

  public bool IsOpen { get; private set; } = false;

  public void Open() {
    IsOpen = true;
    OnOpen?.Invoke();
  }

  public void Close() {
    IsOpen = false;
    OnClose?.Invoke();
  }
}
