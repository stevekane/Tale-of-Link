using UnityEngine;

[DefaultExecutionOrder(1)]
public class Enabler : MonoBehaviour {
  void FixedUpdate() {
    GetComponent<LifeCycleTests>().enabled = true;
  }
}