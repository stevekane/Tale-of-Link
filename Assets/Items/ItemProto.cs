using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/ItemProto")]
public class ItemProto : ScriptableObject {
  // This is the item object that is added to the owning character's hierarchy, and will hold the Ability script for using the item.
  [SerializeField] ClassicAbility ItemAbility;
  [SerializeField] public bool SwordSlot = false;
  [SerializeField] public GameObject HUDGameObject;
  [SerializeField] public GameObject InventoryGameObject;

  public ClassicAbility AddAbilityToCharacter(GameObject character) {
    if (ItemAbility) {
      var instance = Instantiate(ItemAbility, character.transform);
      instance.enabled = true;
      return instance;
    }
    return null;
  }
}