using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToInst : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            PhotonNetwork.Instantiate("aslCube", new Vector3(0, 0, 0), new Quaternion(), 0);
        }
		
	}
}
