using System.Collections.Generic;
using UnityEngine;

public class EquipmentVisibility : MonoBehaviour {
  static void Show(GameObject go) => go.SetActive(true);
  static void Hide(GameObject go) => go.SetActive(false);

  public GameObject Hammer;
  public GameObject Sword;
  public GameObject Shield;

  List<GameObject> BaseObjects = new();
  List<GameObject> CurrentObjects = new();

  public void AddBaseObject(GameObject go) {
    go.SetActive(true);
    BaseObjects.Add(go);
  }

  public void AddCurrentObject(GameObject go) {
    BaseObjects.ForEach(Hide);
    go.SetActive(true);
    CurrentObjects.Add(go);
  }

  public void ClearCurrentObjects() {
    CurrentObjects.ForEach(Hide);
    CurrentObjects.Clear();
    BaseObjects.ForEach(Show);
  }

  void Start() {
    Hammer.SetActive(false);
    Sword.SetActive(false);
    Shield.SetActive(false);
    AddBaseObject(Sword);
    AddBaseObject(Shield);
  }
}