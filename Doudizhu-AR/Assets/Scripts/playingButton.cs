using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playingButton: MonoBehaviour {

	// Use this for initialization
	void Start () {
        Transform trans = gameObject.GetComponent<Transform>();
        trans.Translate(10000, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
