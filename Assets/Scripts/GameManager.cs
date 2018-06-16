using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

namespace Completed
{
    using System.Collections.Generic;       //Allows us to use Lists. 
    using UnityEngine.UI;                   //Allows us to use UI.

    public class GameManager : MonoBehaviour
    {
        public enum State
        {
            LevelStart,
            Play,
            Pause,
            Bestiario,
            RunningAnimation
        }

        public State state = State.Play;

        public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
        public float turnDelay = 0.1f;                          //Delay between each Player turn.
        public string deadText;
        public int playerTotalMoney = 0;                        //Starting value for Player Money.
        public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.

        private GameObject tutorialLayer;

        private GameObject levelSignboard;                      //Image to block out level as levels are being set up, background for levelText.
        private TextMesh levelTitleText;                        //Text to display current level number.
        private TextMesh BookClosedMenuSubtitleTextMesh;
        private TextMesh bookTitleOpenMenuTextMesh;
        private TextMesh bookSubtitleTextMesh;
        private TextMesh bookChapterTextTextMesh;
        private int level = 1;                                  //Current level number, expressed in game as "Day 1".
        public List<Enemy> enemies;                             //List of all Enemy units, used to issue them move commands.
        public AudioClip sceneCompletedAudioClip;
        private bool enemiesDying;                              //Boolean to check if enemies are dying.
        private bool enemiesMoving;                             //Boolean to check if enemies are moving.
        private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.

        //[Verza] Added new property in order to change scene title.
        private string title;
        private string subtitle;
        private string chapterText;
        private int maxStepsForToken;
        public static bool setRestartAvailable = false;

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;

                if (levelTitleText != null)
                    UpdateSceneBookProperty(levelTitleText, value);

                if (bookTitleOpenMenuTextMesh != null)
                    UpdateSceneBookProperty(bookTitleOpenMenuTextMesh, value);
            }
        }

        public string Subtitle
        {
            get
            {
                return subtitle;
            }

            set
            {
                subtitle = value;

                if (bookSubtitleTextMesh != null)
                    UpdateSceneBookProperty(bookSubtitleTextMesh, value);

                if (BookClosedMenuSubtitleTextMesh != null)
                    UpdateSceneBookProperty(BookClosedMenuSubtitleTextMesh, value);
            }
        }

        public string ChapterText
        {
            get
            {
                return chapterText;
            }

            set
            {
                chapterText = value;

                if (bookChapterTextTextMesh != null)
                {
                    string tmpChapterTextAccaped = value;
                    if (tmpChapterTextAccaped.Contains("\\n"))
                    {
                        tmpChapterTextAccaped = tmpChapterTextAccaped.Replace("\\n", "\n");
                    }
                    UpdateSceneBookProperty(bookChapterTextTextMesh, tmpChapterTextAccaped);
                }
            }
        }

        public int MaxStepsForToken
        {
            get
            {
                return maxStepsForToken;
            }

            set
            {
                maxStepsForToken = value;
            }
        }


        //private bool saved = false;



        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //Assign enemies to a new List of Enemy objects.
            enemies = new List<Enemy>();

            //Call the InitGame function to initialize the first level 
            InitGame();
        }

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (instance != null)
            {
                instance.level = SceneManager.GetActiveScene().buildIndex + 1;
                instance.playersTurn = true;
                instance.InitGame();
                switch (SceneManager.GetActiveScene().name)
                {
                    case "0_TUTORIAL - Scene 1":
                    case "0_TUTORIAL - Scene 2":
                    case "0_TUTORIAL - Scene 3":
                    case "Livello1_Stanza1":
                    case "Livello1_Stanza2":
                    case "Livello1_Stanza3":
                    case "Livello2_Stanza1":
                    case "Livello2_Stanza2":
                    case "Livello2_Stanza3":
                    case "Livello3_Stanza1":
                    case "Livello3_Stanza2":
                    case "Livello3_Stanza3":
                    case "_LevelTemplate_Tonno3boss":
                    case "_Prototype_End":
                    case "_Prototype_AfterEnd":
                        break;
                    default:
                        instance.playerTotalMoney = 0;
                        break;
                }
            }
        }


        //Initializes the game for each level.
        void InitGame()
        {
            state = State.LevelStart;

            //While doingSetup is true the player can't move, prevent player from moving while title card is up.
            doingSetup = true;

            tutorialLayer = GameObject.Find("TutorialLayer");
            levelSignboard = GameObject.Find("LevelSignboard");
            GameObject leveTitleGameObject = GameObject.Find("LevelText");

            if (leveTitleGameObject != null)
            {
                //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
                levelTitleText = leveTitleGameObject.GetComponent<TextMesh>();
            }

            if (tutorialLayer != null)
            {
                tutorialLayer.GetComponent<Tutorial>().ClosedTutorialCallback += StartGamePlayPhase;
            }
            else
            {
                // LEVEL SIGNBOARD
                if (levelSignboard != null && title != null)
                {
                    if (levelTitleText != null)
                    {
                        //Set the text of levelText to the string "Day" and append the current level number.
                        //levelText.text = "Day " + level;
                        //[Verza] Added dynamic title.
                        UpdateSceneBookProperty(levelTitleText, title + "\n" + subtitle);

                        //[Verza] Moving title block.
                        //StartCoroutine(MoveTitleBlock());
                        levelSignboard.transform.DOMoveY(levelSignboard.transform.position.y - 8f, 1);

                        //Set levelImage to active blocking player's view of the game board during setup.
                        //levelSignboard.SetActive(true);

                        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
                        Invoke("HideLevelImage", levelStartDelay);
                    }
                }
            }

            // PAUSE BOOK
            GameObject BookClosedMenuSubtitleGameObject = GameObject.Find("BookClosedMenuSubtitle");

            if (BookClosedMenuSubtitleGameObject != null && title != null)
            {
                GameObject bookTitleOpenMenuGameObject = GameObject.Find("BookTitleOpenMenu");
                GameObject bookSubtitleGameObject = GameObject.Find("BookSubtitle");
                GameObject bookChapterTextGameObject = GameObject.Find("BookChapterText");

                if (bookTitleOpenMenuGameObject != null)
                {
                    //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
                    bookTitleOpenMenuTextMesh = bookTitleOpenMenuGameObject.GetComponent<TextMesh>();

                    //Set the text of bookTitleTextMesh to the string "Day" and append the current level number.
                    //bookTitleTextMesh.text = "Day " + level;
                    //[Verza] Added dynamic title.
                    UpdateSceneBookProperty(bookTitleOpenMenuTextMesh, title);
                }

                if (bookSubtitleGameObject != null)
                {
                    //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
                    bookSubtitleTextMesh = bookSubtitleGameObject.GetComponent<TextMesh>();

                    //Set the text of bookTitleTextMesh to the string "Day" and append the current level number.
                    //bookTitleTextMesh.text = "Day " + level;
                    //[Verza] Added dynamic title.
                    UpdateSceneBookProperty(bookSubtitleTextMesh, subtitle);
                }

                if (BookClosedMenuSubtitleGameObject != null)
                {
                    //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
                    BookClosedMenuSubtitleTextMesh = BookClosedMenuSubtitleGameObject.GetComponent<TextMesh>();

                    //Set the text of bookTitleTextMesh to the string "Day" and append the current level number.
                    //bookTitleTextMesh.text = "Day " + level;
                    //[Verza] Added dynamic title.
                    UpdateSceneBookProperty(BookClosedMenuSubtitleTextMesh, subtitle);
                }

                if (bookChapterTextGameObject != null)
                {
                    //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
                    bookChapterTextTextMesh = bookChapterTextGameObject.GetComponent<TextMesh>();

                    //Set the text of bookTitleTextMesh to the string "Day" and append the current level number.
                    //bookTitleTextMesh.text = "Day " + level;
                    //[Verza] Added dynamic title.
                    string tmpChapterTextAccaped = "" + chapterText;
                    if (tmpChapterTextAccaped.Contains("\\n"))
                    {
                        tmpChapterTextAccaped = tmpChapterTextAccaped.Replace("\\n", "\n");
                    }
                    UpdateSceneBookProperty(bookChapterTextTextMesh, tmpChapterTextAccaped);
                }
            }

            //Clear any Enemy objects in our List to prepare for next level.
            enemies.Clear();

        }

        void UpdateSceneBookProperty(Text textObj, string textVal)
        {
            textObj.text = textVal;
        }
        void UpdateSceneBookProperty(TextMesh textObj, string textVal)
        {
            textObj.text = textVal;
        }

        IEnumerator MoveTitleBlock()
        {

            /*Vector3 startPosition = new Vector3(0, 305, 0);
            Vector3 endPosition = new Vector3(0, 5, 0);
            float movingSeconds = 2f;

            levelImage.GetComponent<RectTransform>().position = startPosition;

            while (movingSeconds > 0)
            {
                movingSeconds -= Time.deltaTime;

                levelImage.GetComponent<RectTransform>().position = Vector3.Lerp(levelImage.GetComponent<RectTransform>().position, endPosition, movingSeconds);

                yield return null;
            }*/

            yield return null;
        }


        //Hides black image used between levels
        void HideLevelImage()
        {
            //Disable the levelImage gameObject.
            //levelSignboard.SetActive(false);
            levelSignboard.transform.DOMoveY(levelSignboard.transform.position.y + 8f, 1);

            StartGamePlayPhase();
        }
        void StartGamePlayPhase()
        {
            //Set doingSetup to false allowing player to move again.
            doingSetup = false;

            state = State.Play;
        }

        //Update is called every frame.
        void Update()
        {
            if (setRestartAvailable == true)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    levelSignboard.transform.DOMoveY(levelSignboard.transform.position.y + 8f, 1).OnComplete(ResetScene);
                }

            }
            //Check that  or enemiesMoving or enemiesDying or doingSetup are not currently true.
            if (playersTurn || enemiesMoving || enemiesDying || doingSetup || !GameObject.Find("Player").GetComponent<Player>().isStillAlive)

                //If any of these are true, return and do not start MoveEnemies.
                return;

            //Start moving enemies.
            StartCoroutine(MoveEnemies());

        }
        void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #region Enemies
        //Call this to add the passed in Enemy to the List of Enemy objects.
        public void AddEnemyToList(Enemy script)
        {
            //Add Enemy to List enemies.
            enemies.Add(script);
        }

        //Call this to remove the passed in Enemy from the List of Enemy objects.
        public void RemoveEnemyFromList(Enemy script)
        {
            enemiesDying = true;

            //Remove Enemy from List enemies.
            enemies.Remove(script);

            enemiesDying = false;
        }

        //Coroutine to move enemies in sequence.
        IEnumerator MoveEnemies()
        {
            RangedEnemy tempRangedEnemy;
            Vector2 tempEnd;
            Completed.Enemy.LineOfSight tempEnemyAimingWay;
            int tempTickBeforeChange;

            //While enemiesMoving is true player is unable to move.
            enemiesMoving = true;

            //Wait for turnDelay seconds, defaults to .1 (100 ms).
            yield return new WaitForSeconds(turnDelay);

            //If there are no enemies spawned (IE in first level):
            if (enemies.Count == 0)
            {
                //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
                yield return new WaitForSeconds(turnDelay);
            }

            //Loop through List of Enemy objects.
            for (int i = 0; i < enemies.Count; i++)
            {
                //Call the MoveEnemy function of Enemy at index i in the enemies List.
                enemies[i].MoveEnemy();

                //Wait for Enemy's moveTime before moving next Enemy, 
                yield return new WaitForSeconds(enemies[i].moveTime / 100);
            }
            yield return new WaitForSeconds(0.1f);

            //Loop per cast linea di fuoco
            for (int i = 0; i < enemies.Count; i++)
            {
                tempRangedEnemy = null;

                if (enemies[i] is RangedEnemy)
                {
                    tempRangedEnemy = ((RangedEnemy)enemies[i]);

                    tempTickBeforeChange = tempRangedEnemy.maxTicks - 1;

                    if (tempRangedEnemy.tick == tempTickBeforeChange)
                    {
                        tempEnemyAimingWay = tempRangedEnemy.EnemyAimingWay;

                        Enemy.ChangeAimingDirection(ref tempEnemyAimingWay);
                        tempEnd = tempRangedEnemy.GetVectorDirection(tempEnemyAimingWay);
                        tempRangedEnemy.CheckStoneRaycast(ref tempEnd, ref tempEnemyAimingWay);
                        tempRangedEnemy.InstanceDeadZone(tempEnemyAimingWay);
                    }
                    else
                    {
                        tempRangedEnemy.InstanceLaserDeadZone(tempRangedEnemy.EnemyAimingWay);
                    }
                    //saved = false;
                    //yield return new WaitForSeconds(enemies[i].moveTime / 100);

                }
            }


            //Once Enemies are done moving, set playersTurn to true so player can move.
            playersTurn = true;

            //Enemies are done moving, set enemiesMoving to false.
            enemiesMoving = false;
        }
        #endregion

        //GameOver is called when the player reaches 0 food points
        public void GameOver()
        {
            setRestartAvailable = true;
            //Set levelText to display number of levels passed and game over message
            levelTitleText.text = deadText;

            //Enable black background image gameObject.
            levelSignboard.transform.DOMoveY(levelSignboard.transform.position.y - 8f, 1);


            //Disable this GameManager.
            // enabled = false;
        }

        #region Go To Next Scene
        //Goes to next scene when called.
        public void GoToNextScene(float delay, int playerSteps)
        {
            if (playerSteps > 0)
            {
                SoundManager.instance.PlaySingle(sceneCompletedAudioClip);
            }
            SetTokenInfo(playerSteps);
            Invoke("GoToNextScene_Invoked", delay);
        }
        private void GoToNextScene_Invoked()
        {
            GoToScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        public void GoToScene(int buildIndex)
        {
            //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
            //and not load all the scene object in the current scene.
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        }

        public void SetTokenInfo(int playerSteps)
        {
            if (playerSteps <= maxStepsForToken)
            {
                MenuManager.SetToken(SceneManager.GetActiveScene().name);
            }
        }
        #endregion
    }
}

