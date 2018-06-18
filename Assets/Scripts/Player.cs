using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Completed
{
    //Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
    public class Player : MovingObject
    {
        // Enumeratori
        public enum Gender
        {
            Male,
            Female
        };

        [Header("Player Infos")]
        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
        public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
        public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
        public int attackDamage = 1;                //How much damage a player does to a wall when chopping it.
        public static Gender gender;
        private bool hasSword = false;              //Check if the player has a sword. (REMOVED)
        private float swordDestroyMinScale = 1f;
        private float swordDestroyMaxScale = 4f;
        private float swordDestroyAnimationTime = 1.5f;

        [Header("Player Sounds")]
        public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
        public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
        public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
        public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
        public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
        public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
        public AudioClip gameOverSound;             //Audio clip to play when player dies.
        private TextMesh totalStepsText;                 //UI Text to display current player money total.
        private TextMesh levelStepsText;                 //UI Text to display current player steps total.

        [Header("Animators")]
        public RuntimeAnimatorController maleCharacterWithSwordAnimator;
        public RuntimeAnimatorController maleCharacterWithoutSwordAnimator;
        public RuntimeAnimatorController femaleCharacterWithSwordAnimator;
        public RuntimeAnimatorController femaleCharacterWithoutSwordAnimator;
        private Animator animator;                  //Used to store a reference to the Player's animator component.

        private int totalSteps;                     //Used to store player turns total during level.
        private int levelSteps;                     //Used to store player steps during level.
        [Header("Turns and Moves")]
        public KeyCode turnJumper = KeyCode.Space;
        public Vector2 old_Coordinate;
        public Vector2 new_Coordinate;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif

        public bool isStillAlive = true;

        [Header("Boss Level Only")]
        public AudioClip cannonShot;
        public AudioClip bossScream;

        //Start overrides the Start function of MovingObject
        protected override void Start()
        {
            GetComponent<BoxCollider2D>().enabled = true;
            //Get a component reference to the Player's animator component
            animator = GetComponent<Animator>();

            if (gender == Gender.Male)
            {
                animator.runtimeAnimatorController = (hasSword ? maleCharacterWithSwordAnimator : maleCharacterWithoutSwordAnimator);
            }
            else
            {
                animator.runtimeAnimatorController = (hasSword ? femaleCharacterWithSwordAnimator : femaleCharacterWithoutSwordAnimator);
            }

            //Get the current food point total stored in GameManager.instance between levels.
            totalSteps = GameManager.instance.playerTotalMoney;
            levelSteps = 0;

            GameObject totalStepsGameObject = GameObject.Find("TotalStepsText");
            if (totalStepsGameObject != null)
            {
                totalStepsText = totalStepsGameObject.GetComponent<TextMesh>();
                totalStepsText.text = totalSteps.ToString();
            }

            GameObject levelStepsGameObject = GameObject.Find("LevelStepsText");
            if (levelStepsGameObject != null)
            {
                levelStepsText = levelStepsGameObject.GetComponent<TextMesh>();
                levelStepsText.text = levelSteps.ToString();
            }

            //Call the Start function of the MovingObject base class.
            base.Start();
        }


        ////This function is called when the behaviour becomes disabled or inactive.
        //private void OnDisable()
        //{
        //    //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
        //    GameManager.instance.playerTotalMoney = totalMoney;
        //}


        private void Update()
        {
            if (GameManager.instance == null)
            {
                Debug.LogError("Qualcosa si è rotto nel GameManager!");
                return;
            }

            //If it's not the player's turn, exit the function.
            if (!GameManager.instance.playersTurn) return;

            if (GameManager.instance.state == GameManager.State.Play && isStillAlive && (
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(turnJumper)
                ))
            {
                if (Input.GetKeyDown(turnJumper))
                {
                    animator.SetTrigger("LongIdle");
                }

                int horizontal = 0;     //Used to store the horizontal move direction.
                int vertical = 0;       //Used to store the vertical move direction.

                //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER

                //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
                horizontal = (int)(Input.GetAxisRaw("Horizontal"));

                //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
                vertical = (int)(Input.GetAxisRaw("Vertical"));
                old_Coordinate = this.transform.position;
                //Check if moving horizontally, if so set vertical to zero.
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("LongIdle"))
                {
                    horizontal = 0;
                    vertical = 0;
                }
                else if (horizontal != 0)
                {
                    vertical = 0;
                }
                //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
                //Check if we have a non-zero value for horizontal or vertical
                if (horizontal != 0 || vertical != 0 || Input.GetKeyDown(turnJumper))
                {
                    bool isStillAlive = true;
                    bool proceedWithTheTurn = true;

                    //[Verza] Add check on enemies that can kill the player.
                    RaycastHit2D hit;
                    Vector2 end;
                    CanMove(horizontal, vertical, out hit, out end);
                    if (hit.transform != null)
                    {
                        if (hit.transform.GetComponent<Enemy>() != null)
                        {
                            foreach (Enemy enemy in GameManager.instance.enemies)
                            {
                                if (enemy is PatrollingEnemy)
                                {
                                    enemy.TryToKillPlayer(this, out isStillAlive);
                                }
                                if (!isStillAlive) break;
                            }
                        }
                        else if (hit.transform.CompareTag("Stone"))
                        {
                            proceedWithTheTurn = false;
                        }
                        //       else
                        /*if(hit.transform.tag=="Tentacle")
                        {
                            ExecuteGameOver();
                        }*/
                    }

                    if (isStillAlive && proceedWithTheTurn)
                    {
                        /* È giusto che sia qui dentro. */

                        GameObject[] DestroyDeadZone;
                        GameObject[] DestroyLaserDeadZone;
                        DestroyDeadZone = GameObject.FindGameObjectsWithTag("DeadZone");
                        DestroyLaserDeadZone = GameObject.FindGameObjectsWithTag("LaserDeadZone");
                        if (DestroyLaserDeadZone.Length > 0 || DestroyDeadZone.Length > 0)
                        {
                            for (int i1 = 0; i1 < DestroyDeadZone.Length; i1++)
                            {
                                Destroy(DestroyDeadZone[i1].gameObject);
                            }
                            for (int i1 = 0; i1 < DestroyLaserDeadZone.Length; i1++)
                            {
                                Destroy(DestroyLaserDeadZone[i1].gameObject);
                            }
                        }


                        /**/
                        //Call AttemptMove passing in the generic parameter Enemy, since that is what Player may interact with if they encounter one (by attacking it)
                        //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                        AttemptMove<Enemy>(horizontal, vertical);
                    }
                }
            }
        }

        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            //Every time player moves, subtract from food points total.
            levelSteps++;

            //Update food text display to reflect current score.
            levelStepsText.text = levelSteps.ToString();

            //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
            base.AttemptMove<T>(xDir, yDir);

            ////Hit allows us to reference the result of the Linecast done in Move.
            //RaycastHit2D hit;

            ////If Move returns true, meaning Player was able to move into an empty space.
            //Vector2 end;
            //if (CanMove(xDir, yDir, out hit, out end))
            //{
            //    //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            //    //SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            //
            //    /*if (SoundManager.instance)
            //    {
            //        StartCoroutine(SoundManager.instance.PlayNextStep(0.5f));//moveTime * 5));
            //    }*/
            //}
            new_Coordinate = this.transform.position;
            //Set the playersTurn boolean of GameManager to false now that players turn is over.

            GameManager.instance.playersTurn = false;
            TestScript.Go = true;
        }


        //OnCantMove overrides the abstract function OnCantMove in MovingObject.
        //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
        protected override void OnCantMove<T>(T component)
        {
            if (isStillAlive && component is Enemy)
            {
                //Set hitWall to equal the component passed in as a parameter.
                Enemy hitEnemy = component as Enemy;

                hitEnemy.TryToKillPlayer(this, out isStillAlive);

                if (isStillAlive)
                {
                    if (hasSword)
                    {
                        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
                        animator.SetTrigger("Attack");

                        //Call the DamageWall function of the Wall we are hitting.
                        hitEnemy.DamageEnemy(attackDamage);

                        //Remove the sword.
                        RemoveSword();
                    }
                    else
                    {
                        Vector2 newEnemySight = transform.position - hitEnemy.transform.position;

                        ChangeSightAnimation((int)newEnemySight.x, (int)newEnemySight.y);
                        hitEnemy.AttemptAttack((int)newEnemySight.x, (int)newEnemySight.y, this, out isStillAlive);
                    }
                }
            }
        }

        private void RemoveSword()
        {
            GameManager.instance.state = GameManager.State.RunningAnimation;
            //Remove the sword.
            hasSword = false;

            //Change Animator Controller
            if (gender == Gender.Male)
            {
                animator.runtimeAnimatorController = maleCharacterWithoutSwordAnimator;
            }
            else
            {
                animator.runtimeAnimatorController = femaleCharacterWithoutSwordAnimator;
            }

            StartCoroutine(RemoveSword_CoRoutine());
        }
        private IEnumerator RemoveSword_CoRoutine()
        {
            if (isStillAlive)
            {
                GameObject spadaInteraGameObject = GameObject.Find("SpadaIntera");
                GameObject spadaRottaGameObject = GameObject.Find("SpadaRotta");
                Image spadaInteraImage = null, spadaRottaImage = null;

                if (spadaInteraGameObject != null)
                {
                    spadaInteraImage = spadaInteraGameObject.GetComponent<Image>();

                    if (spadaRottaGameObject != null)
                    {
                        spadaRottaImage = spadaRottaGameObject.GetComponent<Image>();
                    }
                }

                if (spadaInteraImage != null && spadaRottaImage != null)
                {
                    Vector3 initialPosition, finalPosition;

                    //Spada intera
                    initialPosition = new Vector3(swordDestroyMaxScale, swordDestroyMaxScale, spadaInteraImage.rectTransform.localScale.z);
                    finalPosition = new Vector3(swordDestroyMinScale, swordDestroyMinScale, spadaInteraImage.rectTransform.localScale.z);

                    spadaInteraImage.rectTransform.localScale = initialPosition;
                    spadaInteraImage.enabled = true;

                    for (float elapsedTime = 0f; spadaInteraImage.rectTransform.localScale.x > finalPosition.x; elapsedTime += Time.deltaTime)
                    {
                        spadaInteraImage.rectTransform.localScale = Vector3.Lerp(initialPosition, finalPosition, (elapsedTime / (swordDestroyAnimationTime / 2)));
                        yield return null;
                    }
                    spadaInteraImage.enabled = false;

                    //Spada rotta
                    initialPosition = new Vector3(swordDestroyMinScale, swordDestroyMinScale, spadaRottaImage.rectTransform.localScale.z);
                    finalPosition = new Vector3(swordDestroyMaxScale, swordDestroyMaxScale, spadaRottaImage.rectTransform.localScale.z);

                    spadaRottaImage.rectTransform.localScale = (initialPosition);
                    spadaRottaImage.enabled = true;
                    for (float elapsedTime = 0f; spadaRottaImage.rectTransform.localScale.x < finalPosition.x; elapsedTime += Time.deltaTime)
                    {
                        spadaRottaImage.rectTransform.localScale = Vector3.Lerp(initialPosition, finalPosition, (elapsedTime / (swordDestroyAnimationTime / 2)));
                        yield return null;
                    }
                    spadaRottaImage.enabled = false;
                }

                GameManager.instance.state = GameManager.State.Play;
            }

            yield return null;
        }

        //TNT =GameObject.FindGameObjectsWithTag("Soda");
        // MaxCounter = TNT.Length;
        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Check if the tag of the trigger collided with is Exit.
            if (other.tag == "Exit")
            {
                LevelFinished();
            }

            //Check if the tag of the trigger collided with is Food.
            else if (other.tag == "Food")
            {
                //Add pointsPerFood to the players current food total.
                levelSteps -= pointsPerFood;

                //Update foodText to represent current total and notify player that they gained points
                levelStepsText.text = "(-" + pointsPerFood + ") " + levelSteps;

                //Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

                //Disable the food object the player collided with.
                other.gameObject.SetActive(false);
            }


            else if (other.tag == "Tentacle")
            {
                ExecuteGameOver();
            }
            if (other.tag == "Soda")
            {
                StartCoroutine(CannonCoroutine(other.gameObject));
            }
        }

        IEnumerator CannonCoroutine(GameObject otherGameObject)
        {
            otherGameObject.GetComponent<BoxCollider2D>().enabled = false;

            otherGameObject.GetComponentInChildren<Animator>().SetTrigger("Shoot");
            //SoundManager.instance.PlaySingle(cannonShot);
            otherGameObject.GetComponentInChildren<AudioSource>().Play();

            yield return new WaitForSeconds(.5f);

            GameObject.Find("Placeholder10su10").GetComponent<Animator>().SetTrigger("TakeDamage");
            SoundManager.instance.PlaySingle(bossScream);

            yield return new WaitForSeconds(1);

            //other.transform.GetChild(0).transform.position = new Vector3(other.transform.GetChild(0).transform.position.x, other.transform.GetChild(0).transform.position.y, -2);
            Destroy(otherGameObject);
            Debug.Log("NEMICO DANNEGGIATO");

            #region Old SodaScript (no need to open Clear after)
            /* 
            //Add pointsPerSoda to players food points total
            levelSteps -= pointsPerSoda;

            //Update foodText to represent current total and notify player that they gained points
            totalStepsText.text = "(-" + pointsPerSoda + ") " + levelSteps;

            //Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

            //Disable the soda object the player collided with.
            other.gameObject.SetActive(false);
            */
            #endregion

            yield return null;
        }

        public void LevelFinished()
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            this.GetComponent<BoxCollider2D>().enabled = false;

            GameManager.instance.playerTotalMoney += levelSteps;


            GameObject coinContainer = GameObject.Find("CoinContainer");
            Transform coinTransform;
            
            if (coinContainer != null)
            {
                Vector3 mainCameraPosition = GameObject.Find("Main Camera").transform.position;
                Vector3 coinFinalPosition = new Vector3(mainCameraPosition.x, mainCameraPosition.y, 0);
                if (levelSteps <= GameManager.instance.MaxStepsForToken)
                {
                    coinTransform = GameObject.Find("CoinGold").transform;
                }
                else
                {
                    coinTransform = GameObject.Find("CoinSilver").transform;
                }
                coinTransform.DOScale(1, 1);
                coinTransform.DOMove(coinFinalPosition, 1);

                new WaitForSeconds(2);
            }

            GameManager.instance.GoToNextScene(restartLevelDelay, levelSteps);

            //Disable the player object since level is over.
            enabled = false;
        }

        //CheckIfGameOver checks if the player is out of food points and if so, ends the game.
        private void CheckIfGameOver()
        {
            //Check if food point total is less than or equal to zero.
            if (levelSteps >= 100)
            {
                ExecuteGameOver();
            }
        }


        //ExecuteGameOver ends the game.
        public void ExecuteGameOver()
        {
            isStillAlive = false;
            StartCoroutine(ExecuteGameOverCoroutine());
        }

        private IEnumerator ExecuteGameOverCoroutine()
        {
            //[Verza]   Verifico esistenza del componente CameraShake sulla Main Camera.
            //          Se esiste, allora shakero la camera alla morte del player.
            //          Altrimenti il personaggio sta fermo, la camera non shakera ma non si spacca niente.
            CameraShake cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
            if (cameraShake != null)
            {
                // Call al metodo di Shake.
                cameraShake.ShakeCamera(1f, 0.1f);
            }
            animator.SetTrigger("Die");

            yield return new WaitForSeconds(2f);

            //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
            SoundManager.instance.PlaySingle(gameOverSound);

            //Stop the background music.
            SoundManager.instance.musicSource.Stop();

            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();

            yield return null;
        }
        public void GetGender(bool SelectedGender)
        {
            if (SelectedGender)
            {
                gender = Gender.Male;
            }
            else
            {
                gender = Gender.Female;
            }

        }
    }

}

