using UnityEngine;

public interface IItemAbility {
  public enum Buttons { West, North, South };
  public AbilityAction Action { get; }
  public Buttons DefaultButtonAssignment { get; }
}

[CreateAssetMenu(fileName = "Item", menuName = "Items/ItemProto")]
public class ItemProto : ScriptableObject {
  // This is the item object that is added to the owning character's hierarchy, and will hold the Ability script for using the item.
  [SerializeField] Ability ItemAbility;

  public IItemAbility AddAbilityToCharacter(GameObject character) {
    if (ItemAbility) {
      var instance = Instantiate(ItemAbility, character.transform);
      instance.enabled = true;
      return (IItemAbility)instance;
    }
    return null;
  }
}