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
        #region Fields/Properties

        public static class Messages
        {
            public static string PlatformNotFound = "Platform not found. Please reference NodeType enum in Config_Base file.";
        }

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

        #endregion

        public static void Start(NodeType platform)
        {
            NodeType = platform;
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
                    if (value < 64000 && value > 20000)
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


        //        static Config_Base()
        //        {
        //            UWB_Texturing.Config_Base.AbsoluteAssetRootFolder = AbsoluteAssetRootFolder;
        //            UWB_Texturing.Config_Base.AssetSubFolder = RoomResourceSubFolder;
        //        }

        //        public static void SetFolders()
        //        {
        //            // Triggers static constructor
        //        }

        //        #region Fields/Properties


        //        //public static string absoluteAssetRootFolder = Directory.GetCurrentDirectory();//Application.persistentDataPath;
        //        private static string absoluteAssetRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
        //        public static string AbsoluteAssetRootFolder
        //        {
        //            get
        //            {
        //                return absoluteAssetRootFolder;
        //            }
        //            set
        //            {
        //#if UNITY_WSA_10_0
        //                absoluteAssetRootFolder = Application.persistentDataPath;
        //#else
        //                //absoluteAssetRootFolder = Application.dataPath;
        //                absoluteAssetRootFolder = value;
        //                // Put in logic for all node types
        //#endif
        //                UWB_Texturing.Config_Base.AbsoluteAssetRootFolder = absoluteAssetRootFolder;
        //            }
        //        }

        //        private static string assetSubFolder = "ASL/Resources";
        //        public static string AssetSubFolder
        //        {
        //            get
        //            {
        //                return assetSubFolder;
        //            }
        //            set
        //            {
        //#if UNITY_WSA_10_0
        //#else
        //                assetSubFolder = value;
        //#endif
        //            }
        //        }

        //        private static string bundleSubFolder = AssetSubFolder + "/StreamingAssets";
        //        public static string BundleSubFolder
        //        {
        //            get
        //            {
        //                return bundleSubFolder;
        //            }
        //            set
        //            {
        //#if UNITY_WSA_10_0
        //#else
        //                bundleSubFolder = value;
        //#endif
        //            }
        //        }

        //        private static string roomResourceSubFolder = AssetSubFolder + "/Rooms";
        //        public static string RoomResourceSubFolder
        //        {
        //            get
        //            {
        //                return roomResourceSubFolder;
        //            }
        //            set
        //            {
        //#if UNITY_WSA_10_0
        //#else
        //                roomResourceSubFolder = value;
        //                UWB_Texturing.Config_Base.AssetSubFolder = roomResourceSubFolder;
        //#endif
        //            }
        //        }

        //        #endregion

        //        #region Methods

        //        public static string CompileUnityRoomDirectory()
        //        {
        //            return RoomResourceSubFolder;
        //        }
        //        public static string CompileUnityRoomDirectory(string roomName)
        //        {
        //            return RoomResourceSubFolder + '/' + roomName;
        //        }
        //        public static string CompileUnityRoomPath(string filename, string roomName)
        //        {
        //            return CompileUnityRoomDirectory(roomName) + '/' + filename;
        //        }
        //        public static string CompileAbsoluteRoomDirectory()
        //        {
        //#if UNITY_WSA_10_0
        //            return AbsoluteAssetRootFolder;
        //#else
        //            return Path.Combine(AbsoluteAssetRootFolder, RoomResourceSubFolder);
        //#endif
        //        }
        //        public static string CompileAbsoluteRoomDirectory(string roomName)
        //        {
        //#if UNITY_WSA_10_0
        //            return AbsoluteAssetRootFolder;
        //#else
        //            return Path.Combine(AbsoluteAssetRootFolder, Path.Combine(RoomResourceSubFolder, roomName));
        //#endif
        //        }
        //        public static string CompileAbsoluteRoomPath(string filename, string roomName)
        //        {
        //            return Path.Combine(CompileAbsoluteRoomDirectory(roomName), filename);
        //        }
        //        public static string CompileUnityAssetDirectory()
        //        {
        //            //return "Assets/" + AssetSubFolder;
        //            return AssetSubFolder;
        //        }
        //        public static string CompileUnityAssetPath(string filename)
        //        {
        //            return CompileUnityAssetDirectory() + '/' + filename;
        //        }
        //        public static string CompileAbsoluteAssetDirectory()
        //        {
        //#if UNITY_WSA_10_0
        //            return AbsoluteAssetRootFolder;
        //#else
        //            return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
        //#endif
        //        }
        //        public static string CompileAbsoluteAssetPath(string filename)
        //        {
        //            return Path.Combine(CompileAbsoluteAssetDirectory(), filename);
        //        }
        //        public static string CompileRoomResourcesLoadPath(string assetNameWithoutExtension, string roomName)
        //        {
        //            return RoomResourceSubFolder.Substring(RoomResourceSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        //        }
        //        public static string CompileResourcesLoadPath(string assetNameWithoutExtension)
        //        {
        //            return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        //            //return ResourcesSubFolder + '/' + assetNameWithoutExtension;
        //        }
        //        public static string CompileResourcesLoadPath(string assetSubDirectory, string assetNameWithoutExtension)
        //        {
        //            return assetSubDirectory.Substring(assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        //        }

        //        public static string CompileUnityBundleDirectory()
        //        {
        //            return "Assets/" + BundleSubFolder;
        //            //return BundleSubFolder;
        //        }
        //        public static string CompileUnityBundlePath(string filename)
        //        {
        //            //return CompileUnityBundleDirectory() + '/' + filename;
        //            return Path.Combine(CompileUnityBundleDirectory(), filename);
        //        }
        //        public static string CompileAbsoluteBundleDirectory()
        //        {
        //#if UNITY_WSA_10_0
        //            return AbsoluteAssetRootFolder;
        //#else
        //            return Path.Combine(AbsoluteAssetRootFolder, BundleSubFolder);
        //#endif
        //        }
        //        public static string CompileAbsoluteBundlePath(string filename)
        //        {
        //            return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
        //        }

        //#endregion
    }
}