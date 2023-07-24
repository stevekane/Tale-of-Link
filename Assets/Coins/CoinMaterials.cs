using System;
using UnityEngine;

[Serializable]
public struct CoinMaterial {
  public int Value;
  public Material Material;
}

[CreateAssetMenu(fileName = "CoinMaterials", menuName = "Coin/Materials")]
public class CoinMaterials : ScriptableObject {
  [SerializeField] CoinMaterial[] Materials;

  public Material ForValue(int value) {
    Material material = null;
    foreach (var mat in Materials) {
      if (mat.Value == value)
        material = mat.Material;
    }
    return material;
  }
}