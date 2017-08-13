using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWBNetworkingPackage {
    public static class Menu {
#if UNITY_EDITOR && !UNITY_WSA_10_0
        [UnityEditor.MenuItem("ASL/Room Texture/Pack Raw Resources (Offline)", false, 0)]
        public static void PackRawResourcesBundle()
        {
            MenuHandler.PackRawResourcesBundle();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Pack Room Bundle (Offline)", false, 0)]
        public static void PackRoomBundle()
        {
            MenuHandler.PackRoomBundle();
        }

        /// <summary>
        /// Exports to master client.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Export Room Resources", false, 0)]
        public static void ExportRawResources() {
            MenuHandler.ExportRawResources(PhotonNetwork.masterClient.ID);
        }

        /// <summary>
        /// Processes room resources to generate final room, room prefab, and appropraite bundle.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Generate Room", false, 0)]
        public static void ProcessRoomResources()
        {
            MenuHandler.ProcessRoomResources();
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Export Room", false, 0)]
        public static void ExportFinalRoom() {
            MenuHandler.ExportRoom(PhotonNetwork.masterClient.ID);
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Process All Rooms")]
        public static void ProcessAllRooms()
        {
            string[] roomNames = new string[2];
            roomNames[0] = "Room";
            roomNames[1] = "Room2";
            for(int i = 0; i < 2; i++)
            {
                string roomName = roomNames[i];
                UWB_Texturing.Config.RoomObject.GameObjectName = roomName;

                string matrixArrayFilepath = Config.AssetBundle.Current.CompileAbsoluteRoomPath(UWB_Texturing.Config.MatrixArray.CompileFilename(), roomName);
                string materialsDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
                string meshesDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
                string texturesDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
                string imagesDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
                UWB_Texturing.BundleHandler.CreateRoomResources(roomName, matrixArrayFilepath, materialsDirectory, meshesDirectory, texturesDirectory, imagesDirectory);

                string rawRoomBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
                string customMatricesDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
                string customOrientationDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
                string customMeshesDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
                string textureImagesDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
                UWB_Texturing.BundleHandler.InstantiateRoomFromResources(roomName, rawRoomBundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, textureImagesDestinationDirectory, matrixArrayFilepath);
            }
        }
#endif
    }
}
