using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Add this to handle UI elements

public class MMButtonScript : MonoBehaviour
{
    public Button continueButton;  // Reference to the Continue button
    public SceneTransitioner sceneTransitioner;
    public GameObject GamePanel;
    public Button newGameButton;
  

    void Start()
    {
        AudioManager.Instance.FadeIn(3.0f);

        // Check if there's a saved game state
        if (PlayerPrefs.HasKey("CurrentPlayer"))
        {
            continueButton.interactable = true;  // Enable the Continue button if a saved game state exists
        }
        else
        {
            continueButton.interactable = false;  // Disable the Continue button if no saved game state exists
        }

    
    }

    public void NewGame()
    {
        Debug.Log("Starting New Game");
        
        // Clear any saved game state
        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetInt("CallHelp", 1);
        PlayerPrefs.Save();

        sceneTransitioner.WaveTransition("Board");
        // Load the Board scene

    }

    public void Continue()
    {
        // Check if there's a saved game state
        if (PlayerPrefs.HasKey("CurrentPlayer"))
        {
            Debug.Log("Continuing previous Game");
            // Load the Board scene
            sceneTransitioner.WaveTransition("Board");
        }
        else
        {
            Debug.LogWarning("No saved game state found.");
        }
    }
    
    public void QuitGame()
    {
        Application.Quit(); 
    }

    public void OpenGamePanel()
    {
        // Check if there's a saved game state
        if (PlayerPrefs.HasKey("CurrentPlayer"))
        {
            GamePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);

        }
        else
        {
            NewGame();
          
        }

        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }

   
}