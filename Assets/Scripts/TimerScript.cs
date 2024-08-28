using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class TimerScript : MonoBehaviour
{
    public float time;
    public TextMeshProUGUI TimerText;
    public Image Fill;
    public float Max;
    public GameObject panel;  // Reference to the panel to be activated
    public Button ReadyButton;
    public Button SkipButton;
    public Button rightButton;
    public Color fullColor;
    public Color halfColor;
    public Color quarterColor;
    public AudioSource TimerTicking;
    public AudioSource TimerDing;
    private bool isTimerRunning = false;
    private bool hasTickingPlayed = false;  // Flag to track if the ticking sound has been played
    private bool hasDingPlayed = false;     // Flag to track if the ding sound has been played

    private void Start()
    {
        if (ReadyButton != null)
        {
            ReadyButton.gameObject.SetActive(true);
        }

        if (SkipButton != null)
        {
            SkipButton.onClick.AddListener(SkipTimer); // Add listener for skip button
            SkipButton.gameObject.SetActive(false); // Hide skip button at start
        }

        // Initialize the color to fullColor when the timer starts
        Fill.color = fullColor;
        UpdateTimerUI();

        // Ensure the panel is inactive at the start
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            EventSystem.current.SetSelectedGameObject(SkipButton.gameObject);
            time -= Time.deltaTime;
            UpdateTimerUI();
            UpdateTimerSound();
            if (time <= 0)
            {
                time = 0;
                isTimerRunning = false;

                // Activate the panel when the timer runs out
                Invoke("ActivatePanel", 2f);

                // Hide the skip button when timer ends
                if (SkipButton != null)
                {
                    SkipButton.gameObject.SetActive(false);
                }
            }
        }
    }

    public void StartTimer()
    {
        ReadyButton.gameObject.SetActive(false);
        isTimerRunning = true;
        hasTickingPlayed = false;  // Reset the flag when the timer starts
        hasDingPlayed = false;     // Reset the flag when the timer starts

        // Show the skip button when the timer starts
        if (SkipButton != null)
        {
            SkipButton.gameObject.SetActive(true);
        }
    }

    private void UpdateTimerUI()
    {
        TimerText.text = "" + (int)time;
        Fill.fillAmount = time / Max;

        // Change color based on remaining time percentage
        float remainingPercentage = time / Max;

        if (remainingPercentage <= 0.25f)
        {
            // Change to quarterColor when 25% or less of time is remaining
            Fill.color = quarterColor;
        }
        else if (remainingPercentage <= 0.5f)
        {
            // Change to halfColor when 50% or less of time is remaining
            Fill.color = halfColor;
        }
        else
        {
            // Otherwise, keep fullColor
            Fill.color = fullColor;
        }
    }

    private void UpdateTimerSound()
    {
        if (time <= 11 && !hasTickingPlayed)
        {
            // Play the ticking sound when there are 10 seconds left
            TimerTicking.Play();
            hasTickingPlayed = true;  // Set the flag to true so the sound is played only once
        }

        if (time <= 0 && !hasDingPlayed)
        {
            TimerTicking.Stop();
            // Play the ding sound when the time reaches 0
            TimerDing.Play();
            hasDingPlayed = true;  // Set the flag to true so the sound is played only once
        }
    }

    public void SkipTimer()
    {
        if (isTimerRunning)
        {
            time = 0;
            isTimerRunning = false;

            // Activate the panel immediately
            ActivatePanel();

            // Hide the skip button
            if (SkipButton != null)
            {
                SkipButton.gameObject.SetActive(false);
            }

            // Stop any ticking sound that might be playing
            if (TimerTicking != null)
            {
                TimerTicking.Stop();
            }

            // Play the ding sound immediately
            if (TimerDing != null && !hasDingPlayed)
            {
                TimerDing.Play();
                hasDingPlayed = true;
            }
        }
    }

    public void ActivatePanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(rightButton.gameObject);
        }
    }
}
