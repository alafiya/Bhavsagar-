using UnityEngine.Video;
using UnityEngine;

[System.Serializable]
public class QuestionSet
{
    public Sprite questionSprite; 
    
    public Sprite[] answerSprites;
    public int correctAnswerIndex;
    public Sprite hintSprite; // New hint sprite field
}