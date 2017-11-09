using System;
using UnityEngine;
using System.Collections.Generic;
//using HoloToolkit.Unity;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Database is a static class that stores the most recently sent Room Mesh (the Room Mesh is created by a HoloLens),
    /// and allows any classes in the UWBNetworkingPackage to access it
    /// </summary>
    // Note: For future improvment, you should add: a) Parameter check
    public class TangoDatabase
    {
        #region Private Properties

        public struct TangoRoom
        {
            public byte[] _meshes;
            public bool isDirty;
            public int ID;
            public int PhotonPlayer;
            public string name;
        }

        public struct TangoData
        {
            public string name;
            public int size;
        }

        public static int count = 0;
        public static int ID = 0;

        public static List<TangoRoom> Rooms = new List<TangoRoom>();

        private static byte[] _meshes;  // Stores the current Room Mesh data as a serialized byte array

        #endregion

        #region Public Properties

        public static DateTime LastUpdate = DateTime.MinValue;  // Used for keeping the Room Map up-to-date

        #endregion

        /// <summary>
        /// Retrieves the Room Mesh as a deserialized list
        /// </summary>
        /// <returns>Deserialized Room Mesh</returns>
        public static IEnumerable<Mesh> GetMeshAsList()
        {
            return NetworkingPackage.SimpleMeshSerializerTango.Deserialize(_meshes);
        }
        public static IEnumerable<Mesh> GetMeshAsList(TangoRoom T)
        {
            return NetworkingPackage.SimpleMeshSerializerTango.Deserialize(T._meshes);
        }

        public static bool LookUpName(string name)
        {
            lock (Rooms)
            {
                foreach (TangoRoom T in Rooms)
                {
                    if (name == T.name)
                        return true;
                }
            }

            return false;
        }

        public static void CompareList(List<string> TData)
        {
            lock (Rooms)
            {
                foreach (TangoRoom T in Rooms)
                {
                    bool found = false;
                    foreach (string name in TData)
                    {
                        if (T.name == name)
                        {
                            found = true;
                        }
                    }

                    if (found == false)
                    {
                        GameObject.Destroy(GameObject.Find(T.name).gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the Room Mesh as a serialized byte array
        /// </summary>
        /// <returns>Serialized Room Mesh</returns>
        public static byte[] GetMeshAsBytes()
        {
            return _meshes;
        }

        public static byte[] GetMeshAsBytes(TangoRoom T)
        {
            return T._meshes;
        }

        public static TangoRoom GetRoom(int ID)
        {
            lock (Rooms)
            {
                return Rooms[ID];
            }
        }

        public static void DeleteRoom(TangoRoom T)
        {
            lock (Rooms)
            {
                Rooms.Remove(T);
            }
        }

        public static string GetAllRooms()
        {
            string data = Rooms[0].name;
            data += '~';
            data += Rooms[0]._meshes.Length;

            for (int i = 1; i < Rooms.Count; i++)
            {
                data += '~';
                data += Rooms[i].name;
                data += '~';
                data += Rooms[i]._meshes.Length;
            }

            return data;
        }

        public static TangoRoom GetRoomByName(string name)
        {
            lock (Rooms)
            {
                foreach (TangoRoom T in Rooms)
                {
                    if (name == T.name)
                    {
                        return T;
                    }
                }
            }
            return new TangoRoom();
        }

        /// <summary>
        /// Update the currently saved mesh to be the given deserialized Room Mesh
        /// This method will also update the LastUpdate time
        /// </summary>
        /// <param name="newMesh">Deserialized Room Mesh stored in a list</param>
        public static void UpdateMesh(IEnumerable<Mesh> newMesh)
        {
            _meshes = NetworkingPackage.SimpleMeshSerializerTango.Serialize(newMesh);
            
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Update the currently saved mesh to be the given serialized Room Mesh
        /// This method will also update the LastUpdate time
        /// </summary>
        /// <param name="newMesh">Serialized Room Mesh stored in a byte array</param>
        public static void UpdateMesh(byte[] newMesh, int playerID)
        {
            TangoRoom T = new TangoRoom();
            T.isDirty = true;
            T._meshes = newMesh;
            count++;
            T.ID = count;
            T.PhotonPlayer = playerID;
            string name = (string)(playerID + "_" + DateTime.Now);
            name = name.Replace('/', '_');
            name = name.Replace('\\', '_');
            name = name.Replace(' ', '_');
            name = name.Replace(':', '_');
            T.name = name;
            lock (Rooms)
            {
                Rooms.Add(T);
            }
            //_meshes = newMesh;
            LastUpdate = DateTime.Now;
        }

        public static void UpdateMesh(byte[] newMesh, string name)
        {
            TangoRoom T = new TangoRoom();
            T.isDirty = true;
            T._meshes = newMesh;
            count++;
            T.ID = count;
            //T.PhotonPlayer = playerID;
            T.name = (string)(name);
            lock (Rooms)
            {
                Rooms.Add(T);
            }
            //_meshes = newMesh;
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Delete the currently held Room Mesh
        /// This method will also update the LastUpdate time
        /// </summary>
        public static void DeleteMesh()
        {
            Debug.Log("Deleting Mesh");
            _meshes = null;
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Update the currently saved mesh to add the new mesh
        /// This method will also update the LastUpdate time
        /// </summary>
        /// <param name="newMesh">Serialized Room Mesh stored in a byte array</param>
        public static void AddToMesh(byte[] newMesh)
        {
            int length = newMesh.Length + _meshes.Length;
            byte[] totalMesh = new byte[length];
            Buffer.BlockCopy(_meshes, 0, totalMesh, 0, _meshes.Length);
            Buffer.BlockCopy(newMesh, 0, totalMesh, _meshes.Length, newMesh.Length);
            LastUpdate = DateTime.Now;
        }
    }
}