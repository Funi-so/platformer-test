using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogController : MonoBehaviour
{
    public static DialogController controller;
    [SerializeField] GameObject dialogPanel;
    [SerializeField] GameObject choicesPanel;
    [SerializeField] Text dialogText;
    [SerializeField] Text[] optionTexts;
    //[SerializeField] Image NPCSprite;
    //[SerializeField] Sprite playerSprite;
    public GameObject myExclamation;
    public NPCSO currentNPC;
    public bool isTalking;
    public int dialogIndex;



    void Awake()
    {
        if (controller == null) controller = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        dialogPanel.SetActive(false);
    }
    public void StartDialog(NPCSO theNPC)
    {
        currentNPC = theNPC;
        isTalking = true;
        dialogPanel.SetActive(true);
        dialogIndex = 0;
        Dialog();
    }

    public void Dialog()
    {
        if (dialogIndex >= currentNPC.dialog.Length) FinishDialog();
        else
        {
            HideChoices();
            dialogText.text = currentNPC.dialog[dialogIndex].message;
            if (currentNPC.dialog[dialogIndex].speaker == Speaker.NPC)
            {
                choicesPanel.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                //NPCSprite.sprite = currentNPC.NPCSprite;
            }
            else if (currentNPC.dialog[dialogIndex].speaker == Speaker.Player)
            {
                //NPCSprite.sprite = playerSprite;
                if (currentNPC.dialog[dialogIndex].answers.Length > 0)
                {
                    choicesPanel.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    for (int i = 0; i < currentNPC.dialog[dialogIndex].answers.Length; i++)
                    {
                        if (i > 2) break;
                        choicesPanel.transform.GetChild(i).gameObject.SetActive(true);
                        /*choicesPanel.transform.GetChild(i).GetChild(0).GetComponent<Text>().text*/
                        optionTexts[i].text = currentNPC.dialog[dialogIndex].answers[i];
                    }
                }
            }
        }
    }

    public void HideChoices()
    {
        for (int i = 0; i < choicesPanel.transform.childCount; i++)
        {
            choicesPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SkipDialog()
    {
        bool canSkip = true;
        int skips = 1;
        for (int i = 1; i < 4; i++)
        {
            if (!canSkip || dialogIndex-i<=0) break;
            if (currentNPC.dialog[dialogIndex-i].answers.Length > 0)
            {
                canSkip = false;
                switch (i)
                {
                    case 1:
                        skips = currentNPC.dialog[dialogIndex - i].answers.Length;
                        break;
                    case 2:
                        if (currentNPC.dialog[dialogIndex - i].answers.Length == 3)
                            skips = 2;
                        else skips = 1;
                        break;
                    default:
                        break;
                }
            }
        }

        dialogIndex += skips;
        Dialog();
    }
    public void FinishDialog()
    {
        isTalking = false;
        dialogText.text = "";
        dialogPanel.SetActive(false);
        currentNPC = null;
    }

}
