using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class GameLogic : MonoBehaviour
{
    public GameObject[] players;
    public DBController dialogueBoxController;
    public DiceScript diceScript;
    public SceneTransitioner sceneTransition;
    public AudioSource MotiAudio;
    public AudioSource BhawarAudio;
    public GameObject[] boardPositions;
    public GameObject[] startPositions;
    public GameObject GameBoardVisual; // Rename to indicate visual element

    public int finalBoardPositionIndex; // Define the index of the final board position where player wins

    public int panThresholdIndex; // Define the index of the board position to trigger panning

    public AudioSource winAudio; // Audio source for the win sound
    public GameObject finalWinPanel;
    public GameObject[] teamWinPanels;

    private int currentPlayer = 0;
    private int[] playerPositions;
    private bool[] skipNextTurn;
    private PlayersScript[] playerScripts;
    private bool isWaitingForUttar = false;
    private const int StartPositionIndex = -1;
    private bool[] playersHaveWon;

    void Start()
    {
        playerScripts = new PlayersScript[players.Length];
        playerPositions = new int[players.Length];
        skipNextTurn = new bool[players.Length];
        playersHaveWon = new bool[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            playerScripts[i] = players[i].GetComponent<PlayersScript>();
            if (playerScripts[i] == null)
            {
                Debug.LogError("Player " + (i + 1) + " does not have a PlayersScript component.");
            }

            // Parent the players to the GameBoardVisual
            players[i].transform.SetParent(GameBoardVisual.transform);
        }

        if (PlayerPrefs.GetInt("BackToGame_Clicked", 0) == 1)
        {
            Debug.Log("Starting WaitForUttar coroutine");
            diceScript.SetDiceClickable(false);
            StartCoroutine(WaitForUttar());
            PlayerPrefs.DeleteKey("BackToGame_Clicked"); // Clear the flag
            PlayerPrefs.Save();
        }
        else
        {
            isWaitingForUttar = false; // Ensure it's false if not waiting
        }

        LoadGameState();
        dialogueBoxController.ChangeTeamDialogue(currentPlayer);
    }

    public void SetPlayerPosition(int playerIndex, int newPositionIndex)
    {
      
        if (playerIndex < 0 || playerIndex >= players.Length)
        {
            Debug.LogError("Invalid player index.");
            return;
        }

        if (newPositionIndex < 0 || newPositionIndex >= boardPositions.Length)
        {
            Debug.LogError("Invalid board position index.");
            return;
        }

        // Set the player's position immediately
        playerPositions[playerIndex] = newPositionIndex;
        players[playerIndex].transform.localPosition = GameBoardVisual.transform.InverseTransformPoint(boardPositions[newPositionIndex].transform.position);
        
        Debug.Log("Checking current player position to Pan Board");
        CheckAndPanBoard(newPositionIndex);
        
        // Optionally, update any necessary game state after setting the position
        SaveGameState(); // Save the updated game state

        // Example log for confirmation
        Debug.Log("Player " + playerIndex + " moved to position " + newPositionIndex);

        // Check if we need to pan the board
    }

    public void MoveCurrentPlayer(int steps)
    {
        StartCoroutine(MovePlayer(steps));
    }

    private IEnumerator MovePlayer(int steps)
    {
        int currentPosition = playerPositions[currentPlayer];

        Debug.Log("Checking current player position to Pan Board");
        CheckAndPanBoard(currentPosition);

        if (currentPosition < 0)
        {
            // Player is at the start position, move to the first board position
            currentPosition = 0;
            players[currentPlayer].transform.localPosition = GameBoardVisual.transform.InverseTransformPoint(startPositions[currentPlayer].transform.position);
            yield return new WaitForSeconds(0.5f);
        }

        

        int newPosition = Mathf.Min(currentPosition + steps, boardPositions.Length - 1);
        if (newPosition >= boardPositions.Length)
        {
            Debug.Log("Cannot move forward. Player exceeds board bounds.");
            Invoke("EndTurn", 3.0f); // End the turn without moving
            yield break;
        }

        // Move player step by step with a pause
        for (int i = currentPosition; i <= newPosition; i++)
        {
            players[currentPlayer].transform.localPosition = GameBoardVisual.transform.InverseTransformPoint(boardPositions[i].transform.position);
            yield return new WaitForSeconds(0.5f);
        }

        // After movement, update player position
        playerPositions[currentPlayer] = newPosition;
        Debug.Log("Moved player to position " + newPosition);

        // Pan the board again based on the new position after moving
       

        // Check if player landed on Moti or Bhawar
        if (boardPositions[newPosition].CompareTag("Moti"))
        {
            Debug.Log("Player has stepped on Moti");
            MotiAudio.Play();
            dialogueBoxController.ChangeToMotiDialogue();
            SaveGameState();
            // Set PlayerPrefs to indicate waiting for button input
            PlayerPrefs.SetInt("WaitingForUttar", 1);
            PlayerPrefs.Save();
            yield return new WaitForSeconds(5f);
            sceneTransition.WaveTransition("Cards");
            yield break;
        }
        else if (boardPositions[newPosition].CompareTag("Bhawar"))
        {
            Debug.Log("Player has stepped on Bhawar");
            BhawarAudio.Play();
            dialogueBoxController.ChangeToBhawarDialogue();
            int backPosition = Mathf.Max(newPosition - 2, 0);
            yield return new WaitForSeconds(4f);
            Debug.Log("Moving back 2 steps");
            for (int i = newPosition; i >= backPosition; i--)
            {
                players[currentPlayer].transform.localPosition = GameBoardVisual.transform.InverseTransformPoint(boardPositions[i].transform.position);
                yield return new WaitForSeconds(0.5f);
            }
            newPosition = backPosition;

            // After moving back, check if the player landed on a Moti
            if (boardPositions[newPosition].CompareTag("Moti"))
            {
                Debug.Log("Player landed on Moti after moving back from Bhawar");
                MotiAudio.Play();
                dialogueBoxController.ChangeToMotiDialogue();
                SaveGameState();
                // Set PlayerPrefs to indicate waiting for button input
                PlayerPrefs.SetInt("WaitingForUttar", 1);
                PlayerPrefs.Save();
                yield return new WaitForSeconds(5f);
                sceneTransition.WaveTransition("Cards");
                yield break;
            }
        }
        else if (boardPositions[newPosition].CompareTag("Win"))
        {
            Debug.Log("Player has reached the final position!");

            // Play win audio
            winAudio.Play();

            // Mark the player as won
            playersHaveWon[currentPlayer] = true;

            // Save game state after player wins
            SaveGameState();

            if (CheckAllPlayersWon())
            {
                // Show final win panel or handle end game logic
                ShowFinalWinPanel();
                yield break; // Exit the coroutine since the game is won
            }

            // Show the appropriate win panel
            ShowWinPanel(currentPlayer);

            players[currentPlayer].SetActive(false);

            yield break; // Exit the coroutine since the player has won
        }

        // Final update of player position after any adjustments
        playerPositions[currentPlayer] = newPosition;

        // Check if we need to pan the board
        CheckAndPanBoard(newPosition);

        // End the turn
        Invoke("EndTurn", 3.0f);
    }


    private IEnumerator WaitForUttar()
    {
        isWaitingForUttar = true;

        while (true)
        {
            yield return null;

            if (PlayerPrefs.HasKey("UttarChosen"))
            {
                int buttonClicked = PlayerPrefs.GetInt("UttarChosen");
                if (buttonClicked == 1) // RightUttar clicked
                {
                    Debug.Log("Player moves forward 3 steps");
                    dialogueBoxController.ChangeToRightDialogue();
                    Invoke("MoveForwardSteps", 6.0f);
                }
                else if (buttonClicked == 2) // WrongUttar clicked
                {
                    SkipNextTurn();
                }

                PlayerPrefs.DeleteKey("UttarChosen"); // Clear the button click flag
                isWaitingForUttar = false;
                break; // Exit coroutine
            }
        }
    }

    public void SkipNextTurn()
    {
        Debug.Log("Player misses next Turn");
        dialogueBoxController.ChangeToWrongDialogue();
        skipNextTurn[currentPlayer] = true;
        Invoke("EndTurn", 7.0f);
    }

    public void MoveForwardSteps()
    {
        StartCoroutine(MovePlayer(3));
    }

    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    public void EndTurn()
    {
        int currentPosition = playerPositions[currentPlayer];

        if (isWaitingForUttar)
        {
            return; // Do not end turn if still waiting for input
        }

        currentPlayer = (currentPlayer + 1) % players.Length;

        while (skipNextTurn[currentPlayer])
        {
            skipNextTurn[currentPlayer] = false; // Reset skip turn flag
            currentPlayer = (currentPlayer + 1) % players.Length;
        }

        if (playersHaveWon[currentPlayer])
        {
            Debug.Log("Skipping turn for Player " + (currentPlayer + 1) + " because they have already won.");
            Invoke("EndTurn", 0f); // Immediately end turn for players who have already won
            return;
        }
        Debug.Log("Checking to pan board at End Turn");
        CheckAndPanBoard(currentPosition);


        diceScript.SetDiceClickable(true);
        dialogueBoxController.ChangeTeamDialogue(currentPlayer);
    }

    public void SaveGameState()
    {
        for (int i = 0; i < players.Length; i++)
        {
            PlayerPrefs.SetInt("PlayerPosition" + i, playerPositions[i]);
            PlayerPrefs.SetInt("SkipNextTurn" + i, skipNextTurn[i] ? 1 : 0);
            PlayerPrefs.SetInt("PlayerHasWon" + i, playersHaveWon[i] ? 1 : 0);
        }

        PlayerPrefs.SetInt("CurrentPlayer", currentPlayer);
        PlayerPrefs.Save();
    }

    public void LoadGameState()
    {
        for (int i = 0; i < players.Length; i++)
        {
            playerPositions[i] = PlayerPrefs.GetInt("PlayerPosition" + i, StartPositionIndex);
            skipNextTurn[i] = PlayerPrefs.GetInt("SkipNextTurn" + i, 0) == 1;
            playersHaveWon[i] = PlayerPrefs.GetInt("PlayerHasWon" + i, 0) == 1;

            // Update player position in the scene
            if (playerPositions[i] != StartPositionIndex)
            {
                players[i].transform.localPosition = GameBoardVisual.transform.InverseTransformPoint(boardPositions[playerPositions[i]].transform.position);
            }
        }

        currentPlayer = PlayerPrefs.GetInt("CurrentPlayer", 0);
    }

    private void ShowFinalWinPanel()
    {
        finalWinPanel.SetActive(true);
    }

    private void ShowWinPanel(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= teamWinPanels.Length)
        {
            Debug.LogError("Invalid player index for win panel.");
            return;
        }

        teamWinPanels[playerIndex].SetActive(true);
    }

    private bool CheckAllPlayersWon()
    {
        foreach (bool hasWon in playersHaveWon)
        {
            if (!hasWon)
            {
                return false;
            }
        }
        return true;
    }

    private Coroutine panBoardCoroutine = null;
public void CheckAndPanBoard(int positionIndex)
    {
        Debug.Log("Starting Pan Board check");

        Vector3 originalPosition = new Vector3(5.78f, GameBoardVisual.transform.position.y, GameBoardVisual.transform.position.z);
        Vector3 finalPosition = new Vector3(-5.78f, GameBoardVisual.transform.position.y, GameBoardVisual.transform.position.z);

        // Check if the position index has crossed the pan threshold
        if (positionIndex > panThresholdIndex)
        {
            if (GameBoardVisual.transform.position != finalPosition)
            {
                if (panBoardCoroutine != null)
                {
                    StopCoroutine(panBoardCoroutine);
                }
                panBoardCoroutine = StartCoroutine(PanBoard(finalPosition));
            }
        }
        else
        {
            if (GameBoardVisual.transform.position != originalPosition)
            {
                if (panBoardCoroutine != null)
                {
      
                    StopCoroutine(panBoardCoroutine);
                }
        
                panBoardCoroutine = StartCoroutine(PanBoard(originalPosition));
            }
        }
    }

    private IEnumerator PanBoard(Vector3 targetPosition)
    {
        float panDuration = 2f;  // Adjust this value as needed
        Vector3 startPosition = GameBoardVisual.transform.position;

        Debug.Log("Starting to pan the board from " + startPosition + " to " + targetPosition);

        float elapsedTime = 0;

        while (elapsedTime < panDuration)
        {
            GameBoardVisual.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / panDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GameBoardVisual.transform.position = targetPosition;
        Debug.Log("Finished panning the board to " + targetPosition);
    }
}