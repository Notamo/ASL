﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public class ObjectManager : MonoBehaviour
    {
        #region Fields
        private const byte EV_INSTANTIATE = 99;
        private const byte EV_DESTROYOBJECT = 98;
        private string resourceFolderPath;
        #endregion

        #region Methods
        public void Awake()
        {
            PhotonNetwork.OnEventCall += OnEvent;
            resourceFolderPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "ASL/Resources");
        }

        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation)
        {
            GameObject go = Instantiate(prefabName);
            go.transform.position = position;
            go.transform.rotation = rotation;

            return go;
        }

        public GameObject Instantiate(GameObject go)
        {
            if (PhotonNetwork.connectedAndReady)
            {
                go = HandleLocalLogic(go);
                RaiseInstantiateEventHandler(go);

                return go;
            }
            else
            {
                Debug.LogError("Photon network not yet connected. State = " + PhotonNetwork.connectionState);
                return null;
            }
        }

        // Emulates PUN object creation across the PUN network
        public GameObject Instantiate(string prefabName)
        {
            if (PhotonNetwork.connectedAndReady)
            {
                GameObject localObj = InstantiateLocally(prefabName);

                if (localObj != null)
                {
                    RaiseInstantiateEventHandler(localObj);
                }
                return localObj;
            }
            else
            {
                Debug.LogError("Photon Network not yet connected. State = " + PhotonNetwork.connectionState);
                return null;
            }
        }

        // This function exists to be called when a gameobject is destroyed 
        // since OnDestroy callbacks are local to the GameObject being destroyed
        public void DestroyObject(string objectName, int viewID)
        {
            GameObject go = LocateObjectToDestroy(objectName, viewID);
            RaiseDestroyObjectEventHandler(go);
            HandleLocalDestroyLogic(go);
        }

        #region Helper Functions
        private GameObject InstantiateLocally(string prefabName)
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

            HandleLocalLogic(go);

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

        private GameObject HandleLocalLogic(GameObject go)
        {
            go = HandlePUNStuff(go);
            go = SynchCustomScripts(go);

            return go;
        }

        private GameObject HandlePUNStuff(GameObject go)
        {
            return HandlePUNStuff(go, null);
        }

        private GameObject HandlePUNStuff(GameObject go, int[] viewIDs)
        {
            go = AttachPhotonViews(go);
            go = AttachPhotonTransformViews(go);
            if(viewIDs == null)
            {
                SetViewIDs(ref go);
            }
            else
            {
                SynchViewIDs(go, viewIDs);
            }

            AddressFinalPUNSynch(go);

            return go;
        }

        #region PUN Stuff
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
            if (resourcePath.Length > 0)
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
                if (view.ObservedComponents == null)
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
                    if (childView.ObservedComponents == null)
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
        
        private void AddressFinalPUNSynch(GameObject go)
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

        #endregion

        #region PUN Event Stuff
        private void RaiseInstantiateEventHandler(GameObject go)
        {
            //Debug.Log("Attempting to raise event for instantiation");

            NetworkingPeer peer = PhotonNetwork.networkingPeer;

            byte[] content = new byte[2];
            ExitGames.Client.Photon.Hashtable instantiateEvent = new ExitGames.Client.Photon.Hashtable();
            string prefabName = go.name;
            instantiateEvent[(byte)0] = prefabName;

            if (go.transform.position != Vector3.zero)
            {
                instantiateEvent[(byte)1] = go.transform.position;
            }

            if (go.transform.rotation != Quaternion.identity)
            {
                instantiateEvent[(byte)2] = go.transform.rotation;
            }
            
            int[] viewIDs = ExtractPhotonViewIDs(go);
            instantiateEvent[(byte)3] = viewIDs;

            if (peer.currentLevelPrefix > 0)
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

        private void RaiseDestroyObjectEventHandler(GameObject go)
        {
            //Debug.Log("Attempting to raise event for destruction of object");

            NetworkingPeer peer = PhotonNetwork.networkingPeer;
            
            ExitGames.Client.Photon.Hashtable destroyObjectEvent = new ExitGames.Client.Photon.Hashtable();
            string objectName = go.name;
            destroyObjectEvent[(byte)0] = objectName;

            // need the viewID
            
            PhotonView[] views = go.GetPhotonViewsInChildren();
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < views.Length; i++)
            {
                viewIDs[i] = views[i].viewID;
            }
            destroyObjectEvent[(byte)1] = viewIDs;

            destroyObjectEvent[(byte)2] = PhotonNetwork.ServerTimestamp;

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.Others;
            PhotonNetwork.RaiseEvent(EV_DESTROYOBJECT, destroyObjectEvent, true, options);
        }
        
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
            else if (eventCode.Equals(EV_DESTROYOBJECT))
            {
                RemoteDestroyObject((ExitGames.Client.Photon.Hashtable)content);
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

            InstantiateLocally(prefabName, viewIDs, position, rotation);
        }
        
        private void InstantiateLocally(string prefabName, int[] viewIDs, Vector3 position, Quaternion rotation)
        {
            GameObject prefabGo;
            if (!RetrieveFromPUNCache(prefabName, out prefabGo))
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ".");
                return;
            }

            GameObject go = GameObject.Instantiate(prefabGo);
            go.name = prefabGo.name;

            HandleLocalLogic(go, viewIDs);
            go.transform.position = position;
            go.transform.rotation = rotation;
        }

        private GameObject HandleLocalLogic(GameObject go, int[] viewIDs)
        {
            go = HandlePUNStuff(go, viewIDs);
            go = SynchCustomScripts(go);

            return go;
        }

        private void RemoteDestroyObject(ExitGames.Client.Photon.Hashtable eventData)
        {
            string objectName = (string)eventData[(byte)0];
            PhotonView[] views = (PhotonView[])eventData[(byte)1];
            int timeStamp = (int)eventData[(byte)2];

            // Handle destroy logic
            GameObject go = LocateObjectToDestroy(objectName, views[0].viewID);
            
        }
        
        private void HandleLocalDestroyLogic(GameObject go)
        {
            if (go != null)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    HandleLocalDestroyLogic(go.transform.GetChild(i).gameObject);
                }

                PhotonView view = go.GetComponent<PhotonView>();
                if (view != null)
                {
                    // Clear up the local view and delete it from the registration list
                    PhotonNetwork.networkingPeer.LocalCleanPhotonView(view);
                }

                GameObject.Destroy(go);
            }
        }

        #endregion

        private GameObject LocateObjectToDestroy(string objectName, int viewID)
        {
            GameObject objectToDestroy = null;
            GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
            foreach(GameObject go in goArray)
            {
                if (go.name.Equals(objectName))
                {
                    PhotonView view = go.GetComponent<PhotonView>();
                    if(view != null)
                    {
                        if(viewID == view.viewID)
                        {
                            objectToDestroy = go;
                            break;
                        }
                    }
                }
            }

            return objectToDestroy;
        }

        private GameObject SynchCustomScripts(GameObject go)
        {
            go.AddComponent<UWBNetworkingPackage.OwnableObject>();
            go.AddComponent<UWBNetworkingPackage.DestroyObjectSynchronizer>();

            return go;
        }

        private int[] ExtractPhotonViewIDs(GameObject go)
        {
            PhotonView[] views = go.GetPhotonViewsInChildren();
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++)
            {
                viewIDs[i] = views[i].viewID;
            }

            return viewIDs;
        }
        #endregion
#endregion
    }
}