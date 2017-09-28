using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ASL.UI.Keyboard
{
    public class OwnershipTests : MonoBehaviour
    {
        private UWBNetworkingPackage.PerformanceTest testReference;

        protected void Start()
        {
            testReference = gameObject.AddComponent<UWBNetworkingPackage.PerformanceTest>();
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                testReference.CreateCubes();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                testReference.StressTest(testReference.MAX_COUNT);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                testReference.StressTestSimultaneous(testReference.COUNT);
            }
        }

    }
}