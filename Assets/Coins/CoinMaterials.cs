using System;
using UnityEngine;

[Serializable]
public struct CoinMaterial {
  public int Value;
  public Color Color;
  public Material WorldMaterial;
  public Material WallMaterial;
}

public enum CoinSpace {
  World,
  Wall
}

[CreateAssetMenu(fileName = "CoinMaterials", menuName = "Coin/Materials")]
public class CoinMaterials : ScriptableObject {
  [SerializeField] CoinMaterial[] Materials;

  void OnValidate() {
    foreach (var material in Materials) {
      material.WorldMaterial.color = material.Color;
      material.WallMaterial.color = material.Color;
    }
  }

  public Material ForValue(int value, CoinSpace space) {
    Material material = null;
    foreach (var mat in Materials) {
      if (mat.Value == value)
        material = space == CoinSpace.World ? mat.WorldMaterial : mat.WallMaterial;
    }
    return material;
  }
}