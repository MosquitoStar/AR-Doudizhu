using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;//导入UI包
using UnityEngine.SceneManagement;

public class notHangUpButton : MonoBehaviour {

	public static bool isdown;
   
	// Use this for initialization
	void Start () {
        isdown = false;

        Button button = gameObject.GetComponent<Button>() as Button;
        button.onClick.AddListener(myClick);
        // Transform buttonT = gameObject.GetComponent<Transform>();
        // buttonT.Translate(1000, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void myClick()
    {
        isdown = true;
    }

   

}

