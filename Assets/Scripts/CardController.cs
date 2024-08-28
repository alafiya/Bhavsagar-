using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardController : MonoBehaviour
{
    public SceneTransitioner sceneTransitioner;



    private List<Button> cards = new List<Button>();
    private Button currentCard; // Track the currently clicked card

    private List<string> sceneNames = new List<string> { "DumbChar", "SingSong", "FillBlank", "QuizCard" };
    private int currentSceneIndex = 0; // Track the current scene index

    void Start()
    {
        GameObject[] cardObjects = GameObject.FindGameObjectsWithTag("Card"); // Find all buttons with the "Card" tag

        foreach (GameObject cardObject in cardObjects)
        {
            Button button = cardObject.GetComponent<Button>();
            if (button != null)
            {
                cards.Add(button);
                button.onClick.AddListener(() => HandleCardClick(button)); // Add click listener to each button
            }
        }

        LoadCardStates(); // Ensure card states are loaded when scene starts
        LoadSceneIndex(); // Load the last scene index from PlayerPrefs

        // Check if all cards are done, if so, reset them
        if (AllCardsDone())
        {
            ResetAllCards();
            SaveCardStates();
        }
    }

    private void HandleCardClick(Button card)
    {
        if (currentCard == null)
        {
            Debug.Log("Card clicked: " + card.name); // Add this line
            currentCard = card;
            card.interactable = false;
            SaveCardStates();
            Invoke("OpenNextScene", 1f);
            currentCard = null;
        }
    }

   
       
  


    public void SaveCardStates()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            PlayerPrefs.SetInt("CardState_" + i, cards[i].interactable ? 0 : 1);
        }
        PlayerPrefs.Save();
    }

    public void LoadCardStates()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (PlayerPrefs.HasKey("CardState_" + i))
            {
                bool isDone = PlayerPrefs.GetInt("CardState_" + i) == 1;
                cards[i].interactable = !isDone;
            }
        }
    }

    private void SaveSceneIndex()
    {
        PlayerPrefs.SetInt("CurrentSceneIndex", currentSceneIndex);
        PlayerPrefs.Save();
    }

    private void LoadSceneIndex()
    {
        if (PlayerPrefs.HasKey("CurrentSceneIndex"))
        {
            currentSceneIndex = PlayerPrefs.GetInt("CurrentSceneIndex");
        }
    }

    private void OpenNextScene()
    {
        // Find the next scene that isn't excluded by PlayerPrefs
        // int attempts = 0;
        // do
        // {
        //     currentSceneIndex = (currentSceneIndex + 1) % sceneNames.Count;
        //     attempts++;
        // } while (IsSceneExcluded(sceneNames[currentSceneIndex]) && attempts < sceneNames.Count);

        int nextSceneIndex = GetNextScene();
        currentSceneIndex = nextSceneIndex;

        if (nextSceneIndex == -1)//(attempts >= sceneNames.Count)
        {
            Debug.LogError("All scenes are excluded. Resetting scene states.");
            ResetSceneStates();
            currentSceneIndex = 0;
            SaveSceneIndex();
            return;
        }

        SaveSceneIndex(); // Save the current scene index
        //AudioManager.Instance.FadeOut(3.0f);
        sceneTransitioner.WaveTransition(sceneNames[currentSceneIndex]);
        Debug.Log("Attempting to open scene: " + sceneNames[currentSceneIndex]); // Add this line
        // Remaining code...
    }

    private int GetNextScene()
    {
        string lastScene = sceneNames[currentSceneIndex];
        List<string> remainingScenes = new();
        string sceneList = "";
        foreach (var scene in sceneNames)
        {
            if (!IsSceneExcluded(scene) || lastScene != scene)
            {
                remainingScenes.Add(scene);
                sceneList += scene + ", ";
            }
        }
        Debug.Log($"Trying to get next scene, remaining scenes: {sceneList}");

        if (remainingScenes.Count == 0)
        {
            return -1;
        }

        if (lastScene != "QuizCard" && remainingScenes.Contains("QuizCard"))
        {
            Debug.Log("Last scene was not a quiz card and quiz cards are available");
            int roll = Random.Range(0, 100);
            if (roll >= 70)
            {
                Debug.Log("Next scene is QuizCard");
                return sceneNames.IndexOf("QuizCard");
            }
        }
        var nextScene = sceneNames.IndexOf(remainingScenes[Random.Range(0, remainingScenes.Count)]);
        Debug.Log($"Next scene is {remainingScenes[nextScene]}");
        return nextScene;
    }

    private bool IsSceneExcluded(string sceneName)
    {
        if (sceneName == "QuizCard" && PlayerPrefs.GetInt("QCQuestionsOver", 0) == 1)
        {
            return true;
        }
        if (sceneName == "FillBlank" && PlayerPrefs.GetInt("FBQuestionsOver", 0) == 1)
        {
            return true;
        }
        if (sceneName == "SingSong" && PlayerPrefs.GetInt("SSWordsOver", 0) >= 5)
        {
            return true;
        }

        if (sceneName == "DumbChar" && PlayerPrefs.GetInt("DCCardsOver", 0) >= 5)
        {
            return true;
        }

        return false;
    }

    private bool AllCardsDone()
    {
        foreach (Button card in cards)
        {
            if (card.interactable)
            {
                return false;
            }
        }
        return true;
    }

    private void ResetAllCards()
    {
        foreach (Button card in cards)
        {
            card.interactable = true;
        }
    }

    private void ResetSceneStates()
    {
        PlayerPrefs.SetInt("QCQuestionsOver", 0);
        PlayerPrefs.SetInt("FBQuestionsOver", 0);
        PlayerPrefs.SetInt("SSWordsOver", 0);
        PlayerPrefs.Save();
    }
}

