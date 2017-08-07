using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public class ReceivingClientLauncher_Hololens : ReceivingClientLauncher
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0
        // Insert overrides here
        public override void Start()
        {
            base.Start();
            ServerFinder_Hololens.FindServerAsync();
            SocketServer_Hololens.StartAsync();
        }
#endif
    }
}