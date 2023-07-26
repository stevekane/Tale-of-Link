using UnityEngine;

public class TeleporterPad : MonoBehaviour {
  public TeleporterPad Exit;

  void OnDrawGizmos() {
    if (Exit) {
      Gizmos.DrawLine(transform.position, Exit.transform.position);
    }
  }
}