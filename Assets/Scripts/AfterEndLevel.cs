using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AfterEndLevel : MonoBehaviour
{

    public TextMesh stepsValueTextMesh;
    public SpriteRenderer endSceneFader;
    public SpriteRenderer returnToMenuButton;

    private bool isAuthorizedToReturnToMenu = false;

    // Use this for initialization
    void Start()
    {
        stepsValueTextMesh.text = stepsValueTextMesh.text.Replace("@", GameManager.instance.playerTotalMoney.ToString());

        endSceneFader.DOFade(0, 3).OnComplete(FaderFadeComplete);
    }

    void FaderFadeComplete()
    {
        StartCoroutine(EnableReturnButtonCoroutine());
    }

    IEnumerator EnableReturnButtonCoroutine()
    {
        yield return new WaitForSeconds(2);
        returnToMenuButton.DOFade(1, 1);
        isAuthorizedToReturnToMenu = true;
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isAuthorizedToReturnToMenu)
                GameManager.instance.GoToScene(1);
        }
    }
}
