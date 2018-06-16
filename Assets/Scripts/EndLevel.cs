using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameManager.instance.GoToNextScene(5, 0);
    }
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
