using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceScript : MonoBehaviour
{
    public Sprite[] diceFaces;
    public AudioSource diceRollAudio;
    public GameLogic gameLogic;
    private SpriteRenderer diceRenderer;
    private BoxCollider2D diceCollider;
    private bool diceClickable = true; // Start with the dice not clickable
    public Animator diceAnimator;
    public string isActiveParameter = "DiceActive";
    public float delayBeforeMove = 2f; // Time to wait before moving the player
    public DBController dbController; // Reference to the DBController script


    // Start is called before the first frame update
    void Start()
    {
        // Check assignments
        diceRenderer = GetComponent<SpriteRenderer>();
        if (diceRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is not found on this GameObject.");
        }

        diceCollider = GetComponent<BoxCollider2D>();
        if (diceCollider == null)
        {
            Debug.LogError("BoxCollider2D component is not found on this GameObject.");
        }

        if (diceFaces == null || diceFaces.Length == 0)
        {
            Debug.LogError("Dice faces are not assigned.");
        }

        if (diceRollAudio == null)
        {
            Debug.LogError("Dice roll audio source is not assigned.");
        }

        SetDiceVisible(false); // Hide the dice initially
    }

    void Update()
    {
        // Set dice visibility based on its clickable state
        if (diceClickable)
        {
            SetDiceVisible(true);
            diceAnimator.SetBool(isActiveParameter, true);
        }
        else
        {
            diceAnimator.SetBool(isActiveParameter, false);
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!diceClickable)
                return;

            StartCoroutine(RollDice());
        }


        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Toggle the panel's visibility
            if (!diceClickable)
                return;

            StartCoroutine(RollDice());
        }
    
        
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

        diceRollAudio.Play();

        // Simulate rolling effect
        float rollDuration = diceRollAudio.clip.length;
        float changeInterval = 0.1f;
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
        Debug.Log("Dice Outcome : " + diceOutcome);

        // Keep the dice visible during the waiting period
        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(delayBeforeMove);

        // Start the MoveCurrentPlayer method
        gameLogic.MoveCurrentPlayer(diceOutcome);

        // Hide the dice after the roll is complete
        SetDiceVisible(false);

        
        dbController.ClearTeamDialog(); //ClearTeamDialog
        Debug.LogError("Clearing Team Dialog along with Dice");
    }

    public void SetDiceClickable(bool clickable)
    {
        diceClickable = clickable;

        // Update visibility based on the clickable state
        if (clickable)
        {
            SetDiceVisible(true);
        }
        else
        {
            SetDiceVisible(false);
            Debug.LogError("Clearing Team Dialog along with Dice");
            dbController.ClearTeamDialog(); //ClearTeamDialog
        }
    }

    public bool IsDiceClickable()
    {
        return diceClickable;
    }

    private void SetDiceVisible(bool visible)
    {
        diceRenderer.enabled = visible;
        diceCollider.enabled = visible;
    }
}
