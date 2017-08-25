using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public class MasterClientLauncher_PC : MasterClientLauncher
    {
#if !UNITY_WSA_10_0
        /// <summary>
        /// Attempts to connect to the specified Room Name on start, and adds MeshDisplay component
        /// for displaying the Room Mesh
        /// </summary>
        public override void Start()
        {
            // ERROR TESTING - REMOVE THIS METHOD - NOTHING SPECIAL HAPPENS IN IT UNIQUE TO THE MASTER CLIENT ANYMORE
            base.Start();
#if UNITY_EDITOR
            UWB_Texturing.Menu.InstantiateRoom();
#endif
            ServerFinder.ServerStart();
            SocketServer_PC.Start();
        }
#endif
    }
}