using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathWanderer : MonoBehaviour {
  public enum Modes { Looping, BackAndForth };
  public List<PathNode> Nodes;
  public Modes Mode;

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
  List<Segment> Segments = new();
  int CurrentSegment = 0;

  void Awake() {
    Debug.Assert(Nodes.Count > 1, "Path with < 2 nodes don't make no sense ma dude");
    for (var i=1; i < Nodes.Count; i++) {
      Segments.Add(new SegmentTraverse { Start = Nodes[i-1].transform, End = Nodes[i].transform });
      if (Nodes[i] is Waitpoint wait)
        Segments.Add(new SegmentWait { WaitTicks = wait.Duration.Ticks });
    }
    if (Mode == Modes.Looping) {
      Segments.Add(new SegmentTraverse { Start = Nodes[Nodes.Count-1].transform, End = Nodes[0].transform });
    } else {
      var reversed = Segments.Select(s => s.Reversed()).Reverse();
      Segments.AddRange(reversed);
    }

    transform.position = Nodes[0].transform.position;
    transform.rotation = Nodes[0].transform.rotation;
    Segments[CurrentSegment].Begin();
  }
}
