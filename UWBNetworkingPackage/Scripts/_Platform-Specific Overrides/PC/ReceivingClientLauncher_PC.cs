using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
            //string roomBundleStorageDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory();
            //SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.RoomBundle, roomBundleStorageDirectory);
            RequestRoomBundles();
            //string rawRoomBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            //SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.RoomResourceBundle, rawRoomBundleDirectory);
            string assetBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.Bundle, assetBundleDirectory);

            // Generate the rooms
            UWB_Texturing.BundleHandler.InstantiateAllRooms();
        }

        public void RequestRoomBundles()
        {
            string roomBundleStorageDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory();
            SocketClient_PC.RequestFiles(ServerFinder.serverIP, Config.Ports.RoomBundle, roomBundleStorageDirectory);

            string[] tempRoomNames = Directory.GetFiles(roomBundleStorageDirectory);
            List<string> roomNameList = new List<string>();
            foreach(string tempRoomName in tempRoomNames)
            {
                if (!Path.HasExtension(tempRoomName))
                {
                    string[] pass = tempRoomName.Split('/');
                    string[] pass2 = pass[pass.Length - 1].Split('\\');
                    string roomName = pass2[pass2.Length - 1];
                    roomNameList.Add(roomName);
                }
            }

            // Make room directories
            string[] roomNames = roomNameList.ToArray();
            foreach (string roomName in roomNames)
            {
                string roomDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory(roomName);
                if (!Directory.Exists(roomDirectory))
                {
                    AbnormalDirectoryHandler.CreateDirectory(roomDirectory);
                }

                // Copy asset bundles to room directories
                if (File.Exists(Config.Current.Room.CompileAbsoluteAssetPath(roomName)){
                    Debug.Error("Room asset bundle exists! File not copied to room directory to avoid potentially overwriting data. Manually copy from " + roomBundleStorageDirectory + " if you would like to update the room.");
                }
                else
                {
                    string sourceFilePath = Config.Current.Room.CompileAbsoluteAssetPath(roomName);
                    string destinationFilePath = Config.Current.Room.CompileAbsoluteAssetPath(roomName, roomName);
                    File.Copy(sourceFilePath, destinationFilePath);
                }

                // Copy asset bundles to asset bundle directories
                if (File.Exists(Config.Current.AssetBundle.CompileAbsoluteAssetPath(roomName)))
                {
                    File.Delete(Config.Current.AssetBundle.CompileAbsoluteAssetPath(roomName));
                }
                else
                {
                    string sourceFilePath = Config.Current.Room.CompileAbsoluteAssetPath(roomName);
                    string destinationFilePath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(roomName);
                    File.Copy(sourceFilePath, destinationFilePath);
                }
            }
        }
#endif
    }
}