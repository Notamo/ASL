using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace UWBNetworkingPackage
{
    public static partial class SocketClient
    {
#if !WINDOWS_UWP
        // Action passed in is reference to a method that will take the string (filepath)
        // and the byte[] data (from the server's stream) to interpret the data
        // appropriately
        //
        // This must be copied over into another method if requesting multiple files (i.e.
        // Action must have a list of strings? but it wouldn't know what the filenames
        // would be so...gotta figure this out?)
        public static void RequestData(string serverNetworkConfig, int port, System.Action<string, byte[]> interpreter)
        {
            new Thread(() =>
            {
                // Generate the socket
                TcpClient tcp = new TcpClient();

                // Connect to the server
                int serverPort = Config.Ports.ClientServerConnection;
                IPAddress serverIP = IPAddress.Parse(IPManager.ExtractIPAddress(serverNetworkConfig));
                tcp.Connect(serverIP, serverPort);

                // After awaiting the connection, receive data appropriately
                Socket socket = tcp.Client;
                byte[] data = new byte[1024];
                int numBytesReceived = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    while ((numBytesReceived = socket.Receive(data, 1024, SocketFlags.None)) > 0)
                    {
                        ms.Write(data, 0, numBytesReceived);
                        Debug.Log("Data received! Size = " + numBytesReceived);
                    }
                    Debug.Log("Finished receiving data: size = " + ms.Length);

                    byte[] allData = ms.ToArray();
                    interpreter("mystring", allData);

                    // Clean up socket & close connection
                    ms.Close();
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }).Start();
        }


        //public static void ConnectTest(string serverNetworkConfig, int port)
        public static void ConnectTest(int port)
        {
            new Thread(() =>
            {
                // Generate the socket
                TcpClient tcp = new TcpClient();

                // Connect to the server
                int serverPort = Config.Ports.ClientServerConnection;
                //IPAddress serverIP = IPAddress.Parse(IPManager.ExtractIPAddress(serverNetworkConfig));
                IPAddress serverIP = IPAddress.Any;
                tcp.Connect(serverIP, serverPort);

                // After awaiting the connection, receive data appropriately
                Socket socket = tcp.Client;
                byte[] data = new byte[1024];
                int numBytesReceived = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    while ((numBytesReceived = socket.Receive(data, 1024, SocketFlags.None)) > 0)
                    {
                        ms.Write(data, 0, numBytesReceived);
                        Debug.Log("Data received! Size = " + numBytesReceived);
                    }
                    Debug.Log("Finished receiving data: size = " + ms.Length);

                    byte[] allData = ms.ToArray();

                    // Clean up socket & close connection
                    ms.Close();
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }).Start();
        }
#endif
    }
}