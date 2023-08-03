using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Runtime data for a path traversal over a set of path nodes, featuring different traversal modes.
public class PathTraversal {
  public enum Modes { Looping, BackAndForth, OnlyOnce };

  List<Segment> Segments = new();
  int CurrentSegment = 0;
  Modes Mode;
  float TotalDistance = 0f;

  public PathTraversal(List<PathNode> nodes, Modes mode) {
    Mode = mode;
    BuildSegments(nodes, mode);
    Segments[CurrentSegment].Begin();
  }
  // Warps to the given fraction along the path, spatially. Range [0, 1].
  public void WarpTo(ref Vector3 pos, ref Quaternion rotation, float fraction) {
    var distance = TotalDistance * fraction;
    for (var i = 0; i < Segments.Count; i++) {
      if (Segments[i] is SegmentTraverse t) {
        if (distance <= t.TotalDistance) {
          CurrentSegment = i;
          Segments[CurrentSegment].WarpTo(ref pos, ref rotation, distance / t.TotalDistance);
          return;
        }
        distance -= t.TotalDistance;
      }
    }
    Debug.Assert(false, $"Failed to find path segment corresponding to {fraction}");
  }
  public void Advance(ref Vector3 pos, ref Quaternion rotation, float moveSpeed) {
    if (CurrentSegment < Segments.Count && Segments[CurrentSegment].Advance(ref pos, ref rotation, moveSpeed)) {
      CurrentSegment++;
      if (Mode == Modes.OnlyOnce && CurrentSegment >= Segments.Count)
        return;
      CurrentSegment %= Segments.Count;
      Segments[CurrentSegment].Begin();
    }
  }

  abstract class Segment {
    // Called when the wanderer first visits this segment.
    public abstract void Begin();
    // Jumps to a fractional completion of this segment, range [0, 1].
    public abstract void WarpTo(ref Vector3 pos, ref Quaternion rotation, float doneFraction);
    // Returns true when we're done on this segment.
    public abstract bool Advance(ref Vector3 pos, ref Quaternion rotation, float moveSpeed);
  }
  class SegmentTraverse : Segment {
    const float STOP_FRACTION = .999f;
    public Transform Start;
    public Transform End;
    Vector3 Delta, Dir;
    public float TotalDistance;
    public SegmentTraverse(Transform start, Transform end) {
      Start = start;
      End = end;
      Delta = End.position - Start.position;
      Dir = Delta.normalized;
      TotalDistance = Delta.magnitude;
    }
    public override void Begin() { }
    public override void WarpTo(ref Vector3 pos, ref Quaternion rotation, float doneFraction) {
      pos = Vector3.Lerp(Start.position, End.position, doneFraction);
      rotation = Quaternion.Lerp(Start.rotation, End.rotation, doneFraction);
    }
    public override bool Advance(ref Vector3 pos, ref Quaternion rotation, float moveSpeed) {
      pos += moveSpeed * Time.fixedDeltaTime * Dir;
      var distTraveled = (pos - Start.position).magnitude;
      var doneFraction = distTraveled / TotalDistance;
      rotation = Quaternion.Lerp(Start.rotation, End.rotation, doneFraction);
      if (doneFraction >= STOP_FRACTION) {
        pos = End.position;
        rotation = End.rotation;
        return true;
      }
      return false;
    }
  }
  class SegmentWait : Segment {
    public int WaitTicks;
    int Ticks = 0;
    public SegmentWait(Timeval duration) => WaitTicks = duration.Ticks;
    public override void Begin() => Ticks = 0;
    public override void WarpTo(ref Vector3 pos, ref Quaternion rotation, float doneFraction) {
      Ticks = Mathf.RoundToInt(doneFraction * WaitTicks);
    }
    public override bool Advance(ref Vector3 pos, ref Quaternion rotation, float moveSpeed) {
      return (++Ticks >= WaitTicks);
    }
  }

  void BuildSegments(List<PathNode> nodes, Modes mode) {
    Debug.Assert(nodes.Count > 1, "Path with < 2 nodes don't make no sense ma dude");
    for (var i = 1; i < nodes.Count; i++) {
      Segments.Add(new SegmentTraverse(nodes[i-1].transform, nodes[i].transform));
      if (nodes[i] is Waitpoint wait)
        Segments.Add(new SegmentWait(wait.Duration));
    }
    if (mode == Modes.Looping) {
      // Complete the loop.
      Segments.Add(new SegmentTraverse(nodes[nodes.Count-1].transform, nodes[0].transform));
      if (nodes[0] is Waitpoint wait)
        Segments.Add(new SegmentWait(wait.Duration));
    } else if (mode == Modes.BackAndForth) {
      // Add the way back.
      for (var i = nodes.Count-1; i > 0; i--) {
        Segments.Add(new SegmentTraverse(nodes[i].transform, nodes[i-1].transform));
        if (nodes[i-1] is Waitpoint wait)
          Segments.Add(new SegmentWait(wait.Duration));
      }
    }

    TotalDistance = Segments.Where(s => s is SegmentTraverse).Sum(s => ((SegmentTraverse)s).TotalDistance);
  }

  public void DrawGizmos() {
    Gizmos.color = Color.yellow;
    foreach (var s in Segments) {
      if (s is SegmentTraverse t) {
        var start = t.Start.position;
        var end = t.End.position;
        Gizmos.DrawLine(start, end);
      } else if (s is SegmentWait w) {
        // Draw a number?
      }
    }
  }
}

public class Waypoints : MonoBehaviour {
  public List<PathNode> Nodes;
  [Range(0, 1)] public float FollowerOffsetFromStart = 0f;

  public PathTraversal CreatePathTraversal(PathTraversal.Modes mode) {
    return new(Nodes, mode);
  }

  void OnValidate() {
    var followers = FindObjectsOfType<PathController>().Where(p => p.Waypoints == this).ToList();
    followers.ForEach((p, i) => p.SetStartOffset((FollowerOffsetFromStart + (float)i / followers.Count) % 1f));
  }

  void OnDrawGizmosSelected() {
    var path = CreatePathTraversal(PathTraversal.Modes.BackAndForth);
    path.DrawGizmos();

    var followers = FindObjectsOfType<PathController>().Where(p => p.Waypoints == this).ToList();
    followers.ForEach(f => f.OnDrawGizmosSelected());
  }
}
