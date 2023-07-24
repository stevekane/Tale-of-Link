using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class HeartContainers : MonoBehaviour {
  [SerializeField] float FillSpeed = 1;
  [SerializeField] Hearts Hearts;

  float Current;
  float TargetCurrent;

  GridLayoutGroup LayoutGroup;
  RectTransform RectTransform;
  HeartContainer[] Containers;

  void Awake() {
    LayoutGroup = GetComponent<GridLayoutGroup>();
    RectTransform = GetComponent<RectTransform>();
    Containers = GetComponentsInChildren<HeartContainer>();
    Hearts.OnSetCurrent += SetCurrent;
    Hearts.OnChangeCurrent += ChangeCurrent;
    Hearts.OnSetTotal += SetTotal;
    Hearts.OnChangeTotal += SetTotal;
  }

  void OnDestroy() {
    Hearts.OnSetCurrent -= SetCurrent;
    Hearts.OnChangeCurrent -= SetCurrent;
  }

  void LateUpdate() {
    var width = RectTransform.rect.width / 10f;
    var height = RectTransform.rect.height / 2f;
    var dimension = Mathf.Min(width, height);
    LayoutGroup.cellSize = new(dimension, dimension);
  }

  void Update() {
    Current = Mathf.MoveTowards(Current, TargetCurrent, FillSpeed * Time.deltaTime);
    var hearts = Current / 4;
    var whole = Mathf.FloorToInt(hearts);
    var fraction = hearts % 1;
    var lastIndex = whole + (fraction == 0 ? -1 : 0);
    for (var i = 0; i < Containers.Length; i++) {
      Containers[i].TargetFill = i < whole ? 1 : 0;
    }
    for (var i = 0; i < Containers.Length; i++) {
      Containers[i].Beating = i == lastIndex && Current == TargetCurrent;
    }
    if (whole < Containers.Length) {
      Containers[whole].TargetFill = fraction;
    }
  }

  void SetCurrent(int current) {
    TargetCurrent = current;
    Current = current;
  }

  void ChangeCurrent(int current) {
    TargetCurrent = current;
  }

  void SetTotal(int total) {
    var count = total / 4;
    for (var i = 0; i < Containers.Length; i++) {
      Containers[i].gameObject.SetActive(i < count);
    }
  }
}