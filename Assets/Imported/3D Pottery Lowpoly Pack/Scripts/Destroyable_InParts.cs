using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace PotteryLowpolyPack
{

    public class Destroyable_InParts : MonoBehaviour
    {
        [SerializeField] Destroyable_InParts_Name m_destroyableType;
        public Destroyable_InParts_Name m_DestroyableType => m_destroyableType;

        bool m_wasEnabled = false;
        Quaternion[] m_initialRotations;
        Vector3[] m_initialPositions;
        Rigidbody[] m_rigidBodies;
        Collider[] m_collliders;
        Transform[] m_picesOfDestroyables;

        int m_picesOfDestroyablesCount;
        WaitForSecondsRealtime m_disapearDelay;

        void enable()
        {
            m_picesOfDestroyablesCount = transform.childCount;
            m_picesOfDestroyablesCount++;

            m_picesOfDestroyables = new Transform[m_picesOfDestroyablesCount];
            m_initialRotations = new Quaternion[m_picesOfDestroyablesCount];
            m_initialPositions = new Vector3[m_picesOfDestroyablesCount];
            m_rigidBodies = new Rigidbody[m_picesOfDestroyablesCount];
            m_collliders = new Collider[m_picesOfDestroyablesCount];

            m_picesOfDestroyables[0] = this.transform;
            for (int i = 1; i < m_picesOfDestroyablesCount; ++i)
            {
                m_picesOfDestroyables[i] = transform.GetChild(i - 1);
            }

            for (int i = 0; i < m_picesOfDestroyablesCount; ++i)
            {
                m_initialRotations[i] = m_picesOfDestroyables[i].rotation;
                m_initialPositions[i] = m_picesOfDestroyables[i].position;
                m_rigidBodies[i] = m_picesOfDestroyables[i].GetComponent<Rigidbody>();
                m_collliders[i] = m_picesOfDestroyables[i].GetComponent<Collider>();
            }

            m_disapearDelay = new WaitForSecondsRealtime(Destroyable_Manager.m_Instance.m_TimeOfDeseapiring);
            m_wasEnabled = true;
        }


        void OnEnable()
        {
            if (!m_wasEnabled) enable();

            for (int i = 0; i < m_picesOfDestroyablesCount; ++i)
            {
                m_rigidBodies[i].WakeUp();
                m_rigidBodies[i].useGravity = true;
                m_collliders[i].enabled = true;
            }

            StartCoroutine(selfDestroy());
        }

        IEnumerator selfDestroy()
        {
            yield return m_disapearDelay;
            if (Destroyable_Manager.m_Instance.m_RecoverDestroyables) restoreAndReturToQuene();
        }

        void restoreAndReturToQuene()
        {
            for (int i = 0; i < m_picesOfDestroyablesCount; i++)
            {
                m_picesOfDestroyables[i].rotation = m_initialRotations[i];
                m_picesOfDestroyables[i].position = m_initialPositions[i];
                m_rigidBodies[i].useGravity = false;
                m_rigidBodies[i].Sleep();
                m_collliders[i].enabled = false;
            }

            Destroyable_Manager.m_Instance.ReturnToQuene(m_destroyableType, this);
            this.gameObject.SetActive(false);
        }

    }
}
