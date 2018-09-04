using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
[HideMonoScript]
public class TestLevelEditor : MonoBehaviour {

   
	// Use this for initialization
	void Start () {
      
            transform.DetachChildren();
        DestroyImmediate(this.gameObject);
	}
	

}
