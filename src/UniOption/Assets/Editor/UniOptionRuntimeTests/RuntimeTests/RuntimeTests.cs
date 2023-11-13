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
    [Test]
    public void RuntimeTestsSimplePasses() {
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator UnityObjectSerializedReferenceTest() {
        var i = 0;
        testMonoBehaviour.Agent.Do(agent => { }, () => i++);
        Assert.AreEqual(1, i);
        yield return null;
    }
}