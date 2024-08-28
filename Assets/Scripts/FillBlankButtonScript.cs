using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;


public class FillBlankButtonScript : MonoBehaviour
{
    public SceneTransitioner sceneTransitioner;
    private GameLogic gameLogic;

    private bool isMouseActive; //mouseactive
    public GameObject[] toHide; //hiddenobjects

    private float mouseThreshold = 0.01f;

    public void Start()
    {


        gameLogic = FindObjectOfType<GameLogic>();

        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.isMusicPlaying == true)
            {
                AudioManager.Instance.StopMusic();
            }
        }


    }

   void Update()
    {
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


  