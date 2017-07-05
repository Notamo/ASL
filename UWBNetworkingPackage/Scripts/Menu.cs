using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWBNetworkingPackage {
    public static class Menu {
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
    }
}
