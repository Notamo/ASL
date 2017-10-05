using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects
{
    public delegate void ObjectSelectedEventHandler(ObjectSelectedEventArgs e);

    public class ObjectSelectedEventArgs : System.EventArgs
    {
        private GameObject oldObject;
        private GameObject focusObject;

        public ObjectSelectedEventArgs(GameObject oldObject, GameObject focusObject)
        {
            this.oldObject = oldObject;
            this.focusObject = focusObject;
        }

        public new ObjectSelectedEventArgs Empty
        {
            get
            {
                return new ObjectSelectedEventArgs(null, null);
            }
        }

        public GameObject OldObject
        {
            get
            {
                return oldObject;
            }
        }
        public GameObject FocusObject
        {
            get
            {
                return focusObject;
            }
        }
    }
}