using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UttarButtonControl : MonoBehaviour
{
    private bool isCorrectA;
    private bool isCorrectB;
    private bool isCorrectC;
    private bool isCorrectD;
    public AudioSource rightAudio;
    public AudioSource wrongAudio;

    public Button backToGame;
    public Animator BackToGameAnimator;

    private Button[] buttons; // Array to store Button components

    void Start()
    {
        backToGame.interactable = false;
        // Initialize the buttons array with the Button components from AnsA, AnsB, AnsC
        buttons = new Button[4];
        buttons[0] = GameObject.Find("AnsA").GetComponent<Button>();
        buttons[1] = GameObject.Find("AnsB").GetComponent<Button>();
        buttons[2] = GameObject.Find("AnsC").GetComponent<Button>();
        buttons[3] = GameObject.Find("AnsD").GetComponent<Button>();

        // Add onClick listeners to each button
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Capturing index for the listener
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void Update()
    {
        // Check for key presses 1, 2, 3, 4 and simulate button clicks
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            buttons[0].onClick.Invoke(); // Simulate click for AnsA
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            buttons[1].onClick.Invoke(); // Simulate click for AnsB
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            buttons[2].onClick.Invoke(); // Simulate click for AnsC
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            buttons[3].onClick.Invoke(); // Simulate click for AnsD
        }
    }

    public void CorrectAnsA()
    {
        isCorrectA = true;
        isCorrectB = false;
        isCorrectC = false;
        isCorrectD = false;
    }

    public void CorrectAnsB()
    {
        isCorrectA = false;
        isCorrectB = true;
        isCorrectC = false;
        isCorrectD = false;
    }

    public void CorrectAnsC()
    {
        isCorrectA = false;
        isCorrectB = false;
        isCorrectC = true;
        isCorrectD = false;
    }

    public void CorrectAnsD()
    {
        isCorrectA = false;
        isCorrectB = false;
        isCorrectC = false;
        isCorrectD = true;
        Debug.Log("CorrectAnsD called: isCorrectD = " + isCorrectD);
    }

    void OnButtonClick(int buttonIndex)
    {
        // Save PlayerPrefs based on correctness of the clicked button
        SaveUttarChoice(buttonIndex);

        // Change color immediately based on correctness
        ChangeColor(buttonIndex);

        // Wait for a short delay before changing other buttons
        StartCoroutine(ChangeColorAndDisable(buttonIndex));

        backToGame.interactable = true;
    }

    IEnumerator ChangeColorAndDisable(int buttonIndex)
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
        yield return new WaitForSeconds(3f);

        BackToGameAnimator.SetTrigger("AnimateButton");
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
            case 2: // AnsC
                isCorrect = isCorrectC;
                break;
            case 3: // AnsD
                isCorrect = isCorrectD;
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
            case 2: // AnsC
                isCorrect = isCorrectC;
                break;
            case 3: // AnsD
                isCorrect = isCorrectD;
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
