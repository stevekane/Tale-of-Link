using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class WallCoin : MonoBehaviour {
  [SerializeField] CoinMaterials CoinMaterials;
  [SerializeField] Coin Coin;
  [SerializeField] DecalProjector DecalProjector;
  void Update() {
    DecalProjector.material = CoinMaterials.ForValue(Coin.Value);
  }
}
