using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WallMover : MonoBehaviour {
  public Collider Collider;
  public List<RaycastHit> Hits = new();
  public List<RaycastHit> PotentialHits = new();
  public int MaxSearchCount = 10;
  public int Target = 100;
  public float MaxDistance = 1f;
  public float Spacing = .25f;
  public float WallOffset = .05f;
  public bool ShowHits;

  // TODO: Using Update since FixedUpdate doesn't work with ExecuteAlways...yay
  void Update() {
    var position = transform.position;
    var normal = transform.forward;
    var tangent = Vector3.Cross(transform.forward, Vector3.up);
    Hits.Clear();
    for (var i = 0; i < MaxSearchCount; i++) {
      for (var a = -135; a <= 135; a+=45) {
        var d = Quaternion.Euler(0, a, 0) * normal;
        // Debug.DrawRay(position+WallOffset*d, -MaxDistance*d, Color.blue);
        var didHit = Physics.Raycast(position+WallOffset*d, -d, out RaycastHit hit, MaxDistance);
        var didHitBackward = didHit
          ? Physics.Raycast(hit.point-WallOffset*hit.normal, hit.normal, MaxDistance)
          : false;
        if (i == Target) {
          var color = (didHit, didHitBackward) switch {
            (true, true) => Color.yellow,
            (true, false) => Color.green,
            _ => Color.red
          };
          Debug.DrawRay(position-WallOffset*d, MaxDistance*d, color);
        }
        if (didHit && !didHitBackward) {
          PotentialHits.Add(hit);
        }
      }
      if (PotentialHits.Count > 0) {
        float bestScore = float.MinValue;
        RaycastHit hit = PotentialHits[0];
        for (var p = 0; p < PotentialHits.Count; p++)  {
          var score = Vector3.Dot(normal, PotentialHits[p].normal);
          if (score > bestScore) {
            bestScore = score;
            hit = PotentialHits[p];
          }
        }
        position = hit.point;
        normal = hit.normal;
        tangent = Vector3.Cross(normal, Vector3.up);
        position += Spacing * tangent;
        Hits.Add(hit);
        PotentialHits.Clear();
      } else {
        break;
      }
    }
    // Debug.Log(Hits.Count);
  }

  void OnDrawGizmos() {
    if (!ShowHits)
      return;
    foreach (var hit in Hits) {
      Debug.DrawRay(hit.point, hit.normal);
    }
  }
}