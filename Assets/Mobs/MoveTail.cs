using UnityEngine;

public class MoveTail : MonoBehaviour {
  public Transform Head;
  public Transform[] TailBones;
  public float[] TailSeparations;

  void Start() {
    // Detach the tail so it moves separately.
    TailBones.ForEach(t => t.SetParent(null));
  }

  void OnDestroy() {
    TailBones.ForEach(t => { if (t) t.gameObject.Destroy(); });
  }

  void FixedUpdate() {
    for (int i = TailBones.Length - 1; i >= 0; i--)
      MoveTailBone(TailBones[i], i > 0 ? TailBones[i-1] : Head, TailSeparations[i]);
  }

  void MoveTailBone(Transform tb, Transform tbNext, float maxDist) {
    var delta = tbNext.position - tb.position;
    var dir = delta.normalized;
    if (delta.sqrMagnitude > maxDist.Sqr())
      tb.position = tbNext.position - maxDist*dir;
    if (delta.sqrMagnitude > .001f)
      tb.forward = dir;
  }
}