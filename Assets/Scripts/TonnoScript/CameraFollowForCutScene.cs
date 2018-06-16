using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowForCutScene : MonoBehaviour {
    public GameObject[] ImmaginiDellaScena;

    // Use this for initialization
    void Start() {
		transform.position = new Vector3 (ImmaginiDellaScena [0].transform.position.x, ImmaginiDellaScena [0].transform.position.y, -1);
            }
    int i = 0;
    int iNext;
    float newX, newY;
    private float moveSpeed=1;


    // Update is called once per frame
    void Update () {
       if (Input.GetKeyDown(KeyCode.Space))
        {

            StartCoroutine(PassaggioCamera());
        }
    }
    private float transitionDuration=1f;
    private IEnumerator PassaggioCamera()
    {
        if (i == ImmaginiDellaScena.Length-1)
        {
            i = 0;
        }
        else
        {
            i++;
        }
        float t = 0.0f;
        Vector2 StartingPosition = transform.position;
        while(t<1.0f)
        {
            t += Time.deltaTime * (Time.timeScale / transitionDuration);
            transform.position = Vector2.Lerp(StartingPosition, ImmaginiDellaScena[i].transform.position, t);

                yield return 0;

        }
       
       
      
    }

}
