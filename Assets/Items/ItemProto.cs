using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/ItemProto")]
public class ItemProto : ScriptableObject {
  [SerializeField] ClassicAbility ItemAbility;
  [SerializeField] public bool SwordSlot = false;
  [SerializeField] public GameObject DisplayGameObject;
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