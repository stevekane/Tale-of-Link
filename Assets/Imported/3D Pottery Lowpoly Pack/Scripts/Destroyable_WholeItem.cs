using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotteryLowpolyPack
{

    public class Destroyable_WholeItem : MonoBehaviour
    {
        public Destroyable_InParts_Name m_DestroyableType;
        OnCollistionActionType m_collistionActionType;

        public void Destroy()
        {
            Destroyable_InParts _new_Destroyable_InParts = Destroyable_Manager.m_Instance.Grab_Destroyable_InParts(m_DestroyableType);
            _new_Destroyable_InParts.transform.position = this.transform.position;
            _new_Destroyable_InParts.transform.rotation = this.transform.rotation;
            _new_Destroyable_InParts.transform.localScale = this.transform.localScale;
            _new_Destroyable_InParts.gameObject.SetActive(true);
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            m_collistionActionType = Destroyable_Manager.m_Instance.m_OnCollisionActionType;
            if (m_collistionActionType == OnCollistionActionType.None) return;

            if (m_collistionActionType == OnCollistionActionType.DESTROY)
            {
                Destroy();
                return;
            }

            if (m_collistionActionType == OnCollistionActionType.SINGLE_TAG_Comparsion)
            {
                if (Destroyable_Manager.m_Instance.Single_TAG_Confirmation(collision.gameObject.tag)) Destroy();
                return;
            }

            if (m_collistionActionType == OnCollistionActionType.MULTIPLE_TAG_Comparsion)
            {
                if (Destroyable_Manager.m_Instance.Multiple_TAG_Confirmation(collision.gameObject.tag)) Destroy();
                return;
            }
        }
    }



    public enum Destroyable_InParts_Name
    {
        Pottery_1_1b,
        Pottery_1_2b,
        Pottery_1_3b,
        Pottery_2_1b,
        Pottery_2_2b,
        Pottery_2_3b,
        Pottery_3_1b,
        Pottery_3_2b,
        Pottery_3_3b,
        Pottery_4_1b,
        Pottery_4_2b,
        Pottery_4_3b,
        Pottery_5_1b,
        Pottery_5_2b,
        Pottery_5_3b,
        Pottery_6_1b,
        Pottery_6_2b,
        Pottery_6_3b,
        Pottery_7_1b,
        Pottery_7_2b,
        Pottery_7_3b,
        Pottery_8_1b,
        Pottery_8_2b,
        Pottery_8_3b,
        Pottery_9_1b,
        Pottery_9_2b,
        Pottery_9_3b,
        Pottery_10_1b,
        Pottery_10_2b,
        Pottery_10_3b,
        Pottery_11_1b,
        Pottery_11_2b,
        Pottery_11_3b,
        Pottery_12_1b,
        Pottery_12_2b,
        Pottery_12_3b, 
        Pottery_13_1b,
        Pottery_13_2b,
        Pottery_13_3b,
        Pottery_14_1b,
        Pottery_14_2b,
        Pottery_14_3b,
        Pottery_15_1b,
        Pottery_15_2b,
        Pottery_15_3b,
        Pottery_16_1b,
        Pottery_16_2b,
        Pottery_16_3b,
        Pottery_17_1b,
        Pottery_17_2b,
        Pottery_17_3b,
        Pottery_18_1b,
        Pottery_18_2b,
        Pottery_18_3b,
        Pottery_19_1b,
        Pottery_19_2b,
        Pottery_19_3b,
        Pottery_20_1b,
        Pottery_20_2b,
        Pottery_20_3b,
        Pottery_21_1b,
        Pottery_21_2b,
        Pottery_21_3b,
        Pottery_22_1b,
        Pottery_22_2b,
        Pottery_22_3b,
        Pottery_23_1b,
        Pottery_23_2b,
        Pottery_23_3b,
        Pottery_24_1b,
        Pottery_24_2b,
        Pottery_24_3b
    }
}

