using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class WallSegment : MonoBehaviour {
  public Color Color;
  public float Width = 1f;
  public float Height = 1;
  public float Depth = .125f;
  public DecalProjector Projector;
  public float Min;
  public float Max;

  void LateUpdate() {
    Projector.size = new Vector3((Max-Min) * Width, Height, Depth);
    Projector.uvBias = new Vector3(Min, 0);
    Projector.uvScale = new Vector3(Max-Min, 1);
    // NOTE: The "Pivot" setting of DecalProjector is important.
    // I don't entirely understand it yet but I have set it manually
    // to work with magic values in the code... should fix this
  }
}