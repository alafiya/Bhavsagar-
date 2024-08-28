using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class DumbCharButtonScript : MonoBehaviour
{

   
    public SceneTransitioner sceneTransitioner;
    private GameLogic gameLogic;
    public AudioSource rightAudio;
    public AudioSource wrongAudio;
   
    public Button backToGame;
    public Animator BackToGameAnimator;
    public Button rightButton;
    public Button wrongButton;
    public GameObject HowToPlay;

    private bool isMouseActive; //mouseactive
    public GameObject[] toHide; //hiddenobjects

    private float mouseThreshold = 0.01f;

    public void Start()
    {
       
        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.isMusicPlaying == true)
            {
                AudioManager.Instance.StopMusic();
            }
        }


        if (SceneManager.GetActiveScene().name == "DumbChar")
        {
            if (!PlayerPrefs.HasKey("DCCardsOver"))
            {
                PlayerPrefs.SetInt("DCCardsOver", 1);
            }
            else
            {
                PlayerPrefs.SetInt("DCCardsOver", PlayerPrefs.GetInt("DCCardsOver") + 1);
            }
        }
        if (SceneManager.GetActiveScene().name == "SingSong")
        {
            if (!PlayerPrefs.HasKey("SSWordsOver"))
            {
                PlayerPrefs.SetInt("SSWordsOver", 1);
            }
            else
            {
                PlayerPrefs.SetInt("SSWordsOver", PlayerPrefs.GetInt("SSWordsOver") + 1);
            }
        }

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


    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the panel's visibility
            if (backToGame.interactable == true)
            {
                PlayerPrefs.SetInt("BackToGame_Clicked", 1);
                PlayerPrefs.Save();

                sceneTransitioner.WaveTransition("Board");
                AudioManager.Instance.FadeIn(3.0f);
            }
     
        }

        if (Mathf.Abs(Input.GetAxis("Mouse X")) > mouseThreshold || Mathf.Abs(Input.GetAxis("Mouse Y")) > mouseThreshold || Input.anyKeyDown)
        {
            isMouseActive = true;
        }
        else
        {
            isMouseActive = false;
        }

        // Update asset visibility based on mouse activity
        UpdateAssetVisibility();
    }

    private void UpdateAssetVisibility()
    {
        foreach (var asset in toHide)
        {
            asset.SetActive(!isMouseActive); // Hide assets when the mouse is active
        }
    }

    public void RightButton()
    {
        PlayerPrefs.SetInt("UttarChosen", 1); // Correct
        rightAudio.Play();
        Debug.Log("Right Answer");
        BackToGameAnimator.SetTrigger("AnimateButton");

        rightButton.interactable = false;
        wrongButton.interactable = false;
     
        backToGame.interactable = true;
    }
    public void WrongButton()
    {
        PlayerPrefs.SetInt("UttarChosen", 2); // Incorrect
        wrongAudio.Play();
        Debug.Log("Wrong Answer");
        BackToGameAnimator.SetTrigger("AnimateButton");

        rightButton.interactable = false;
        wrongButton.interactable = false;
    
        backToGame.interactable = true;

    }

    public void Help()
    {
        HowToPlay.SetActive(true);
    }

    public void CloseHelp()
    {
        HowToPlay.SetActive(false);
    }

}




