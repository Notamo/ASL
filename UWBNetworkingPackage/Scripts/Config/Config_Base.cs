using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public enum NodeType
    {
        Android,
        Hololens,
        Kinect,
        Oculus,
        Vive,
        PC
    };

    public class Config_Base
    {
        private static NodeType nodeType = NodeType.PC;
        public static NodeType NodeType
        {
            get
            {
                return nodeType;
            }
            set
            {
                nodeType = value;
            }
        }

        //public static string absoluteAssetRootFolder = Directory.GetCurrentDirectory();//Application.persistentDataPath;
        public static string absoluteAssetRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
        public static string AbsoluteAssetRootFolder
        {
            get
            {
                return absoluteAssetRootFolder;
            }
            set
            {
#if UNITY_WSA_10_0
                absoluteAssetRootFolder = Application.persistentDataPath;
#else
                absoluteAssetRootFolder = Application.dataPath;
                // Put in logic for all node types
#endif
            }
        }

        public static string AssetSubFolder = "ASL/Resources";
        public static string BundleSubFolder = AssetSubFolder + "/StreamingAssets";
        public static string RoomResourceSubFolder = AssetSubFolder + "/Rooms";

        public static string CompileUnityRoomDirectory()
        {
            return RoomResourceSubFolder;
        }
        public static string CompileUnityRoomDirectory(string roomName)
        {
            return RoomResourceSubFolder + '/' + roomName;
        }
        public static string CompileUnityRoomPath(string filename, string roomName)
        {
            return CompileUnityRoomDirectory(roomName) + '/' + filename;
        }
        public static string CompileAbsoluteRoomDirectory()
        {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
            return Path.Combine(AbsoluteAssetRootFolder, RoomResourceSubFolder);
#endif
        }
        public static string CompileAbsoluteRoomDirectory(string roomName)
        {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
            return Path.Combine(AbsoluteAssetRootFolder, Path.Combine(RoomResourceSubFolder, roomName));
#endif
        }
        public static string CompileAbsoluteRoomPath(string filename, string roomName)
        {
            return Path.Combine(CompileAbsoluteRoomDirectory(roomName), filename);
        }
        public static string CompileUnityAssetDirectory()
        {
            //return "Assets/" + AssetSubFolder;
            return AssetSubFolder;
        }
        public static string CompileUnityAssetPath(string filename)
        {
            return CompileUnityAssetDirectory() + '/' + filename;
        }
        public static string CompileAbsoluteAssetDirectory()
        {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
            return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
#endif
        }
        public static string CompileAbsoluteAssetPath(string filename)
        {
            return Path.Combine(CompileAbsoluteAssetDirectory(), filename);
        }
        public static string CompileRoomResourcesLoadPath(string assetNameWithoutExtension, string roomName)
        {
            return RoomResourceSubFolder.Substring(RoomResourceSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        }
        public static string CompileResourcesLoadPath(string assetNameWithoutExtension)
        {
            return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
            //return ResourcesSubFolder + '/' + assetNameWithoutExtension;
        }
        public static string CompileResourcesLoadPath(string assetSubDirectory, string assetNameWithoutExtension)
        {
            return assetSubDirectory.Substring(assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        }

        public static string CompileUnityBundleDirectory()
        {
            return "Assets/" + BundleSubFolder;
            //return BundleSubFolder;
        }
        public static string CompileUnityBundlePath(string filename)
        {
            //return CompileUnityBundleDirectory() + '/' + filename;
            return Path.Combine(CompileUnityBundleDirectory(), filename);
        }
        public static string CompileAbsoluteBundleDirectory()
        {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
            return Path.Combine(AbsoluteAssetRootFolder, BundleSubFolder);
#endif
        }
        public static string CompileAbsoluteBundlePath(string filename)
        {
            return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
        }

    }
}