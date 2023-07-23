using UnityEngine;

public class WallCollider : MonoBehaviour {
  void OnTriggerEnter(Collider c) => SendMessage("OnWallTriggerEnter", c, SendMessageOptions.DontRequireReceiver);
  void OnTriggerStay(Collider c) => SendMessage("OnWallTriggerStay", c, SendMessageOptions.DontRequireReceiver);
  void OnTriggerExit(Collider c) => SendMessage("OnWallTriggerExit", c, SendMessageOptions.DontRequireReceiver);
}