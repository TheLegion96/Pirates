using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


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

        public ENEMY_TYPE Enemy_Type;

        [SerializeField] private GameObject Deadzone;

        [EnableIf("isHorizontal")]
        [EnableIf("isVertical")]
        [SerializeField] public Vector2 startPos;

        [EnableIf("isHorizontal")]
        [EnableIf("isVertical")]
        [SerializeField] public Vector2 finalPos;

       

        public bool DeadZoneONorOFF = true;

        protected Animator _animator;


        private void Awake()
        {

        }
        private void Start()
        {
            _animator = GetComponent<Animator>();

        }

        private void Update()
        {
            
        }

        public void MoveEnemy()
        {
            switch(Enemy_Type)
            {
                case ENEMY_TYPE.Melee_Clockwork:

                    break;
                case ENEMY_TYPE.Melee_Horizontal:

                    break;
                case ENEMY_TYPE.Melee_Vertical:

                    break;
                case ENEMY_TYPE.Ranged:

                    break;
            }

        }
        public void CheckNextCell()
        {

        }

        #region NeoEnemyTool
       [HideInInspector] public bool isHorizontal;
        [HideInInspector] public bool isVertical;
        [HideInInspector] public bool isClockwork;
        [HideInInspector] public bool isRanged;

        [ExecuteInEditMode]
        private void ChangeUI()
        {
            if(Enemy_Type== ENEMY_TYPE.Melee_Horizontal)
            {
                isHorizontal = true;
                isVertical = false;
                isRanged = false;
                isClockwork = false;
            }
            if (Enemy_Type == ENEMY_TYPE.Melee_Vertical)
            {
                isVertical = true;
                isHorizontal = false;
                isRanged = false;
                isClockwork = false;
            }
            if (Enemy_Type == ENEMY_TYPE.Melee_Clockwork)
            {
                isClockwork = true;
                isHorizontal = false;
                isRanged = false;
                isVertical = false;
            }
            if (Enemy_Type == ENEMY_TYPE.Ranged)
            {
                isRanged = true;
                isHorizontal = false;
                isVertical = false;
                isClockwork = false;

            }
        }

        #endregion




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
    }
}