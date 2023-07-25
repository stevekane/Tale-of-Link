using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/ItemProto")]
public class ItemProto : ScriptableObject {
  // This is the item object that is added to the owning character's hierarchy, and will hold the Ability script for using the item.
  [SerializeField] TmpAbility ItemAbility;

  public TmpAbility AddAbilityToCharacter(GameObject character) {
    if (ItemAbility) {
      var instance = Instantiate(ItemAbility, character.transform);
      instance.AbilityManager = character.GetComponent<Player>(); // TODO
      return instance;
    }
    return null;
  }
}