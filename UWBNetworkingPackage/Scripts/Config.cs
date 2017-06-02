using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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