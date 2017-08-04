using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace UWBNetworkingPackage
{
    public partial class ServerFinder
    {
        public static UdpClient listener;
        public static string serverIP;

        public static void ServerStart()
        {
            int listenerPort = Config.Ports.FindServer;
            listener = new UdpClient(listenerPort);
            serverIP = IPManager.GetLocalIpAddress().ToString();
            AcceptClient();
        }

        public static void AcceptClient()
        {
            byte[] serverIPBytes = Encoding.UTF8.GetBytes(serverIP);

            new Thread(() =>
            {
                while (true)
                {
                    IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    // ERROR TESTING - NEED TO ASSIGN THE SYSTEM A PORT # THAT WORKS, INSTEAD OF 0 ABOVE
                    byte[] clientIPBytes = listener.Receive(ref clientEndpoint);
                    //string clientIPString = Encoding.UTF8.GetString(clientIPBytes);
                    listener.Send(serverIPBytes, serverIPBytes.Length, clientEndpoint);
                }
            }).Start();
        }

        // IPAddress string
        public static string FindServer()
        {
            string IPString = string.Empty;

            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            int findServerPort = Config.Ports.FindServer;
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Broadcast, findServerPort);
            byte[] clientIPBytes = Encoding.UTF8.GetBytes(IPManager.GetLocalIpAddress().ToString());
            client.Send(clientIPBytes, clientIPBytes.Length, serverEndpoint);
            byte[] serverIPBytes = client.Receive(ref serverEndpoint);

            IPString = Encoding.UTF8.GetString(serverIPBytes);
            serverIP = IPString;
            return IPString;
        }
    }
}