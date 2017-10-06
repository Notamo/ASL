using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects
{
    public class ObjectInteractionManager : MonoBehaviour
    {
        private UWBNetworkingPackage.NodeType platform;
        public event ObjectSelectedEventHandler FocusObjectChangedEvent;

        public void Focus(GameObject obj)
        {
            OnObjectSelected(obj);
        }

        protected void OnObjectSelected(GameObject obj)
        {
            Debug.Log("About to trigger On Object Selected event");
            FocusObjectChangedEvent(new ObjectSelectedEventArgs(null, obj));
            Debug.Log("Event triggered");
        }

        public void Awake()
        {
#if UNITY_WSA_10_0
#elif UNITY_ANDROID
#else
            UWBNetworkingPackage.NodeType platform = UWBNetworkingPackage.Config.NodeType;

            gameObject.AddComponent<MoveObject>();
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