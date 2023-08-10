using UnityEngine;

[ExecuteAlways]
public class WorldCoin : MonoBehaviour {
  [SerializeField] CoinMaterials CoinMaterials;
  [SerializeField] Coin Coin;
  [SerializeField] MeshRenderer MeshRenderer;
  void Update() {
    var material = CoinMaterials.ForValue(Coin.Value, CoinSpace.World);
    if (material)
      MeshRenderer.sharedMaterial = material;
  }
}