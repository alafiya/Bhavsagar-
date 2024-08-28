using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class QuizButtonSpecial : MonoBehaviour
{
    public CanvasGroup VideoPanel1;
    public CanvasGroup VideoPanel2;
    public CanvasGroup VideoPanel3;
    public CanvasGroup QPanel;
    public Button ForwardButton;
    public Button BackButton;
    public Button[] AnswerButtons; // Array of answer buttons
    
    public Image PPImage1;
    public Image PPImage2;
    public Sprite PlaySprite;
    public Sprite PauseSprite;


    public VideoPlayer videoPlayer1; 
    public VideoPlayer videoPlayer2; 
    public VideoPlayer videoPlayer3;

    private int currentPanelIndex = 0;
    private CanvasGroup[] panels;
    private bool isAnswerButtonClicked = false;

    void Start()
    {
        
            panels = new CanvasGroup[] { VideoPanel1, VideoPanel2, QPanel};
        

        // Show the first panel
        ShowPanel(0);

        
        SetPanelVisible(VideoPanel2, false);
        
        UpdateButtonStates();
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

        
        if (index != 2 || isAnswerButtonClicked)
        {
            panels[index].alpha = 1;
            panels[index].interactable = true;
            panels[index].blocksRaycasts = true;

            // Pause all videos
            videoPlayer1.Pause();
            videoPlayer2.Pause();
            videoPlayer3.Pause();

            // Play video for the active panel
            if (index == 0)
            {
                videoPlayer1.Play();
            }
            else if (index == 1)
            {
                videoPlayer2.Play();
            }
            else if (index == 2)
            {
                videoPlayer2.Play();
            }
            else if (index == 3)
            {
                videoPlayer3.Play();
            }
        }

        currentPanelIndex = index;

        // Update button states
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        // Disable Back button if on the first panel
        BackButton.interactable = currentPanelIndex > 0;

        // Disable Forward button if on the last panel
        ForwardButton.interactable = currentPanelIndex < panels.Length - 1;

        // Disable Forward button if on QPanel (in non-special case) or VideoPanel2 (in special case)
        if (currentPanelIndex == 2 || currentPanelIndex == 1)
        {
            ForwardButton.interactable = false;
        }
    }

    private void SetPanelVisible(CanvasGroup panel, bool visible)
    {
        panel.alpha = visible ? 1 : 0;
        panel.interactable = visible;
        panel.blocksRaycasts = visible;
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

    public void OnAnswerButtonClick()
    {
        
        
            
            StartCoroutine(ShowVideoPanel2WithDelay(5.0f));
        
    }

    private IEnumerator ShowVideoPanel2WithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
    }

    public void TogglePlayPause1()
    {
        if (PPImage1 != null && PlaySprite != null && PauseSprite != null)
        {
            if (videoPlayer1.isPlaying)
            {
                videoPlayer1.Pause();
                PPImage1.sprite = PlaySprite;
            }
            else
            {
                videoPlayer1.Play();
                PPImage1.sprite = PauseSprite;
            }
        }
    }

    public void ReplayVideo1()
    {
        videoPlayer1.time = 0;
        videoPlayer1.Play();
    }

    public void TogglePlayPause2()
    {
        if (PPImage2 != null && PlaySprite != null && PauseSprite != null)
        {
            if (videoPlayer2.isPlaying)
            {
                videoPlayer2.Pause();
                PPImage2.sprite = PlaySprite;
            }
            else
            {
                videoPlayer2.Play();
                PPImage2.sprite = PauseSprite;
            }
        }
    }

    public void ReplayVideo2()
    {
        videoPlayer2.time = 0;
        videoPlayer2.Play();
    }


}