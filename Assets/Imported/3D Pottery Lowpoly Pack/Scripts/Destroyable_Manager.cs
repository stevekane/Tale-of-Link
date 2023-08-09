using System.Collections.Generic;
using UnityEngine;

namespace PotteryLowpolyPack
{

    public class Destroyable_Manager : MonoBehaviour
    {
        public static Destroyable_Manager m_Instance;


        [Header("Settings")]
        [SerializeField] bool m_recoverDestroyables;
        public bool m_RecoverDestroyables => m_recoverDestroyables;


        [SerializeField] OnCollistionActionType m_onCollisionActionType;
        public OnCollistionActionType m_OnCollisionActionType => m_onCollisionActionType;
        [SerializeField] string[] m_multipleTAGs;
        [SerializeField] string m_singleTAG;

        [SerializeField][Range(1f, 5f)] float m_timeOfDeseapiring;
        public float m_TimeOfDeseapiring => m_timeOfDeseapiring;

        [SerializeField] Destroyable_InParts[] m_destroyable_InParts;

        Dictionary<Destroyable_InParts_Name, Queue<Destroyable_InParts>> m_dictionary_Of_Quenes = new Dictionary<Destroyable_InParts_Name, Queue<Destroyable_InParts>>();
        Dictionary<Destroyable_InParts_Name, Destroyable_InParts> m_dictionary_Of_Prefabs = new Dictionary<Destroyable_InParts_Name, Destroyable_InParts>();


        void Awake()
        {
            if (m_Instance == null) m_Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            for (int i = 0; i < m_destroyable_InParts.Length; i++)
            {
                if (!m_dictionary_Of_Quenes.ContainsKey(m_destroyable_InParts[i].m_DestroyableType))
                {
                    m_dictionary_Of_Quenes.Add(m_destroyable_InParts[i].m_DestroyableType, new Queue<Destroyable_InParts>());
                }
                else Debug.Log("Error: multiple destroyables have same Destroyable_InParts_Name assigned");


                if (!m_dictionary_Of_Prefabs.ContainsKey(m_destroyable_InParts[i].m_DestroyableType))
                {
                    m_dictionary_Of_Prefabs.Add(m_destroyable_InParts[i].m_DestroyableType, m_destroyable_InParts[i]);
                }
                else Debug.Log("Error: multiple destroyables have same Destroyable_InParts_Name assigned");
            }
        }

        public Destroyable_InParts Grab_Destroyable_InParts(Destroyable_InParts_Name _required_Destroyable_InParts_Name)
        {
            if (!m_dictionary_Of_Quenes.ContainsKey(_required_Destroyable_InParts_Name))
            {
                Debug.Log("Error: required destroyable not found in dictionary");
            }

            if (m_dictionary_Of_Quenes[_required_Destroyable_InParts_Name].Count > 0) return m_dictionary_Of_Quenes[_required_Destroyable_InParts_Name].Dequeue();
            else
            {
                Destroyable_InParts _new_destroyable_InParts = Instantiate(m_dictionary_Of_Prefabs[_required_Destroyable_InParts_Name]);
                _new_destroyable_InParts.transform.SetParent(this.transform);
                return _new_destroyable_InParts;
            };
        }
        public void ReturnToQuene(Destroyable_InParts_Name _returning_destroyableType, Destroyable_InParts _destroyable_InParts)
        {
            if (!m_dictionary_Of_Quenes.ContainsKey(_returning_destroyableType))
            {
                Debug.Log("Error: returning destroyable not found in dictionary");
            }

            m_dictionary_Of_Quenes[_returning_destroyableType].Enqueue(_destroyable_InParts);
        }
        public bool Multiple_TAG_Confirmation(string _tagOfCollidedObject)
        {
            if (m_multipleTAGs.Length ==0) return false;
            for (int i = 0; i < m_multipleTAGs.Length; i++)
            {
                if (m_multipleTAGs[i] == _tagOfCollidedObject) return true;
            }
            return false;
        }
        public bool Single_TAG_Confirmation(string _tagOfCollidedObject)
        {
            if (m_singleTAG == _tagOfCollidedObject) return true;
            return false;
        }
    }
    public enum OnCollistionActionType
    {
        None,
        DESTROY,
        SINGLE_TAG_Comparsion,
        MULTIPLE_TAG_Comparsion
    }
}
 