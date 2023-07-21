using UnityEngine;

public class Player : MonoBehaviour {
  [HideInInspector] public PlayerMove Move;
  [HideInInspector] public PlayerSword Sword;
  [HideInInspector] public PlayerHammer Hammer;

  private void Awake() {
    this.InitComponent(out Sword); Sword.Player = this;
    this.InitComponent(out Move); Move.Player = this;
    this.InitComponent(out Hammer); Hammer.Player = this;
  }
}