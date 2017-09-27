using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class aslPerfTest : Photon.PunBehaviour {

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

        if (!testing)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                testing = true;

                this.runTest1();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                testing = true;

                this.runTest2();

            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                testing = true;

                this.runTest3();

            }
        }
	}

    void runTest1()
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
                
        PhotonNetwork.DestroyAll();
        
        testing = false;
    }

    // Should only be run when there are not multiple clients connected.
    void runTest2()
    {
        Vector3 position;
        long ops = 0;
        string func;
        int index;

        // Create a lot of aslBase cubes.
        for (int i = 0; i < COUNT * 2; ++i)
        {
            position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 5.0f));
            PhotonNetwork.Instantiate("aslCube", position, Quaternion.identity, 0);
        }

        var photonViews = Photon.PunBehaviour.FindObjectsOfType<PhotonView>();

        Debug.LogWarning("aslPerfTest: Created " + (photonViews.Length - 1) + " aslCubes.");
        Debug.LogWarning("aslPerfTest: Beginning single-client ownership testing.");

        stopwatch.Reset();
        stopwatch.Start();

        // Run for a random length of time, doing random ops on objects
        while (true)
        {
            // Count ops performed
            ++ops;

            // Randomly choose an object to either grab or release.
            func = (Random.Range(0, 1.0f) >= 0.5f) ? "grab" : "release";
            
            // Generate new position
            position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 5.0f));

            // Select index to operate on, and call randomly-selected func on that PhotonView
            index = (int)Random.Range(1, photonViews.Length-1);
            photonViews[index].RPC(func, PhotonTargets.AllBuffered);

            if(photonViews[index].ownerId == PhotonNetwork.player.ID)
            {
                photonViews[index].transform.position = position;
            }
            
            // End after random period
            if (Random.Range(0, 1.0f) <= 0.001)
            {
                break;
            }           
        }

        stopwatch.Stop();

        Debug.LogWarning("aslPerfTest: Finished.  Operations: " + ops + "; elapsed (ms): " + stopwatch.ElapsedMilliseconds);

        PhotonNetwork.DestroyAll();

        testing = false;
        
    }


    // Manually run this script on each client; needs some way to start 
    void runTest3()
    {
        Vector3 position;
        
        // Create a lot of aslBase cubes.
        for (int i = 0; i < COUNT/2; ++i)
        {
            position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 5.0f));
            PhotonNetwork.Instantiate("aslCube", position, Quaternion.identity, 0);
        }
                
        Debug.LogWarning("aslPerfTest: Created " + COUNT/2 + " aslCubes.");
        Debug.LogWarning("aslPerfTest: Beginning multi-client ownership testing.");

        stopwatch.Reset();
        stopwatch.Start();

        // Set up hashtable of custom properties; for synch between MasterClient and other clients
        Hashtable props = new Hashtable();
        props.Add("start", true);

        // Notify other clients to begin testing.
        PhotonNetwork.room.SetCustomProperties(props);

        // Run the actual ownership test portion
        StartCoroutine(runTest3B());

    }

    // Use Custom Room Properties to synch various test clients
    public override void OnPhotonCustomRoomPropertiesChanged( Hashtable propertiesThatChanged)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            // Relies on early eject from compound conditionals
            if (propertiesThatChanged.ContainsKey("start") && (bool)propertiesThatChanged["start"])
            {
                Debug.LogWarning("aslPerfTest: Starting multi-client ownership test as client.");
                // Set up testing state
                testing = true;
                stopwatch.Reset();
                stopwatch.Start();
                StartCoroutine(runTest3B());
            }
        }
    }

    // Must use coroutine for non-blocking sleeps ;_;
    private IEnumerator runTest3B()
    {
        Vector3 position;
        int index;

        var photonViews = Photon.PunBehaviour.FindObjectsOfType<PhotonView>();

        for (int i = 0; i < COUNT / 10; i++)
        {
            for (int j = 0; j < 100; ++j)
            {
                // Find a random index within the chunk i ~ i + COUNT/10 (of COUNT/2 total objects).
                // A sloppy way to ensure index hits.
                index = (int)Random.Range(i * 5, (i + 1) * (COUNT/2 / (COUNT/10) - 1));

                // Randomly choose an object to either grab or release.
                if (Random.Range(0, 1.0f) >= 0.5f)
                {
                    photonViews[index].RPC("grabWithDelay", PhotonTargets.AllBuffered, 1000);

                    if (photonViews[index].ownerId == PhotonNetwork.player.ID)
                    {
                        // Generate new position
                        position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 5.0f));
                        photonViews[index].transform.position = position;
                    }

                }
                else
                {
                    photonViews[index].RPC("release", PhotonTargets.AllBuffered);
                }

                yield return new WaitForSeconds(0.03f);
            }
        }

        stopwatch.Stop();

        Debug.LogWarning("aslPerfTest: Finished multi-client ownership testing.  Operations: " + COUNT * 10 + "; elapsed (ms): " + stopwatch.ElapsedMilliseconds);

        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        testing = false;

        yield break;
    }
}
