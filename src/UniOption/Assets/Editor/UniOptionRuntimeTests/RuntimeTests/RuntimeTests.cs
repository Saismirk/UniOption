using System.Collections;
using System.Collections.Generic;
using Editor.UniOptionRuntimeTests.RuntimeTests;
using NUnit.Framework;
using UniOption;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class RuntimeTests {
    GameObject           gameObject;
    TestMonoBehaviour    testMonoBehaviour;

    [SetUp]
    public void Setup() {
        gameObject = new GameObject();
        testMonoBehaviour = gameObject.AddComponent<TestMonoBehaviour>();
    }

    [UnityTest]
    public IEnumerator UnityObjectSerializedReferenceTest() {
        var i = 0;
        Assert.IsTrue(testMonoBehaviour.Transform.IsSome);
        testMonoBehaviour.Transform.Do((t, index) => { }, i, _ => { });
        if (testMonoBehaviour.NavMeshAgent.IsNone) {
            i++;
        }
        Assert.AreEqual(1, i);
        yield return null;
    }
}