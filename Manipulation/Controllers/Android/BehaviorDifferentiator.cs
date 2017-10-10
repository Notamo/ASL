using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Controllers.Android
{
    public class BehaviorDifferentiator : MonoBehaviour
    {

#if UNITY_ANDROID || UNITY_EDITOR
        private TapBehavior tapBehavior;
        private PinchBehavior pinchBehavior;

        public void Awake()
        {
            gameObject.AddComponent<TapBehavior>();
            gameObject.AddComponent<PinchBehavior>();

            // Remove the following AddComponent, separate general PUN creation/synchronization behavior in a different file, and then make sure that objectinteractionmanager script attaches it automatically
            gameObject.AddComponent<ASL.Manipulation.Objects.CreateObject>();
        }

        public void FixedUpdate()
        {
            Touch[] touches = Input.touches;
            if (touches.Length == 1)
            {
                tapBehavior.Handle(touches[0]);
            }
            if (touches.Length == 2)
            {
                pinchBehavior.Handle(touches);
            }
        }
#endif
    }
}