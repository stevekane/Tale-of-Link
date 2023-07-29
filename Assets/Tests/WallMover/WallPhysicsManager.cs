using UnityEngine;

/*
There are two kinds of entities:

  MovingWall
  WallSpaceController

Here is what a fixed update loop MUST look like to be sane:
  - You want to evaluate the current physics world as a WallSpaceController.
    This will find points in the current space you would like to move along.
  - You also want to determine which PhysicsMover you are attached to if any
    and register yourself with them.
  - You register your desire to move as a delta in worldspace.

  You allow all PhysicsMovers to record their desired motion in world space.
  You then update the PhysicsMovers and change their locations.
  You then update the Controllers and change their locations as the sum of their delta and their mover's delta;

  UpdatePhysicsWorld
*/
public class WallPhysicsManager : LevelManager<WallPhysicsManager> {
}