using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditorTool : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.DetachChildren();
        Destroy(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        
	}
}
