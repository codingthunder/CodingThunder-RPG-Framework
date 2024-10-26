using NUnit.Framework;
using Moq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodingThunder.RPGUtilities.Cmds;
using System;
using UnityEngine.TestTools;
using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder;
using CodingThunder.RPGUtilities.Mechanics;

//TODO: Make this not suck. But writing Unit tests is no fun. So I'm moving back to more fun things for now.
[TestFixture]
public class CmdSequenceTests
{
    private GameObject cmdRunnerObject;
    private CmdSequence cmdSequence;
    private FakeComponent fakeComponent;

    [SetUp]
    public void Setup()
    {
        var mockResolver = new Mock<LookupResolver>();

        // Create a test GameObject and add a Movement2D component to it
        var testGameObject = new GameObject("MockObject");
        testGameObject.AddComponent<Movement2D>();

        // Set up the mock to return testGameObject when Resolve<GameObject> is called
        mockResolver.Setup(resolver => resolver.Resolve<GameObject>(It.IsAny<string>()))
                    .Returns(testGameObject);

        LookupResolver.SetInstanceForTesting(mockResolver.Object);

        cmdRunnerObject = new GameObject("CmdRunner");
        fakeComponent = cmdRunnerObject.AddComponent<FakeComponent>();
        cmdSequence = new CmdSequence
        {
            condition = new Condition { conditionExpression = "true" },
            repeat = 1,
            delay = 0.5f
        };
    }

    [TearDown]
    public void Teardown()
    {
        LookupResolver.SetInstanceForTesting(null);
        GameObject.DestroyImmediate(cmdRunnerObject);
    }

    [UnityTest]
    public IEnumerator ExecuteCmdSequence_WithValidCondition_ExecutesAllCmdExpressions()
    {
        cmdSequence.AddCmds(new List<string> { "Cmd=Brake:Target=TestPlayerString" });
        var isCompleted = false;

        yield return cmdSequence.ExecuteCmdSequence(
            fakeComponent,
            _ => isCompleted = true,
            _ => Assert.Fail("Cancel callback should not be called when condition is met.")
        );

        Assert.IsTrue(isCompleted, "CmdSequence should complete if condition is met.");
    }

    [UnityTest]
    public IEnumerator ExecuteCmdSequence_WithSuspendedState_WaitsUntilResumed()
    {
        cmdSequence.SetIsSuspended(true);
        cmdSequence.AddCmds(new List<string> { "Cmd=Brake:Target=TestPlayerString" });
        bool isCompleted = false;

        var executeCoroutine = fakeComponent.StartCoroutine(
            cmdSequence.ExecuteCmdSequence(
                fakeComponent,
                _ => isCompleted = true,
                _ => Assert.Fail("Cancel callback should not be called.")
            )
        );

        yield return new WaitForSeconds(1);
        Assert.IsFalse(isCompleted, "CmdSequence should be suspended and not completed yet.");

        cmdSequence.SetIsSuspended(false);
        yield return executeCoroutine;
        Assert.IsTrue(isCompleted, "CmdSequence should complete after being resumed.");
    }
}
