using System;
using UnityEngine;

[Serializable]
public class FloorGroup {
  public float MinRenderHeight;
  public GameObject[] Chunks;
}

public class FloorManager : LevelManager<FloorManager> {
  public FloorGroup[] FloorGroups;
  public Player Player;
  public Action<int> OnFloorChange;

  int CurrentFloor;

  void Start() {
    base.Awake();
    PlayerManager.Instance.OnPlayerSpawn += SpawnPlayer;
  }

  void OnDestroy() {
    PlayerManager.Instance.OnPlayerSpawn -= SpawnPlayer;
  }

  void SpawnPlayer(Player player) {
    Player = player;
  }

  void FixedUpdate() {
    if (!Player)
      return;
    var height = Player.transform.position.y;
    var currentFloor = 0;
    for (var i = 0; i < FloorGroups.Length; i++) {
      var floorGroup = FloorGroups[i];
      if (height >= floorGroup.MinRenderHeight) {
        currentFloor = i;
      }
    }
    if (currentFloor != CurrentFloor) {
      OnFloorChange?.Invoke(currentFloor);
    }
    CurrentFloor = currentFloor;
    // enable all floors we are on and below
    foreach (var group in FloorGroups) {
      var isActive = height >= group.MinRenderHeight;
      foreach (var chunk in group.Chunks) {
        chunk.SetActive(isActive);
      }
    }
  }

  void DespawnPlayer(Player player) {
    FloorGroups.ForEach(group => group.Chunks.ForEach(chunk => chunk.SetActive(false)));
  }
}