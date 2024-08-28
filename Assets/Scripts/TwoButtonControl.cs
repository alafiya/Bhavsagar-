using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class TwoButtonControl : MonoBehaviour
{
    public CanvasGroup VideoPanel1;
    public CanvasGroup VideoPanel2;
    public CanvasGroup VideoPanel3;
    public CanvasGroup QPanel;
    public Button ForwardButton;
    public Button BackButton;

    public bool isCorrectA;
    public bool isCorrectB;

    public Image PPImage1;
    public Image PPImage2;
    public Image PPImage3;
    public Sprite PlaySprite;
    public Sprite PauseSprite;

    public VideoPlayer videoPlayer3;
    public VideoPlayer videoPlayer1;
    public VideoPlayer videoPlayer2;

    private bool isPlaying1;
    private bool isPlaying2;
    private bool isPlaying3;

    public bool isVideoPanel3Active = false;
    private int currentPanelIndex = 0;
    private CanvasGroup[] panels;

    public AudioSource rightAudio;
    public AudioSource wrongAudio;

    public Button backToGame;
    public Animator BackToGameAnimator;
    public Animator ForwardButtonAnimator;

    private Button[] buttons;
    private bool isAnswerClicked = false; // Flag to check if an answer button has been clicked

    private bool isMouseActive; //mouseactive
    public GameObject[] toHide; //hiddenobjects

    private float mouseThreshold = 0.01f;

    void Start()
    {
        isVideoPanel3Active = false;
        backToGame.interactable = false;

        // Initialize the buttons array with the Button components from AnsA, AnsB
        buttons = new Button[2];
        buttons[0] = GameObject.Find("AnsA").GetComponent<Button>();
        buttons[1] = GameObject.Find("AnsB").GetComponent<Button>();

        // Initialize panels array
        panels = new CanvasGroup[] { VideoPanel1, VideoPanel2, QPanel, VideoPanel3 };

        // Show the first panel
        ShowPanel(0);

        UpdateButtonSprite();

        // Update button states
        UpdateButtonStates();
        isAnswerClicked = false;

        // Add onClick listeners to each button
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Capturing index for the listener
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void Update()
    {
        UpdateButtonSprite();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            buttons[0].onClick.Invoke(); // Simulate click for AnsA
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            buttons[1].onClick.Invoke(); // Simulate click for AnsB
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentPanelIndex == 0 || currentPanelIndex == 1 || currentPanelIndex == 3)
            {
                if (currentPanelIndex == 0)
                {
                    TogglePlayPause1();
                }
                else
                {
                    if (currentPanelIndex == 1)
                    {
                        TogglePlayPause2();
                    }
                    else
                    {
                        TogglePlayPause3();
                    }
                }
            }
        
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
                else
                {
                    if (currentPanelIndex == 3)
                    {
                        ReplayVideo3();
                    }
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
        else if (index == 3)
        {
            videoPlayer3.Play();
        }

        currentPanelIndex = index;

        // Update button states
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        // Disable Back button if on the first panel
        BackButton.interactable = currentPanelIndex > 0;

        // Check if VideoPanel3 is active and currently displayed
        if (isVideoPanel3Active && panels[currentPanelIndex] == VideoPanel3)
        {
            ForwardButton.interactable = false;
        }
        else if (currentPanelIndex == 2 && !isAnswerClicked)
        {
            ForwardButton.interactable = false; // Disable Forward button if in QPanel and no answer clicked
        }
        else
        {
            ForwardButton.interactable = currentPanelIndex < panels.Length - 1;
            
            ForwardButtonAnimator.SetBool("isInteractable", ForwardButton.interactable);
        }
    }

    void OnButtonClick(int buttonIndex)
    {
        // Save PlayerPrefs based on correctness of the clicked button
        SaveUttarChoice(buttonIndex);

        // Change color immediately based on correctness
        ChangeColor(buttonIndex);

        // Wait for a short delay before changing other buttons
        StartCoroutine(AnswerButton(buttonIndex));

        backToGame.interactable = true;
        EventSystem.current.SetSelectedGameObject(backToGame.gameObject);

        // Set the flag to true when an answer button is clicked
        isAnswerClicked = true;

        // Update button states
        UpdateButtonStates();
    }

    IEnumerator AnswerButton(int buttonIndex)
    {
        // Disable all buttons after color change
        DisableAllButtons();
        yield return new WaitForSeconds(2f);

        // Change colors of other buttons based on correctness
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != buttonIndex)
            {
                ChangeColor(i);
            }
        }
        yield return new WaitForSeconds(4f);
        ShowPanel(3);
        videoPlayer3.Play();
        isVideoPanel3Active = true;
        while (videoPlayer3.isPlaying)
        {
            yield return null; // Wait for the next frame
        }
        BackToGameAnimator.SetTrigger("AnimateButton");
    }

    public void OnForwardButtonClick()
    {
        if (currentPanelIndex == 2 && !isAnswerClicked)
        {
            return; // Do nothing if in QPanel and no answer clicked
        }

        if (isAnswerClicked && panels[currentPanelIndex] == QPanel)
        {
            ShowPanel(3); // Show VideoPanel3
        }
        else if (currentPanelIndex < panels.Length - 1)
        {
            ShowPanel(currentPanelIndex + 1);
        }
    }

    public void OnBackButtonClick()
    {
        if (isVideoPanel3Active && panels[currentPanelIndex] == VideoPanel3)
        {
            ShowPanel(2); // Show QPanel
        }
        else if (currentPanelIndex > 0)
        {
            ShowPanel(currentPanelIndex - 1);
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

    public void ReplayVideo3()
    {
        videoPlayer3.time = 0;
        videoPlayer3.Play();
    }

    public void TogglePlayPause3()
    {
        if (isPlaying3)
        {
            videoPlayer3.Pause();
        }
        else
        {
            videoPlayer3.Play();
        }

        isPlaying3 = !isPlaying3;
        UpdateButtonSprite();
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

        if (videoPlayer3.isPlaying)
        {
            PPImage3.sprite = PauseSprite;
            isPlaying3 = true;
        }
        else
        {
            PPImage3.sprite = PlaySprite;
            isPlaying3 = false;
        }
    }

    void ChangeColor(int buttonIndex)
    {
        // Determine correctness based on button index
        bool isCorrect = false;
        switch (buttonIndex)
        {
            case 0: // AnsA
                isCorrect = isCorrectA;
                break;
            case 1: // AnsB
                isCorrect = isCorrectB;
                break;
            default:
                break;
        }

        // Change color based on correctness
        if (isCorrect)
        {
            buttons[buttonIndex].image.color = Color.green;
        }
        else
        {
            buttons[buttonIndex].image.color = Color.red;
        }
    }

    void DisableAllButtons()
    {
        // Disable all buttons
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    void SaveUttarChoice(int buttonIndex)
    {
        // Determine correctness based on button index
        bool isCorrect = false;
        switch (buttonIndex)
        {
            case 0: // AnsA
                isCorrect = isCorrectA;
                break;
            case 1: // AnsB
                isCorrect = isCorrectB;
                break;
            default:
                break;
        }

        // Save PlayerPrefs based on correctness of the clicked button
        if (isCorrect)
        {
            PlayerPrefs.SetInt("UttarChosen", 1); // Correct
            rightAudio.Play();
            Debug.Log("Right Answer");
        }
        else
        {
            PlayerPrefs.SetInt("UttarChosen", 2); // Incorrect
            wrongAudio.Play();
            Debug.Log("Wrong Answer");
        }
    }
}
