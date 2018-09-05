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

        public LayerMask iceMask;
        Vector3 touchPosWorld;
        public static PlayerMobileMovement instance;
        //Change me to change the touch phase used.
        TouchPhase touchPhase = TouchPhase.Ended;
        public bool playerOnIce;
        Vector2 correttoreCoordinate = new Vector2(0.5f, 0.15f);

        // Use this for initialization
        void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            playerOnIce = false;
        }
        // Update is called once per frame
        bool touched = false;
        void Update()
        {

            Debug.Log(PlayerMobileMovement.instance.gameObject.transform.position);

            if (NeoGameManager.instance.state == NeoGameManager.State.Wait)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //We transform the touch position into word space from screen space and store it.
                    touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

                    //We now raycast with this information. If we have hit something we can process it.
                    RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

                    //  Debug.Log(hitInformation.collider.gameObject.name + " , " + hitInformation.collider.gameObject.tag);
                    if (hitInformation.collider != null)
                    {
                        if (hitInformation.collider.tag == "Player")
                        {
                            //We should have hit something with a 2D Physics collider!
                            GameObject touchedObject = hitInformation.transform.gameObject;
                            //touchedObject should be the object someone touched.

                            int amount = 1;

                            Vector3 dir = (Vector3)GetDirection(touchedObject.name);
                            Vector3 position = touchedObject.transform.position + -Vector3.forward;
                            position -= dir;

                            while (Physics2D.Raycast(position + dir * amount, Vector2.zero, 50, iceMask))
                            {
                                amount++;
                            }
                            
                            GiveMovement(dir, amount);
                            NeoGameManager.instance.giveDirection(direction);
                            NeoGameManager.instance.SetState(NeoGameManager.State.MovePlayer);
                        }
                    }
                }
            }
        }

        Vector2 direction;
        public float moveTime = 0.5f;

        public Vector2 GetDirection(string direction)
        {
            switch (direction)
            {
                case "UP":
                    return Vector2.up;
                case "DOWN":
                    return Vector2.down;
                case "LEFT":
                    return Vector2.left;
                case "RIGHT":
                    return Vector2.right;
            }

            return Vector2.zero;
        }

        public void GiveMovement(Vector2 direction, int amount)
        {
            IsMoving();

            this.gameObject.transform.DOMove(transform.position + (Vector3)direction * amount, moveTime * amount).
                OnStart(DisableButton).OnComplete(RoundAndEnable);
            
            IsMoving();
        }

        public void DisableButton()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public int counter = 0;

        public void RoundAndEnable()
        {
            /*
             Vector3 RoundedPosition;
             RoundedPosition = Vector3.zero;
             if (direction == Vector2.up)
             {
                 RoundedPosition.x = Mathf.Round(transform.position.x)-0.5f;
                 RoundedPosition.y = Mathf.Round(transform.position.y)+0.15f;
             }
             if (direction == -Vector2.up)
             {
                 RoundedPosition.x = Mathf.Round(transform.position.x) - 0.5f;
                 RoundedPosition.y = Mathf.Round(transform.position.y) +0.15f;
             }
             if (direction == Vector2.right)
             {
                 RoundedPosition.x = Mathf.Round(transform.position.x)+0.5f;
                 RoundedPosition.y = Mathf.Round(transform.position.y) +0.15f;
             }
             if (direction == -Vector2.right)
             {
                 RoundedPosition.x = Mathf.Round(transform.position.x) -0.5f;
                 RoundedPosition.y = Mathf.Round(transform.position.y) + 0.15f;
             }
             if(counter==1)
             {
                 if(direction==Vector2.right)
                 {
                     RoundedPosition.x -= 1;
                 }
                 if (direction == -Vector2.right)
                 {
                     RoundedPosition.x += 1;
                 }
                 counter = 0;
             }
             else { counter++; }

             this.gameObject.transform.position = RoundedPosition;
             */
            EnableButton();
        }
        public void EnableButton()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Ice")
            {
                playerOnIce = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Ice")
            {
                playerOnIce = false;
            }
        }
        public bool isMoving = false;
        public void IsMoving()
        {
            isMoving = !isMoving;
        }
    }
}