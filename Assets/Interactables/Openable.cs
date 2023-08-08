using UnityEngine;
using UnityEngine.Events;

public class Openable : MonoBehaviour {
  [SerializeField] UnityEvent OnOpen;
  [SerializeField] UnityEvent OnClose;

  public GameObject Opener { get; private set; } = null;
  public bool IsOpen { get; private set; } = false;

  public void Open(GameObject opener) {
    Opener = opener;
    IsOpen = true;
    OnOpen?.Invoke();
  }

  public void Close() {
    Opener = null;
    IsOpen = false;
    OnClose?.Invoke();
  }
}
