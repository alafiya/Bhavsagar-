using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverButtonManager : MonoBehaviour
{
    public CanvasGroup buttonGroup; // CanvasGroup for the buttons
    public float fadeDuration = 0.2f; // Duration for the fade effect

    private Coroutine fadeCoroutine;

    void Start()
    {
        // Ensure buttons are initially hidden
        buttonGroup.alpha = 0;
        buttonGroup.interactable = false;
        buttonGroup.blocksRaycasts = false;
    }

    public void OnMouseEnter()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(buttonGroup, buttonGroup.alpha, 1, fadeDuration));
    }

    public void OnMouseExit()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(buttonGroup, buttonGroup.alpha, 0, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = end;
        canvasGroup.interactable = end > 0;
        canvasGroup.blocksRaycasts = end > 0;
    }
}
