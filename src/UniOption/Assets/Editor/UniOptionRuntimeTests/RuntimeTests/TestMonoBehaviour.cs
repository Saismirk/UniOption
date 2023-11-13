using UniOption;
using UnityEngine;
using UnityEngine.AI;

namespace Editor.UniOptionRuntimeTests.RuntimeTests {
    public class TestMonoBehaviour : MonoBehaviour {
        [SerializeField] Option<NavMeshAgent> _agent = Option<NavMeshAgent>.None;

        public Option<NavMeshAgent> Agent => _agent;
    }
}