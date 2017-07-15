using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace UWBNetworkingPackage
{
    public class RoomTextureManager
    {

        public static class Messages
        {
            public static class Errors
            {
                public static string RawRoomBundleNotAvailable = "The raw room bundle (raw resources used to generate bundle) is unavailable. Please generate it through the appropriate means and ensure it is in the correct folder.";
                public static string RoomBundleNotAvailable = "The final room bundle is unavailable. Please generate it through the appropriate means and ensure it is in the correct folder.";
            }
        }

        public static void UpdateRawRoomBundle()
        {
            string bundleName = UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename();
            string ASLBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(bundleName);
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(bundleName);
            if (!File.Exists(ASLBundlePath))
            {
                if (File.Exists(GeneratedBundlePath))
                {
                    File.Copy(GeneratedBundlePath, ASLBundlePath);
                }
                else
                {
                    Debug.Log(Messages.Errors.RawRoomBundleNotAvailable);
                    return;
                }
            }
            else if (File.Exists(GeneratedBundlePath))
            {
                DateTime ASLDateTime = File.GetLastWriteTime(ASLBundlePath);
                DateTime RoomTextureDateTime = File.GetLastWriteTime(GeneratedBundlePath);

                if (DateTime.Compare(ASLDateTime, RoomTextureDateTime) < 0)
                {
                    File.Copy(GeneratedBundlePath, ASLBundlePath);
                }
            }
        }

        public static void UpdateRoomBundle()
        {
            string bundleName = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename();
            string ASLBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(bundleName);
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(bundleName);
            //string GeneratedBundlePath = Config.AssetBundle.PC.CompileAbsoluteAssetPath(Config.AssetBundle.PC.CompileFilename(bundleName));
            if (!File.Exists(ASLBundlePath))
            {
                if (File.Exists(GeneratedBundlePath))
                {
                    File.Copy(GeneratedBundlePath, ASLBundlePath);
                }
                else
                {
                    Debug.Log(Messages.Errors.RoomBundleNotAvailable);
                    return;
                }
            }
            else if (File.Exists(GeneratedBundlePath))
            {
                DateTime ASLDateTime = File.GetLastWriteTime(ASLBundlePath);
                DateTime RoomTextureDateTime = File.GetLastWriteTime(GeneratedBundlePath);

                if (DateTime.Compare(ASLDateTime, RoomTextureDateTime) < 0)
                {
                    File.Copy(GeneratedBundlePath, ASLBundlePath);
                }
            }
        }
    }
}