using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomWordPicker : MonoBehaviour
{
    public Image wordImage;
    public Sprite[] sprites;
    private int previousIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (sprites.Length > 0)
        {
            SelectRandomWord();
        }

    }

    public void SelectRandomWord()
    {
        if (sprites.Length == 0)
        {
            Debug.Log("All words seen");
            PlayerPrefs.SetInt("SSWordsOver", 1);
            return;
        }


        int newIndex;
        do
        {
            newIndex = Random.Range(0, sprites.Length);
        } while (newIndex == previousIndex);

        wordImage.sprite = sprites[newIndex];
        previousIndex = newIndex;
    }

}
