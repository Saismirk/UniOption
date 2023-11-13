using System;
using UniOption;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Editor.UniOptionRuntimeTests.RuntimeTests {
    public class TestMonoBehaviour : MonoBehaviour {
        [field: SerializeField] public Option<NavMeshAgent> Agent { get; set; } = null;

        void Start() {
            Debug.Assert(Agent.IsNone);
            Agent.Do(agent => {
                agent.updateRotation = false;
                agent.updateUpAxis = false;
            });
        }

        [CustomEditor(typeof(TestMonoBehaviour))]
        public class TestMonoBehaviourEditor : UnityEditor.Editor {
            TestMonoBehaviour TestMonoBehaviour => target as TestMonoBehaviour;
            public override void OnInspectorGUI() {
                base.OnInspectorGUI();
                EditorGUILayout.LabelField(TestMonoBehaviour.Agent.IsSome ? "Some" : "None");
            }
        }
    }
}