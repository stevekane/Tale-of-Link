using UnityEngine;
using TMPro;

public class CollectionInfo : MonoBehaviour {
  [SerializeField] TextMeshProUGUI Text;

  public void SetInfo(string info) {
    Text.text = info;
  }
}