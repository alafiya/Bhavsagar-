using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DiceRoller : MonoBehaviour
{
    public Sprite[] diceFaces;
    public AudioSource diceRollAudio; 
    public AudioSource MotiAudio;
    public AudioSource BhawarAudio;
    public SceneTransitioner sceneTransition;
    public int totalPlayers;
    public GameObject[] players; 
    public float moveSpeed = 5f; 
    public DBController dialogueBoxController; 
    public SpriteRenderer diceRenderer;
    

    private BoxCollider2D diceCollider;
    private int currentPlayer = 0;
    private int[] playerPositions;
    private Transform[] boardPositions; 
    private bool diceClickable = true;
    private PlayerAnimatorController[] playerAnimators;

    void Start()
    {
        // Initialize diceRenderer
        diceRenderer = GetComponent<SpriteRenderer>();
        if (diceRenderer == null)
            Debug.LogError("SpriteRenderer component is not found on this GameObject.");

        diceCollider = GetComponent<BoxCollider2D>();
        if (diceCollider == null)
            Debug.LogError("BoxCollider2D component is not found on this GameObject.");

        // Check assignments
        if (diceFaces == null || diceFaces.Length == 0)
            Debug.LogError("Dice faces are not assigned.");

        if (diceRollAudio == null)
            Debug.LogError("Dice roll audio source is not assigned.");

        if (players == null || players.Length == 0)
            Debug.LogError("Players are not assigned.");

        totalPlayers = players.Length;
        playerPositions = new int[totalPlayers];
        playerAnimators = new PlayerAnimatorController[totalPlayers];

        // Store references to player animator controllers
        for (int i = 0; i < totalPlayers; i++)
        {
            playerAnimators[i] = players[i].GetComponent<PlayerAnimatorController>();
            if (playerAnimators[i] == null)
            {
                Debug.LogError("Player " + (i + 1) + " does not have a PlayerAnimatorController component.");
            }
        }

        
        boardPositions = FindBoardPositions();

        // Load player positions and current player index if saved
        LoadGameState();

        UpdatePlayerAnimations();
        // Update the dialogue box for the current player's turn
        dialogueBoxController.ChangeTeamDialogue(currentPlayer);
    }

    void OnMouseDown()
    {
        if (!diceClickable)
            return;

        StartCoroutine(RollDice());
    }

    private IEnumerator RollDice()
    {
        // Disable dice clickable until the next turn
        diceClickable = false;

        // Play the dice roll audio
        diceRollAudio.Play();

        // Simulate rolling effect
        float rollDuration = diceRollAudio.clip.length; // Duration of the roll animation
        float changeInterval = 0.1f; // Interval between sprite changes
        float elapsedTime = 0f;

        while (elapsedTime < rollDuration)
        {
            // Randomly pick a dice face to simulate rolling
            int randomFace = Random.Range(0, diceFaces.Length);
            diceRenderer.sprite = diceFaces[randomFace];

            // Wait for a short interval before changing again
            yield return new WaitForSeconds(changeInterval);
            elapsedTime += changeInterval;
        }

        // Determine the final dice face outcome
        int diceOutcome = Random.Range(1, 7);
        diceRenderer.sprite = diceFaces[diceOutcome - 1];
        Debug.Log("DiceOutcome:" + diceOutcome); 

        yield return new WaitForSeconds(1f);
        // Move the player and wait for the movement to complete
        yield return StartCoroutine(MovePlayer(diceOutcome));

        // Wait for the specified delay before the next turn
        yield return new WaitForSeconds(1f);

        // Update the current player for the next turn
        currentPlayer = (currentPlayer + 1) % totalPlayers;

        UpdatePlayerAnimations();
        // Update the dialogue box for the current player's turn
        dialogueBoxController.ChangeTeamDialogue(currentPlayer);


        // Reset dice clickable for the next player's turn
        diceClickable = true;
    }

    private IEnumerator MovePlayer(int steps)
    {


        int currentPosition = playerPositions[currentPlayer];
        int newPosition = currentPosition + steps;

        // Ensure newPosition does not exceed boardPositions array length
        newPosition = Mathf.Min(newPosition, boardPositions.Length - 1);

        // Move player to the newPosition
        yield return StartCoroutine(MovePlayerAlongPath(players[currentPlayer].transform, currentPosition, newPosition));

        // Update the dialogue box based on the new position
        if (boardPositions[newPosition].CompareTag("Moti"))
        {
            Debug.Log("Player " + (currentPlayer + 1) + " landed on Moti.");
            MotiAudio.Play();
            dialogueBoxController.ChangeToMotiDialogue();

            // Update player position to the new position
            playerPositions[currentPlayer] = newPosition;
            // Save game state before loading the new scene
            SaveGameState();
            yield return new WaitForSeconds(5f);
            sceneTransition.WaveTransition("Cards");
            yield break; // Exit coroutine early after loading scene
        }
        else if (boardPositions[newPosition].CompareTag("Bhawar"))
        {
            Debug.Log("Player " + (currentPlayer + 1) + " landed on Bhawar.");
            BhawarAudio.Play();
            dialogueBoxController.ChangeToBhawarDialogue();

            int backPosition = Mathf.Max(newPosition - 2, 0); // Ensure the player doesn't go below position 0
            yield return new WaitForSeconds(4f);
            yield return StartCoroutine(MovePlayerAlongPath(players[currentPlayer].transform, newPosition, backPosition));
            newPosition = backPosition;
        }

        // Update playerPositions to newPosition
        playerPositions[currentPlayer] = newPosition;
        Debug.Log("Player " + (currentPlayer + 1) + " moves to position " + newPosition);

        
    }

    private IEnumerator MovePlayerAlongPath(Transform player, int startPosition, int endPosition)
    {
        float elapsedTime = 0f;

        for (int i = startPosition + 1; i <= endPosition; i++)
        {
            Vector3 start = boardPositions[i - 1].position;
            Vector3 end = boardPositions[i].position;
            float distance = Vector3.Distance(start, end);
            float duration = distance / moveSpeed; // Calculate duration based on distance and speed

            while (elapsedTime < duration)
            {
                player.position = Vector3.Lerp(start, end, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f; // Reset for the next segment
        }

        for (int i = startPosition - 1; i >= endPosition; i--)
        {
            Vector3 start = boardPositions[i + 1].position;
            Vector3 end = boardPositions[i].position;
            float distance = Vector3.Distance(start, end);
            float duration = distance / moveSpeed; // Calculate duration based on distance and speed

            while (elapsedTime < duration)
            {
                player.position = Vector3.Lerp(start, end, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f; // Reset for the next segment
        }
    }

    private void UpdatePlayerAnimations()
    {
        for (int i = 0; i < totalPlayers; i++)
        {
            playerAnimators[i].SetActive(i == currentPlayer);
        }
    }

    private Transform[] FindBoardPositions()
    {
        int positionIndex = 0;
        List<Transform> positions = new List<Transform>();

        while (true)
        {
            GameObject positionObject = GameObject.Find("boardPos" + (positionIndex == 0 ? "" : $" ({positionIndex})"));
            if (positionObject == null)
            {
                break;
            }

            positions.Add(positionObject.transform);
            positionIndex++;
        }

        return positions.ToArray();
    }

    public void SaveGameState()
    {
        for (int i = 0; i < totalPlayers; i++)
        {
            PlayerPrefs.SetInt("PlayerPosition_" + i, playerPositions[i]);
        }
        PlayerPrefs.SetInt("CurrentPlayer", (currentPlayer + 1) % totalPlayers); // Save the next player's turn
        PlayerPrefs.Save();
    }

    private void LoadGameState()
    {
        for (int i = 0; i < totalPlayers; i++)
        {
            if (PlayerPrefs.HasKey("PlayerPosition_" + i))
            {
                playerPositions[i] = PlayerPrefs.GetInt("PlayerPosition_" + i);
                players[i].transform.position = boardPositions[playerPositions[i]].position;
            }
        }

        if (PlayerPrefs.HasKey("CurrentPlayer"))
        {
            currentPlayer = PlayerPrefs.GetInt("CurrentPlayer");
        }
    }

    public int GetCurrentPlayerIndex()
    {
        return currentPlayer;
    }


}