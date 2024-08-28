using UnityEngine;

public class PlayersScript : MonoBehaviour
{
    private Animator animator;
    private Renderer objectRenderer;

    public string isActiveParameter = "isActive"; // The boolean parameter to indicate active state
    public int activeSortingOrder = 5; // Sorting order for active state
    public int inactiveSortingOrder = 3; // Sorting order for inactive state

    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        objectRenderer = GetComponent<Renderer>();

        if (animator == null)
        {
            Debug.LogError("Animator component is missing on " + gameObject.name);
        }

        if (objectRenderer == null)
        {
            Debug.LogError("Renderer component is missing on " + gameObject.name);
        }
    }

    private void Start()
    {
        
          
    }

    public void SetActive(bool isActive)
    {
        animator.SetBool(isActiveParameter, isActive);
        objectRenderer.sortingOrder = isActive ? activeSortingOrder : inactiveSortingOrder;
        
    }

    public static void UpdatePlayerAnimations(PlayersScript[] players, int currentPlayerIndex)
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(i == currentPlayerIndex);
        }
    }
}