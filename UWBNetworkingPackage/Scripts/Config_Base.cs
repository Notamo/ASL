using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public enum NodeType
    {
        MasterClient,
        Android,
        Hololens,
        Kinect,
        Oculus,
        Vive,
        PC
    };

    public class Config_Base
    {
        private NodeType nodeType = NodeType.MasterClient;
        public NodeType NodeType
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
        
        public static string absoluteAssetRootFolder = Application.dataPath;
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

        public static string CompileUnityAssetDirectory()
        {
            return "Assets/" + AssetSubFolder;
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
        public static string CompileResourcesLoadPath(string assetNameWithoutExtension)
        {
            return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
            //return ResourcesSubFolder + '/' + assetNameWithoutExtension;
        }

        public static string CompileUnityBundleDirectory()
        {
            return "Assets/" + BundleSubFolder;
        }
        public static string CompileUnityBundlePath(string filename)
        {
            return CompileUnityBundleDirectory() + '/' + filename;
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