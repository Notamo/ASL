using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public class Config
    {
        public class AssetBundle
        {
            public class Current : Config_Base_AssetBundle
            {

            }

            public class Android : Config_Base_AssetBundle
            {
                public Android()
                {
                    NodeType = NodeType.Android;
                }

                public new static string AssetSubFolder = "ASL/Resources";
                public new static string BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesAndroid";

                public new static string CompileUnityBundleDirectory()
                {
                    return "Assets/" + BundleSubFolder;
                }
                public new static string CompileUnityBundlePath(string filename)
                {
                    return CompileUnityBundleDirectory() + '/' + filename;
                }
                public new static string CompileAbsoluteBundleDirectory()
                {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
                    return Path.Combine(AbsoluteAssetRootFolder, BundleSubFolder);
#endif
                }
                public new static string CompileAbsoluteBundlePath(string filename)
                {
                    return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
                }
            }

            public class Hololens : Config_Base_AssetBundle
            {
                public Hololens()
                {
                    NodeType = NodeType.Hololens;
                }
            }

            public class Kinect : Config_Base_AssetBundle
            {
                public Kinect()
                {
                    NodeType = NodeType.Kinect;
                }
            }

            public class Oculus : Config_Base_AssetBundle
            {
                public Oculus()
                {
                    NodeType = NodeType.Oculus;
                }
            }

            public class Vive : Config_Base_AssetBundle
            {
                public Vive()
                {
                    NodeType = NodeType.Vive;
                }
            }

            public class PC : Config_Base_AssetBundle
            {
                public PC()
                {
                    NodeType = NodeType.PC;
                }
                
                public new static string AssetSubFolder = "ASL/Resources";
                public new static string BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesPC";

                public new static string CompileUnityBundleDirectory()
                {
                    return "Assets/" + BundleSubFolder;
                }
                public new static string CompileUnityBundlePath(string filename)
                {
                    return CompileUnityBundleDirectory() + '/' + filename;
                }
                public new static string CompileAbsoluteBundleDirectory()
                {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
                    return Path.Combine(AbsoluteAssetRootFolder, BundleSubFolder);
#endif
                }
                public new static string CompileAbsoluteBundlePath(string filename)
                {
                    return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
                }
            }

        }

        public static class Ports
        {
            private static int port = 21288;
            public static int Base
            {
                get
                {
                    return port;
                }
                set
                {
                    if(value < 64000 && value > 20000)
                    {
                        port = value;
                    }
                    else
                    {
                        Debug.Log("Invalid port chosen. Please select a port between 20000 and 64000");
                    }
                }
            }

            public static int Bundle
            {
                get
                {
                    return Base + 2;
                }
            }
            public static int RawRoomBundle
            {
                get
                {
                    return Base + 3;
                }
            }
            public static int RoomBundle
            {
                get
                {
                    return Base + 4;
                }
            }
        }
    }
}