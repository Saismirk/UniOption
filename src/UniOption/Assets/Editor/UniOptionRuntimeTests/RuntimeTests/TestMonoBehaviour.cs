using System;
using UniOption;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Editor.UniOptionRuntimeTests.RuntimeTests {
    public class TestMonoBehaviour : MonoBehaviour {
        [field: SerializeField] public SerializableOption<Transform>    Transform    { get; set; }
        [field: SerializeField] public SerializableOption<NavMeshAgent> NavMeshAgent { get; set; }

        void Start() {
            Debug.Assert(Transform.IsSome);
            Debug.Assert(NavMeshAgent.IsSome);
            Transform.Do(Debug.Log);
        }
    }
}