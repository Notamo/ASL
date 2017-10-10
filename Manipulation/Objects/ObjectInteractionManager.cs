using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects
{
    public class ObjectInteractionManager : MonoBehaviour
    {
        private UWBNetworkingPackage.NodeType platform;
        public event ObjectSelectedEventHandler FocusObjectChangedEvent;

        public void RequestOwnership(GameObject obj, int focuserID)
        {
            OnObjectSelected(obj, focuserID);
            if (obj.GetPhotonView() != null)
            {
                obj.GetPhotonView().RPC("Grab", PhotonTargets.Others);
            }
        }

        public void Focus(GameObject obj, int focuserID)
        {
            OnObjectSelected(obj, focuserID);
        }

        protected void OnObjectSelected(GameObject obj, int focuserID)
        {
            //Debug.Log("About to trigger On Object Selected event");
            if (obj != null)
            {
                if (obj.GetPhotonView() != null)
                {
                    FocusObjectChangedEvent(new ObjectSelectedEventArgs(obj, obj.GetPhotonView().owner.ID, focuserID));
                }
                else
                {
                    FocusObjectChangedEvent(new ObjectSelectedEventArgs(obj, 0, focuserID));
                }
            }
            else
            {
                FocusObjectChangedEvent(new ObjectSelectedEventArgs(obj, 0, focuserID));
            }
            //Debug.Log("Event triggered");
        }

        public void Awake()
        {
#if UNITY_WSA_10_0
#elif UNITY_ANDROID
            gameObject.AddComponent<ASL.Manipulation.Controllers.Android.BehaviorDifferentiator>();
#else
            UWBNetworkingPackage.NodeType platform = UWBNetworkingPackage.Config.NodeType;

            gameObject.AddComponent<MoveObject>();
            gameObject.AddComponent<CreateObject>();
            gameObject.AddComponent<ASL.Manipulation.Controllers.PC.Mouse>();
            gameObject.AddComponent<ASL.Manipulation.Controllers.PC.Keyboard>();
#endif
        }

        public void FixedUpdate()
        {
            // reset manager if platform is too quick to update properly at startup
            if (platform != UWBNetworkingPackage.Config.NodeType)
            {
                Resources.Load("Prefabs/ObjectInteractionManager");
                //GameObject.Destroy(gameObject);
            }
        }
    }
}