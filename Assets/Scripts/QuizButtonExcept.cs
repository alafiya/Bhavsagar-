using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class QuizButtonExcept : MonoBehaviour
{
    public CanvasGroup VideoPanel1;
    public CanvasGroup VideoPanel2;

    public CanvasGroup QPanel;
    public Button ForwardButton;
    public Button BackButton;

    public Image PPImage1;
    public Image PPImage2;
    public Sprite PlaySprite;
    public Sprite PauseSprite;

    public VideoPlayer videoPlayer1; // VideoPlayer for VideoPanel1
    public VideoPlayer videoPlayer2; // VideoPlayer for VideoPanel2

    private bool isPlaying1;
    private bool isPlaying2;
    private int currentPanelIndex = 0;
    private CanvasGroup[] panels;

    public Animator forwardButtonAnimator; // Reference to the Animator component

    private bool isMouseActive; //mouseactive
    public GameObject[] toHide; //hiddenobjects
    
    private float mouseThreshold = 0.01f;
    void Start()
    {
        // Initialize panels array
        panels = new CanvasGroup[] { VideoPanel1, VideoPanel2, QPanel };

        // Show the first panel
        ShowPanel(0);

        UpdateButtonSprite();

        // Update button states
        UpdateButtonStates();
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
            OnForwardButtonClick();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnBackButtonClick();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentPanelIndex == 0)
            {
                ReplayVideo1();
            }
            else
            {
                if (currentPanelIndex == 1)
                {
                    ReplayVideo2();
                }
            }
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
        videoPlayer1.Pause();
        videoPlayer2.Pause();

        // Play video for the active panel
        if (index == 0)
        {
            videoPlayer1.Play();
        }
        else if (index == 1)
        {
            videoPlayer2.Play();
        }

        currentPanelIndex = index;

        // Update button states
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        BackButton.interactable = currentPanelIndex > 0;

        //forwardButtonAnimator.SetBool("isInteractable", ForwardButton.interactable);
    }

    public void OnForwardButtonClick()
    {
        if (currentPanelIndex < panels.Length - 1)
        {
            ShowPanel(currentPanelIndex + 1);
        }
    }

    public void OnBackButtonClick()
    {
        if (currentPanelIndex > 0)
        {
            ShowPanel(currentPanelIndex - 1);
        }
    }

    private void TogglePlayPause()
    {
        if (currentPanelIndex == 0) // If VideoPanel1 is active
        {
            TogglePlayPause1();
        }
        else if (currentPanelIndex == 1) // If VideoPanel2 is active
        {
            TogglePlayPause2();
        }
    }

    public void TogglePlayPause1()
    {
        if (isPlaying1)
        {
            videoPlayer1.Pause();
        }
        else
        {
            videoPlayer1.Play();
        }

        isPlaying1 = !isPlaying1;
        UpdateButtonSprite();
    }

    public void ReplayVideo1()
    {
        videoPlayer1.time = 0;
        videoPlayer1.Play();
    }

    public void TogglePlayPause2()
    {
        if (isPlaying2)
        {
            videoPlayer2.Pause();
        }
        else
        {
            videoPlayer2.Play();
        }

        isPlaying2 = !isPlaying2;
        UpdateButtonSprite();
    }

    public void ReplayVideo2()
    {
        videoPlayer2.time = 0;
        videoPlayer2.Play();
    }

    private void UpdateButtonSprite()
    {
        if (videoPlayer1.isPlaying)
        {
            PPImage1.sprite = PauseSprite;
            isPlaying1 = true;
        }
        else
        {
            PPImage1.sprite = PlaySprite;
            isPlaying1 = false;
        }

        if (videoPlayer2.isPlaying)
        {
            PPImage2.sprite = PauseSprite;
            isPlaying2 = true;
        }
        else
        {
            PPImage2.sprite = PlaySprite;
            isPlaying2 = false;
        }
    }
}
