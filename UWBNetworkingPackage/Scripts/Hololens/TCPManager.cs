using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using Windows.Networking.Sockets;
#endif

namespace UWBNetworkingPackage {
    public partial class TCPManager
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0

        private static string serverIP = "0.0.0.0";
        
        public static Dictionary<int, StreamSocket> socketMap;




        public static Queue<StreamSocketListener> listenerQueue;
        public static int numListeners = 15;

        public static void Start()
        {
            //socketMap = new Dictionary<int, StreamSocket>();
            listenerQueue = new Queue<StreamSocketListener>();
            string networkConfigString = IPManager.CompileNetworkConfigString(Config.Ports.ClientServerConnection);
            string ip = IPManager.ExtractIPAddress(networkConfigString);
            string port = IPManager.ExtractPort(networkConfigString).ToString();

            for(int i = 0; i < numListeners; i++)
            {
                StreamSocketListener listener = new StreamSocketListener();
                listener.Control.QualityOfService = SocketQualityOfService.Normal;
                listener.ConnectionReceived += OnConnection;
                HostName localHostName = new HostName(ip);
                await listener.BindEndpointAsync(localHostName, port);
                listenerQueue.Enqueue(listener);
            }
        }

        private async void OnConnection(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Debug.Log("Connected");
        } 
        
        //public async static StreamSocketListener GetSocketListener(int port)
        //{

        //}

        //public static void SearchForServer()
        //{
        //    new Task(async delegate
        //    {
        //        StreamSocket socket = new StreamSocket();

        //        using (DataReader reader = new DataReader(socket.InputStream))
        //        {
        //            string configString = IPManager.CompileConfigString();
        //            int port = IPManager.ExtractPort(configString);
        //            string ip = IPManager.ExtractIPAddress(configString).ToString();

        //            HostName localHostName = new HostName(ip);
        //            string localServiceName = port.ToString();
        //            HostName remoteHostName = new HostName(serverIP);
        //            string remoteServiceName = port.ToString();

        //            EndpointPair pair = new EndpointPair(localHostName, localServiceName, remoteHostName, remoteServiceName);
        //            await socket.ConnectAsync(pair);
        //            //while (!reader.ReadBoolean())
        //            //{
        //            //    socket.ConnectAsync(pair);
        //            //    await Task.Delay(TimeSpan.FromSeconds(10));
        //            //}

        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                byte[] value = new byte[1024];
        //                uint numBytesRead = reader.UnconsumedBufferLength % 1024 + 1;
        //                while (numBytesRead > 0)
        //                {
        //                    reader.ReadBytes(value);
        //                    ms.Write(value, 0, (int)numBytesRead);
        //                }

        //                byte[] data = new byte[ms.Length];
        //                data = ms.ToArray();

        //                filename = System.Text.Encoding.UTF8.GetString(data);
        //            }

        //            reader.DetachStream();
        //        }

        //        socket.Dispose();

        //    }).Start();
        //}

        //public static void ServerToClient(string clientIP, int port)
        //{
        //    new Task(async delegate
        //    {
        //        StreamSocket socket = new StreamSocket();

        //        using (DataReader reader = new DataReader(socket.InputStream))
        //        {
        //            string configString = IPManager.CompileConfigString();
        //            int port = IPManager.ExtractPort(configString);
        //            string ip = IPManager.ExtractIPAddress(configString).ToString();

        //            HostName localHostName = new HostName(ip);
        //            string localServiceName = port.ToString();
        //            HostName remoteHostName = new HostName(clientIP);
        //            string remoteServiceName = port.ToString();

        //            EndpointPair pair = new EndpointPair(localHostName, localServiceName, remoteHostName, remoteServiceName);
        //            await socket.ConnectAsync(pair);
        //            //while (!reader.ReadBoolean())
        //            //{
        //            //    socket.ConnectAsync(pair);
        //            //    await Task.Delay(TimeSpan.FromSeconds(10));
        //            //}

        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                byte[] value = new byte[1024];
        //                uint numBytesRead = reader.UnconsumedBufferLength % 1024 + 1;
        //                while (numBytesRead > 0)
        //                {
        //                    reader.ReadBytes(value);
        //                    ms.Write(value, 0, (int)numBytesRead);
        //                }

        //                byte[] data = new byte[ms.Length];
        //                data = ms.ToArray();

        //                filename = System.Text.Encoding.UTF8.GetString(data);
        //            }

        //            reader.DetachStream();
        //        }

        //        socket.Dispose();

        //    }).Start();
        //}

        //public static void 
#endif
    }
    //namespace Hololens {
    //    public class TCPManager : MonoBehaviour {
    //        //#if UNITY_UWP
    //        // Use this for initialization

    //    #endif
    //    }
    //}
}