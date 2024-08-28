using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    private List<string> scenes = new List<string> { "DumbChar", "QuizCard", "FillBlank", "SingSong" };
    private string lastSceneOpened;
    public SceneTransitioner sceneTransitioner;

    public void OpenRandomScene()
    {
        string sceneToLoad = GetRandomScene();
        sceneTransitioner.WaveTransition(sceneToLoad);
        lastSceneOpened = sceneToLoad;
    }

    private string GetRandomScene()
    {
        string sceneToLoad;
        do
        {
            sceneToLoad = scenes[Random.Range(0, scenes.Count)];
        } while (sceneToLoad == lastSceneOpened); // Ensure we don't open the same scene consecutively

        return sceneToLoad;
    }
}
