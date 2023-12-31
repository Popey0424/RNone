using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainGame : MonoBehaviour
{

    public Canvas MainCanvas;
    public TMP_Text TextDialog;
    public TMP_Text TextCharacterName;
    public Image ImageCharacter;
    public DialogSequence[] Dialogs;
    public GameObject spriteBackground;
    public Image ImageCharacter2;
    public Button choiceButton1;
    

    public Button choiceButton2;

    public Button ButtonContinuer;


    public string[] lines;
    public float textSpeed;
    private bool isTextFinish = false;
    [SerializeField] private Image ImageFade2;
    [SerializeField] private Image ImageFade4;
    private int index;
    private int dialog = 0;
    [SerializeField] private int dialogueTriggerNumber;
    [SerializeField] private int dialogueTriggerNumberVictory;
    int _sequenceNumber;

    #region BuildIn
    void Start()
    {
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        ButtonContinuer.gameObject.SetActive(false);

        choiceButton1.onClick.AddListener(delegate { ClickOnChoiceButton(1); });
        choiceButton2.onClick.AddListener(delegate { ClickOnChoiceButton(2); });

        _sequenceNumber = 0;
        UpdateDialogSequence(Dialogs[_sequenceNumber]);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {


            StopAllCoroutines();
            TextDialog.text = Dialogs[_sequenceNumber].TextDialog;
            isTextFinish = true;
        }

        if (!isTextFinish)
        {
            ButtonContinuer.gameObject.SetActive(false);
        }

        if (isTextFinish)
        {
            ShowButtonsToContinue(Dialogs[_sequenceNumber]);
        }

        if (_sequenceNumber == dialogueTriggerNumber)
        {
            ImageFade2.DOFade(1, 2.9f).OnComplete(FadeComplete);
            void FadeComplete()
            {
                SceneManager.LoadScene("DeathScene");
            }
        }
        else if (_sequenceNumber == dialogueTriggerNumberVictory)
        {
            ImageFade4.DOFade(1, 2.9f).OnComplete(FadeComplete);
            void FadeComplete()
            {
                SceneManager.LoadScene("VictoryScene");
            }
        }
    }
    #endregion


    #region MyMethods

    public void ClickOnChoiceButton(int number)
    {
        dialog = number;
        switch (dialog)
        {
            case 1:
                _sequenceNumber = Dialogs[_sequenceNumber].IDNextDialogChoice1;
                UpdateDialogSequence(Dialogs[_sequenceNumber]);
                break;
            case 2:

                _sequenceNumber = Dialogs[_sequenceNumber].IDNextDialogChoice2;
                UpdateDialogSequence(Dialogs[_sequenceNumber]);
                break;


        }

        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
    }

    IEnumerator TypeLine(DialogSequence sequence)
    {
        isTextFinish = false;
        TextDialog.text = string.Empty;
        foreach (char c in sequence.TextDialog.ToCharArray())
        {
            TextDialog.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTextFinish = true;

    }

    private void ShowButtonsToContinue(DialogSequence sequence)
    {
        if (sequence.HasChoice)
        {
            ShowChoices(sequence);
        }
        else
        {
            ButtonContinuer.gameObject.SetActive(true);
        }
    }

    void UpdateDialogSequence(DialogSequence sequence)
    {
        //if (sequence.HasChoice)
        //{
        //    ShowChoices(sequence);

        //}
        StartCoroutine(TypeLine(sequence));

        TextCharacterName.text = sequence.TextNameCharacter;

        if  (spriteBackground != null )
        {
            GameObject.Destroy(spriteBackground);
        }

        spriteBackground = GameObject.Instantiate( sequence.SpriteBackground, MainCanvas.transform , false);
        spriteBackground.transform.SetAsFirstSibling();

        UpdateCharacter(ImageCharacter, sequence.SpriteCharacter);
        UpdateCharacter(ImageCharacter2, sequence.SpriteCharacter2);



    }

    private void UpdateCharacter(Image charImage, Sprite newSprite)
    {
        if (newSprite == null)
        {
            charImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            charImage.color = new Color(1, 1, 1, 1);
            if (charImage.sprite != newSprite)
            {
                charImage.sprite = newSprite;
            }
        }
    }

    public void OnClickNextDialog()
    {
        if (_sequenceNumber == 0)
        {
            _sequenceNumber++;
        }
        else
        {
            _sequenceNumber = Dialogs[_sequenceNumber].IDNextDialogNext;
        }
        if (_sequenceNumber >= Dialogs.Length)
        {
            ButtonContinuer.gameObject.SetActive(false);
        }

        if (_sequenceNumber < Dialogs.Length)
        {
            UpdateDialogSequence(Dialogs[_sequenceNumber]);
        }

        else
        {

            dialog++;
        }

        FindObjectOfType<AudioManager>().Play("NextDialog");

    }

    private void ShowChoices(DialogSequence sequence)
    {
        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);
        choiceButton1.GetComponentInChildren<TMP_Text>().text = sequence.TextChoice1;
        choiceButton2.GetComponentInChildren<TMP_Text>().text = sequence.TextChoice2;
    }
    #endregion
}

