using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuPause : MonoBehaviour
{

    public Transform[] pause;
    public Player player;
    public float bookSpeed;
    //private bool isPaused = false;
    private int pauseIndex;
    //public GameObject obj;
    private MeshRenderer bookClosedMenuSubtitleMeshRenderer;

    [Header("MenuVoices")]
    public GameObject[] MenuVoices;
    public GameObject MenuPointer;
    private int menuIndex;

    [Header("MenuSounds")]
    public AudioClip switchSelection;
    public AudioClip confirmSelection;

    private SpriteRenderer bestiarioSpriteRenderer;
    [SerializeField] SpriteRenderer[] Bestie;
    [SerializeField] TextMesh[] TestiBestie;
    [SerializeField] SpriteRenderer[] TastiniBelli;
    [SerializeField] TextMesh Indietro;

    // Use this for initialization
    void Start()
    {
        transform.position = pause[0].position;
        pauseIndex = 0;

        bookClosedMenuSubtitleMeshRenderer = GameObject.Find("BookClosedMenuSubtitle").GetComponent<MeshRenderer>();
        bestiarioSpriteRenderer = GameObject.Find("Bestiario").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Open/Close Pause menu.
        if (GameManager.instance.state == GameManager.State.Play || GameManager.instance.state == GameManager.State.Pause)
        {
            if (Input.GetKeyDown(KeyCode.P) && player.isStillAlive)
            {
                if (transform.position == pause[pauseIndex].position)
                {
                    OpenMenu();
                }
            }
            if (Input.GetKeyDown(KeyCode.P) && player.isStillAlive)
            {
                if (pauseIndex >= pause.Length)
                {
                    CloseMenu();
                }
            }
        }

        //Move into Pause menu.
        if (GameManager.instance.state == GameManager.State.Pause && (
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.DownArrow)
            ))
        {
            menuIndex = Mathf.Clamp(menuIndex - (int)Input.GetAxisRaw("Vertical"), 0, MenuVoices.Length - 1);
            SetMenuVoice(menuIndex);
        }

        if (transform.position != pause[pauseIndex].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, pause[pauseIndex].position, bookSpeed * Time.deltaTime); // Move the object to Arrays
        }
        if (transform.position == pause[0].position)
        {
            bookClosedMenuSubtitleMeshRenderer.enabled = true;
            if (menuIndex != 0)
            {
                menuIndex = 0;
                SetMenuVoice(menuIndex);
            }
        }
        else
        {
            bookClosedMenuSubtitleMeshRenderer.enabled = false;
        }


        if (GameManager.instance.state == GameManager.State.Bestiario && Input.GetKeyDown(KeyCode.Backspace))
        {
            SoundManager.instance.PlaySingle(confirmSelection);
            CloseBestiario();
        }
        //Confirm selection into Pause menu.
        else if (GameManager.instance.state == GameManager.State.Pause && Input.GetKeyDown(KeyCode.Return))
        {
            SoundManager.instance.PlaySingle(confirmSelection);
            switch (MenuVoices[menuIndex].name)
            {
                case "MenuVoiceBestiario":
                    OpenBestiario();
                    break;
                case "MenuVoiceRiprendi":
                    CloseMenu();
                    break;
                case "MenuVoiceRicominciaStanza":
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                    break;
                case "MenuVoiceRicominciaLivello":
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                    break;
                case "MenuVoiceTornaAlMainMenu":
                    SceneManager.LoadScene("MainMenu_simone", LoadSceneMode.Single);
                    break;
            }
        }

        if (GameManager.instance.state == GameManager.State.Bestiario /*&& (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.RightArrow))*/)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                NavigateBestiario(KeyCode.LeftArrow);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NavigateBestiario(KeyCode.RightArrow);
            }
        }

        //Move book from A to B.
        if (transform.position != pause[pauseIndex].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, pause[pauseIndex].position, bookSpeed * Time.deltaTime); // Move the object to Arrays
        }
        if (transform.position == pause[0].position)
        {
            bookClosedMenuSubtitleMeshRenderer.enabled = true;
            if (menuIndex != 0)
            {
                menuIndex = 0;
                SetMenuVoice(menuIndex);
            }
        }
        else
        {
            bookClosedMenuSubtitleMeshRenderer.enabled = false;
        }

        /*if (Input.GetKeyDown(KeyCode.P) && isPaused)
        {
                transform.position = new Vector3(8f, 10f, 0);
                isPaused = false;
                Debug.Log(isPaused);
        }
        
        if (Input.GetKeyDown(KeyCode.P) && !isPaused)
        {
            transform.position = new Vector3(8f, -10f, 0);
            isPaused = true;
            Debug.Log(isPaused);
        } /*else if(Input.GetKeyDown(KeyCode.P) && isPaused) {
            transform.position = new Vector3(0f, 10f, 0);
            isPaused = false;*/

    }
    List<GameObject> Child = new List<GameObject>();

    private void OpenBestiario()
    {
        if (GameManager.instance != null) GameManager.instance.state = GameManager.State.Bestiario;


        foreach (Transform child in transform)
        {
            Child.Add(child.gameObject);
        }
        bestiarioSpriteRenderer.DOFade(1, 1);
        for (int i = 0; i < Child.Count; i++)
        {
            if (Child[i].name == "Bestiario")
            {
                Child[i].SetActive(true);
                bestiarioSpriteRenderer.DOFade(0, 1);
                NavigateBestiario();
            }
            else
            {
                Child[i].SetActive(false);
            }
        }
        for (int i = 0; i < TastiniBelli.Length; i++)
        {
            TastiniBelli[i].enabled = true;
        }
        Indietro.gameObject.SetActive(true);
    }
    private void CloseBestiario()
    {
        bestiarioSpriteRenderer.DOFade(1, 1);
        for (int i = 0; i < Child.Count; i++)
        {
            if (Child[i].name == "Bestiario")
            {
                Child[i].SetActive(false);
            }
            else
            {
                Child[i].SetActive(true);
                bestiarioSpriteRenderer.DOFade(0, 1);
            }
        }
        for (int i = 0; i < TastiniBelli.Length; i++)
        {
            TastiniBelli[i].enabled = false;
        }
        Indietro.gameObject.SetActive(false);
        bestiarioSpriteRenderer.DOFade(0, 1);
        if (GameManager.instance != null) GameManager.instance.state = GameManager.State.Pause;
    }

    private void OpenMenu()
    {
        //bookClosedMenuSubtitleMeshRenderer.enabled = false;
        pauseIndex++;
        GetComponent<AudioSource>().Play();
        if (GameManager.instance != null) GameManager.instance.state = GameManager.State.Pause;
    }

    private void CloseMenu()
    {
        //bookClosedMenuSubtitleMeshRenderer.enabled = true;
        pauseIndex = 0;
        if (GameManager.instance != null) GameManager.instance.state = GameManager.State.Play;
    }

    private void SetMenuVoice(int menuIndex)
    {
        GameObject tempItem;
        Vector3 defaultLocalScale = new Vector3(1, 1, 1);
        Vector3 selectedLocalScale = new Vector3(1.1f, 1.1f, 1);

        for (int i = 0; i < MenuVoices.Length; i++)
        {
            tempItem = MenuVoices[i];

            if (i == menuIndex)
            {
                tempItem.transform.localScale = selectedLocalScale;
            }
            else if (tempItem.transform.localScale == selectedLocalScale)
            {
                tempItem.transform.localScale = defaultLocalScale;
            }
        }

        MenuPointer.transform.position = new Vector3(MenuPointer.transform.position.x, MenuVoices[menuIndex].transform.position.y, MenuPointer.transform.position.z);
        SoundManager.instance.PlaySingle(switchSelection);
    }
    private int index;
    private void NavigateBestiario()
    {
        switch (index)
        {
            case 0:
                Bestie[0].enabled = true;
                TestiBestie[0].gameObject.SetActive(true);


                Bestie[1].enabled = false;
                TestiBestie[1].gameObject.SetActive(false);


                Bestie[2].enabled = false;
                TestiBestie[2].gameObject.SetActive(false);

                Bestie[3].enabled = false;
                TestiBestie[3].gameObject.SetActive(false);

                break;
            case 1:
                Bestie[0].enabled = false;
                TestiBestie[0].gameObject.SetActive(false);


                Bestie[1].enabled = true;
                TestiBestie[1].gameObject.SetActive(true);


                Bestie[2].enabled = false;
                TestiBestie[2].gameObject.SetActive(false);

                Bestie[3].enabled = false;
                TestiBestie[3].gameObject.SetActive(false);
                break;
            case 2:
                Bestie[0].enabled = false;
                TestiBestie[0].gameObject.SetActive(false);


                Bestie[1].enabled = false;
                TestiBestie[1].gameObject.SetActive(false);


                Bestie[2].enabled = true;
                TestiBestie[2].gameObject.SetActive(true);

                Bestie[3].enabled = false;
                TestiBestie[3].gameObject.SetActive(false);
                break;
            case 3:
                Bestie[0].enabled = false;
                TestiBestie[0].gameObject.SetActive(false);


                Bestie[1].enabled = false;
                TestiBestie[1].gameObject.SetActive(false);


                Bestie[2].enabled = false;
                TestiBestie[2].gameObject.SetActive(false);

                Bestie[3].enabled = true;
                TestiBestie[3].gameObject.SetActive(true);
                break;
        }
    }

    private void NavigateBestiario(KeyCode In)
    {
        if (In == KeyCode.RightArrow)
        {
            index++;
            if (index > 4)
            { index = 0; }
        }
        if (In == KeyCode.LeftArrow)
        {
            index--;
            if (index < 0)
            { index = 3; }

        }

        switch (index)
        {
            case 0:
                Bestie[0].enabled = true;
                TestiBestie[0].gameObject.SetActive(true);


                Bestie[1].enabled = false;
                TestiBestie[1].gameObject.SetActive(false);


                Bestie[2].enabled = false;
                TestiBestie[2].gameObject.SetActive(false);

                Bestie[3].enabled = false;
                TestiBestie[3].gameObject.SetActive(false);

                break;
            case 1:
                Bestie[0].enabled = false;
                TestiBestie[0].gameObject.SetActive(false);


                Bestie[1].enabled = true;
                TestiBestie[1].gameObject.SetActive(true);


                Bestie[2].enabled = false;
                TestiBestie[2].gameObject.SetActive(false);

                Bestie[3].enabled = false;
                TestiBestie[3].gameObject.SetActive(false);
                break;
            case 2:
                Bestie[0].enabled = false;
                TestiBestie[0].gameObject.SetActive(false);


                Bestie[1].enabled = false;
                TestiBestie[1].gameObject.SetActive(false);


                Bestie[2].enabled = true;
                TestiBestie[2].gameObject.SetActive(true);

                Bestie[3].enabled = false;
                TestiBestie[3].gameObject.SetActive(false);
                break;
            case 3:
                Bestie[0].enabled = false;
                TestiBestie[0].gameObject.SetActive(false);


                Bestie[1].enabled = false;
                TestiBestie[1].gameObject.SetActive(false);


                Bestie[2].enabled = false;
                TestiBestie[2].gameObject.SetActive(false);

                Bestie[3].enabled = true;
                TestiBestie[3].gameObject.SetActive(true);
                break;
        }
    }
}


