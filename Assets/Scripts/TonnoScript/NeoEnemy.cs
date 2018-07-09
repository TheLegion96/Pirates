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
        #region enum_Definition
        //Enumeratore dei tipi di nemici: Melee con movimento orizzontale, verticale, in senso orario&antiorario, e a distanza
        public enum ENEMY_TYPE
        {
            Melee_Horizontal,
            Melee_Vertical,
            Melee_Clockwork,
            Ranged
        }
        //Enumeratore della linea di tiro, serve a determinare il prossimo passo dei nemici e la linea di tiro dei nemici a distanza
        public enum LineOfSight { up, left, down, right }
        #endregion

        #region Variabili

        //-------------------------------------------START VARIABILI-------------------------------------------------------------------------------

        //Velocita di movimento tra caselle
        public float moveTime = 0.5f;

        [Header("Type of Enemy")]
        [Space]
        [InfoBox("Select the type of enemy you want to create")]
        [Space]
        public ENEMY_TYPE EnemyType; 
        public LineOfSight Sight;

        [Space]
        [Space]
        [ShowIf("isLinear")]
        [InfoBox("Choose the coordinates that the enemy must use as a reference for the  movements. In this case the limit A")]
        [Space]
        [SerializeField] public Vector2 startPos;
        [ShowIf("isLinear")]
        public GameObject PointA;
        [ShowIf("isLinear")]
        [InfoBox("Choose the coordinates that the enemy must use as a reference for the  movements. In this case the limit B")]
        [Space]
        [SerializeField] public Vector2 finalPos;
     
        [ShowIf("isLinear")]
        public GameObject PointB;

        [Space]
        [Space]
        [ShowIf("isClockwork")]
        [InfoBox("Choose the coordinates that the enemy must use as a reference for the movements.In this case the vertices in which it will rotate")]
        [SerializeField] public Transform[] PatrolPoint;
        [ShowIf("isClockwork")]
        [InfoBox("Current patrol point")]
        public int index;


        [Header("Ranged Only")]
   

        public int tick;
        public int CDTick;
        public int maxTicks;
        [SerializeField] private TextMesh CountDownMesh;

        Vector2 end;

        public LayerMask blockingLayer;  
        public LayerMask exitLayer;       

        BoxCollider2D boxColliderEnemy;

        [SerializeField] private GameObject Deadzone;
        [SerializeField] protected GameObject LaserDeadzone;


        public bool DeadZoneONorOFF = true;

        protected Animator _animator;

        protected List<Transform> _DeadZone = new List<Transform>();
        protected List<Transform> _LaserDeadZone = new List<Transform>();

        [SerializeField] public Transform _ParentDeadZone;
        [SerializeField] public Transform _ParentLaserDeadZone;


        Vector3 oldPos;

   
        //------------------------------------------------Fine Variabili------------------------------------------------------------------------------------
        #endregion

        #region Metodi Unity : Awake Start Update
        private void Awake()
        {
            //_ParentDeadZone = GameObject.Find("DeadZones").GetComponent<Transform>();
            //_ParentLaserDeadZone = GameObject.Find("LaserDeadZones").GetComponent<Transform>();
            CountDownMesh = GameObject.Find("CountDown").GetComponent<TextMesh>();
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            NeoGameManager.instance.AddEnemyToList(this);
            boxColliderEnemy = GetComponent<BoxCollider2D>();
            if(EnemyType== ENEMY_TYPE.Ranged)
            {               
                InstanceLaserDeadZone(Sight);
            }
        }

        private void Update()
        {

        }
        #endregion


        public void MoveEnemy() //Metodo di movimento base dei nemici
        {
            switch (EnemyType) //In base al tipo di nemico 
            {
                case ENEMY_TYPE.Melee_Horizontal: //Se è orrizzontale

                    
                   
                    if (Sight == LineOfSight.left) //Se sta guardando a sinistra
                    {
                        this.gameObject.transform.DOMove(transform.position - transform.right, moveTime).OnComplete(IDZ);//Si muove a sinistra e quando ha finito instanzia la nuova DeadZone                  
                    }
                    if (Sight == LineOfSight.right) //Se sta guardando a destra
                    {
                        this.gameObject.transform.DOMove(transform.position + transform.right, moveTime).OnComplete(IDZ);//Si muove a destra e quando ha finito instanzia la nuova DeadZone 
                    }
                
                    break;

                    
                case ENEMY_TYPE.Melee_Vertical://Se è Verticale

                    
                    if (Sight == LineOfSight.down) //Se sta guardando giù
                    {
                        this.gameObject.transform.DOMove(transform.position - transform.up, moveTime).OnComplete(IDZ);//Si mmuove in basso e quando ha finito insanzia la nuova DeadZone
                      
                    }
                    if (Sight == LineOfSight.up)//Se sta guardando sù
                    {
                        this.gameObject.transform.DOMove(transform.position + transform.up, moveTime).OnComplete(IDZ);//Si mmuove in alto e quando ha finito insanzia la nuova DeadZone

                    }
                    break;


                case ENEMY_TYPE.Melee_Clockwork://Se ha movimento in senso orario/Antiorario

                    if(index==PatrolPoint.Length)//Se è arrivato all'inidice massimo lo resetta
                    {
                        index = 0;
                    }
                  
                    transform.DOMove(PatrolPoint[index].position,moveTime).OnStart(SaveMyOldPosition)/*.OnUpdate(CheckForChange)*/.OnComplete(IDZ);//Ed esegue il movimento verso il punto successivo eseguendo le seguenti istruzioni nel  mentre:
                    /*
                     * OnStart(SaveMyOldPosition) Memorizza la posizione prima del movimento
                     * OnUpdate(CheckForChange) Controlla se durante il movimento il nemico deve cambiare la mira
                     * OnComplete(IDZ) Alla fine del movimento reinstanzia la DeadZone
                     */ 
              
                    index++; //e infine aggiorna l'indice
                    break;


              case ENEMY_TYPE.Ranged: //Se il nemico è di tipo "a Distanza"

                    ChangeSightAnimation(Sight); //Aggiorna la sua linea di tiro
          
                    boxColliderEnemy.enabled = false; //Disabilita di suo boxCollider per il calcolo del raycast alla fine

                    end = GetVectorDirection(Sight); //Ottiene il vettore di direzione della linea di tiro

                    tick++; //Aggiorna il turno attuale 

                    CDTick = maxTicks - tick; //Aggiorna il CountDown sottraendo ai tick massimi i tick attuali

                    int tickbeforechange = maxTicks - 1; //Crea il trigger del turno prima di cambiare la mira

                    if (CDTick == 0) //Se il CountDown e a 0
                    {
                        CDTick = maxTicks; //Si resetta 
                        //  CheckIfRaycastIs0();
                    }
                    CountDownMesh.text = CDTick.ToString(); //Aggiorna la Mesh del CoundDown

                
                    if (tick == maxTicks) //Se i tick sono al massimo
                    {
                        ChangeAimingDirection(ref Sight); //Cambia la direzione della mira
                        end = GetVectorDirection(Sight);//E aggiorna il vettore della lina di tiro

                        CheckStoneRaycast(ref end, ref Sight); //Controlla che nella lina di tiro non ci siano roccie e/o ostacoli da saltare

                        ChangeSightAnimation(Sight); //Cambia la direzione di mira

                        InstanceLaserDeadZone(Sight); //Instanzia una nuova Laser DeadZone

                        tick = 0; //e Azzera i suoi tick
                    }

                    if (tick != tickbeforechange)
                    {
                        InstanceLaserDeadZone(Sight);
                        //  CheckIfRaycastIs0();
                    }   
                    else
                    {
                        InstanceDeadZone(Sight);
                    }

                    RaycastHit2D Bullet = Physics2D.Raycast(transform.position, end, 9f, blockingLayer);
                    if (Bullet.collider == null)
                    {
                        // Check se sto beccando la porta.
                        Bullet = Physics2D.Raycast(transform.position, end, 9f, exitLayer);
                    }

                    if (Bullet.transform != null && Bullet.transform.tag == "Player")
                    {
                        ////Set the attack trigger of animator to trigger Enemy attack animation.
                        //animator.SetTrigger("Attack");

                        ////Stop the background music.
                        //SoundManager.instance.musicSource.Stop();

                        ////Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
                        //SoundManager.instance.RandomizeSfx(attackSound1);

                        //Bullet.transform.GetComponent<Player>().ExecuteGameOver();
                    }
                    boxColliderEnemy.enabled = true;
                    break;


            }
            Debug.Log("e alla fine di tutto io sono a:" + Sight);
            NeoGameManager.instance.SetState(NeoGameManager.State.Wait);

        }

        private void switchAim(ref LineOfSight sight, ENEMY_TYPE enemyType)
        {
           if(enemyType== ENEMY_TYPE.Melee_Horizontal)
            {
                if(sight== LineOfSight.right)
                {
                    sight = LineOfSight.left;
                    ChangeSightAnimation(sight);
                }
              else if(sight== LineOfSight.left)
                {
                    sight = LineOfSight.right;
                    ChangeSightAnimation(sight);
                }
            }
            if (enemyType == ENEMY_TYPE.Melee_Vertical)
            {
                if (sight == LineOfSight.up)
                {
                    sight = LineOfSight.down;
                    ChangeSightAnimation(sight);
                }
                else if(sight == LineOfSight.down)
                {
                    sight = LineOfSight.up;
                    ChangeSightAnimation(sight); 
                }
            }
        }

        public void CheckStoneRaycast(ref Vector2 parEnd, ref LineOfSight parEnemyAimingWay)
        {
            //Check se devo disabilitare e riattivare il box collider o, se è già spento, lasciarlo così perché se ne occupa qualcun altro.
            bool isBoxColliderToManageHere = boxColliderEnemy.enabled;

            bool isStoneRaycasted;
            RaycastHit2D CheckBlockingLayerObject;
            int aimingDirectionCheck = 0;

            if (isBoxColliderToManageHere)
            {
                boxColliderEnemy.enabled = false;
            }

            do
            {
                CheckBlockingLayerObject = Physics2D.Raycast(transform.position, parEnd, 1f, blockingLayer);

                isStoneRaycasted = CheckBlockingLayerObject && CheckBlockingLayerObject.transform.tag == "Stone";
                if (isStoneRaycasted)
                {
                    aimingDirectionCheck++;

                    ChangeAimingDirection(ref parEnemyAimingWay);
                    parEnd = GetVectorDirection(parEnemyAimingWay);

                    // Ha fatto il giro completo e ha trovato solo muri. Cattivi level designers!
                    if (aimingDirectionCheck == 3)
                    {
                        break;
                    }
                }

            } while (isStoneRaycasted);

            if (isBoxColliderToManageHere)
            {
                boxColliderEnemy.enabled = true;
            }
        }

        private void IDZ()
        {
            switch(EnemyType)
            {

                case ENEMY_TYPE.Ranged:   InstanceDeadZone(Sight); InstanceLaserDeadZone(Sight);
                    break;

                case ENEMY_TYPE.Melee_Clockwork:
                    checkNextDeadZone();
                    InstanceNextDeadZone(Sight); break;

                case ENEMY_TYPE.Melee_Horizontal:
                    checkForPosition();

                    InstanceNextDeadZone(Sight);
                    break;
                case ENEMY_TYPE.Melee_Vertical:
                    checkForPosition();

                    InstanceNextDeadZone(Sight);
                    break;
            }
          
        }

        private void checkForPosition()
        {
            if (transform.position == (Vector3)(startPos + correttoreCoordinate))
            {
                switchAim(ref Sight, EnemyType);
            }

            if (transform.position == (Vector3)(finalPos + correttoreCoordinate))//Controlla che non sia arrivato al capolinea
            {
                switchAim(ref Sight, EnemyType);  //In tal caso deve invertire la marcia
            }
        }

        private void SaveMyOldPosition()
        {
            oldPos = transform.position;
        }

        private void CheckForChange()
        {
            if(oldPos.x>transform.position.x)
            {
                ChangeSightAnimation(LineOfSight.left);
                Sight = LineOfSight.left;
            }
            if (oldPos.y > transform.position.y)
            {
                ChangeSightAnimation(LineOfSight.down);
                Sight = LineOfSight.down;
            }
            if (oldPos.x < transform.position.x)
            {
                ChangeSightAnimation(LineOfSight.right);
                Sight = LineOfSight.right;
            }
            if (oldPos.y < transform.position.y)
            {
                ChangeSightAnimation(LineOfSight.up);
                Sight = LineOfSight.up;
            }
        }

         private void checkNextDeadZone()
        {
            //Controllare l'index della cella prossima, controllare la posizione della prossima cella e dopo instanziare la nuova deadzone
            //in quella cella 
           
            if (transform.position.x > PatrolPoint[index].position.x)
            { 
                ChangeSightAnimation(LineOfSight.left);  
                Sight = LineOfSight.left;
            }
            if (transform.position.y > PatrolPoint[index].position.y)
            {
                ChangeSightAnimation(LineOfSight.down);
                Sight = LineOfSight.down;
            }
            if (transform.position.x < PatrolPoint[index].position.x)
            {
                ChangeSightAnimation(LineOfSight.right);
                Sight = LineOfSight.right;
            }
            if (transform.position.y < PatrolPoint[index].position.y)
            {
                ChangeSightAnimation(LineOfSight.up);
                Sight = LineOfSight.up;
            }
          
        }
        
        public void ChangeSightAnimation(LineOfSight sight)
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

        public static void ChangeAimingDirection(ref LineOfSight posizione)
        {
            switch (posizione)
            {
                case LineOfSight.down:
                    posizione = LineOfSight.left;
                    break;
                case LineOfSight.left:
                    posizione = LineOfSight.up;
                    break;
                case LineOfSight.up:
                    posizione = LineOfSight.right;
                    break;
                case LineOfSight.right:
                    posizione = LineOfSight.down;
                    break;
            }
        }

        protected void ChangeSightAnimation(float xDir, float yDir)
        {
            _animator.SetFloat("x", xDir);
            _animator.SetFloat("y", yDir);
        }

        public Vector2 GetVectorDirection(LineOfSight aimingDirection)
        {
            Vector2 direction = new Vector2();

            switch (aimingDirection)
            {
                case LineOfSight.down:
                    direction = -transform.up;
                    break;
                case LineOfSight.up:
                    direction = transform.up;
                    break;
                case LineOfSight.right:
                    direction = transform.right;
                    break;
                case LineOfSight.left:
                    direction = -transform.right;
                    break;
            }

            return direction;
        }
     
        //Linear DZ
        public void InstanceNextDeadZone(LineOfSight nextStep)
        {
            Vector3 _tempPosition;
            Transform _TempDeadZone = Instantiate(Deadzone.transform, this.transform.position, Quaternion.identity, _ParentDeadZone);
            switch (nextStep)
            {
                case LineOfSight.down:
                    _tempPosition = _TempDeadZone.position;
                    //_TempEndPosition.y -= 0.35f;

                    _tempPosition.y -= transform.up.y;
                    _TempDeadZone.position = _tempPosition;
                    break;
                case LineOfSight.left:

                    _tempPosition = _TempDeadZone.position;
                    //_TempEndPosition.y -= 0.35f;
                    _tempPosition.x -= transform.right.x;
                    _TempDeadZone.position = _tempPosition;
                    break;
                case LineOfSight.up:
                    _tempPosition = _TempDeadZone.position;
                    _tempPosition.y += transform.up.y;
                    //_TempEndPosition.y -= 0.35f;

                    _TempDeadZone.position = _tempPosition;
                    break;
                case LineOfSight.right:
                    _tempPosition = _TempDeadZone.position;
                    //_TempEndPosition.y -= 0.35f;
                    _tempPosition.x += transform.right.x;
                    _TempDeadZone.position = _tempPosition;
                    break;
            }

        }

        //Ranged DZ
        public void InstanceDeadZone(LineOfSight parEnemyAimingWay)
        {
            // Transform _TempLaserDeadZone = Instantiate(LaserDeadzone.transform, new Vector3(this.transform.position.x + 4, this.transform.position.y), Quaternion.identity);
            LineOfSight _tSight;
            _tSight = parEnemyAimingWay;
            Vector2 tempEnd;
            tempEnd = GetVectorDirection(_tSight);
            CheckStoneRaycast(ref tempEnd,ref _tSight);
            ChangeAimingDirection(ref _tSight);
            Vector3 _TempEndPosition = new Vector3();
            for (int i = 1; i < 9; i++)
            {
                Transform _TempDeadZone = Instantiate(Deadzone.transform, this.transform.position, Quaternion.identity, _ParentDeadZone);
                _TempEndPosition = new Vector3();
                switch (_tSight)
                {
                    case LineOfSight.down:
                        _TempEndPosition = _TempDeadZone.position;
                      
                        _TempEndPosition.y -= i;
                        _TempDeadZone.position = _TempEndPosition;
                        break;
                    case LineOfSight.left:

                        _TempEndPosition = _TempDeadZone.position;
                      
                        _TempEndPosition.x -= i;
                        _TempDeadZone.position = _TempEndPosition;
                        break;
                    case LineOfSight.up:
                        _TempEndPosition = _TempDeadZone.position;
                        _TempEndPosition.y += i;
                    

                        _TempDeadZone.position = _TempEndPosition;
                        break;
                    case LineOfSight.right:
                        _TempEndPosition = _TempDeadZone.position;
                
                        _TempEndPosition.x += i;
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
                        break;
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
                    _DeadZone.Add(_TempDeadZone);
                }

            }


        }

        public void InstanceLaserDeadZone(LineOfSight parEnemyAimingWay)
        {

            for (int i = 1; i < 9; i++)
            {
                Transform _TempLaserDeadZone = Instantiate(LaserDeadzone.transform, this.transform.position, Quaternion.identity,_ParentLaserDeadZone);
                Vector3 _TempEndPosition = new Vector3();
                switch (parEnemyAimingWay)
                {
                    case LineOfSight.down:
                        _TempEndPosition = _TempLaserDeadZone.position;
                    

                        _TempEndPosition.y -= i;
                        _TempLaserDeadZone.position = _TempEndPosition;
                        _TempLaserDeadZone.GetChild(0).GetChild(0).Rotate(0, 0, 90);
                        break;
                    case LineOfSight.left:

                        _TempEndPosition = _TempLaserDeadZone.position;
                        
                        _TempEndPosition.x -= i;
                        _TempLaserDeadZone.position = _TempEndPosition;
                        break;
                    case LineOfSight.up:
                        _TempEndPosition = _TempLaserDeadZone.position;
                        _TempEndPosition.y += i;
                      
                        _TempLaserDeadZone.GetChild(0).GetChild(0).Rotate(0, 0, 90);
                        _TempLaserDeadZone.position = _TempEndPosition;
                        break;
                    case LineOfSight.right:
                        _TempEndPosition = _TempLaserDeadZone.position;
                      
                        _TempEndPosition.x += i;
                        _TempLaserDeadZone.position = _TempEndPosition;
                        break;
                }
                RaycastHit2D checkCollision;
                checkCollision = Physics2D.Linecast(_TempLaserDeadZone.position, _TempLaserDeadZone.position);
                if (checkCollision.transform != null)
                {
                    if (checkCollision.transform.tag == "Stone" || checkCollision.transform.tag == "Enemy")
                    {
                        Destroy(_TempLaserDeadZone.gameObject);
                        break;


                    }
                    /*
                    if (checkCollision.transform.tag == "Enemy")
                    {
                        if (checkCollision.transform.gameObject is RangedEnemy)
                        {
                        }

                    }
                    if (checkCollision.transform.tag == "Stone"/*||checkCollision.transform.tag != "Enemy"* /)
                    {
                        Destroy(_TempLaserDeadZone.gameObject);
                        break;

                    }

                    */
                    // }
                    else
                    {
                        _TempLaserDeadZone.GetComponent<BoxCollider2D>().enabled = true;
                    }
                }

                if (_TempLaserDeadZone != null)
                {

                    _TempLaserDeadZone.position = _TempEndPosition;
                    _LaserDeadZone.Add(_TempLaserDeadZone);
                }

            }
        }
      

        #region NeoEnemyTool
        [HideInInspector] public bool isLinear;
      
        [HideInInspector] public bool isClockwork;
        [HideInInspector] public bool isRanged;
        [HideInInspector] Vector2 correttoreCoordinate = new Vector2(0.5f,0.15f);
        [ExecuteInEditMode]
         void OnValidate()
        {
            if (EnemyType == ENEMY_TYPE.Melee_Horizontal||EnemyType== ENEMY_TYPE.Melee_Vertical)
            {
                isLinear = true;              
                isRanged = false;
                isClockwork = false;
                PointA.transform.position = startPos+correttoreCoordinate;
                PointB.transform.position = finalPos+correttoreCoordinate;
            }
            else
            {
                PointA = null;
                PointB = null;
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