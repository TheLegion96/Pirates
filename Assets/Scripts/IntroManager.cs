using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {

    public GameObject fader;
    public GameObject logo;
    public GameObject title;
    public GameObject subtitle;

    // Use this for initialization
    void Start () {
        StartCoroutine(StartMethod_Coroutine());
    }

    IEnumerator StartMethod_Coroutine()
    {
        yield return new WaitForSeconds(2);

        fader.transform.DOScale(0, 2);
        logo.GetComponent<SpriteRenderer>().DOFade(0, 1);
        title.transform.DOScale(0.5f, 2);
        title.transform.DOMoveY(3.5f, 2);
        title.transform.DOMoveX(1.5f, 2);
        subtitle.transform.DOScale(0.5f, 2);
        subtitle.transform.DOMoveY(3.5f, 2);
        subtitle.transform.DOMoveX(1.5f, 2);

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("MainMenu_simone", LoadSceneMode.Single);

        yield return null;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
