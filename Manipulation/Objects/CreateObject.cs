using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ASL.Manipulation.Objects
{
    public class CreateObject : MonoBehaviour
    {
        public GameObject CreatePUNObject(GameObject go)
        {
            go = AttachPhotonViews(go);
            go = AttachPhotonTransformViews(go);
            go = SetViewIDs(go);
            HandlePUNStuff(go);
            
            PhotonView[] viewIDs = go.GetPhotonViewsInChildren();
            PhotonNetwork.RPC(go.GetPhotonView(), "InstantiateOnRemote", PhotonTargets.Others, false, go.name, viewIDs);

            return go;
        }

        // Emulates PUN object creation across the PUN network
        public GameObject CreatePUNObject(string prefabName)
        {
            GameObject localObj = InstantiateOnLocal(prefabName);

            if (localObj != null)
            {
                localObj.AddComponent<UWBNetworkingPackage.OwnableObject>();
                if (PhotonNetwork.connectedAndReady)
                {
                    PhotonView[] viewIDs = localObj.GetPhotonViewsInChildren();
                    PhotonNetwork.RPC(localObj.GetPhotonView(), "InstantiateOnRemote", PhotonTargets.Others, false, prefabName, viewIDs);
                }
            }
            return localObj;
        }

        private GameObject InstantiateOnLocal(string prefabName)
        {
            bool connected = PhotonNetwork.connectedAndReady;
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            // safeguard
            if (!connected)
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + PhotonNetwork.connectionStateDetailed);
                return null;
            }

            // retrieve PUN object from cache
            GameObject prefabGo;
            if(!RetrieveFromPUNCache(prefabName, out prefabGo))
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
                return null;
            }
            GameObject go = GameObject.Instantiate(prefabGo);
            go = AttachPhotonViews(go);
            go = AttachPhotonTransformViews(go);
            go = SetViewIDs(go);

            HandlePUNStuff(go);

            return go;
        }

        [PunRPC]
        public void InstantiateOnRemote(string prefabName, int[] viewIDs)
        {
            GameObject prefabGo;
            if(!RetrieveFromPUNCache(prefabName, out prefabGo))
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
                return;
            }

            GameObject go = GameObject.Instantiate(prefabGo);
            go = AttachPhotonViews(go);
            go = AttachPhotonTransformViews(go);
            SynchViewIDs(go, viewIDs);
            HandlePUNStuff(go);
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

        private GameObject AttachPhotonViews(GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            // Generate and attach Photon Views
            if (go.GetComponent<PhotonView>() == null)
            {
                PhotonView pv = go.AddComponent<PhotonView>();
                //pv.viewID = PhotonNetwork.AllocateViewID();
                networkingPeer.RegisterPhotonView(pv);

                for (int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject child = go.transform.GetChild(i).gameObject;
                    PhotonView childPV = child.AddComponent<PhotonView>();
                    //childPV.viewID = PhotonNetwork.AllocateViewID();
                }
            }

            return go;
        }

        private GameObject AttachPhotonTransformViews(GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            if(go.GetComponent<UWBPhotonTransformView>() == null)
            {
                UWBPhotonTransformView ptv = go.AddComponent<UWBPhotonTransformView>();
                for(int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject child = go.transform.GetChild(i).gameObject;
                    UWBPhotonTransformView childPTV = child.AddComponent<UWBPhotonTransformView>();
                    childPTV.enableSyncPos();
                    childPTV.enableSyncRot();
                    childPTV.enableSyncScale();
                }
            }

            return go;
        }

        private GameObject SetViewIDs(GameObject go)
        {
            Component[] views = (Component[])go.GetPhotonViewsInChildren();
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++) // ignore the main gameobject
            {
                //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
                viewIDs[i] = PhotonNetwork.AllocateViewID();
            }

            return go;
        }

        private bool RetrieveFromPUNCache(string prefabName, out GameObject prefabGo)
        {

            bool UsePrefabCache = true;

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

            return prefabGo != null;
        }

        private void HandlePUNStuff(GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

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
                //photonViews[i].viewID = viewIDs[i];
                photonViews[i].viewID = go.GetPhotonViewsInChildren()[i].viewID;
            }

            // Send OnPhotonInstantiate callback to newly created GO.
            // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
            go.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(PhotonNetwork.player, PhotonNetwork.ServerTimestamp, null), SendMessageOptions.DontRequireReceiver);

            // Instantiate the GO locally (but the same way as if it was done via event). This will also cache the instantiationId
            //return networkingPeer.DoInstantiate(instantiateEvent, networkingPeer.LocalPlayer, prefabGo);
        }

        private void SynchViewIDs(GameObject go, int[] viewIDs)
        {
            PhotonView[] PVs = go.GetPhotonViewsInChildren();
            for (int i = 0; i < PVs.Length; i++)
            {
                PVs[i].viewID = viewIDs[i];
            }
        }
    }
}