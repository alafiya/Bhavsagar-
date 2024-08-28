using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Added for Image component
using TMPro;

public class QuestionManager : MonoBehaviour
{
    public List<QuestionSet> questionSets;

    public Image questionImage; 
    public Image answerAImage;
    public Image answerBImage;
    public Image answerCImage;
    public Image answerDImage;
    public Image hintImage; // New hint image field
    public UttarButtonControl uttarButtonControl;

    private HashSet<int> usedIndices = new HashSet<int>();

    void Start()
    {
        // Load used indices from PlayerPrefs
        LoadUsedIndices();

        // Randomly select a question set that hasn't been used
        int selectedIndex = GetRandomUnusedIndex();

        if (selectedIndex != -1)
        {
            DisplayQuestionSet(selectedIndex);
            usedIndices.Add(selectedIndex);
            SaveUsedIndices();
        }
        else
        {
            Debug.Log("All question sets have been used.");
            PlayerPrefs.SetInt("FBQuestionsOver", 1);
        }
    }

    private void LoadUsedIndices()
    {
        string usedIndicesString = PlayerPrefs.GetString("UsedIndices", "");
        if (!string.IsNullOrEmpty(usedIndicesString))
        {
            string[] indices = usedIndicesString.Split(',');
            foreach (string index in indices)
            {
                if (int.TryParse(index, out int result))
                {
                    usedIndices.Add(result);
                }
            }
        }
    }

    private void SaveUsedIndices()
    {
        List<string> indices = new List<string>();
        foreach (int index in usedIndices)
        {
            indices.Add(index.ToString());
        }
        PlayerPrefs.SetString("UsedIndices", string.Join(",", indices));
    }

    private int GetRandomUnusedIndex()
    {
        List<int> unusedIndices = new List<int>();
        for (int i = 0; i < questionSets.Count; i++)
        {
            if (!usedIndices.Contains(i))
            {
                unusedIndices.Add(i);
            }
        }

        if (unusedIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, unusedIndices.Count);
            return unusedIndices[randomIndex];
        }
        else
        {
            return -1; // All questions have been used
        }
    }

    private void DisplayQuestionSet(int index)
    {
        QuestionSet selectedSet = questionSets[index];
        // Assigning the question image sprite
        questionImage.sprite = selectedSet.questionSprite;
        answerAImage.sprite = selectedSet.answerSprites[0];
        answerBImage.sprite = selectedSet.answerSprites[1];
        answerCImage.sprite = selectedSet.answerSprites[2];
        answerDImage.sprite = selectedSet.answerSprites[3];

        Debug.Log("Selected Correct Answer Index: " + selectedSet.correctAnswerIndex);
       
        // Display hint sprite only for question set 3
        if (index == 3) // Assuming question set 3 is at index 2
        {
            hintImage.sprite = selectedSet.hintSprite; // Assuming QuestionSet has a hintSprite field
            hintImage.gameObject.SetActive(true);
        }
        else
        {
            hintImage.gameObject.SetActive(false);
        }
        // Call the correct method on UttarButtonControl
        switch (selectedSet.correctAnswerIndex)
        {
            case 0:
                uttarButtonControl.CorrectAnsA();
                Debug.Log("CorrectAnsA() called");
                break;
            case 1:
                uttarButtonControl.CorrectAnsB();
                Debug.Log("CorrectAnsB() called");
                break;
            case 2:
                uttarButtonControl.CorrectAnsC();
                Debug.Log("CorrectAnsC() called");
                break;
            case 3:
                uttarButtonControl.CorrectAnsD();
                Debug.Log("CorrectAnsD() called");
                break;
            default:
                Debug.LogWarning("Invalid correct answer index");
                break;
        }
    }
}