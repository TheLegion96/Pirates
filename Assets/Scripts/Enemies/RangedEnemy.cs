using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] protected GameObject Deadzone;
    [SerializeField] protected GameObject LaserDeadzone;
    [SerializeField] private TextMesh CountDownMesh;
    public bool DoThisOnlyWhenAllDeadZoneAreON = false;
    private int CDTick;


    //Start overrides the virtual Start function of the base class. 
    protected override void Start()
    {
        //Force Enemy Type.
        enemyTipe = EnemyType.Ranged;
        //Call the start function of our base class Enemy.
        base.Start();
        //Call custom code for this type.
        ChangeSightAnimation(EnemyAimingWay);
        InstanceLaserDeadZone(EnemyAimingWay);
        //InstanceDeadZone(EnemyAimingWay);
    }

    public override void CheckNextCell(out int xDir, out int yDir)
    {
        xDir = 0;
        yDir = 0;

        //Pattern RangedEnemy
        boxColliderEnemy.enabled = false;

        end = GetVectorDirection(EnemyAimingWay);

        tick++;
        CDTick = maxTicks - tick;
        int tickbeforechange = maxTicks - 1;
        if (CDTick == 0)
        {
            CDTick = maxTicks;
            //  CheckIfRaycastIs0();
        }
        CountDownMesh.text = CDTick.ToString();

        if (tick == maxTicks)
        {
            ChangeAimingDirection(ref EnemyAimingWay);
            end = GetVectorDirection(EnemyAimingWay);

            CheckStoneRaycast(ref end, ref EnemyAimingWay);

            ChangeSightAnimation(EnemyAimingWay);
            //InstanceLaserDeadZone(EnemyAimingWay);
            tick = 0;
        }
        if (tick != tickbeforechange)
        {
            //InstanceLaserDeadZone(EnemyAimingWay);
            //  CheckIfRaycastIs0();
        }




        //[Verza] Spostato nel Game Manager.
        //InstanceDeadZone();

        RaycastHit2D Bullet = Physics2D.Raycast(transform.position, end, 9f, blockingLayer);
        if (Bullet.collider == null)
        {
            // Check se sto beccando la porta.
            Bullet = Physics2D.Raycast(transform.position, end, 9f, exitLayer);
        }

        if (Bullet.transform != null && Bullet.transform.tag == "Player")
        {
            //Set the attack trigger of animator to trigger Enemy attack animation.
            animator.SetTrigger("Attack");

            //Stop the background music.
            SoundManager.instance.musicSource.Stop();

            //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
            SoundManager.instance.RandomizeSfx(attackSound1);

            Bullet.transform.GetComponent<Player>().ExecuteGameOver();
        }
        boxColliderEnemy.enabled = true;

    }
    private void CheckIfRaycastIs0()
    {

        /*
        int counterLaserDeadZone= 0;
        counterLaserDeadZone = (int)GameObject.FindGameObjectsWithTag("LaserDeadZone").Length;
        if(counterLaserDeadZone==0)
        {
            InstanceLaserDeadZone(EnemyAimingWay);
        }*/
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

    public void InstanceLaserDeadZone(LineOfSight parEnemyAimingWay)
    {

        for (int i = 1; i < 9; i++)
        {
            Transform _TempLaserDeadZone = Instantiate(LaserDeadzone.transform, this.transform.position, Quaternion.identity);
            Vector3 _TempEndPosition = new Vector3();
            switch (parEnemyAimingWay)
            {
                case LineOfSight.down:
                    _TempEndPosition = _TempLaserDeadZone.position;
                    //_TempEndPosition.y -= 0.35f;

                    _TempEndPosition.y -= i;
                    _TempLaserDeadZone.position = _TempEndPosition;
                    _TempLaserDeadZone.GetChild(0).GetChild(0).Rotate(0, 0, 90);
                    break;
                case LineOfSight.left:

                    _TempEndPosition = _TempLaserDeadZone.position;
                    //_TempEndPosition.y -= 0.35f;
                    _TempEndPosition.x -= i;
                    _TempLaserDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.up:
                    _TempEndPosition = _TempLaserDeadZone.position;
                    _TempEndPosition.y += i;
                    //_TempEndPosition.y -= 0.35f;
                    _TempLaserDeadZone.GetChild(0).GetChild(0).Rotate(0, 0, 90);
                    _TempLaserDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.right:
                    _TempEndPosition = _TempLaserDeadZone.position;
                    //_TempEndPosition.y -= 0.35f;
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


    public void InstanceDeadZone(LineOfSight parEnemyAimingWay)
    {
        // Transform _TempLaserDeadZone = Instantiate(LaserDeadzone.transform, new Vector3(this.transform.position.x + 4, this.transform.position.y), Quaternion.identity);

        Vector3 _TempEndPosition = new Vector3();
        for (int i = 1; i < 9; i++)
        {
            Transform _TempDeadZone = Instantiate(Deadzone.transform, this.transform.position, Quaternion.identity);
            _TempEndPosition = new Vector3();
            switch (parEnemyAimingWay)
            {
                case LineOfSight.down:
                    _TempEndPosition = _TempDeadZone.position;
                    //_TempEndPosition.y -= 0.35f;

                    _TempEndPosition.y -= i;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.left:

                    _TempEndPosition = _TempDeadZone.position;
                    //_TempEndPosition.y -= 0.35f;
                    _TempEndPosition.x -= i;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.up:
                    _TempEndPosition = _TempDeadZone.position;
                    _TempEndPosition.y += i;
                    //_TempEndPosition.y -= 0.35f;

                    _TempDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.right:
                    _TempEndPosition = _TempDeadZone.position;
                    //_TempEndPosition.y -= 0.35f;
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



}
