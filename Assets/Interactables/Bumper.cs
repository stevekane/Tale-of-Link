using System;
using System.Collections;
using UnityEngine;

public class Bumper : MonoBehaviour {
  public Transform Model;
  public Collider Collider;
  public float BumpForce = 10f;

  void OnTriggerEnter(Collider other) {
    if (other.TryGetComponent(out WorldSpaceController controller)) {
      Bump(controller);
    }
  }

  void Bump(WorldSpaceController controller) {
    StopAllCoroutines();
    StartCoroutine(Animate());
    var delta = controller.Position - transform.position;
    controller.PhysicsVelocity += BumpForce * delta.normalized;
  }

  public float AnimationDecay = 1f;
  public float AnimationAmplitude = 1f;
  public float AnimationFrequency = 3f;
  IEnumerator Animate() {
    var t = 0f;
    var origScale = Model.localScale;
    while (true) {
      var decay = Mathf.Exp(-AnimationDecay * t);
      if (decay < .1f)
        break;
      Model.localScale = origScale + AnimationAmplitude * decay * Mathf.Sin(AnimationFrequency * t) * Vector3.one;
      t += Time.fixedDeltaTime;
      yield return new WaitForFixedUpdate();
    }
    Model.localScale = origScale;
  }
}
