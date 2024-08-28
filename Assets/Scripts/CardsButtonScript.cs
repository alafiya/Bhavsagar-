using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardsButtonScript : MonoBehaviour
{

    public GameObject QuitPanel;
    public CardController CardController;
    public Button boardButton;
    public Button card1;
    
    private bool isMouseActive; //mouseactive
    public GameObject[] toHide; //hiddenobjects
    private float mouseThreshold = 0.01f;

    public void LoadBoardScene()
    {
        SceneManager.LoadScene("Board");
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

        if (CardController != null)
        {
            CardController.SaveCardStates();
        }

        Application.Quit();
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
                EventSystem.current.SetSelectedGameObject(card1.gameObject);
            }
            else
            {
                QuitPanel.SetActive(true); // Open the panel if it's closed
                EventSystem.current.SetSelectedGameObject(boardButton.gameObject);
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
}

