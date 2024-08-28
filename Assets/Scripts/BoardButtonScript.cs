using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardButtonScript : MonoBehaviour
{
    public GameLogic gameLogic;
    public GameObject finalWinPanel;
    public GameObject Team1_WinSlate;
    public GameObject Team2_WinSlate;
    public GameObject Team3_WinSlate;
    public GameObject QuitPanel;
    public Button boardButton;

    public SceneTransitioner sceneTransitioner;
    public int ChooseCurrentPlayer;
    public int ChoosePlayerPosition;
    public HowToPlayScript HowToPlay;
   
    private bool isMouseActive; //mouseactive
    public GameObject[] toHide; //hiddenobjects

    private float mouseThreshold = 0.01f;
    private void Start()
    {
        if (PlayerPrefs.HasKey("CallHelp") && PlayerPrefs.GetInt("CallHelp") == 1)
        {

            HowToPlay.Help();

            PlayerPrefs.DeleteKey("CallHelp");
            PlayerPrefs.Save();
        }
    }

    public void SetPosition()
    {
        gameLogic.SetPlayerPosition(ChooseCurrentPlayer, ChoosePlayerPosition);
    }


    public void OpenQuitPanel()
    {
        QuitPanel.SetActive(true);
    }

    public void BackToScene()
    {
        QuitPanel.SetActive(false);
    }

    public void CloseGame()
    {

        if (gameLogic != null)
        {
            gameLogic.SaveGameState();
        }
        else
        {
            Debug.LogError("GameLogic component not found on GameController GameObject.");
        }

        Application.Quit();
    }
    public void CloseFinalWin()
    {
        Debug.Log("Ending Game");

        // Clear any saved game state
        PlayerPrefs.DeleteAll();

        AudioManager.Instance.FadeOut(1.0f);
        sceneTransitioner.WaveTransition("MainMenu");
    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.X))
        {
            // Toggle the panel's visibility
            if (QuitPanel.activeSelf)
            {
                QuitPanel.SetActive(false); // Close the panel if it's open
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                QuitPanel.SetActive(true); // Open the panel if it's closed
                EventSystem.current.SetSelectedGameObject(boardButton.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Team1_WinSlate.activeSelf || Team2_WinSlate.activeSelf || Team3_WinSlate.activeSelf)
            {
                if (Team1_WinSlate.activeSelf)
                {
                    Team1_WinSlate.SetActive(false);
                    gameLogic.EndTurn();
                }
                else
                {
                    if (Team2_WinSlate.activeSelf)
                    {
                        Team2_WinSlate.SetActive(false);
                        gameLogic.EndTurn();
                    }

                    else 
                    {
                        Team3_WinSlate.SetActive(false);
                        gameLogic.EndTurn();
                    }
                }
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

    public void CloseWinSlate1()
    {
        Team1_WinSlate.SetActive(false);
        gameLogic.EndTurn();
    }

    public void CloseWinSlate2()
    {
        Team2_WinSlate.SetActive(false);
        gameLogic.EndTurn();
    }

    public void CloseWinSlate3()
    {
        Team3_WinSlate.SetActive(false);
        gameLogic.EndTurn();
    }

    public void BackToMain()
    {
        gameLogic.SaveGameState();
        sceneTransitioner.WaveTransition("MainMenu");
    }
}