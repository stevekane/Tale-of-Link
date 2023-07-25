using System;
using UnityEngine;

/*
Player in air
Player on ground
  Move
  Attack
  Block
  Item1
    ...items
  Item2
    ...items
  Action
    MergeWall
    Grab
    Interact
Player in wall
  Popout
  Move
*/
public class Player : MonoBehaviour {
  [HideInInspector] public PlayerMove Move;
  private void Awake() {
    this.InitComponent(out Move); Move.Player = this;
  }
}