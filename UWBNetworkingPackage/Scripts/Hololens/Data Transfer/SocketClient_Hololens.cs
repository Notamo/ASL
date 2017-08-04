using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using Windows.System.Threading;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams; // DataReader/DataWriter & Streams
using Windows.Security.Cryptography; // Convert string to bytes
#endif

namespace UWBNetworkingPackage
{
    public class SocketClient_Hololens : Socket_Base_Hololens
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0
        public static void RequestFiles(int port, string receiveDirectory)
        {
            RequestFiles(ServerFinder.serverIP, port, receiveDirectory);
        }

        public static void RequestFiles(string serverIP, int port, string receiveDirectory)
        {
            new Task(() =>
            {
                // Generate the socket and connect to the server
                StreamSocket socket = new StreamSocket();
                string serverIP = ServerFinder.serverIP;
                int serverPort = Config.Ports.ClientServerConnection;
                EndpointPair endpointPair = new EndpointPair(new HostName(IPManager.GetLocalIpAddress()), port.ToString(), new HostName(serverIP), serverPort.ToString());
                socket.ConnectAsync(endpointPair);

                // After awaiting the connection, receive data appropriately
                ReceiveFiles(socket, receiveDirectory);

                socket.Dispose();
            }).Start();
        }

        public static void SendFile(string remoteIP, int port, string filepath)
        {
            SocketClient.SendFiles(remoteIP, port, new string[1] { filepath });
        }

        public static void SendFiles(string remoteIP, int port, string[] filepaths)
        {
            new Thread(() =>
            {
                int serverPort = Config.Ports.ClientServerConnection;
                string serverIP = ServerFinder.serverIP;
                EndpointPair endpointPair = new EndpointPair(new HostName(IPManager.GetLocalIpAddress()), port.ToString(), new HostName(serverIP), serverPort.ToString());
                StreamSocket socket = new StreamSocket();
                socket.ConnectAsync(endpointPair);
                Socket_Base_Hololens.SendFiles(filepaths, socket);
            }).Start();
        }

        ////public static void ConnectTest(string serverNetworkConfig, int port)
        //public static void ConnectTest(int port)
        //{
        //    new Thread(() =>
        //    {
        //        // Generate the socket
        //        TcpClient tcp = new TcpClient();

        //        // Connect to the server
        //        int serverPort = Config.Ports.ClientServerConnection;
        //        //IPAddress serverIP = IPAddress.Parse(IPManager.ExtractIPAddress(serverNetworkConfig));
        //        IPAddress serverIP = IPAddress.Any;
        //        tcp.Connect(serverIP, serverPort);

        //        // After awaiting the connection, receive data appropriately
        //        Socket socket = tcp.Client;
        //        byte[] data = new byte[1024];
        //        int numBytesReceived = 0;
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            while ((numBytesReceived = socket.Receive(data, 1024, SocketFlags.None)) > 0)
        //            {
        //                ms.Write(data, 0, numBytesReceived);
        //                Debug.Log("Data received! Size = " + numBytesReceived);
        //            }
        //            Debug.Log("Finished receiving data: size = " + ms.Length);

        //            byte[] allData = ms.ToArray();

        //            // Clean up socket & close connection
        //            ms.Close();
        //            socket.Shutdown(SocketShutdown.Both);
        //            socket.Close();
        //        }
        //    }).Start();
        //}

        //// Assumes that all information will be separated by lengths of data and that it will be one long continuous stream of data
        //public static void ConnectTest2(int port)
        //{
        //    new Thread(() =>
        //    {
        //        // Generate the socket
        //        TcpClient tcp = new TcpClient();

        //        // Connect to the server
        //        int serverPort = Config.Ports.ClientServerConnection;
        //        //IPAddress serverIP = IPAddress.Parse(IPManager.ExtractIPAddress(serverNetworkConfig));
        //        IPAddress serverIP = IPAddress.Any;
        //        tcp.Connect(serverIP, serverPort);

        //        // After awaiting the connection, receive data appropriately
        //        Socket socket = tcp.Client;
        //        int bufferLength = 1024;
        //        byte[] data = new byte[bufferLength];
        //        int numBytesReceived = 0;
                
        //        using (MemoryStream fileStream = new MemoryStream())
        //        {
        //            int headerIndex = 0;
        //            //string filename = dataHeader.Split(';')[headerIndex++];
        //            int dataLengthIndex = 0;
        //            //int numReceives = 0;

        //            // Get directory to save it to
        //            string directory = "C:\\Users\\Thomas\\Documents\\tempwritefile";

        //            string dataHeader = string.Empty;

        //            do
        //            {
        //                int numBytesAvailable = bufferLength;
        //                int dataIndex = 0;

        //                // Get the first receive from the socket
        //                numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None);

        //                // If there are any bytes that continue a file from the last buffer read, handle that here
        //                if (dataLengthIndex > 0 && dataLengthIndex < bufferLength)
        //                {
        //                    fileStream.Write(data, 0, dataLengthIndex);
        //                    string filename = dataHeader.Split(';')[headerIndex++];
        //                    File.WriteAllBytes(Path.Combine(directory, filename), fileStream.ToArray());
        //                }

        //                // While there are file pieces we can get from the gathered data,
        //                // determine where the bytes designating the lengths of files about to be
        //                // transferred over are and then grab the file lengths and file bytes
        //                while (dataLengthIndex < bufferLength)
        //                {
        //                    // Get the 4 bytes indicating the length
        //                    int dataLength = 0;
        //                    if (dataLengthIndex <= bufferLength - 4)
        //                    {
        //                        // If length is shown fully within the buffer (i.e. length bytes aren't split between reads)...
        //                        dataLength = System.BitConverter.ToInt32(data, dataLengthIndex);
                                
        //                    }
        //                    else
        //                    {
        //                        // Else length bytes are split between reads...
        //                        byte[] dataLengthBuffer = new byte[4];
        //                        int numDataLengthBytesCopied = bufferLength - dataLengthIndex;
        //                        System.Buffer.BlockCopy(data, dataLengthIndex, dataLengthBuffer, 0, numDataLengthBytesCopied);
        //                        numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None);
        //                        System.Buffer.BlockCopy(data, 0, dataLengthBuffer, numDataLengthBytesCopied, 4 - numDataLengthBytesCopied);

        //                        dataLength = System.BitConverter.ToInt32(dataLengthBuffer, 0);
        //                        dataLengthIndex -= bufferLength;
        //                    }
        //                    dataIndex = dataLengthIndex + 4;
        //                    dataLengthIndex = dataIndex + dataLength; // Update the data length index for the while loop check
        //                    numBytesAvailable = numBytesReceived - dataIndex;

        //                    // Handle instances where whole file is contained in part of buffer
        //                    if (dataIndex + dataLength < numBytesAvailable)
        //                    {
        //                        byte[] fileData = new byte[dataLength];
        //                        System.Buffer.BlockCopy(data, dataIndex, fileData, 0, dataLength);
        //                        if (dataHeader.Equals(string.Empty))
        //                        {
        //                            // If the header hasn't been received yet
        //                            dataHeader = System.Text.Encoding.UTF8.GetString(fileData);
        //                        }
        //                        else
        //                        {
        //                            // If the header's been received, that means we're looking at actual file data
        //                            string filename = dataHeader.Split(';')[headerIndex++];
        //                            File.WriteAllBytes(Path.Combine(directory, filename), fileData);
        //                        }
                               
        //                    }
        //                }

        //                // Write remainder of bytes in buffer to the file memory stream to store for the next buffer read
        //                fileStream.Write(data, dataIndex, numBytesAvailable);
        //                dataLengthIndex -= bufferLength;
        //                // continue;


        //            } while (numBytesReceived == bufferLength);

        //            fileStream.Close();
        //        }

        //        // Clean up socket & close connection
        //        //ms.Close();
        //        socket.Shutdown(SocketShutdown.Both);
        //        socket.Close();
        //        //}
        //    }).Start();
        //}
#endif
    }
}