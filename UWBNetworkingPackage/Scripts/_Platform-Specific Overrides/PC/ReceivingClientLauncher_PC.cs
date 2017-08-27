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

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            
            //string roomBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            //SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.RoomBundle, roomBundleDirectory);
            //string rawRoomBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            //SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.RoomResourceBundle, rawRoomBundleDirectory);
            string assetBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.Bundle, assetBundleDirectory);

            string[] roomNames = RoomManager.GetAllRoomNames();
            foreach(string roomName in roomNames)
            {
                RoomManager.UpdateRawRoomBundle(roomName);
                RoomManager.UpdateRoomBundle(roomName);
            }

            // Generate the room
            foreach(string roomName in roomNames)
            {
                UWB_Texturing.BundleHandler.InstantiateAllRooms();
            }
        }
#endif
    }
}