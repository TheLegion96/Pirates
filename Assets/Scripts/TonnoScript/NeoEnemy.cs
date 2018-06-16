﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NeoCompleted
{
    public class NeoEnemy : MonoBehaviour
    {
        public  BoxCollider2D boxColliderEnemy;
        protected Animator animator;

        public BoxCollider2D boxCollider;
        public Rigidbody2D rb2D;
        // Enumeratori
        public enum EnemyType
        {
            Horizontal,     // 0 Movimento A => B su Asse X
            Vertical,       // 1 Movimento A => B su Asse Y
            Ranged,         // 2 Movimento Auto Rotativo Nemico Ranged

            CustomPatrol    // 4 Movimento custom definito da Unity
        };

        [Header("Enemy properties")]
        public EnemyType enemyTipe;                         // Indica il tipo di nemico
        public int hp = 1;                                  //hit points for the enemy.
        public Vector2 start;
        public Vector2 end;

        [Header("Horizontal/Vertical only")]
        private float step = 1f;
        public float pA;
        public float pB;
        public bool wayOfMovement;

        [Header("Ranged only")]
        public int maxTicks;
     //   public LineOfSight EnemyAimingWay;
        public int tick;

        protected List<Transform> _DeadZone = new List<Transform>();
        protected List<Transform> _LaserDeadZone = new List<Transform>();

        //Patrolling
        [Header("Patrolling only")]
        public Transform[] patrolPoints;
        protected int patrolIndex;

        //Cose...
    //    Player hitPlayer;

        // Use this for initialization
        void Start()
        {
            boxColliderEnemy = GetComponent<BoxCollider2D>();
            //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
            //This allows the GameManager to issue movement commands.
           

            //Get and store a reference to the attached Animator component.
            animator = GetComponent<Animator>();

            if (enemyTipe == EnemyType.Horizontal)
            {

                pA = this.transform.position.x + 3f;
                pB = this.transform.position.x - 3f;
            }
            else if (enemyTipe == EnemyType.Vertical)
            {
                pA = this.transform.position.y + 3f;
                pB = this.transform.position.y - 3f;

            }
            //Call the start function of our base class MovingObject.
            //Get a component reference to this object's BoxCollider2D
            boxCollider = GetComponent<BoxCollider2D>();

            //Get a component reference to this object's Rigidbody2D
            rb2D = GetComponent<Rigidbody2D>();

          

            //Inizializziamo l'animator.
         //   _animator = GetComponent<Animator>();
            //if (enemyTipe == EnemyType.Ranged)
            //{
            //    ChangeSightAnimation(EnemyAimingWay);
            //}
            NeoGameManager.instance.AddEnemyToList(this);
        }

        // Update is called once per frame
        void Update()
        {

        }



        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public void MoveEnemy()
        {

            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            int xDir = 0;
            int yDir = 0;

            CheckNextCell(out xDir, out yDir);

            //AttemptMove<Player>(xDir, yDir);
        }
        public virtual void CheckNextCell(out int xDir, out int yDir)
        {
            xDir = 0;
            yDir = 0;

            //  Vector3 _tempEnd = new Vector3();
            Vector2 newPos;


            switch (enemyTipe)
            {
                case EnemyType.CustomPatrol: //Patrol defined by level designers.

                    if (transform.position == patrolPoints[patrolIndex].position)
                    {
                        patrolIndex++;
                    }
                    if (patrolIndex >= patrolPoints.Length)
                    {
                        patrolIndex = 0;
                    }

                    xDir = (int)(patrolPoints[patrolIndex].position.x - transform.position.x);
                    yDir = (int)(patrolPoints[patrolIndex].position.y - transform.position.y);
                    break;

                case EnemyType.Horizontal://Pattern AB_AsseX

                    if (wayOfMovement == true)
                    {
                        xDir = (int)step;
                        newPos = new Vector2(this.transform.position.x + step, this.transform.position.y);
                    }
                    else
                    {
                        xDir = -(int)step;
                        newPos = new Vector2(this.transform.position.x - step, this.transform.position.y);
                    }
                    if (newPos.x == pA)
                    {
                        wayOfMovement = false;
                    }
                    if (newPos.x == pB)
                    {
                        wayOfMovement = true;
                    }
                    break;


                case EnemyType.Vertical://Pattern AB_AsseY 

                    if (wayOfMovement == true)
                    {
                        yDir = (int)step;
                        newPos = new Vector2(this.transform.position.x, this.transform.position.y + step);
                    }
                    else
                    {
                        yDir = -(int)step;
                        newPos = new Vector2(this.transform.position.x, this.transform.position.y - step);
                    }
                    if (newPos.y == pA)
                    {
                        wayOfMovement = false;
                    }
                    if (newPos.y == pB)
                    {
                        wayOfMovement = true;
                    }
                    break;

            }
        }

        //public Vector2 GetVectorDirection(LineOfSight aimingDirection)
        //{
        //    Vector2 direction = new Vector2();

        //    switch (aimingDirection)
        //    {
        //        case LineOfSight.down:
        //            direction = -transform.up;
        //            break;
        //        case LineOfSight.up:
        //            direction = transform.up;
        //            break;
        //        case LineOfSight.right:
        //            direction = transform.right;
        //            break;
        //        case LineOfSight.left:
        //            direction = -transform.right;
        //            break;
        //    }

        //    return direction;
        //}

        ////MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        //public void MoveEnemy()
        //{

        //    //Declare variables for X and Y axis move directions, these range from -1 to 1.
        //    //These values allow us to choose between the cardinal directions: up, down, left and right.
        //    int xDir = 0;
        //    int yDir = 0;

        //    CheckNextCell(out xDir, out yDir);

        //    AttemptMove<Player>(xDir, yDir);
        //}



        //public static void ChangeAimingDirection(ref LineOfSight posizione)
        //{
        //    switch (posizione)
        //    {
        //        case LineOfSight.down:
        //            posizione = LineOfSight.left;
        //            break;
        //        case LineOfSight.left:
        //            posizione = LineOfSight.up;
        //            break;
        //        case LineOfSight.up:
        //            posizione = LineOfSight.right;
        //            break;
        //        case LineOfSight.right:
        //            posizione = LineOfSight.down;
        //            break;
        //    }
        //}

        ////OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject
        ////and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
        ////Player hitPlayer;
        //protected override void OnCantMove<T>(T component)
        //{
        //    //Declare hitPlayer and set it to equal the encountered component.
        //    Player hitPlayer = component as Player;

        //    //Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
        //    //	hitPlayer.LoseFood (playerDamage);

        //    //Set the attack trigger of animator to trigger Enemy attack animation.
        //    animator.SetTrigger("Attack");

        //    //Stop the background music.
        //    SoundManager.instance.musicSource.Stop();

        //    //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
        //    SoundManager.instance.RandomizeSfx(attackSound1);

        //    hitPlayer.ExecuteGameOver();

        //}


    }
}