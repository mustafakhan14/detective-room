using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DetectiveRoomBootstrapPlayModeTests
{
    private const string GeneratedRootName = "__DetectiveRoomGenerated";

    private GameObject host;

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (host != null)
        {
            Object.Destroy(host);
            host = null;
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator BootstrapBuildsRuntimeRoomWithExpectedInteractables()
    {
        host = new GameObject("Bootstrap PlayMode Test Host");
        DetectiveRoomBootstrap bootstrap = host.AddComponent<DetectiveRoomBootstrap>();

        yield return null;

        Transform generatedRoot = bootstrap.transform.Find(GeneratedRootName);

        Assert.IsNotNull(generatedRoot);
        Assert.IsNotNull(generatedRoot.GetComponent<InteractionController>());
        Assert.IsNotNull(generatedRoot.GetComponent<GameObjective>());
        Assert.IsNotNull(generatedRoot.Find("2.5D Orthographic Camera"));
        Assert.IsNotNull(generatedRoot.Find("Detective"));
        AssertInteractables(generatedRoot);
    }

    private static void AssertInteractables(Transform generatedRoot)
    {
        HashSet<string> evidenceIds = new HashSet<string>();
        Interactable[] interactables = generatedRoot.GetComponentsInChildren<Interactable>(true);

        for (int i = 0; i < interactables.Length; i++)
        {
            evidenceIds.Add(interactables[i].evidenceId);
        }

        CollectionAssert.Contains(evidenceIds, "broken_glass");
        CollectionAssert.Contains(evidenceIds, "ledger");
        CollectionAssert.Contains(evidenceIds, "locked_door");
        CollectionAssert.Contains(evidenceIds, "radio_dispatcher");
    }
}
