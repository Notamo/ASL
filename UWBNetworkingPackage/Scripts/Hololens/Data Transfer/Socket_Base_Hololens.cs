using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using Windows.System.Threading;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams; // DataReader/DataWriter & Streams
using Windows.Security.Cryptography; // Convert string to bytes
#endif

namespace UWBNetworkingPackage
{
    public class Socket_Base_Hololens
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0
        public static void SendFile(string filepath, StreamSocket socket)
        {
            SendFiles(new string[1] { filepath }, socket);
        }

        public static void SendFiles(string[] filepaths, StreamSocket socket)
        {
            // Needs to tell the client socket what the server's ip is
            //string configString = IPManager.CompileNetworkConfigString(Config.Ports.ClientServerConnection);

            foreach (string filepath in filepaths)
            {
                Debug.Log("Sending " + Path.GetFileName(filepath));
            }

            MemoryStream ms = new MemoryStream();
            PrepSocketData(filepaths, ref ms);
            DataWriter writer = new DataWriter(socket.OutputStream);
            writer.WriteBytes(ms.ToArray());

            socket.Dispose();
        }

        public static void PrepSocketData(string[] filepaths, ref MemoryStream ms)
        {
            string header = BuildSocketHeader(filepaths);

            byte[] headerData = CryptographicBuffer.ConvertStringToBinary(header, BinaryStringEncoding.Utf8);
            // Add header data length
            ms.Write(System.BitConverter.GetBytes(headerData.Length), 0, System.BitConverter.GetBytes(headerData.Length).Length);
            // Add header data
            ms.Write(headerData, 0, headerData.Length);

            foreach (string filepath in filepaths)
            {
                byte[] fileData = File.ReadAllBytes(filepath);
                // Add file data length
                ms.Write(System.BitConverter.GetBytes(fileData.Length), 0, System.BitConverter.GetBytes(fileData.Length).Length);
                // Add file data
                ms.Write(fileData, 0, fileData.Length);
            }
        }

        public static string BuildSocketHeader(string[] filepaths)
        {
            System.Text.StringBuilder headerBuilder = new System.Text.StringBuilder();

            foreach (string filepath in filepaths)
            {
                headerBuilder.Append(Path.GetFileName(filepath));
                headerBuilder.Append(';');
            }
            headerBuilder.Remove(headerBuilder.Length - 1, 1); // Remove the last separator (';')

            return headerBuilder.ToString();
        }

        public static void ReceiveFiles(StreamSocket socket, string receiveDirectory)
        {
            MemoryStream fileStream = new MemoryStream();

            DataReader reader = new DataReader(socket.InputStream);

            UInt32 loadSize = 1048576;
            int numBytesRemaining = 0;
            string headerData = string.Empty;

            bool streamHasRemainingPartsToHave = true;
            do
            {
                await reader.LoadAsync(loadSize);

                // Handle remaining file part
                if(numBytesRemaining > 0)
                {
                    for(; numBytesRemaining > 0; numBytesRemaining--)
                    {
                        fileStream.Write(reader.ReadByte());
                    }
                }

                // Break out of the loop if stream is finished
                if(reader.UnconsumedBufferLength <= 0)
                {
                    // The stream is finished
                    break;
                }

                while (reader.UnconsumedBufferLength > 0) {
                    // Read in the first four bytes for the length
                    int dataLength = 0;
                    if (reader.UnconsumedBufferLength >= 4) {
                        byte[] dataLengthBuffer = new byte[4];
                        for (int i = 0; i < 4; i++)
                        {
                            dataLengthBuffer[i] = reader.ReadByte();
                        }

                        // Convert to length
                        dataLength = System.BitConverter.ToInt32(dataLengthBuffer, 0);
                    }
                    else
                    {
                        // If the stream is done, just exit the read loop
                        break;
                    }

                    // Grab the data
                    if (dataLength > reader.UnconsumedBufferLength)
                    {
                        numBytesRemaining = reader.UnconsumedBufferLength - dataLength;

                        if (headerData.Equals(string.Empty))
                        {
                            byte[] headerBytes = new byte[4];
                            headerData = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, reader.ReadBytes(headerBytes));
                        }
                        else
                        {
                            byte[] partialFileBytes = new byte[reader.UnconsumedBufferLength];
                            for (int i = 0; i < dataLength; i++)
                            {
                                partialFileBytes[i] = reader.ReadByte();
                            }
                            fileStream.Write(partialFileBytes, 0, partialFileBytes.Length);
                        }
                    }
                    else {
                        byte[] fullFileBytes = new byte[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            fullFileBytes[i] = reader.ReadByte();
                        }
                        fileStream.Write(fullFileBytes, 0, dataLength);
                    }
                }
            } while (true);

            fileStream.Close();
            fileStream.Dispose();
        }


        //public static void ReceiveFiles(StreamSocket socket, string receiveDirectory)
        //{
        //    int bufferLength = 1024;
        //    byte[] data = new byte[bufferLength];
        //    int numBytesReceived = 0;

        //    MemoryStream fileStream = new MemoryStream();

        //    int headerIndex = 0;
        //    int dataLengthIndex = 0;
        //    string dataHeader = string.Empty;

        //    DataReader reader = new DataReader(socket.InputStream);
        //    UInt32 readerBytesRead = 0;
        //    UInt32 readerLoadSize = 1048576;

        //    do
        //    {
        //        // Get the first receive from the socket
        //        //numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None);
        //        await reader.LoadAsync(readerLoadSize); // Temporarily load up a megabyte

        //        numBytesReceived = 0;

        //        while (numBytesReceived < bufferLength)
        //        {
        //            byte b = reader.ReadByte();
        //            if (b == null)
        //                break;
        //            else
        //            {
        //                ++numBytesReceived;
        //                fileStream
        //            }
        //        }
        //        int numBytesAvailable = numBytesReceived;
        //        int dataIndex = 0;

        //        // If there are any bytes that continue a file from the last buffer read, handle that here
        //        if (dataLengthIndex > 0 && dataLengthIndex < numBytesReceived)
        //        {
        //            fileStream.Write(data, 0, dataLengthIndex);
        //            string filename = dataHeader.Split(';')[headerIndex++];
        //            File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileStream.ToArray());
        //            // MemoryStream flush does literally nothing.
        //            fileStream.Close();
        //            fileStream.Dispose();
        //            fileStream = new MemoryStream();
        //        }
        //        else if (numBytesReceived <= 0)
        //        {
        //            string filename = dataHeader.Split(';')[headerIndex++];
        //            File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileStream.ToArray());
        //            // MemoryStream flush does literally nothing.
        //            fileStream.Close();
        //            fileStream.Dispose();
        //            fileStream = new MemoryStream();
        //        }

        //        // While there are file pieces we can get from the gathered data,
        //        // determine where the bytes designating the lengths of files about to be
        //        // transferred over are and then grab the file lengths and file bytes
        //        while (dataLengthIndex >= 0 && dataLengthIndex < numBytesReceived)
        //        {
        //            // Get the 4 bytes indicating the length
        //            int dataLength = 0;
        //            if (dataLengthIndex <= numBytesReceived - 4)
        //            {
        //                // If length is shown fully within the buffer (i.e. length bytes aren't split between reads)...
        //                dataLength = System.BitConverter.ToInt32(data, dataLengthIndex);

        //                if (dataLength <= 0)
        //                {
        //                    // Handle case where end of stream is reached
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                // Else length bytes are split between reads...
        //                byte[] dataLengthBuffer = new byte[4];
        //                int numDataLengthBytesCopied = numBytesReceived - dataLengthIndex;
        //                System.Buffer.BlockCopy(data, dataLengthIndex, dataLengthBuffer, 0, numDataLengthBytesCopied);
        //                readerBytesRead += numBytesReceived;
        //                numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None);
        //                System.Buffer.BlockCopy(data, 0, dataLengthBuffer, numDataLengthBytesCopied, 4 - numDataLengthBytesCopied);

        //                dataLength = System.BitConverter.ToInt32(dataLengthBuffer, 0);
        //                dataLengthIndex -= numBytesReceived;
        //            }
        //            dataIndex = dataLengthIndex + 4;
        //            dataLengthIndex = dataIndex + dataLength; // Update the data length index for the while loop check
        //            numBytesAvailable = numBytesReceived - dataIndex;

        //            // Handle instances where whole file is contained in part of buffer
        //            if (dataIndex + dataLength < numBytesAvailable)
        //            {
        //                byte[] fileData = new byte[dataLength];
        //                System.Buffer.BlockCopy(data, dataIndex, fileData, 0, dataLength);
        //                if (dataHeader.Equals(string.Empty))
        //                {
        //                    // If the header hasn't been received yet
        //                    dataHeader = System.Text.Encoding.UTF8.GetString(fileData);
        //                }
        //                else
        //                {
        //                    // If the header's been received, that means we're looking at actual file data
        //                    string filename = dataHeader.Split(';')[headerIndex++];
        //                    File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileData);
        //                }
        //            }
        //        }

        //        // Write remainder of bytes in buffer to the file memory stream to store for the next buffer read
        //        if (numBytesAvailable < 0)
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            fileStream.Write(data, dataIndex, numBytesAvailable);
        //            dataLengthIndex -= bufferLength;
        //            readerBytesRead += numBytesReceived;
        //        }
        //        // continue;

        //    } while (numBytesReceived > 0);

        //    fileStream.Close();
        //    fileStream.Dispose();

        //    reader.Dispose();
        //}
#endif

    }
}