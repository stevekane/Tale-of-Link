using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PotteryLowpolyPack
{
    public class CameraRay : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.CompareTag("Destroyable"))
                    {
                        hit.collider.gameObject.GetComponent<Destroyable_WholeItem>().Destroy();
                    }
                }
            }
        }
    }
}
