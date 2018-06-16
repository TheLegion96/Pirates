using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneta : MonoBehaviour {

    public Sprite theGoldenOne;
    public int levelToCheck;

	// Use this for initialization
	void Start () {
        if (MenuManager.GetToken(levelToCheck))
        {
            GetComponent<SpriteRenderer>().sprite = theGoldenOne;
        }
	}
	
	//// Update is called once per frame
	//void Update () {
	//	
	//}
}
