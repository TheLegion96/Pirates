using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;

namespace NeoCompleted
{
    public class NeoEnemy : SerializedMonoBehaviour
    {
        public enum ENEMY_TYPE
        {
            Melee_Horizontal,
            Melee_Vertical,
            Melee_Clockwork,
            Ranged
        }
        public enum LineOfSight { up, left, down, right }

        protected LineOfSight Sight;
      public float  moveTime=0.5f;

        [Header("Type of Enemy")]
        [Space]
        [InfoBox("Select the type of enemy you want to create")]
        [Space]
        public ENEMY_TYPE EnemyType;

        [Space]
        [Space]
        [ShowIf("isLinear")]
        [InfoBox("Choose the coordinates that the enemy must use as a reference for the  movements. In this case the limit A")]
        [Space]
        [SerializeField] public Vector2 startPos;

        [ShowIf("isLinear")]
        [InfoBox("Choose the coordinates that the enemy must use as a reference for the  movements. In this case the limit B")]
        [Space]
        [SerializeField] public Vector2 finalPos;


        [Space]
        [Space]
        [ShowIf("isClockwork")]
        [InfoBox("Choose the coordinates that the enemy must use as a reference for the movements.In this case the vertices in which it will rotate")]
        [SerializeField] public Transform[] PatrolPoint;
        [ShowIf("isClockwork")]
        [InfoBox("Current patrol point")]
        public int index;

    

    [SerializeField] private GameObject Deadzone;

        public bool DeadZoneONorOFF = true;

        protected Animator _animator;


       
        private void Start()
        {
            _animator = GetComponent<Animator>();
            NeoGameManager.instance.AddEnemyToList(this);
            if(EnemyType== ENEMY_TYPE.Ranged)
            {
                InstanceDeadZone(Sight);

            }
        }

        private void Update()
        {

        }

        public void MoveEnemy()
        {
            switch (EnemyType)
            {
                case ENEMY_TYPE.Melee_Horizontal:
                    if (transform.position == (Vector3)startPos|| transform.position == (Vector3)finalPos)
                    {
                        //Deve invertire la marcia
                    }
                    if (Sight == LineOfSight.left)
                    {

                        this.gameObject.transform.DOMove(transform.position - transform.right, moveTime);
                      
                    }
                    if (Sight == LineOfSight.right)
                    {
                        this.gameObject.transform.DOMove(transform.position + transform.right, moveTime);
                    }
                    
                    break;
                case ENEMY_TYPE.Melee_Vertical:
                    if (transform.position == (Vector3)startPos || transform.position == (Vector3)finalPos)
                    {
                        //Deve invertire la marcia
                    }
                    if (Sight == LineOfSight.down)
                    {
                        this.gameObject.transform.DOMove(transform.position - transform.up, moveTime);
                    }
                    if (Sight == LineOfSight.up)
                    {
                        this.gameObject.transform.DOMove(transform.position + transform.up, moveTime);

                    }
                    break;
                case ENEMY_TYPE.Melee_Clockwork:
                    if(index==PatrolPoint.Length)
                    {
                        index = 0;
                    }
                  
                    transform.DOMove(PatrolPoint[index].position,moveTime).OnStart(SaveMyOldPosition).OnComplete(CheckForChange);
                  
                    index++;
                    break;
                case ENEMY_TYPE.Ranged:

                    break;


            }
            NeoGameManager.instance.SetState(NeoGameManager.State.Wait);

        }
        Vector3 oldPos;
        private void SaveMyOldPosition()
        {
            oldPos = transform.position;
        }
        private void CheckForChange()
        {
            if(oldPos.x>transform.position.x)
            {
                ChangeSightAnimation(LineOfSight.left);
            }
            if (oldPos.x < transform.position.x)
            {
                ChangeSightAnimation(LineOfSight.right);
            }
            if (oldPos.y > transform.position.y)
            {
                ChangeSightAnimation(LineOfSight.down);
            }
            if (oldPos.y < transform.position.y)
            {
                ChangeSightAnimation(LineOfSight.up);
            }
        }    
        //Cambio Lato Animazione Oggetto
        protected void ChangeSightAnimation(LineOfSight sight)
        {
            switch (sight)
            {
                case LineOfSight.up:
                    ChangeSightAnimation(0f, 1f);
                    break;

                case LineOfSight.right:
                    ChangeSightAnimation(1f, 0f);
                    break;

                case LineOfSight.down:
                    ChangeSightAnimation(0f, -1f);
                    break;

                case LineOfSight.left:
                    ChangeSightAnimation(-1f, 0f);
                    break;
            }
        }
        protected void ChangeSightAnimation(float xDir, float yDir)
        {
            _animator.SetFloat("x", xDir);
            _animator.SetFloat("y", yDir);
        }
        public void InstanceDeadZone(LineOfSight parEnemyAimingWay)
        {

            Vector3 _TempEndPosition = new Vector3();
            Transform _TempDeadZone = Instantiate(Deadzone.transform, this.transform.position, Quaternion.identity);
            _TempEndPosition = new Vector3();
            switch (parEnemyAimingWay)
            {
                case LineOfSight.down:
                    _TempEndPosition = _TempDeadZone.position;
                    _TempEndPosition.y -= 1;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.left:
                    _TempEndPosition = _TempDeadZone.position;
                    _TempEndPosition.x -= 1;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.up:
                    _TempEndPosition = _TempDeadZone.position;
                    _TempEndPosition.y += 1;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.right:
                    _TempEndPosition = _TempDeadZone.position;
                    _TempEndPosition.x += 1;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
            }
            RaycastHit2D checkCollision;
            checkCollision = Physics2D.Linecast(_TempDeadZone.position, _TempDeadZone.position);
            if (checkCollision.transform != null)
            {
                if (checkCollision.transform.tag == "Stone" || checkCollision.transform.tag == "Enemy")
                {
                    Destroy(_TempDeadZone.gameObject);

                }
                else if (checkCollision.transform.tag == "DeadZone")
                {
                    Destroy(_TempDeadZone.gameObject);
                }
                else
                {
                    _TempDeadZone.GetComponent<BoxCollider2D>().enabled = true;
                }
            }

            if (_TempDeadZone != null)
            {

                _TempDeadZone.position = _TempEndPosition;

            }
        }
        #region NeoEnemyTool
        [HideInInspector] public bool isLinear;
      
        [HideInInspector] public bool isClockwork;
        [HideInInspector] public bool isRanged;

        [ExecuteInEditMode]
         void OnValidate()
        {
            if (EnemyType == ENEMY_TYPE.Melee_Horizontal||EnemyType== ENEMY_TYPE.Melee_Vertical)
            {
                isLinear = true;              
                isRanged = false;
                isClockwork = false;
            }
           
            if (EnemyType == ENEMY_TYPE.Melee_Clockwork)
            {
                isClockwork = true;
                isLinear = false;
                isRanged = false;
               
            }
            if (EnemyType == ENEMY_TYPE.Ranged)
            {
                isRanged = true;
                isLinear = false;
                isClockwork = false;

            }
        } 

        #endregion
    }
}