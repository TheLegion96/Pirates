using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeoCompleted
{
    public class NeoGameManager : MonoBehaviour
    {

        public enum State
        {
            LevelStart,
            Wait,
            MovePlayer,
            MoveEnemy,
            Pause,
            Bestiario,
        }
        public State state = State.Wait;
        public float turnDelay = 0.1f;



        public static NeoGameManager instance;


        public List<NeoEnemy> enemies;
        [HideInInspector] public bool playersTurn = true;
        private bool enemiesMoving;
        //-----------------------------CODE----------------------------------------------------------------



        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            enemies = new List<NeoEnemy>();
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(state);
            if(state==State.Wait)
            {
                playersTurn = true;
            }

            if (state == State.MovePlayer)
            {
                playersTurn = false;
                SetState(State.MoveEnemy);
            }

            if (state== State.MoveEnemy)
            {
                StartCoroutine(MoveEnemies());
            }
        }

        public void SetState(State _state)
        {
            state = _state;
        }


        public void AddEnemyToList( NeoEnemy script)
        {
            //Add Enemy to List enemies.
            enemies.Add(script);
        }

        //Coroutine to move enemies in sequence.
        IEnumerator MoveEnemies()
        {
            RangedEnemy tempRangedEnemy;
            Vector2 tempEnd;
            Completed.Enemy.LineOfSight tempEnemyAimingWay;
            int tempTickBeforeChange;

          
            enemiesMoving = true;

         
            yield return new WaitForSeconds(turnDelay);

           
            if (enemies.Count == 0)
            {            
                yield return new WaitForSeconds(turnDelay);
            }

           
            for (int i = 0; i < enemies.Count; i++)
            {
              
                enemies[i].MoveEnemy();

                yield return new WaitForSeconds(enemies[i].moveTime / 100);
            }
            yield return new WaitForSeconds(0.1f);

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
                   
                }
            }


            //Once Enemies are done moving, set playersTurn to true so player can move.
            state = State.Wait;
            //Enemies are done moving, set enemiesMoving to false.
            enemiesMoving = false;
        }

    }
}