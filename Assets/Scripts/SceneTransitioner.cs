using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public Animator wavetransition;
    public Animator cloudtransition;
    public AudioSource waveSound;
    public float transTime;
    private string sceneToTrans;

    public void WaveTransition(string sceneName)
    {
        sceneToTrans = sceneName;
        StartCoroutine(WaveTrans());
    }

    IEnumerator WaveTrans()
    {
        Debug.Log("Wave Transition Started");

        wavetransition.SetTrigger("Start");

        if (waveSound != null)
        {
            waveSound.Play();
        }

        yield return new WaitForSeconds(transTime);

        Debug.Log("Loading Scene: " + sceneToTrans);
        SceneManager.LoadScene(sceneToTrans);
    }

    public void CloudTransition(string sceneName)
    {
        sceneToTrans = sceneName;
        StartCoroutine(CloudTrans());
    }

    IEnumerator CloudTrans()
    {
        Debug.Log("Cloud Transition Started");

        cloudtransition.SetTrigger("Start");

        yield return new WaitForSeconds(transTime);

        Debug.Log("Loading Scene: " + sceneToTrans);
        SceneManager.LoadScene(sceneToTrans);
    }
}