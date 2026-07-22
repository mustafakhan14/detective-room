using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectiveRoomSceneTests
{
    private const string ScenePath = "Assets/Scenes/DetectiveRoom.unity";
    private const string GeneratedRootName = "__DetectiveRoomGenerated";

    [Test]
    public void DetectiveRoomSceneHasBootstrapAndGeneratedRoom()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        Assert.IsTrue(scene.IsValid());
        Assert.AreEqual("DetectiveRoom", scene.name);

        DetectiveRoomBootstrap bootstrap = Object.FindObjectOfType<DetectiveRoomBootstrap>();
        Assert.IsNotNull(bootstrap);

        bootstrap.RebuildDetectiveRoom();
        Transform generatedRoot = bootstrap.transform.Find(GeneratedRootName);

        Assert.IsNotNull(generatedRoot);
        Assert.IsNotNull(generatedRoot.GetComponent<InteractionController>());
        Assert.IsNotNull(generatedRoot.GetComponent<GameObjective>());
        Assert.IsNotNull(generatedRoot.Find("2.5D Orthographic Camera"));
    }
}
