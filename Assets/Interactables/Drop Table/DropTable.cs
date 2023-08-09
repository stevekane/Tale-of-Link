using System;
using UnityEngine;

[Serializable]
public struct DropTableEntry {
  public float Weight;
  public GameObject Prefab;
}

[CreateAssetMenu(fileName = "Drop Table", menuName = "DropTable")]
public class DropTable : ScriptableObject {
  public DropTableEntry[] Entries;

  public bool TryGet(out GameObject gameObject) {
    var totalWeight = 0f;
    foreach (var entry in Entries) {
      totalWeight += entry.Weight;
    }
    var randomWeight = UnityEngine.Random.Range(0, totalWeight);
    foreach (var entry in Entries) {
      if (randomWeight <= entry.Weight) {
        gameObject = entry.Prefab;
        return entry.Prefab;
      }
      randomWeight -= entry.Weight;
    }
    gameObject = null;
    return false;
  }
}