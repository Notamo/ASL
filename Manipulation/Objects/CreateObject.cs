﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ExitGames.Client.Photon;

namespace ASL.Manipulation.Objects
{
    // PhotonHandler.cs updates things every 1 second (?)
    public class CreateObject : MonoBehaviour
    {
        private const byte EV_INSTANTIATE = 99;
        private string resourceFolderPath;

        public void Awake()
        {
            PhotonNetwork.OnEventCall += OnEvent;
            resourceFolderPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "ASL/Resources");
        }

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
                //PhotonNetwork.RPC(go.GetPhotonView(), "InstantiateOnRemote", PhotonTargets.Others, false, go.name, viewIDs);
                RaiseEventHandler(go);

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

                        //Debug.Log("Attemping to send RPC now...");
                        if (localObj.GetPhotonView() != null)
                        {
                            //Debug.Log("Locally created object contains valid Photon view. View ID = " + localObj.GetPhotonView().viewID);
                            bool isRPC = ASL.Adapters.PUN.RPCManager.IsAnRPC("InstantiateOnRemote");
                            //Debug.Log("InstantiateOnRemote is an RPC?" + isRPC);
                            //Debug.Log("prefab name = " + prefabName);
                            //Debug.Log("length of viewIDs = " + viewIDs.Length);
                        }
                        //PhotonNetwork.RPC(localObj.GetPhotonView(), "InstantiateOnRemote", PhotonTargets.Others, false, prefabName, viewIDs);
                        RaiseEventHandler(localObj);
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
            if (!RetrieveFromPUNCache(prefabName, out prefabGo))
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ".");
                return null;
            }
            GameObject go = GameObject.Instantiate(prefabGo);
            go.name = prefabGo.name;
            go = AttachPhotonViews(go);
            go = AttachPhotonTransformViews(go);
            go = SetViewIDs(ref go);

            //Debug.Log("ViewID of object after exiting SetViewIDs method = " + go.GetComponent<PhotonView>().viewID);

            HandlePUNStuff(go);

            go.AddComponent<UWBNetworkingPackage.OwnableObject>();

            return go;
        }

        //[PunRPC]
        //public void InstantiateOnRemote(string prefabName, int[] viewIDs)
        //{
        //    GameObject prefabGo;
        //    if (!RetrieveFromPUNCache(prefabName, out prefabGo))
        //    {
        //        Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
        //        return;
        //    }

        //    GameObject go = GameObject.Instantiate(prefabGo);
        //    go = AttachPhotonViews(go);
        //    go = AttachPhotonTransformViews(go);
        //    SynchViewIDs(go, viewIDs);
        //    HandlePUNStuff(go);

        //    go.AddComponent<UWBNetworkingPackage.OwnableObject>();
        //}

        private GameObject ResourceDive(string prefabName, string directory)
        {
            string resourcePath = ConvertToResourcePath(directory, prefabName);
            GameObject prefabGo = (GameObject)Resources.Load(resourcePath, typeof(GameObject));
            
            if (prefabGo == null)
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string dir in subdirectories)
                {
                    prefabGo = ResourceDive(prefabName, dir);
                    if (prefabGo != null)
                    {
                        break;
                    }
                }
            }

            return prefabGo;
        }

        private string ConvertToResourcePath(string directory, string prefabName)
        {
            string resourcePath = directory.Substring(directory.IndexOf("Resources") + "Resources".Length);
            if(resourcePath.Length > 0)
            {
                resourcePath = resourcePath.Substring(1) + '/' + prefabName;
                resourcePath.Replace('\\', '/');
            }
            else
            {
                resourcePath = prefabName;
            }

            return resourcePath;
            //return string.Join("/", directory.Split('\\')) + "/" + prefabName;
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
                pv.synchronization = ViewSynchronization.UnreliableOnChange;

                for (int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject child = go.transform.GetChild(i).gameObject;
                    PhotonView childPV = child.AddComponent<PhotonView>();
                    //childPV.viewID = PhotonNetwork.AllocateViewID();
                    childPV.synchronization = ViewSynchronization.UnreliableOnChange;
                }
            }

            return go;
        }

        private GameObject AttachPhotonTransformViews(GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            if (go.GetComponent<UWBPhotonTransformView>() == null)
            {
                UWBPhotonTransformView ptv = go.AddComponent<UWBPhotonTransformView>();
                ptv.enableSyncPos();
                ptv.enableSyncRot();
                ptv.enableSyncScale();

                PhotonView view = go.GetComponent<PhotonView>();
                if(view.ObservedComponents == null)
                {
                    view.ObservedComponents = new List<Component>();
                }
                view.ObservedComponents.Add(ptv);

                for (int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject child = go.transform.GetChild(i).gameObject;
                    UWBPhotonTransformView childPTV = child.AddComponent<UWBPhotonTransformView>();
                    childPTV.enableSyncPos();
                    childPTV.enableSyncRot();
                    childPTV.enableSyncScale();

                    PhotonView childView = child.GetComponent<PhotonView>();
                    if(childView.ObservedComponents == null)
                    {
                        childView.ObservedComponents = new List<Component>();
                    }
                    childView.ObservedComponents.Add(childPTV);
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
            for (int i = 0; i < go.transform.childCount; i++)
            {
                views[i + 1] = go.transform.gameObject.GetComponent<PhotonView>();
            }

            //Debug.Log("Found " + views.Length + " photon views in " + go.name + " object and its children");
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++) // ignore the main gameobject
            {
                //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
                viewIDs[i] = PhotonNetwork.AllocateViewID();
                //Debug.Log("Allocated an id of " + viewIDs[i]);
                views[i].viewID = viewIDs[i];
                views[i].instantiationId = viewIDs[i];
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
                    string directory = resourceFolderPath;
                    //directory = ConvertToResourcePath(directory);
                    prefabGo = ResourceDive(prefabName, directory);
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
            if (viewIDs != null && viewIDs.Length > 0)
            {
                go.GetPhotonView().instantiationId = viewIDs[0];
            }
        }


        private void RaiseEventHandler(GameObject go)
        {
            //Debug.Log("Attempting to raise event for instantiation");

            NetworkingPeer peer = PhotonNetwork.networkingPeer;
            
            byte[] content = new byte[2];
            ExitGames.Client.Photon.Hashtable instantiateEvent = new ExitGames.Client.Photon.Hashtable();
            string prefabName = go.name;
            instantiateEvent[(byte)0] = prefabName;

            if(go.transform.position != Vector3.zero)
            {
                instantiateEvent[(byte)1] = go.transform.position;
            }

            if(go.transform.rotation != Quaternion.identity)
            {
                instantiateEvent[(byte)2] = go.transform.rotation;
            }

            PhotonView[] views = go.GetPhotonViewsInChildren();
            int[] viewIDs = new int[views.Length];
            for(int i = 0; i < views.Length; i++)
            {
                viewIDs[i] = views[i].viewID;
            }
            instantiateEvent[(byte)3] = viewIDs;
            
            if(peer.currentLevelPrefix > 0)
            {
                instantiateEvent[(byte)4] = peer.currentLevelPrefix;
            }

            instantiateEvent[(byte)5] = PhotonNetwork.ServerTimestamp;
            instantiateEvent[(byte)6] = go.GetPhotonView().instantiationId;

            //RaiseEventOptions options = new RaiseEventOptions();
            //options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

            //Debug.Log("All items packed. Attempting to literally raise event now.");

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.Others;
            PhotonNetwork.RaiseEvent(EV_INSTANTIATE, instantiateEvent, true, options);

            //peer.OpRaiseEvent(EV_INSTANTIATE, instantiateEvent, true, null);
        }

        //private void OnEvent(EventData photonEvent)
        //{
            //if(PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            //{
            //    Debug.Log(string.Format("Custom OnEvent for CreateObject: {0}", photonEvent.ToString()));
            //}

            //int actorNr = -1;
            //PhotonPlayer originatingPlayer = null;

            //if (photonEvent.Parameters.ContainsKey(ParameterCode.ActorNr))
            //{
            //    actorNr = (int)photonEvent[ParameterCode.ActorNr];
            //    originatingPlayer = PhotonNetwork.networkingPeer.GetPlayerWithId(actorNr);
            //}

            //if (photonEvent.Code.Equals(EV_INSTANTIATE))
            //{
            //    //RemoteInstantiate((ExitGames.Client.Photon.Hashtable)photonEvent[ParameterCode.Data], originatingPlayer, null);
            //    RemoteInstantiate((ExitGames.Client.Photon.Hashtable)photonEvent[ParameterCode.Data]);
            //}
        //}

        private void OnEvent(byte eventCode, object content, int senderID)
        {
            Debug.Log("OnEvent method triggered.");

            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.Log(string.Format("Custom OnEvent for CreateObject: {0}", eventCode.ToString()));
            }

            //int actorNr = -1;
            //PhotonPlayer originatingPlayer = null;

            //if (photonEvent.Parameters.ContainsKey(ParameterCode.ActorNr))
            //{
            //    actorNr = (int)photonEvent[ParameterCode.ActorNr];
            //    originatingPlayer = PhotonNetwork.networkingPeer.GetPlayerWithId(actorNr);
            //}

            //if (photonEvent.Code.Equals(EV_INSTANTIATE))
            //{
            //    //RemoteInstantiate((ExitGames.Client.Photon.Hashtable)photonEvent[ParameterCode.Data], originatingPlayer, null);
            //    RemoteInstantiate((ExitGames.Client.Photon.Hashtable)photonEvent[ParameterCode.Data]);
            //}

            if (eventCode.Equals(EV_INSTANTIATE))
            {
                RemoteInstantiate((ExitGames.Client.Photon.Hashtable)content);
            }
        }

        private void RemoteInstantiate(ExitGames.Client.Photon.Hashtable eventData)
        {
            string prefabName = (string)eventData[(byte)0];
            Vector3 position = Vector3.zero;
            if (eventData.ContainsKey((byte)1))
            {
                position = (Vector3)eventData[(byte)1];
            }
            Quaternion rotation = Quaternion.identity;
            if (eventData.ContainsKey((byte)2))
            {
                rotation = (Quaternion)eventData[(byte)2];
            }
            
            int[] viewIDs = (int[])eventData[(byte)3];
            if (eventData.ContainsKey((byte)4))
            {
                uint currentLevelPrefix = (uint)eventData[(byte)4];
            }

            int serverTimeStamp = (int)eventData[(byte)5];
            int instantiationID = (int)eventData[(byte)6];

            InstantiateOnRemote(prefabName, viewIDs, position, rotation);
        }
        
        private void InstantiateOnRemote(string prefabName, int[] viewIDs, Vector3 position, Quaternion rotation)
        {
            GameObject prefabGo;
            if (!RetrieveFromPUNCache(prefabName, out prefabGo))
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ".");
                return;
            }

            GameObject go = GameObject.Instantiate(prefabGo);
            go.name = prefabGo.name;
            go = AttachPhotonViews(go);
            go = AttachPhotonTransformViews(go);
            SynchViewIDs(go, viewIDs);
            HandlePUNStuff(go);

            go.transform.position = position;
            go.transform.rotation = rotation;

            go.AddComponent<UWBNetworkingPackage.OwnableObject>();
        }
    }
}