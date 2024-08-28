using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class QuestButtonScript : MonoBehaviour
{
    private GameLogic gameLogic;
    public VideoPlayer videoPlayer;
    public Image PPImage;
    public Sprite PlaySprite;
    public Sprite PauseSprite;
    public Button BackButton;
    public Button ForwardButton;
    public CanvasGroup videoPanel;
    public CanvasGroup QPanel;
    private int currentPanelIndex = 0;
    private CanvasGroup[] panels;
    private bool isPlaying;
    private bool isMouseActive; //mouseactive
    public GameObject[] toHide; //hiddenobjects

    public Animator forwardButtonAnimator; // Reference to the Animator component

    private float mouseThreshold = 0.01f;

    public void Start()
    {
        // Initialize panels array
        panels = new CanvasGroup[] { videoPanel, QPanel };

        // Show the first panel
        ShowPanel(0);

        UpdateButtonSprite();

        // Update button states
        UpdateButtonStates();

        gameLogic = FindObjectOfType<GameLogic>();

     
    }

    void Update()
    {
        UpdateButtonSprite();
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePlayPause();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ForwardArrow();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            BackArrow();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
          
          ReplayVideo();
          
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
    private void ShowPanel(int index)
    {
        // Hide all panels
        foreach (var panel in panels)
        {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }

        // Show the selected panel
        panels[index].alpha = 1;
        panels[index].interactable = true;
        panels[index].blocksRaycasts = true;

        // Pause all videos
        videoPlayer.Pause();

        // Play video for the active panel
        if (index == 0)
        {
            videoPlayer.Play();
        }

        currentPanelIndex = index;

        // Update button states
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        BackButton.interactable = currentPanelIndex > 0;

    }

    public void TogglePlayPause()
    {
        if (isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }

        isPlaying = !isPlaying;
        UpdateButtonSprite();
    }

    private void UpdateButtonSprite()
    {
        if (videoPlayer.isPlaying)
        {
            PPImage.sprite = PauseSprite;
            isPlaying = true;
        }
        else
        {
            PPImage.sprite = PlaySprite;
            isPlaying = false;
        }
    }

    public void ReplayVideo()
    {
        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    public void BackArrow()
    {
        if (currentPanelIndex > 0)
        {
            ShowPanel(currentPanelIndex - 1);
        }
    }

    public void ForwardArrow()
    {
        if (currentPanelIndex < panels.Length - 1)
        {
            ShowPanel(currentPanelIndex + 1);
        }
    }
}
