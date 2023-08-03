using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class ItemDictionary : SerializableDictionary<ItemProto, int> { }
public class Inventory : MonoBehaviour {
  [SerializeField] ItemDictionary Items = new();

  public ItemDictionary Contents => Items;
  public Action<(AbilityAction, bool)> OnNewItemAbility;
  public Action<ItemProto> OnAddItem;
  public Action<ItemProto> OnRemoveItem;

  public int Count(ItemProto item) => Items.GetValueOrDefault(item);
  public void Add(ItemProto item, int count = 1) {
    Items.Increment(item, count);
    OnAddItem?.Invoke(item);
    if (item.AddAbilityToCharacter(gameObject) is var ability && ability != null) {
      OnNewItemAbility?.Invoke((ability.Main, item.SwordSlot));
    }
  }
  public void Remove(ItemProto item, int count = 1) {
    Debug.Assert(Items[item] >= count);
    OnRemoveItem?.Invoke(item);
    Items.Decrement(item, count);
  }
  public void MoveTo(Inventory other) {
    foreach (var kv in Items)
      other.Add(kv.Key, kv.Value);
    Items.Clear();
  }
  public void MoveTo(Inventory other, ItemProto item, int count = 1) {
    Remove(item, count);
    other.Add(item, count);
  }
}