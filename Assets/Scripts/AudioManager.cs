using UnityEngine;
using System.Collections;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClip[] soundtracks; // Array to hold your soundtracks
    private AudioSource audioSource;
    public bool isMusicPlaying = false;
    void Awake()
    {
      
           
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
      
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        

    }

    // Method to randomly select one of the soundtracks
    private void RandomizeSoundtrack()
    {
        int index = Random.Range(0, soundtracks.Length);
        audioSource.clip = soundtracks[index];
    }

    // Method to fade out the music over a specified duration
    public void FadeOut(float fadeDuration)
    {
        StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    private IEnumerator FadeOutCoroutine(float fadeDuration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        isMusicPlaying = false;
        audioSource.volume = startVolume;
    }

    // Method to fade in the music over a specified duration
    public void FadeIn(float fadeDuration)
    {
        StartCoroutine(FadeInCoroutine(fadeDuration));
    }

    private IEnumerator FadeInCoroutine(float fadeDuration)
    {
        RandomizeSoundtrack();
        audioSource.Play();
        isMusicPlaying = true;
        audioSource.volume = 0f;

        float targetVolume = 1f;

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
