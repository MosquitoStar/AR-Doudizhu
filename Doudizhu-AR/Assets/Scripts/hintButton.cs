using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;//导入UI包
using UnityEngine.SceneManagement;

public class hintButton : MonoBehaviour {

	public static bool isdown;
	// Use this for initialization
	void Start () {
        isdown = false;

        Button button = gameObject.GetComponent<Button>() as Button;
        button.onClick.AddListener(myClick);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void myClick()
    {
        isdown = true;
    }

}
