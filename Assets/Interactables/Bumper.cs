using System;
using System.Collections;
using UnityEngine;

public class Bumper : MonoBehaviour {
  public Transform Model;
  public Collider Collider;
  public float BumpForce = 10f;

  Vector3 BaseScale;
  void Start() {
    GetComponentInParent<Combatant>().OnHurt += OnHurt;
    BaseScale = Model.localScale;
  }

  void OnHurt(HitEvent obj) {
    StopAllCoroutines();
    StartCoroutine(Animate());
  }

  void OnTriggerEnter(Collider other) {
    if (other.TryGetComponent(out AbilityManager am)) {
      Bump(am);
    }
  }

  void Bump(AbilityManager am) {
    StopAllCoroutines();
    StartCoroutine(Animate());
    var knockback = am.Abilities.Find(a => a is Knockback) as Knockback;
    if (knockback) {
      var delta = am.transform.position - transform.position;
      knockback.Run(BumpForce * delta.XZ().normalized);
    }
    //var delta = controller.Position - transform.position;
    //controller.PhysicsVelocity += BumpForce * delta.normalized;
  }

  public float AnimationDecay = 1f;
  public float AnimationAmplitude = 1f;
  public float AnimationFrequency = 3f;
  IEnumerator Animate() {
    var t = 0f;
    while (true) {
      var decay = Mathf.Exp(-AnimationDecay * t);
      if (decay < .1f)
        break;
      Model.localScale = BaseScale + AnimationAmplitude * decay * Mathf.Sin(AnimationFrequency * t) * Vector3.one;
      t += Time.fixedDeltaTime;
      yield return new WaitForFixedUpdate();
    }
    Model.localScale = BaseScale;
  }
}
