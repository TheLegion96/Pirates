using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicEnemy : Enemy
{

    [Header("Mimic only")]
    public Player PlayerInfo;

    //Start overrides the virtual Start function of the base class. 
    protected override void Start()
    {
        //Force Enemy Type.
        // enemyTipe = EnemyType.Mimic;
        //Call the start function of our base class Enemy.
        base.Start();
        //Call custom code for this type.
        //(...)
    }

  /*  public override void CheckNextCell(out int xDir, out int yDir)
    {
        xDir = 0;
        yDir = 0;

        /*
        if (PlayerInfo.new_Coordinate.x > PlayerInfo.old_Coordinate.x)
        {
            //MoveRight
        }
        else if (PlayerInfo.new_Coordinate.x < PlayerInfo.old_Coordinate.x)
        {
            //MoveLeft
        }
        else if (PlayerInfo.new_Coordinate.y > PlayerInfo.old_Coordinate.y)
        {
            //MoveDown
        }
        else if (PlayerInfo.new_Coordinate.y < PlayerInfo.old_Coordinate.y)
        {
            //MoveUp
        }

        */
    //}*/
}
