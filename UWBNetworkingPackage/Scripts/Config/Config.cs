using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public class Config
    {
        static Config()
        {
            Config_Base.SetFolders();
        }

        public static void Start()
        {
            // Triggers static constructor
        }

        public class AssetBundle
        {
            public class Current : Config_Base_AssetBundle
            {
                public new static string AssetSubFolder = "ASL/Resources";
                public new static string BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesPC";

                public Current()
                {
                    // Set the node type depending on what platform this is (PC, Android, etc.)
                    // if PC, NodeType = NodeType.PC;

                    AssetSubFolder = "ASL/Resources";
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesPC";
                            break;
                        case NodeType.Android:
                            BundleSubFolder = AssetSubFolder + "/StreamingAssets/AssetBundlesAndroid";
                            break;
                        case NodeType.Hololens:
                            break;
                        case NodeType.Kinect:
                            break;
                        case NodeType.Oculus:
                            break;
                        case NodeType.Vive:
                            break;
                        default:
                            break;
                    }
                }

                public new static string CompileUnityBundleDirectory()
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.CompileUnityBundleDirectory();
                        case NodeType.Android:
                            return Android.CompileUnityBundleDirectory();
                        case NodeType.Hololens:
                            return Hololens.CompileUnityBundleDirectory();
                        case NodeType.Kinect:
                            return Kinect.CompileUnityBundleDirectory();
                        case NodeType.Oculus:
                            return Oculus.CompileUnityBundleDirectory();
                        case NodeType.Vive:
                            return Vive.CompileUnityBundleDirectory();
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public new static string CompileUnityBundlePath(string filename)
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.CompileUnityBundlePath(filename);
                        case NodeType.Android:
                            return Android.CompileUnityBundlePath(filename);
                        case NodeType.Hololens:
                            return Hololens.CompileUnityBundlePath(filename);
                        case NodeType.Kinect:
                            return Kinect.CompileUnityBundlePath(filename);
                        case NodeType.Oculus:
                            return Oculus.CompileUnityBundlePath(filename);
                        case NodeType.Vive:
                            return Vive.CompileUnityBundlePath(filename);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public new static string CompileAbsoluteBundleDirectory()
                {
                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.CompileAbsoluteBundleDirectory();
                        case NodeType.Android:
                            return Android.CompileAbsoluteBundleDirectory();
                        case NodeType.Hololens:
                            return Hololens.CompileAbsoluteBundleDirectory();
                        case NodeType.Kinect:
                            return Kinect.CompileAbsoluteBundleDirectory();
                        case NodeType.Oculus:
                            return Oculus.CompileAbsoluteBundleDirectory();
                        case NodeType.Vive:
                            return Vive.CompileAbsoluteBundleDirectory();
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }

                public new static string CompileAbsoluteBundlePath(string filename)
                {

                    switch (NodeType)
                    {
                        case NodeType.PC:
                            return PC.CompileAbsoluteBundlePath(filename);
                        case NodeType.Android:
                            return Android.CompileAbsoluteBundlePath(filename);
                        case NodeType.Hololens:
                            return Hololens.CompileAbsoluteBundlePath(filename);
                        case NodeType.Kinect:
                            return Kinect.CompileAbsoluteBundlePath(filename);
                        case NodeType.Oculus:
                            return Oculus.CompileAbsoluteBundlePath(filename);
                        case NodeType.Vive:
                            return Vive.CompileAbsoluteBundlePath(filename);
                        default:
                            throw new System.Exception("Unrecognized platform.");
                    }
                }
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
                    return BundleSubFolder;
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
                    return BundleSubFolder;
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

        public class Room : Config_Base
        {

        }

        public static class Ports
        {
            public enum Types
            {
                Bundle,
                Bundle_ClientToServer,
                RoomResourceBundle,
                RoomResourceBundle_ClientToServer,
                RoomBundle,
                RoomBundle_ClientToServer,
                ClientServerConnection,
                FindServer
            }

            public static int GetPort(Types portType)
            {
                switch (portType)
                {
                    case Types.Bundle:
                        return Bundle;
                    case Types.Bundle_ClientToServer:
                        return Bundle_ClientToServer;
                    case Types.RoomResourceBundle:
                        return RoomResourceBundle;
                    case Types.RoomResourceBundle_ClientToServer:
                        return RoomResourceBundle_ClientToServer;
                    case Types.RoomBundle:
                        return RoomBundle;
                    case Types.RoomBundle_ClientToServer:
                        return RoomBundle_ClientToServer;
                    case Types.ClientServerConnection:
                        return ClientServerConnection;
                    case Types.FindServer:
                        return FindServer;
                }

                return Base;
            }

            private static int port = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().Port;
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
            public static int FindServer
            {
                get
                {
                    return Base + 1;
                }
            }
            public static int ClientServerConnection
            {
                get
                {
                    return Base + 2;
                }
            }
            public static int Bundle
            {
                get
                {
                    return Base + 3;
                }
            }
            public static int Bundle_ClientToServer
            {
                get
                {
                    return Base + 4;
                }
            }
            public static int RoomResourceBundle
            {
                get
                {
                    return Base + 5;
                }
            }
            public static int RoomResourceBundle_ClientToServer
            {
                get
                {
                    return Base + 6;
                }
            }
            public static int RoomBundle
            {
                get
                {
                    return Base + 7;
                }
            }
            public static int RoomBundle_ClientToServer
            {
                get
                {
                    return Base + 8;
                }
            }
        }
    }
}