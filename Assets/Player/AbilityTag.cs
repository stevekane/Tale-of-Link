using System;

public static class FlagLikeExtensions {
  public static AbilityTag AddFlags(this ref AbilityTag tag, AbilityTag mask) => tag |= mask;
  public static AbilityTag ClearFlags(this ref AbilityTag tag, AbilityTag mask) => tag &= ~mask;
  public static bool HasAllFlags(this AbilityTag tag, AbilityTag mask) => (tag & mask) == mask;
  public static bool HasAnyFlags(this AbilityTag tag, AbilityTag mask) => (tag & mask) != 0;
}

[Serializable, Flags]
public enum AbilityTag {
  Grounded = 1 << 0,
  Airborne = 1 << 1,
  WorldSpace = 1 << 2,
  WallSpace = 1 << 3,
  InSpaceTransition = 1 << 4,
  CanMove = 1 << 5,
  CanRotate = 1 << 6,
  CanAttack = 1 << 7,
  CanUseItem = 1 << 8,
  IsAttacking = 1 << 9,
  IsUsingItem = 1 << 10,
  Spawning = 1 << 11,
  Alive = 1 << 12,
  Dying = 1 << 13,
  Dead = 1 << 14,
  Reviving = 1 << 15
}