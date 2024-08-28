using UnityEngine;


public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private Renderer objectRenderer;  // Reference to the Renderer component
    public string isActiveParameter = "isActive"; // The boolean parameter to indicate active state
    public int activeSortingOrder = 5; // Sorting order for active state
    public int inactiveSortingOrder = 3; // Sorting order for inactive state

    void Awake()
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

    public void SetActive(bool isActive)
    {
        animator.SetBool(isActiveParameter, isActive);
        objectRenderer.sortingOrder = isActive ? activeSortingOrder : inactiveSortingOrder;
    }
}
