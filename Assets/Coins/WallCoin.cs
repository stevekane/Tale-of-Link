using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class WallCoin : MonoBehaviour {
  [SerializeField] CoinMaterials CoinMaterials;
  [SerializeField] Coin Coin;
  [SerializeField] DecalProjector DecalProjector;
  void Update() {
    var material = CoinMaterials.ForValue(Coin.Value, CoinSpace.Wall);
    if (material)
      DecalProjector.material = material;
  }
}
