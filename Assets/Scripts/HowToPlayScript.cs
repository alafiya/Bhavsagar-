using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayScript : MonoBehaviour
{
    public GameObject HowToPlay;
    public GameObject[] toHide;
    private bool isMouseActive; //mouseactive
    private float mouseThreshold = 0.01f;
    void Start()
    {
        HowToPlay.SetActive(false);
    }

    public void Help()
    {
        HowToPlay.SetActive(true);
    }

    public void CloseHelp()
    {
        HowToPlay.SetActive(false);
    }

    void Update()
    {
        // Check if the Q key is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Toggle the panel's visibility
            if (HowToPlay.activeSelf)
            {
                CloseHelp(); // Close the panel if it's open
            }
            else
            {
                Help(); // Open the panel if it's closed
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

}