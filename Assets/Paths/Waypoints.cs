using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Runtime data for a path traversal over a set of path nodes, featuring different traversal modes.
public class PathTraversal {
  public enum Modes { Looping, BackAndForth };

  List<Segment> Segments = new();
  int CurrentSegment = 0;

  public PathTraversal(List<PathNode> nodes, Modes mode) {
    BuildSegments(nodes, mode);
    Segments[CurrentSegment].Begin();
  }
  public void Advance(ref Vector3 pos, ref Quaternion rotation, float moveSpeed) {
    if (Segments[CurrentSegment].Advance(ref pos, ref rotation, moveSpeed)) {
      CurrentSegment = (CurrentSegment+1)%Segments.Count;
      Segments[CurrentSegment].Begin();
    }
  }

  abstract class Segment {
    // Called when the wanderer first visits this segment.
    public abstract void Begin();
    // Returns true when we're done on this segment.
    public abstract bool Advance(ref Vector3 pos, ref Quaternion rotation, float moveSpeed);
    public abstract Segment Reversed();
  }
  class SegmentTraverse : Segment {
    const float STOP_FRACTION = .99f;
    public Transform Start;
    public Transform End;
    Vector3 Delta, Dir;
    float TotalDistance;
    public override void Begin() {
      Delta = End.position - Start.position;
      Dir = Delta.normalized;
      TotalDistance = Delta.magnitude;
    }
    public override bool Advance(ref Vector3 pos, ref Quaternion rotation, float moveSpeed) {
      pos += moveSpeed * Time.fixedDeltaTime * Dir;
      var distTraveled = (pos - Start.position).magnitude;
      var doneFraction = distTraveled / TotalDistance;
      rotation = Quaternion.Lerp(Start.rotation, End.rotation, doneFraction);
      return doneFraction >= STOP_FRACTION;
    }
    public override Segment Reversed() => new SegmentTraverse { End = Start, Start = End };
  }
  class SegmentWait : Segment {
    public int WaitTicks;
    int Ticks = 0;
    public override void Begin() => Ticks = 0;
    public override bool Advance(ref Vector3 pos, ref Quaternion rotation, float moveSpeed) {
      return (++Ticks >= WaitTicks);
    }
    public override Segment Reversed() => new SegmentWait { WaitTicks = WaitTicks };
  }

  void BuildSegments(List<PathNode> nodes, Modes mode) {
    Debug.Assert(nodes.Count > 1, "Path with < 2 nodes don't make no sense ma dude");
    for (var i = 1; i < nodes.Count; i++) {
      Segments.Add(new SegmentTraverse { Start = nodes[i-1].transform, End = nodes[i].transform });
      if (nodes[i] is Waitpoint wait)
        Segments.Add(new SegmentWait { WaitTicks = wait.Duration.Ticks });
    }
    if (mode == Modes.Looping) {
      Segments.Add(new SegmentTraverse { Start = nodes[nodes.Count-1].transform, End = nodes[0].transform });
    } else {
      var reversed = Segments.Select(s => s.Reversed()).Reverse();
      Segments.AddRange(reversed);
    }
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

  public PathTraversal CreatePathTraversal(PathTraversal.Modes mode) {
    return new(Nodes, mode);
  }

  void OnDrawGizmosSelected() {
    var path = CreatePathTraversal(PathTraversal.Modes.BackAndForth);
    path.DrawGizmos();
  }
}
