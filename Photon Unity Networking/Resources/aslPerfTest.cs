using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aslPerfTest : MonoBehaviour {

    private int COUNT = 250;

    private System.Diagnostics.Stopwatch stopwatch;

    private bool testing = false;

	// Use this for initialization
	void Start () {

        // Begin timekeeping
        //InvokeRepeating("addMS", 2.0f, 0.01f);

        stopwatch = new System.Diagnostics.Stopwatch();

	}

	// Update is called once per frame
	void Update () {
        
        if (!testing && Input.GetKeyDown(KeyCode.T))
        {
            testing = true;

            this.runTest();
        }
	}

    void runTest()
    {
        Vector3 position;
        
        // Using Stopwatch
        stopwatch.Reset();
        stopwatch.Start();

        // Create a number of aslCubes
        for (int i = 0; i < COUNT; ++i)
        {
            position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 5.0f));
            PhotonNetwork.Instantiate("aslCube", position, Quaternion.identity, 0);
        }
        stopwatch.Stop();

        Debug.LogWarning("aslPerfTest: Total time to create " + COUNT + " aslBase instances: " + stopwatch.ElapsedMilliseconds);

        stopwatch.Reset();
        stopwatch.Start();
        for (int i = 0; i < COUNT; ++i)
        {
            position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 5.0f));
            PhotonNetwork.Instantiate("Cube", position, Quaternion.identity, 0);
        }
        stopwatch.Stop();

        Debug.LogWarning("aslPerfTest: Total time to create " + COUNT + " Cube instances: " + stopwatch.ElapsedMilliseconds);
        
        testing = false;
    }

}
