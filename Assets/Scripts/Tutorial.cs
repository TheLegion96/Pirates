using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Tutorial : MonoBehaviour
{
    public TextMesh tutorialTextComponent;
    public SpriteRenderer tutorialSkipButtonComponent;
    [Tooltip("Usare il carattere uguale \"=\" per andare a capo.")]
    public string[] tutorialTexts;

    public event Action ClosedTutorialCallback;

    private int tutorialTextsIndex = 0;
    private bool isWriting = false;
    private Animator an;

    // Use this for initialization
    void Start()
    {
        an = GetComponent<Animator>();
        
        StartCoroutine(StartTextAfterAnimation());
    }

    private IEnumerator StartTextAfterAnimation()
    {
        yield return new WaitForSeconds(1);
        tutorialSkipButtonComponent.DOFade(1, 1);
        StartCoroutine(AnimateText());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isWriting)
            {
                tutorialTextsIndex++;
                if (tutorialTextsIndex >= tutorialTexts.Length)
                {
                    CloseTutorial();
                }
                else
                { 
                    StartCoroutine(AnimateText());
                }
            }
            else
            {
                StopAllCoroutines();
                tutorialTextComponent.text = tutorialTexts[tutorialTextsIndex].Replace("=", "\n");
                isWriting = false;
            }
        }
    }

    IEnumerator AnimateText()
    {
        isWriting = true;
        for (int i = 0; i < (tutorialTexts[tutorialTextsIndex].Length + 1); i++)
        {
            tutorialTextComponent.text = tutorialTexts[tutorialTextsIndex].Substring(0, i).Replace("=", "\n");
            yield return new WaitForSeconds(.03f);
        }

        isWriting = false;
        yield return null;
    }

    public void CloseTutorial()
    {
        tutorialTextComponent.gameObject.SetActive(false);
        tutorialSkipButtonComponent.gameObject.SetActive(false);

        GetComponent<Animator>().SetTrigger("Close");

        StartCoroutine(CloseTutorialCoRoutine());
    }
    IEnumerator CloseTutorialCoRoutine()
    {
        yield return new WaitForSeconds(1.1f);

        if (ClosedTutorialCallback != null)
        {
            ClosedTutorialCallback();
        }

        this.gameObject.SetActive(false);

        yield return null;
    }
}
