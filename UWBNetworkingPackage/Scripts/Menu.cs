using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWBNetworkingPackage {
    public static class Menu {
#if UNITY_EDITOR
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
        [UnityEditor.MenuItem("ASL/Room Texture/Export Raw Resources", false, 0)]
        public static void ExportRawResources() {
            MenuHandler.ExportRawResources(PhotonNetwork.masterClient.ID);
        }

        [UnityEditor.MenuItem("ASL/Room Texture/Export Room", false, 0)]
        public static void ExportFinalRoom() {
            MenuHandler.ExportRoom(PhotonNetwork.masterClient.ID);
        }
#endif
    }
}
