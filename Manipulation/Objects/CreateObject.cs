using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ASL.Manipulation.Objects
{
    public class CreateObject : MonoBehaviour
    {
        public GameObject CreatePUNObject(string prefabName, Vector3 position, Quaternion rotation)
        {
            GameObject go = CreatePUNObject(prefabName);
            go.transform.position = position;
            go.transform.rotation = rotation;

            return go;
        }

        public GameObject CreatePUNObject(GameObject go)
        {
            if (PhotonNetwork.connected)
            {
                go = AttachPhotonViews(go);
                go = AttachPhotonTransformViews(go);
                go = SetViewIDs(ref go);
                HandlePUNStuff(go);

                PhotonView[] views = go.GetPhotonViewsInChildren();
                int[] viewIDs = new int[views.Length];
                for (int i = 0; i < viewIDs.Length; i++)
                {
                    viewIDs[i] = views[i].viewID;
                }
                PhotonNetwork.RPC(go.GetPhotonView(), "InstantiateOnRemote", PhotonTargets.Others, false, go.name, viewIDs);

                return go;
            }
            else
            {
                Debug.LogError("Photon network not yet connected. State = " + PhotonNetwork.connectionState);
                return null;
            }
        }

        // Emulates PUN object creation across the PUN network
        public GameObject CreatePUNObject(string prefabName)
        {
            if (PhotonNetwork.connected)
            {
                GameObject localObj = InstantiateOnLocal(prefabName);

                if (localObj != null)
                {
                    if (PhotonNetwork.connectedAndReady)
                    {
                        PhotonView[] views = localObj.GetPhotonViewsInChildren();
                        int[] viewIDs = new int[views.Length];
                        for (int i = 0; i < viewIDs.Length; i++)
                        {
                            viewIDs[i] = views[i].viewID;
                        }
                        PhotonNetwork.RPC(localObj.GetPhotonView(), "InstantiateOnRemote", PhotonTargets.Others, false, prefabName, viewIDs);
                    }
                }
                return localObj;
            }
            else
            {
                Debug.LogError("Photon Network not yet connected. State = " + PhotonNetwork.connectionState);
                return null;
            }
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
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ".");
                return null;
            }
            GameObject go = GameObject.Instantiate(prefabGo);
            go = AttachPhotonViews(go);
            go = AttachPhotonTransformViews(go);
            go = SetViewIDs(ref go);

            //Debug.Log("ViewID of object after exiting SetViewIDs method = " + go.GetComponent<PhotonView>().viewID);

            HandlePUNStuff(go);

            go.AddComponent<UWBNetworkingPackage.OwnableObject>();

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

            go.AddComponent<UWBNetworkingPackage.OwnableObject>();
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
            //NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            //Debug.Log("Attempting to attach photon view");

            // Generate and attach Photon Views
            if (go.GetComponent<PhotonView>() == null)
            {
                //Debug.Log("Photon view not found...attaching...");

                PhotonView pv = go.AddComponent<PhotonView>();
                //pv.viewID = PhotonNetwork.AllocateViewID();
                //networkingPeer.RegisterPhotonView(pv);

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

        private GameObject SetViewIDs(ref GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            //Component[] views = (Component[])go.GetPhotonViewsInChildren();
            //PhotonView[] views = go.GetPhotonViewsInChildren();

            PhotonView[] views = new PhotonView[go.GetPhotonViewsInChildren().Length];
            views[0] = go.GetComponent<PhotonView>();
            for(int i = 0; i < go.transform.childCount; i++)
            {
                views[i+1] = go.transform.gameObject.GetComponent<PhotonView>();
            }

            //Debug.Log("Found " + views.Length + " photon views in " + go.name + " object and its children");
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++) // ignore the main gameobject
            {
                //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
                viewIDs[i] = PhotonNetwork.AllocateViewID();
                //Debug.Log("Allocated an id of " + viewIDs[i]);
                views[i].viewID = viewIDs[i];
                //views[i].instantiationId = viewIDs[i];
                //Debug.Log("Assigning view id of " + viewIDs[i] + ", so now the view id is " + go.GetPhotonView().viewID + " for gameobject " + go.name);
                networkingPeer.RegisterPhotonView(views[i]);
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
                //photonViews[i].viewID = 0; // why is this included in the original?

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