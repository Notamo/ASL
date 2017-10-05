using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ASL.Manipulation.Objects
{
    public class CreateObject : MonoBehaviour
    {
        public void CreateASLObject(string prefabName)
        {

        }

        public void CreateASLObject(GameObject obj)
        {

        }

        // Emulates PUN object creation across the PUN network
        public GameObject CreatePUNObject(string prefabName)
        {
            bool connected = PhotonNetwork.connectedAndReady;
            bool UsePrefabCache = true;
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            if (!connected)
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + PhotonNetwork.connectionStateDetailed);
                return null;
            }

            GameObject prefabGo;
            if (!UsePrefabCache || !PhotonNetwork.PrefabCache.TryGetValue(prefabName, out prefabGo))
            {
                //List<string> prefabFolderPossibilities = new List<string>();
                prefabGo = (GameObject)Resources.Load(prefabName, typeof(GameObject));
                if (prefabGo == null)
                {
                    string directory = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "ASL/Resources");
                    directory = ConvertToResourcePath(directory);
                    ResourceDive(prefabName, directory);
                }
                if (UsePrefabCache)
                {
                    PhotonNetwork.PrefabCache.Add(prefabName, prefabGo);
                }
            }

            if (prefabGo == null)
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
                return null;
            }

            //// a scene object instantiated with network visibility has to contain a PhotonView
            //if (prefabGo.GetComponent<PhotonView>() == null)
            //{
            //    PhotonView pv = prefabGo.AddComponent<PhotonView>();
            //    List<int> pvIDList = new List<int>(networkingPeer.photonViewList.Keys);
            //    int maxObjectID = 1;
            //    foreach(int id in pvIDList)
            //    {
            //        if(id % PhotonNetwork.MAX_VIEW_IDS > maxObjectID)
            //        {
            //            maxObjectID = id % PhotonNetwork.MAX_VIEW_IDS;
            //        }
            //    }
            //    pv.viewID = PhotonNetwork.player.ID * PhotonNetwork.MAX_VIEW_IDS + maxObjectID;
            //    networkingPeer.RegisterPhotonView(pv);

            //    //Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            //    //return null;
            //}

            GameObject go = GameObject.Instantiate(prefabGo);
            if (go.GetComponent<PhotonView>() == null)
            {
                PhotonView pv = go.AddComponent<PhotonView>();
                pv.viewID = PhotonNetwork.AllocateViewID();
                networkingPeer.RegisterPhotonView(pv);

                for (int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject child = go.transform.GetChild(i).gameObject;
                    PhotonView childPV = child.AddComponent<PhotonView>();
                    childPV.viewID = PhotonNetwork.AllocateViewID();
                }
            }

            Component[] views = (Component[])go.GetPhotonViewsInChildren();
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++)
            {
                //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
                viewIDs[i] = PhotonNetwork.AllocateViewID();
            }

            // Send to others, create info
            //Hashtable instantiateEvent = networkingPeer.SendInstantiate(prefabName, position, rotation, group, viewIDs, data, false);
            RaiseEventOptions options = new RaiseEventOptions();
            bool isGlobalObject = false;
            options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

            PhotonView[] photonViews = go.GetPhotonViewsInChildren();
            for (int i = 0; i < photonViews.Length; i++)
            {
                photonViews[i].didAwake = false;
                photonViews[i].viewID = 0;

                //photonViews[i].prefix = objLevelPrefix;
                photonViews[i].prefix = networkingPeer.currentLevelPrefix;
                //photonViews[i].instantiationId = instantiationId;
                photonViews[i].instantiationId = go.GetComponent<PhotonView>().viewID;
                photonViews[i].isRuntimeInstantiated = true;
                //photonViews[i].instantiationDataField = incomingInstantiationData;
                photonViews[i].instantiationDataField = null;

                photonViews[i].didAwake = true;
                //photonViews[i].viewID = viewsIDs[i];    // with didAwake true and viewID == 0, this will also register the view
                photonViews[i].viewID = viewIDs[i];
            }

            // Send OnPhotonInstantiate callback to newly created GO.
            // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
            go.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(PhotonNetwork.player, PhotonNetwork.ServerTimestamp, null), SendMessageOptions.DontRequireReceiver);

            // Instantiate the GO locally (but the same way as if it was done via event). This will also cache the instantiationId
            //return networkingPeer.DoInstantiate(instantiateEvent, networkingPeer.LocalPlayer, prefabGo);

            PhotonNetwork.RPC(go.GetPhotonView(), "InstantiateOnRemote", PhotonTargets.Others, false, prefabName);

            return go;
        }

        [PunRPC]
        public void InstantiateOnRemote(string prefabName)
        {
            bool connected = PhotonNetwork.connectedAndReady;
            bool UsePrefabCache = true;
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            GameObject prefabGo;
            if (!UsePrefabCache || !PhotonNetwork.PrefabCache.TryGetValue(prefabName, out prefabGo))
            {
                //List<string> prefabFolderPossibilities = new List<string>();
                prefabGo = (GameObject)Resources.Load(prefabName, typeof(GameObject));
                if (prefabGo == null)
                {
                    string directory = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "ASL/Resources");
                    directory = ConvertToResourcePath(directory);
                    ResourceDive(prefabName, directory);
                }
                if (UsePrefabCache)
                {
                    PhotonNetwork.PrefabCache.Add(prefabName, prefabGo);
                }
            }

            if (prefabGo == null)
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
                return;
            }

            //// a scene object instantiated with network visibility has to contain a PhotonView
            //if (prefabGo.GetComponent<PhotonView>() == null)
            //{
            //    PhotonView pv = prefabGo.AddComponent<PhotonView>();
            //    List<int> pvIDList = new List<int>(networkingPeer.photonViewList.Keys);
            //    int maxObjectID = 1;
            //    foreach(int id in pvIDList)
            //    {
            //        if(id % PhotonNetwork.MAX_VIEW_IDS > maxObjectID)
            //        {
            //            maxObjectID = id % PhotonNetwork.MAX_VIEW_IDS;
            //        }
            //    }
            //    pv.viewID = PhotonNetwork.player.ID * PhotonNetwork.MAX_VIEW_IDS + maxObjectID;
            //    networkingPeer.RegisterPhotonView(pv);

            //    //Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            //    //return null;
            //}

            GameObject go = GameObject.Instantiate(prefabGo);
            if (go.GetComponent<PhotonView>() == null)
            {
                PhotonView pv = go.AddComponent<PhotonView>();
                pv.viewID = PhotonNetwork.AllocateViewID();
                networkingPeer.RegisterPhotonView(pv);

                for (int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject child = go.transform.GetChild(i).gameObject;
                    PhotonView childPV = child.AddComponent<PhotonView>();
                    childPV.viewID = PhotonNetwork.AllocateViewID();
                }
            }

            Component[] views = (Component[])go.GetPhotonViewsInChildren();
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++)
            {
                //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
                viewIDs[i] = PhotonNetwork.AllocateViewID();
            }

            // Send to others, create info
            //Hashtable instantiateEvent = networkingPeer.SendInstantiate(prefabName, position, rotation, group, viewIDs, data, false);
            RaiseEventOptions options = new RaiseEventOptions();
            bool isGlobalObject = false;
            options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

            PhotonView[] photonViews = go.GetPhotonViewsInChildren();
            for (int i = 0; i < photonViews.Length; i++)
            {
                photonViews[i].didAwake = false;
                photonViews[i].viewID = 0;

                //photonViews[i].prefix = objLevelPrefix;
                photonViews[i].prefix = networkingPeer.currentLevelPrefix;
                //photonViews[i].instantiationId = instantiationId;
                photonViews[i].instantiationId = go.GetComponent<PhotonView>().viewID;
                photonViews[i].isRuntimeInstantiated = true;
                //photonViews[i].instantiationDataField = incomingInstantiationData;
                photonViews[i].instantiationDataField = null;

                photonViews[i].didAwake = true;
                //photonViews[i].viewID = viewsIDs[i];    // with didAwake true and viewID == 0, this will also register the view
                photonViews[i].viewID = viewIDs[i];
            }
        }

        public void CreatePUNObject(GameObject obj)
        {

        }

        private GameObject ResourceDive(string prefabName, string directory)
        {
            GameObject prefabGo = (GameObject)Resources.Load(prefabName, typeof(GameObject));

            if (prefabGo == null)
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string dir in subdirectories)
                {
                    string resourceDirectory = ConvertToResourcePath(dir);
                    prefabGo = ResourceDive(prefabName, resourceDirectory);
                    if (prefabGo != null)
                    {
                        break;
                    }
                }
            }

            return prefabGo;
        }

        private string ConvertToResourcePath(string directory)
        {
            return string.Join("/", directory.Split('\\'));
        }
    }
}