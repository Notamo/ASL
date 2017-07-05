using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWBNetworkingPackage
{
    public class MenuHandler : MonoBehaviour
    {
        private static PhotonView photonView;

        public void Start()
        {
            //photonView = PhotonView.Get(GameObject.Find("NetworkManager").GetComponent<PhotonView>());
            photonView = PhotonView.Find(1);
            if (photonView == null)
                Debug.Log("Wrong photon view id given");
            // ERROR TESTING
        }

        public static void ExportRawResources(int targetID)
        {
            string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();

            //BuildTarget[] targetPlatforms = new BuildTarget[4];
            //targetPlatforms[0] = BuildTarget.StandaloneWindows;
            //targetPlatforms[1] = BuildTarget.Android;

            //foreach (BuildTarget target in targetPlatforms)
            //{
            //    // Set destination directory
            //    switch (target)
            //    {
            //        case BuildTarget.StandaloneWindows:
            //            destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();
            //            break;
            //        case BuildTarget.Android:
            //            destinationDirectory = Config.AssetBundle.Android.CompileAbsoluteBundleDirectory();
            //            break;
            //    }

            //    UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(destinationDirectory, target);
            //}

            UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
            string bundleName = UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename();
            string bundlePath = Config.AssetBundle.PC.CompileAbsoluteBundlePath(Config.AssetBundle.PC.CompileFilename(bundleName)); // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
            int rawRoomBundlePort = Config.Ports.RawRoomBundle;
            Launcher.SendAssetBundle(targetID, bundlePath, rawRoomBundlePort);
            photonView.RPC("ReceiveRawRoomModelInfo", PhotonPlayer.Find(targetID), IPManager.CompileNetworkConfigString(rawRoomBundlePort));
        }

        public static void ExportRoom(int targetID)
        {
            string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();

            UWB_Texturing.BundleHandler.PackFinalRoomBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
            string bundleName = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename();
            string bundlePath = Config.AssetBundle.PC.CompileAbsoluteBundlePath(Config.AssetBundle.PC.CompileFilename(bundleName)); // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
            int finalRoomBundlePort = Config.Ports.RoomBundle;
            Launcher.SendAssetBundle(targetID, bundlePath, finalRoomBundlePort);
            photonView.RPC("ReceiveRoomModel", PhotonPlayer.Find(targetID), IPManager.CompileNetworkConfigString(finalRoomBundlePort));

        }
    }
}