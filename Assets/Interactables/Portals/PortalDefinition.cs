using UnityEngine;

public class PortalDefinition : MonoBehaviour {
  [SerializeField] TeleporterPad Origin;
  [SerializeField] TeleporterPad Destination;
  [SerializeField] bool Bidirectional;

  void Start() {
    Origin.Exit = Destination;
    if (Bidirectional)
      Destination.Exit = Origin;
  }

  void OnDrawGizmosSelected() {
    if (!Origin || !Destination)
      return;
    Debug.DrawLine(Origin.transform.position, Destination.transform.position, Bidirectional ? Color.magenta : Color.cyan);
  }
}