using UnityEngine;

[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LocalTime))]
public class LocalTimeRigidBody : MonoBehaviour {
  Rigidbody Rigidbody;
  LocalTime LocalTime;

  Vector3 StoredVelocity;
  Vector3 StoredAngularVelocity;

  void Awake() {
    this.InitComponent(out Rigidbody);
    this.InitComponent(out LocalTime);
  }

  void FixedUpdate() {
    if (LocalTime.TimeScale <= 0 && !Rigidbody.isKinematic) {
      StoredVelocity = Rigidbody.velocity;
      StoredAngularVelocity = Rigidbody.angularVelocity;
      Rigidbody.isKinematic = true;
    } else if (LocalTime.TimeScale > 0 && Rigidbody.isKinematic) {
      Rigidbody.isKinematic = false;
      Rigidbody.velocity = StoredVelocity;
      Rigidbody.angularVelocity = StoredAngularVelocity;
    }
  }
}