using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace UWBNetworkingPackage
{
    public static class IPManager
    {
        public static IPAddress GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    return ip;
                }
            }
            return null;
        }

        public static string CompileNetworkConfigString(int port)
        {
            return GetLocalIpAddress() + ":" + port;
        }

        public static string ExtractIPAddress(string networkConfigString)
        {
            return networkConfigString.Split(':')[0];
        }

        public static string ExtractPort(string networkConfigString)
        {
            return networkConfigString.Split(':')[1];
        }
    }
}