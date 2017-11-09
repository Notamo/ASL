using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
//using GameDevWare.Serialization;
using Tango;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public class ReceivingClientLauncher_Tango : ReceivingClientLauncher_PC
    {
        [PunRPC]
        public override void SendTangoMesh()
        {
            UpdateMesh();

            photonView.RPC("ReceiveTangoMesh", PhotonTargets.MasterClient, PhotonNetwork.player.ID, TangoDatabase.GetMeshAsBytes().Length);
        }

        private void UpdateMesh()
        {
            var tangoApplication =
                GameObject.Find("Tango Manager")
                    .GetComponent<TangoApplication>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Color32> colors = new List<Color32>();
            List<int> triangles = new List<int>();
            tangoApplication.Tango3DRExtractWholeMesh(vertices, normals, colors,
                triangles);

            Vector3 V;
            Quaternion Q;
            Transform T = GameObject.Find("Dynamic_GameObjects").transform;
            V = T.transform.position;
            Q = T.transform.rotation;
            //Matrix4x4 M = Matrix4x4.TRS(Vector3.zero, Q, Vector3.one);
            //GameObject.Find("Canvas").GetComponent<switchCamera>().SetText(M);
            float angle;
            Vector3 axis;
            Q.ToAngleAxis(out angle, out axis);
            //Q.SetAxisAngle(axis, -angle);
            Q = Quaternion.AngleAxis(-angle, axis);

            //Quaternion newRotation = new Quaternion();
            //newRotation.eulerAngles = new Vector3(0, Q.eulerAngles.y, 0);

            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] -= V;
                vertices[i] = Q * vertices[i]; //inverse Q
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.colors32 = colors.ToArray();
            mesh.triangles = triangles.ToArray();
            List<Mesh> meshList = new List<Mesh>();
            meshList.Add(mesh);

            TangoDatabase.UpdateMesh(meshList);
            Debug.Log("Mesh Updated");
        }
    }
}