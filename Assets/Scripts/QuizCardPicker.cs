using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

public class QuizCardPicker : MonoBehaviour
{
    public GameObject[] canvases;
    private List<int> remainingCanvases;
    public SceneTransitioner sceneTransitioner;
    

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.isMusicPlaying == true)
            {
                AudioManager.Instance.StopMusic();
            }
        }




        if (canvases.Length == 0)
        {
            Debug.LogWarning("No canvases assigned in the inspector.");
            return;
        }

        InitializeCanvases();

        // Check if all canvases have been chosen
        if (remainingCanvases.Count == 0)
        {
            Debug.Log("All question assets seen");
            PlayerPrefs.SetInt("QCQuestionsOver", 0);
            
            return;
        }

        // Select a random canvas to activate
        PickRandomCanvas();
    }

    void InitializeCanvases()
    {
        // Deactivate all canvases at the start
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(false);
        }

        // Initialize the list of remaining canvases
        remainingCanvases = new List<int>();

        for (int i = 0; i < canvases.Length; i++)
        {
            if (!PlayerPrefs.HasKey("Canvas_" + i))
            {
                remainingCanvases.Add(i);
            }
        }

        // If all canvases have been used, clear the PlayerPrefs and reset the list
        if (remainingCanvases.Count == 0)
        {
            Debug.Log("All question assets seen");
            PlayerPrefs.DeleteAll();
            for (int i = 0; i < canvases.Length; i++)
            {
                remainingCanvases.Add(i);
            }
        }
    }

    void PickRandomCanvas()
    {
        if (remainingCanvases.Count == 0)
        {
            Debug.Log("All question assets seen");
            PlayerPrefs.SetInt("QCQuestionsOver", 1);
            return;
        }

        int randomIndex = Random.Range(0, remainingCanvases.Count);
        int canvasIndex = remainingCanvases[randomIndex];

        // Activate the chosen canvas
        canvases[canvasIndex].SetActive(true);

        // Mark the canvas as seen
        PlayerPrefs.SetInt("Canvas_" + canvasIndex, 1);

        // Remove the chosen canvas from the list
        remainingCanvases.RemoveAt(randomIndex);

       

    }

    public void LoadCardsScene()
    {
        SceneManager.LoadScene("Cards");
        AudioManager.Instance.FadeIn(3.0f);

        // Ensure card states are updated when returning to Cards scene
        CardController cardController = FindObjectOfType<CardController>();
        if (cardController != null)
        {
            cardController.LoadCardStates();
        }
    }

    public void BackToBoard()
    {
        PlayerPrefs.SetInt("BackToGame_Clicked", 1);
        PlayerPrefs.Save();
        sceneTransitioner.WaveTransition("Board");
        AudioManager.Instance.FadeIn(3.0f);
    }

   

    

    

   

}
