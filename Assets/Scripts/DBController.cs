using System.Collections;
using UnityEngine;

public class DBController : MonoBehaviour
{
    public Sprite[] Team_Dialogue;
    public Sprite moti_dialogue;
    public Sprite bhawar_dialogue;
    public Sprite rightUttar_dialogue;
    public Sprite wrongUttar_dialogue;
    private SpriteRenderer spriteRenderer;
    public float dialogDisplayTime = 6f;
    private bool isTeamDialogueActive = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer component is not found on this GameObject.");
    }

    public void ChangeTeamDialogue(int teamIndex)
    {
        if (teamIndex >= 0 && teamIndex < Team_Dialogue.Length)
        {
            isTeamDialogueActive = true;
            spriteRenderer.sprite = Team_Dialogue[teamIndex];
            StartCoroutine(DisplayTeamDialog());
        }
        else
        {
            Debug.LogError("Invalid team index.");
        }
    }

    private IEnumerator DisplayTeamDialog()
    {
        yield return new WaitForSeconds(dialogDisplayTime);

        if (!isTeamDialogueActive)
        {
            spriteRenderer.sprite = null;
        }
    }

    public void ClearTeamDialog()
    {
        isTeamDialogueActive = false;
        spriteRenderer.sprite = null;
    }

    public void ChangeToMotiDialogue()
    {
        StartCoroutine(DisplayDialogForTime(moti_dialogue));
    }

    public void ChangeToBhawarDialogue()
    {
        StartCoroutine(DisplayDialogForTime(bhawar_dialogue));
    }

    public void ChangeToRightDialogue()
    {
        StartCoroutine(DisplayDialogForTime(rightUttar_dialogue));
    }

    public void ChangeToWrongDialogue()
    {
        StartCoroutine(DisplayDialogForTime(wrongUttar_dialogue));
    }

    private IEnumerator DisplayDialogForTime(Sprite dialogSprite)
    {

        yield return new WaitForSeconds(1f);
        spriteRenderer.sprite = dialogSprite;
        yield return new WaitForSeconds(dialogDisplayTime);
        spriteRenderer.sprite = null;
    }
}
