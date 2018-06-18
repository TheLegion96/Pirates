﻿using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace NeoCompleted
{
    public class NeoGameManager : SerializedMonoBehaviour
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
            yield return null;
        }

    }
}