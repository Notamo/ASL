using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public class ReceivingClientLauncher_PC : ReceivingClientLauncher
    {
#if !UNITY_WSA_10_0
        // Insert PC-specific code here
        public override void Start()
        {
            base.Start();
            ServerFinder.FindServer();
            SocketServer_PC.Start(); // For sending files to other non-master clients
        }
#endif
    }
}