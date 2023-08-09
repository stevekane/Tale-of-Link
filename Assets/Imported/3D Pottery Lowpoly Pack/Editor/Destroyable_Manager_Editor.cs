using PotteryLowpolyPack;
using UnityEditor;


namespace PotteryLowpolyPack
{
    [CustomEditor(typeof(Destroyable_Manager))]
    public class Destroyable_Manager_Editor : Editor
    {

        SerializedProperty m_recoverDestroyables;
        SerializedProperty m_onCollisionActionType;
        SerializedProperty m_multipleTAGs;
        SerializedProperty m_singleTAG;
        SerializedProperty m_timeOfDeseapiring;
        SerializedProperty m_destroyable_InParts;

        private void OnEnable()
        {
            m_recoverDestroyables = serializedObject.FindProperty("m_recoverDestroyables");
            m_onCollisionActionType = serializedObject.FindProperty("m_onCollisionActionType");
            m_multipleTAGs = serializedObject.FindProperty("m_multipleTAGs");
            m_singleTAG = serializedObject.FindProperty("m_singleTAG");
            m_timeOfDeseapiring = serializedObject.FindProperty("m_timeOfDeseapiring");
            m_destroyable_InParts = serializedObject.FindProperty("m_destroyable_InParts");

        }

        public override void OnInspectorGUI()
        {
            Destroyable_Manager _destroyable_Manager = (Destroyable_Manager)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_recoverDestroyables);
            if (_destroyable_Manager.m_RecoverDestroyables)
            {
                EditorGUILayout.PropertyField(m_timeOfDeseapiring);
            }

            EditorGUILayout.PropertyField(m_onCollisionActionType);

            if (_destroyable_Manager.m_OnCollisionActionType == OnCollistionActionType.SINGLE_TAG_Comparsion)
            {
                EditorGUILayout.PropertyField(m_singleTAG);
            }
            if (_destroyable_Manager.m_OnCollisionActionType == OnCollistionActionType.MULTIPLE_TAG_Comparsion)
            {
                EditorGUILayout.PropertyField(m_multipleTAGs);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

