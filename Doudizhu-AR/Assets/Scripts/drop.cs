using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drop : MonoBehaviour {
    public Rigidbody r;
    private string tag;
	// Use this for initialization
	void Start () {
        r = GetComponent<Rigidbody>();
        r.useGravity = false;
        tag = this.GetComponent<Collider>().tag;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.tag == tag)
                {
                    r.useGravity=true;
                    transform.Translate(new Vector3(0.0f, 0.0f, -0.1f));
                    r.AddForce(0.0f, 50.0f, 100.0f);
                }
            }
        }
	}
}
