using UnityEngine;

[ExecuteAlways]
public class WorldCoin : MonoBehaviour {
  [SerializeField] CoinMaterials CoinMaterials;
  [SerializeField] Coin Coin;
  [SerializeField] MeshRenderer MeshRenderer;
  void Update() {
    MeshRenderer.sharedMaterial = CoinMaterials.ForValue(Coin.Value);
  }
}
