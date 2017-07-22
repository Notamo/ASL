using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;

using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UWBNetworkingPackage
{
    public static partial class TCPManager
    {
#if !UNITY_UWP
        public static Dictionary<int, TcpListener> PortListenerMap;

        [Serializable]
        public struct TCPHeader
        {
            public long MessageSize;
            public short FilepathLength;
            public string Filepath;
        }

        public static class Messages
        {
            public static class Errors
            {
                public static string ListenerNotPending = "TCPListener is uninitialized";//"TCPListener is either uninitialized or has no clients waiting to send/request messages.";
                public static string SendFileFailed = "Sending of file data failed. File not found.";
                public static string SendDataFailed = "Sending of data failed. Byte array is zero length or null.";
                public static string ReceiveDataFailed = "Data stream was empty.";
            }
        }

        public static void Start()
        {
            PortListenerMap = new Dictionary<int, TcpListener>();
        }

        public static TcpListener GetListener(int port)
        {
            if (PortListenerMap.ContainsKey(port))
            {
                return PortListenerMap[port];
            }
            else
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                PortListenerMap.Add(port, listener);
                return listener;
            }
        }

        public static bool CloseListener(int port)
        {
            if (PortListenerMap.ContainsKey(port))
            {
                PortListenerMap[port].Stop();
                PortListenerMap.Remove(port);
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public static bool SendData(TcpListener listener, byte[] data)
        {
            if (listener != null)
            {
                if (data != null && data.Length > 0)
                {
                    new Thread(() =>
                    {
                        var client = listener.AcceptTcpClient();

                        using (var stream = client.GetStream())
                        {
                        //needs to be changed back
                        stream.Write(data, 0, data.Length);
                            client.Close();
                        }
                    }).Start();

                    return true;
                }
                else
                {
                    var client = listener.AcceptTcpClient();
                    client.Close();
                    Debug.Log(Messages.Errors.SendDataFailed);
                }
            }
            else
            {
                Debug.Log(Messages.Errors.ListenerNotPending);
            }

            return false;
        }
        
        public static bool SendData(Config.Ports.Types portType, byte[] data)
        {
            int port = Config.Ports.GetPort(portType);
            TcpListener portListener = GetListener(port);
            return SendData(portListener, data);
        }

        public static bool SendDataFromFile(TcpListener listener, string filepath)
        {
            if (listener != null)
            {
                if (File.Exists(filepath))
                {
                    new Thread(() =>
                    {
                        var client = listener.AcceptTcpClient();

                        using (var stream = client.GetStream())
                        {
                        //needs to be changed back
                        byte[] data = File.ReadAllBytes(filepath);
                            stream.Write(data, 0, data.Length);
                            client.Close();
                        }
                    }).Start();

                    return true;
                }
                else
                {
                    var client = listener.AcceptTcpClient();
                    client.Close();
                    Debug.Log(Messages.Errors.SendFileFailed);
                }
            }
            else
            {
                Debug.Log(Messages.Errors.ListenerNotPending);
            }

            return false;
        }

        public static bool SendDataFromFile(Config.Ports.Types portType, string filepath)
        {
            int port = Config.Ports.GetPort(portType);
            TcpListener portListener = GetListener(port);
            return SendDataFromFile(portListener, filepath);
        }

        public static byte[] ReceiveData(string networkConfig)
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(IPManager.ExtractIPAddress(networkConfig)), Int32.Parse(IPManager.ExtractPort(networkConfig)));
            Debug.Log("Client connected to server!");
            using (var stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                Debug.Log("Byte array allocated");

                using (MemoryStream ms = new MemoryStream())
                {
                    Debug.Log("MemoryStream created");
                    int numBytesRead;
                    while ((numBytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, numBytesRead);
                        Debug.Log("Data received! Size = " + numBytesRead);
                    }

                    Debug.Log("Finish receiving data: size = " + ms.Length);
                    byte[] data = new byte[ms.Length];
                    data = ms.ToArray();

                    client.Close();

                    return data;
                }
            }
        }

        public static bool ReceiveDataToFile(string networkConfig, string filepath)
        {
            //bool fileOverwritten = File.Exists(filepath);

            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(IPManager.ExtractIPAddress(networkConfig)), Int32.Parse(IPManager.ExtractPort(networkConfig)));
            Debug.Log("Client connected to server!");
            using (var stream = client.GetStream())
            {
                byte[] data = new byte[1024];
                Debug.Log("Byte array allocated");

                using (MemoryStream ms = new MemoryStream())
                {
                    Debug.Log("MemoryStream created");
                    int numBytesRead;
                    while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
                    {
                        ms.Write(data, 0, numBytesRead);
                        Debug.Log("Data received! Size = " + numBytesRead);
                    }
                    Debug.Log("Finish receiving bundle: size = " + ms.Length);
                    client.Close();

                    //AssetBundle bundle = AssetBundle.LoadFromMemory(ms.ToArray());
                    //string bundleName = bundle.name;

                    // Save the asset bundle
                    //filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(Config.AssetBundle.Current.CompileFilename(bundleName));

                    //if (!Directory.Exists(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory()))
                    //{
                    //    Directory.CreateDirectory(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory());
                    //}

                    AbnormalDirectoryHandler.CreateDirectoryFromFile(filepath);

                    if(ms.Length <= 0)
                    {
                        Debug.Log(Messages.Errors.ReceiveDataFailed);
                        return false;
                    }
                    else
                    {
                        File.WriteAllBytes(filepath, ms.ToArray());
                        return true;
                    }

                    //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/StreamingAssets/AssetBundlesPC/" + bundleName + ".asset"), ms.ToArray());
                    //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/StreamingAssets/AssetBundlesAndroid/" + bundleName + ".asset"), ms.ToArray());

                    //AssetBundle newBundle = AssetBundle.LoadFromMemory(ms.ToArray());
                    //bundles.Add(bundleName, newBundle);
                    //Debug.Log("You loaded the bundle successfully.");

                    //bundle.Unload(true);
                }
            }

            //client.Close();
            return true;
        }

        public static TCPHeader ConstructTCPHeader(string filepath)
        {
            FileInfo info = new FileInfo(filepath);
            long filesize = info.Length;
            byte[] filepathBytes = System.Text.Encoding.UTF8.GetBytes(filepath);

            TCPHeader header = new TCPHeader()
            {
                MessageSize = filesize,
                FilepathLength = (short)filepathBytes.Length,
                Filepath = filepath
            };

            return header;
        }

        public static void ConstructTCPHeaderBytes(string filepath)
        {

        }

        public static void SendDataFromFile(string targetNetworkConfig, string filepath)
        {
            byte[] data = File.ReadAllBytes(filepath);
            StreamSocket socket = new StreamSocket();

            using (DataWriter writer = new DataWriter(socket.OutputStream))
            {
                string port = IPManager.ExtractPort(targetNetworkConfig).ToString();
                string ip = IPManager.ExtractIPAddress(targetNetworkConfig);

                HostName localHostName = new HostName(ip);
            }
        }

        //public static void ReceiveAssetBundle(string networkConfig, out string bundlePath)
        //{
        //    //var networkConfigArray = networkConfig.Split(':');
        //    //Debug.Log("Start receiving bundle.");
        //    //TcpClient client = new TcpClient();
        //    //Debug.Log("IP Address = " + IPAddress.Parse(networkConfigArray[0]).ToString());
        //    //Debug.Log("networkConfigArray[1] = " + Int32.Parse(networkConfigArray[1]));
        //    //client.Connect(IPAddress.Parse(networkConfigArray[0]), Int32.Parse(networkConfigArray[1]));


        //    TcpClient client = new TcpClient();
        //    client.Connect(IPAddress.Parse(IPManager.ExtractIPAddress(networkConfig)), Int32.Parse(IPManager.ExtractPort(networkConfig)));
        //    Debug.Log("Client connected to server!");
        //    using (var stream = client.GetStream())
        //    {
        //        byte[] data = new byte[1024];
        //        Debug.Log("Byte array allocated");

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            Debug.Log("MemoryStream created");
        //            int numBytesRead;
        //            while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
        //            {
        //                ms.Write(data, 0, numBytesRead);
        //                Debug.Log("Data received! Size = " + numBytesRead);
        //            }
        //            Debug.Log("Finish receiving bundle: size = " + ms.Length);
        //            client.Close();

        //            AssetBundle bundle = AssetBundle.LoadFromMemory(ms.ToArray());
        //            string bundleName = bundle.name;

        //            // Save the asset bundle
        //            bundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(Config.AssetBundle.Current.CompileFilename(bundleName));

        //            if (!Directory.Exists(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory()))
        //            {
        //                Directory.CreateDirectory(Config.AssetBundle.Current.CompileAbsoluteBundleDirectory());
        //            }

        //            File.WriteAllBytes(bundlePath, ms.ToArray());

        //            //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/StreamingAssets/AssetBundlesPC/" + bundleName + ".asset"), ms.ToArray());
        //            //File.WriteAllBytes(Path.Combine(Application.dataPath, "ASL/StreamingAssets/AssetBundlesAndroid/" + bundleName + ".asset"), ms.ToArray());

        //            //AssetBundle newBundle = AssetBundle.LoadFromMemory(ms.ToArray());
        //            //bundles.Add(bundleName, newBundle);
        //            Debug.Log("You loaded the bundle successfully.");

        //            bundle.Unload(true);
        //        }
        //    }

        //    client.Close();
        //}

        public static int[] Ports
        {
            get
            {
                var keys = PortListenerMap.Keys;
                int[] ports = new int[keys.Count];
                int index = 0;
                foreach (var key in keys)
                {
                    ports[index++] = key;
                }
                return ports;
            }
        }
#endif
    }
}