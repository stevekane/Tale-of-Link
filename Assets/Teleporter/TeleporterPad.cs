using UnityEngine;

public class TeleporterPad : MonoBehaviour {
  public TeleporterPad Exit;
  public GameObject Model;
  public bool Active;

  public void Show() => Model.SetActive(true);
  public void Activate() => Active = true;

  void OnDrawGizmos() {
    if (Exit) {
      Gizmos.DrawLine(transform.position, Exit.transform.position);
    }
  }
}