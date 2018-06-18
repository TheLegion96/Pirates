using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoCompleted
{
    public class PlayerMobileMovement : SerializedMonoBehaviour
    {


        Vector3 touchPosWorld;

        //Change me to change the touch phase used.
        TouchPhase touchPhase = TouchPhase.Ended;

        // Use this for initialization
        void Start()
        {

        }
        // Update is called once per frame
        bool touched = false;
        void Update()
        {
            if (NeoGameManager.instance.state == NeoGameManager.State.Wait)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == touchPhase)
                {
                    //We transform the touch position into word space from screen space and store it.
                    touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                    Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

                    //We now raycast with this information. If we have hit something we can process it.
                    RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

                    if (hitInformation.collider != null)
                    {

                        //We should have hit something with a 2D Physics collider!
                        GameObject touchedObject = hitInformation.transform.gameObject;
                        //touchedObject should be the object someone touched.
                        GiveMovement(touchedObject);
                    }
                NeoGameManager.instance.SetState(NeoGameManager.State.MovePlayer);
                }
            }
        }

        public float moveTime = 0.05f;
        public void GiveMovement(GameObject _direction)
        {

            switch (_direction.name)
            {
                case "Player": //Do Pause
                    break;
                case "Player_MoveUp"://Do Up
                    this.gameObject.transform.DOMove(transform.position + transform.up, moveTime).OnStart(DisableButton).OnComplete(EnableButton);
                    break;
                case "Player_MoveDown": //Do Down
                    this.gameObject.transform.DOMove(transform.position - transform.up, moveTime).OnStart(DisableButton).OnComplete(EnableButton);
                    break;
                case "Player_MoveRight"://Do Right
                    this.gameObject.transform.DOMove(transform.position + transform.right, moveTime).OnStart(DisableButton).OnComplete(EnableButton);
                    break;
                case "Player_MoveLeft"://Do Left
                    this.gameObject.transform.DOMove(transform.position - transform.right, moveTime).OnStart(DisableButton).OnComplete(EnableButton);
                    break;
                default: break;
            }

        }

        public void DisableButton()
        {

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        public void EnableButton()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}