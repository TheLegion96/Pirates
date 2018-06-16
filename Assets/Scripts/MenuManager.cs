using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using Completed;

public class MenuManager : MonoBehaviour
{

    public Animator openMap;

    private Text Capitolo1, Capitolo2, Capitolo3, Capitolo4, Capitolo5,
                 Capitolo6, Capitolo7, Capitolo8, Capitolo9, Capitolo10,
                 TestoCrediti;

    public AudioClip switchSelection;
    public AudioClip confirmSelection;

    private static bool isToken_1 = false;
    private static bool isToken_2 = false;
    private static bool isToken_3 = false;
    private static bool isToken_4 = false;
    private static bool isToken_5 = false;
    private static bool isToken_6 = false;
    private static bool isToken_7 = false;
    private static bool isToken_8 = false;
    private static bool isToken_9 = false;
    private static bool isToken_10 = false;

    // Level Icons
    private SpriteRenderer lvl1Enabled, lvl2Enabled, lvl3Enabled, lvl4Enabled, lvl5Enabled,
                           lvl6Enabled, lvl7Enabled, lvl8Enabled, lvl9Enabled, lvl10Enabled,
                           buttonDown, buttonLeft, buttonRight, buttonUp, buttonReturn, buttonBackspace,
                           Grupp8;

    // Tokens Render
    private SpriteRenderer Token_1, Token_2, Token_3, Token_4, Token_5,
                           Token_6, Token_7, Token_8, Token_9, Token_10,
                           TokenGrey_1, TokenGrey_2, TokenGrey_3, TokenGrey_4, TokenGrey_5,
                           TokenGrey_6, TokenGrey_7, TokenGrey_8, TokenGrey_9, TokenGrey_10;

    private Vector3 position = new Vector3(0f, 0f, 0f);
    private Vector3 scale = new Vector3(0.56f, 0.56f, 0f);

    private float mainMenuTextsOpacity = 0;
    private bool isCreditsEnabled = false;
    private float creditsTextsOpacity = 0;

    private TextMesh[] openMapTextMeshes;

    [Header("Credits Info")]
    public TextMesh creditsText;
    public SpriteRenderer creditsImage;

    void Start()
    {
        transform.position = position;
        transform.localScale = scale;

        openMapTextMeshes = openMap.GetComponentsInChildren<TextMesh>();

        lvl1Enabled = GameObject.Find("Selezione1").GetComponent<SpriteRenderer>();
        lvl2Enabled = GameObject.Find("Selezione2").GetComponent<SpriteRenderer>();
        lvl3Enabled = GameObject.Find("Selezione3").GetComponent<SpriteRenderer>();
        lvl4Enabled = GameObject.Find("Selezione4").GetComponent<SpriteRenderer>();
        lvl5Enabled = GameObject.Find("Selezione5").GetComponent<SpriteRenderer>();
        lvl6Enabled = GameObject.Find("Selezione6").GetComponent<SpriteRenderer>();
        lvl7Enabled = GameObject.Find("Selezione7").GetComponent<SpriteRenderer>();
        lvl8Enabled = GameObject.Find("Selezione8").GetComponent<SpriteRenderer>();
        lvl9Enabled = GameObject.Find("Selezione9").GetComponent<SpriteRenderer>();
        lvl10Enabled = GameObject.Find("Selezione10").GetComponent<SpriteRenderer>();

        Token_1 = GameObject.Find("Token_1").GetComponent<SpriteRenderer>();
        Token_2 = GameObject.Find("Token_2").GetComponent<SpriteRenderer>();
        Token_3 = GameObject.Find("Token_3").GetComponent<SpriteRenderer>();
        Token_4 = GameObject.Find("Token_4").GetComponent<SpriteRenderer>();
        Token_5 = GameObject.Find("Token_5").GetComponent<SpriteRenderer>();
        Token_6 = GameObject.Find("Token_6").GetComponent<SpriteRenderer>();
        Token_7 = GameObject.Find("Token_7").GetComponent<SpriteRenderer>();
        Token_8 = GameObject.Find("Token_8").GetComponent<SpriteRenderer>();
        Token_9 = GameObject.Find("Token_9").GetComponent<SpriteRenderer>();
        Token_10 = GameObject.Find("Token_10").GetComponent<SpriteRenderer>();

        TokenGrey_1 = GameObject.Find("TokenGrey_1").GetComponent<SpriteRenderer>();
        TokenGrey_2 = GameObject.Find("TokenGrey_2").GetComponent<SpriteRenderer>();
        TokenGrey_3 = GameObject.Find("TokenGrey_3").GetComponent<SpriteRenderer>();
        TokenGrey_4 = GameObject.Find("TokenGrey_4").GetComponent<SpriteRenderer>();
        TokenGrey_5 = GameObject.Find("TokenGrey_5").GetComponent<SpriteRenderer>();
        TokenGrey_6 = GameObject.Find("TokenGrey_6").GetComponent<SpriteRenderer>();
        TokenGrey_7 = GameObject.Find("TokenGrey_7").GetComponent<SpriteRenderer>();
        TokenGrey_8 = GameObject.Find("TokenGrey_8").GetComponent<SpriteRenderer>();
        TokenGrey_9 = GameObject.Find("TokenGrey_9").GetComponent<SpriteRenderer>();
        TokenGrey_10 = GameObject.Find("TokenGrey_10").GetComponent<SpriteRenderer>();

        Capitolo1 = GameObject.Find("Capitolo1").GetComponentInChildren<Text>();
        Capitolo2 = GameObject.Find("Capitolo2").GetComponentInChildren<Text>();
        Capitolo3 = GameObject.Find("Capitolo3").GetComponentInChildren<Text>();
        Capitolo4 = GameObject.Find("Capitolo4").GetComponentInChildren<Text>();
        Capitolo5 = GameObject.Find("Capitolo5").GetComponentInChildren<Text>();
        Capitolo6 = GameObject.Find("Capitolo6").GetComponentInChildren<Text>();
        Capitolo7 = GameObject.Find("Capitolo7").GetComponentInChildren<Text>();
        Capitolo8 = GameObject.Find("Capitolo8").GetComponentInChildren<Text>();
        Capitolo9 = GameObject.Find("Capitolo9").GetComponentInChildren<Text>();
        Capitolo10 = GameObject.Find("Capitolo10").GetComponentInChildren<Text>();

        buttonDown = GameObject.Find("Down").GetComponent<SpriteRenderer>();
        buttonUp = GameObject.Find("Up").GetComponent<SpriteRenderer>();
        buttonLeft = GameObject.Find("Left").GetComponent<SpriteRenderer>();
        buttonRight = GameObject.Find("Right").GetComponent<SpriteRenderer>();
        buttonReturn = GameObject.Find("Return").GetComponent<SpriteRenderer>();
        buttonBackspace = GameObject.Find("Backspace").GetComponent<SpriteRenderer>();

        //Grupp8 = GameObject.Find("Grupp8").GetComponent<SpriteRenderer>();
        //TestoCrediti = GameObject.Find("TestoCrediti").GetComponentInChildren<Text>();
    }

    void Update()
    {
        AnimatorStateInfo animatorStateInfo = openMap.GetCurrentAnimatorStateInfo(0);

        if (!animatorStateInfo.IsName("OpenMap"))
        {
            if (
                animatorStateInfo.IsName("Map1Anim") ||
                animatorStateInfo.IsName("Map2Anim") ||
                animatorStateInfo.IsName("Map3Anim") ||
                animatorStateInfo.IsName("Map4Anim")
                )
            {
                if (mainMenuTextsOpacity < 1)
                {
                    mainMenuTextsOpacity = Mathf.Clamp(mainMenuTextsOpacity + Time.deltaTime, 0, 1);

                    foreach (TextMesh item in openMapTextMeshes)
                    {
                        item.color = new Color(item.color.r, item.color.g, item.color.b, mainMenuTextsOpacity);
                    }
                }
            }
            else if (mainMenuTextsOpacity > 0)
            {
                mainMenuTextsOpacity = 0;

                foreach (TextMesh item in openMapTextMeshes)
                {
                    item.color = new Color(item.color.r, item.color.g, item.color.b, mainMenuTextsOpacity);
                }
            }

            // Navigation
            if (!animatorStateInfo.IsName("Credits"))
            {
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    openMap.SetTrigger("Next");
                    SoundManager.instance.PlaySingle(switchSelection);

                }

                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    openMap.SetTrigger("Previous");
                    SoundManager.instance.PlaySingle(switchSelection);
                }

                if (creditsTextsOpacity > 0)
                {
                    // BLOCCO SPOSTATO SOTTO

                    //creditsTextsOpacity = 0;

                    //creditsImage.color = new Color(creditsImage.color.r, creditsImage.color.g, creditsImage.color.b, creditsTextsOpacity);
                    //creditsText.color = new Color(creditsText.color.r, creditsText.color.g, creditsText.color.b, creditsTextsOpacity);
                }
            }
            // Credits
            else
            {
                if (isCreditsEnabled && creditsTextsOpacity < 1)
                {
                    creditsTextsOpacity = Mathf.Clamp(creditsTextsOpacity + Time.deltaTime, 0, 1);
                    
                    creditsImage.color = new Color(creditsImage.color.r, creditsImage.color.g, creditsImage.color.b, creditsTextsOpacity);
                    creditsText.color = new Color(creditsText.color.r, creditsText.color.g, creditsText.color.b, creditsTextsOpacity);
                }
            }

            // Load levels
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SoundManager.instance.PlaySingle(confirmSelection);

                if (animatorStateInfo.IsName("Map1Anim")) // Start Game
                {
                    SceneManager.LoadScene("Character_Selection");
                }
                else if (animatorStateInfo.IsName("Map2Anim")) // Enter to Select Level Menu
                {
                    openMap.SetTrigger("Enter");

                    StartCoroutine(ActivateSceneSelection());
                }
                else if (animatorStateInfo.IsName("Map3Anim")) // Enter to Credits
                {
                    openMap.SetTrigger("Enter");
                    StartCoroutine(ShowCreditsInfo());
                }
                else if (animatorStateInfo.IsName("Map4Anim"))
                {
                    Application.Quit();
                }
            }

            bool checkSelectedLevelEnabled = true;

            if (animatorStateInfo.IsName("Level1") ||
                animatorStateInfo.IsName("Level2") ||
                animatorStateInfo.IsName("Level3") ||
                animatorStateInfo.IsName("Level4") ||
                animatorStateInfo.IsName("Level5") ||
                animatorStateInfo.IsName("Level6") ||
                animatorStateInfo.IsName("Level7") ||
                animatorStateInfo.IsName("Level8") ||
                animatorStateInfo.IsName("Level9") ||
                animatorStateInfo.IsName("Boss") ||
                animatorStateInfo.IsName("Credits"))
            {
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    // Force hide elements
                    lvl1Enabled.gameObject.SetActive(false);
                    lvl2Enabled.gameObject.SetActive(false);
                    lvl3Enabled.gameObject.SetActive(false);
                    lvl4Enabled.gameObject.SetActive(false);
                    lvl5Enabled.gameObject.SetActive(false);
                    lvl6Enabled.gameObject.SetActive(false);
                    lvl7Enabled.gameObject.SetActive(false);
                    lvl8Enabled.gameObject.SetActive(false);
                    lvl9Enabled.gameObject.SetActive(false);
                    lvl10Enabled.gameObject.SetActive(false);

                    Token_1.gameObject.SetActive(false);
                    Token_2.gameObject.SetActive(false);
                    Token_3.gameObject.SetActive(false);
                    Token_4.gameObject.SetActive(false);
                    Token_5.gameObject.SetActive(false);
                    Token_6.gameObject.SetActive(false);
                    Token_7.gameObject.SetActive(false);
                    Token_8.gameObject.SetActive(false);
                    Token_9.gameObject.SetActive(false);
                    Token_10.gameObject.SetActive(false);

                    TokenGrey_1.gameObject.SetActive(false);
                    TokenGrey_2.gameObject.SetActive(false);
                    TokenGrey_3.gameObject.SetActive(false);
                    TokenGrey_4.gameObject.SetActive(false);
                    TokenGrey_5.gameObject.SetActive(false);
                    TokenGrey_6.gameObject.SetActive(false);
                    TokenGrey_7.gameObject.SetActive(false);
                    TokenGrey_8.gameObject.SetActive(false);
                    TokenGrey_9.gameObject.SetActive(false);
                    TokenGrey_10.gameObject.SetActive(false);

                    Capitolo1.gameObject.SetActive(false);
                    Capitolo2.gameObject.SetActive(false);
                    Capitolo3.gameObject.SetActive(false);
                    Capitolo4.gameObject.SetActive(false);
                    Capitolo5.gameObject.SetActive(false);
                    Capitolo6.gameObject.SetActive(false);
                    Capitolo7.gameObject.SetActive(false);
                    Capitolo8.gameObject.SetActive(false);
                    Capitolo9.gameObject.SetActive(false);
                    Capitolo10.gameObject.SetActive(false);

                    // Back to main menu
                    openMap.SetTrigger("Back");

                    checkSelectedLevelEnabled = false;

                    //Grupp8.enabled = false;
                    //TestoCrediti.enabled = false;
                    isCreditsEnabled = false;
                    creditsTextsOpacity = 0;

                    creditsImage.color = new Color(creditsImage.color.r, creditsImage.color.g, creditsImage.color.b, creditsTextsOpacity);
                    creditsText.color = new Color(creditsText.color.r, creditsText.color.g, creditsText.color.b, creditsTextsOpacity);
                }
            }

            // Navigate Levels
            if (checkSelectedLevelEnabled)
            {
                if (animatorStateInfo.IsName("Level1"))
                {
                    lvl1Enabled.enabled = true;
                    Capitolo1.enabled = true;

                    if (isToken_1)
                    {
                        Token_1.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = true;
                        TokenGrey_2.enabled = false;
                        TokenGrey_3.enabled = false;
                        TokenGrey_4.enabled = false;
                        TokenGrey_5.enabled = false;
                        TokenGrey_6.enabled = false;
                        TokenGrey_7.enabled = false;
                        TokenGrey_8.enabled = false;
                        TokenGrey_9.enabled = false;
                        TokenGrey_10.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("Livello1_Stanza1");
                    }
                }
                else
                {
                    lvl1Enabled.enabled = false;
                    Capitolo1.enabled = false;

                }

                if (animatorStateInfo.IsName("Level2"))
                {
                    lvl2Enabled.enabled = true;
                    Capitolo2.enabled = true;

                    if (isToken_2)
                    {
                        Token_2.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = false;
                        TokenGrey_2.enabled = true;
                        TokenGrey_3.enabled = false;
                        TokenGrey_4.enabled = false;
                        TokenGrey_5.enabled = false;
                        TokenGrey_6.enabled = false;
                        TokenGrey_7.enabled = false;
                        TokenGrey_8.enabled = false;
                        TokenGrey_9.enabled = false;
                        TokenGrey_10.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("Livello1_Stanza2");
                    }
                }
                else
                {
                    lvl2Enabled.enabled = false;
                    Capitolo2.enabled = false;

                }

                if (animatorStateInfo.IsName("Level3"))
                {
                    lvl3Enabled.enabled = true;
                    Capitolo3.enabled = true;

                    if (isToken_3)
                    {
                        Token_3.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = false;
                        TokenGrey_2.enabled = false;
                        TokenGrey_3.enabled = true;
                        TokenGrey_4.enabled = false;
                        TokenGrey_5.enabled = false;
                        TokenGrey_6.enabled = false;
                        TokenGrey_7.enabled = false;
                        TokenGrey_8.enabled = false;
                        TokenGrey_9.enabled = false;
                        TokenGrey_10.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("Livello1_Stanza3");
                    }
                }
                else
                {
                    lvl3Enabled.enabled = false;
                    Capitolo3.enabled = false;
                }

                if (animatorStateInfo.IsName("Level4"))
                {
                    lvl4Enabled.enabled = true;
                    Capitolo4.enabled = true;

                    if (isToken_4)
                    {
                        Token_4.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = false;
                        TokenGrey_2.enabled = false;
                        TokenGrey_3.enabled = false;
                        TokenGrey_4.enabled = true;
                        TokenGrey_5.enabled = false;
                        TokenGrey_6.enabled = false;
                        TokenGrey_7.enabled = false;
                        TokenGrey_8.enabled = false;
                        TokenGrey_9.enabled = false;
                        TokenGrey_10.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("Livello2_Stanza1");
                    }
                }
                else
                {
                    lvl4Enabled.enabled = false;
                    Capitolo4.enabled = false;
                }

                if (animatorStateInfo.IsName("Level5"))
                {
                    lvl5Enabled.enabled = true;
                    Capitolo5.enabled = true;

                    if (isToken_5)
                    {
                        Token_5.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = false;
                        TokenGrey_2.enabled = false;
                        TokenGrey_3.enabled = false;
                        TokenGrey_4.enabled = false;
                        TokenGrey_5.enabled = true;
                        TokenGrey_6.enabled = false;
                        TokenGrey_7.enabled = false;
                        TokenGrey_8.enabled = false;
                        TokenGrey_9.enabled = false;
                        TokenGrey_10.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("Livello2_Stanza2");
                    }
                }
                else
                {
                    lvl5Enabled.enabled = false;
                    Capitolo5.enabled = false;
                }

                if (animatorStateInfo.IsName("Level6"))
                {
                    lvl6Enabled.enabled = true;
                    Capitolo6.enabled = true;

                    if (isToken_6)
                    {
                        Token_6.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = false;
                        TokenGrey_2.enabled = false;
                        TokenGrey_3.enabled = false;
                        TokenGrey_4.enabled = false;
                        TokenGrey_5.enabled = false;
                        TokenGrey_6.enabled = true;
                        TokenGrey_7.enabled = false;
                        TokenGrey_8.enabled = false;
                        TokenGrey_9.enabled = false;
                        TokenGrey_10.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("Livello2_Stanza3");
                    }
                }
                else
                {
                    lvl6Enabled.enabled = false;
                    Capitolo6.enabled = false;
                }

                if (animatorStateInfo.IsName("Level7"))
                {
                    lvl7Enabled.enabled = true;
                    Capitolo7.enabled = true;

                    if (isToken_7)
                    {
                        Token_7.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = false;
                        TokenGrey_2.enabled = false;
                        TokenGrey_3.enabled = false;
                        TokenGrey_4.enabled = false;
                        TokenGrey_5.enabled = false;
                        TokenGrey_6.enabled = false;
                        TokenGrey_7.enabled = true;
                        TokenGrey_8.enabled = false;
                        TokenGrey_9.enabled = false;
                        TokenGrey_10.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("Livello3_Stanza1");
                    }
                }
                else
                {
                    lvl7Enabled.enabled = false;
                    Capitolo7.enabled = false;
                }

                if (animatorStateInfo.IsName("Level8"))
                {
                    lvl8Enabled.enabled = true;
                    Capitolo8.enabled = true;

                    if (isToken_8)
                    {
                        Token_8.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = false;
                        TokenGrey_2.enabled = false;
                        TokenGrey_3.enabled = false;
                        TokenGrey_4.enabled = false;
                        TokenGrey_5.enabled = false;
                        TokenGrey_6.enabled = false;
                        TokenGrey_7.enabled = false;
                        TokenGrey_8.enabled = true;
                        TokenGrey_9.enabled = false;
                        TokenGrey_10.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("Livello3_Stanza2");
                    }
                }
                else
                {
                    lvl8Enabled.enabled = false;
                    Capitolo8.enabled = false;
                }

                if (animatorStateInfo.IsName("Level9"))
                {
                    lvl9Enabled.enabled = true;
                    Capitolo9.enabled = true;

                    if (isToken_9)
                    {
                        Token_9.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = false;
                        TokenGrey_2.enabled = false;
                        TokenGrey_3.enabled = false;
                        TokenGrey_4.enabled = false;
                        TokenGrey_5.enabled = false;
                        TokenGrey_6.enabled = false;
                        TokenGrey_7.enabled = false;
                        TokenGrey_8.enabled = false;
                        TokenGrey_9.enabled = true;
                        TokenGrey_10.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("Livello3_Stanza3");
                    }
                }
                else
                {
                    lvl9Enabled.enabled = false;
                    Capitolo9.enabled = false;
                }

                if (animatorStateInfo.IsName("Boss"))
                {
                    lvl10Enabled.enabled = true;
                    Capitolo10.enabled = true;

                    if (isToken_10)
                    {
                        Token_10.enabled = true;
                    }
                    else
                    {
                        TokenGrey_1.enabled = false;
                        TokenGrey_2.enabled = false;
                        TokenGrey_3.enabled = false;
                        TokenGrey_4.enabled = false;
                        TokenGrey_5.enabled = false;
                        TokenGrey_6.enabled = false;
                        TokenGrey_7.enabled = false;
                        TokenGrey_8.enabled = false;
                        TokenGrey_9.enabled = false;
                        TokenGrey_10.enabled = true;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SoundManager.instance.PlaySingle(confirmSelection);
                        SceneManager.LoadScene("_LevelTemplate_Tonno3boss");
                    }
                }
                else
                {
                    lvl10Enabled.enabled = false;
                    Capitolo10.enabled = false;
                }

                if (!animatorStateInfo.IsName("Level1"))
                    Token_1.enabled = false;

                if (!animatorStateInfo.IsName("Level2"))
                    Token_2.enabled = false;

                if (!animatorStateInfo.IsName("Level3"))
                    Token_3.enabled = false;

                if (!animatorStateInfo.IsName("Level4"))
                    Token_4.enabled = false;

                if (!animatorStateInfo.IsName("Level5"))
                    Token_5.enabled = false;

                if (!animatorStateInfo.IsName("Level6"))
                    Token_6.enabled = false;

                if (!animatorStateInfo.IsName("Level7"))
                    Token_7.enabled = false;

                if (!animatorStateInfo.IsName("Level8"))
                    Token_8.enabled = false;

                if (!animatorStateInfo.IsName("Level9"))
                    Token_9.enabled = false;

                if (!animatorStateInfo.IsName("Boss"))
                    Token_10.enabled = false;
            }
        }
    }

    IEnumerator ActivateSceneSelection() {
        yield return new WaitForSeconds(.5f);

        lvl1Enabled.gameObject.SetActive(true);
        lvl2Enabled.gameObject.SetActive(true);
        lvl3Enabled.gameObject.SetActive(true);
        lvl4Enabled.gameObject.SetActive(true);
        lvl5Enabled.gameObject.SetActive(true);
        lvl6Enabled.gameObject.SetActive(true);
        lvl7Enabled.gameObject.SetActive(true);
        lvl8Enabled.gameObject.SetActive(true);
        lvl9Enabled.gameObject.SetActive(true);
        lvl10Enabled.gameObject.SetActive(true);

        Token_1.gameObject.SetActive(true);
        Token_2.gameObject.SetActive(true);
        Token_3.gameObject.SetActive(true);
        Token_4.gameObject.SetActive(true);
        Token_5.gameObject.SetActive(true);
        Token_6.gameObject.SetActive(true);
        Token_7.gameObject.SetActive(true);
        Token_8.gameObject.SetActive(true);
        Token_9.gameObject.SetActive(true);
        Token_10.gameObject.SetActive(true);

        TokenGrey_1.gameObject.SetActive(true);
        TokenGrey_2.gameObject.SetActive(true);
        TokenGrey_3.gameObject.SetActive(true);
        TokenGrey_4.gameObject.SetActive(true);
        TokenGrey_5.gameObject.SetActive(true);
        TokenGrey_6.gameObject.SetActive(true);
        TokenGrey_7.gameObject.SetActive(true);
        TokenGrey_8.gameObject.SetActive(true);
        TokenGrey_9.gameObject.SetActive(true);
        TokenGrey_10.gameObject.SetActive(true);

        Capitolo1.gameObject.SetActive(true);
        Capitolo2.gameObject.SetActive(true);
        Capitolo3.gameObject.SetActive(true);
        Capitolo4.gameObject.SetActive(true);
        Capitolo5.gameObject.SetActive(true);
        Capitolo6.gameObject.SetActive(true);
        Capitolo7.gameObject.SetActive(true);
        Capitolo8.gameObject.SetActive(true);
        Capitolo9.gameObject.SetActive(true);
        Capitolo10.gameObject.SetActive(true);

        yield return null;
    }

    IEnumerator ShowCreditsInfo()
    {
        yield return new WaitForSeconds(1);
        isCreditsEnabled = true;
        //Grupp8.enabled = true;
        //TestoCrediti.enabled = true;
        yield return null;
    }

    public static void SetToken(string sceneName)
    {
        switch (sceneName)
        {
            // Lvl 1
            case "Livello1_Stanza1":
                isToken_1 = true;
                break;
            case "Livello1_Stanza2":
                isToken_2 = true;
                break;
            case "Livello1_Stanza3":
                isToken_3 = true;
                break;

            case "Livello2_Stanza1":
                isToken_4 = true;
                break;
            case "Livello2_Stanza2":
                isToken_5 = true;
                break;
            case "Livello2_Stanza3":
                isToken_6 = true;
                break;
            // Lvl 3
            case "Livello3_Stanza1":
                isToken_7 = true;
                break;
            case "Livello3_Stanza2":
                isToken_8 = true;
                break;
            case "Livello3_Stanza3":
                isToken_9 = true;
                break;

            // Boss
            case "_LevelTemplate_Tonno3boss":
                isToken_10 = true;
                break;
        }
    }

    public static bool GetToken(int roomNumber)
    {
        switch (roomNumber)
        {
            case 1: return isToken_1;
            case 2: return isToken_2;
            case 3: return isToken_3;
            case 4: return isToken_4;
            case 5: return isToken_5;
            case 6: return isToken_6;
            case 7: return isToken_7;
            case 8: return isToken_8;
            case 9: return isToken_9;
            case 10: return isToken_10;
        }

        return false;
    }

}
