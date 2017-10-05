using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects
{
    public class MoveObject : MonoBehaviour
    {
        
        public GameObject focusObject;
        private float moveScale = 0.30f;

        public void Awake()
        {
            GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>().FocusObjectChangedEvent += SetObject;
        }

        private void SetObject(ObjectSelectedEventArgs e)
        {
            focusObject = e.FocusObject;
        }

        public void Up()
        {
            if(focusObject != null)
            {
                focusObject.transform.Translate(Vector3.up * MoveScale);
            }
        }

        public void Down()
        {
            if (focusObject != null)
            {
                focusObject.transform.Translate(Vector3.down * MoveScale);
            }
        }

        public void Left()
        {
            if (focusObject != null)
            {
                focusObject.transform.Translate(Vector3.left * MoveScale);
            }
        }

        public void Right()
        {
            if (focusObject != null)
            {
                focusObject.transform.Translate(Vector3.right * MoveScale);
            }
        }

        #region Properties
        public float MoveScale
        {
            get
            {
                return moveScale;
            }
            set
            {
                if (value > 0.0f)
                {
                    moveScale = value;
                }
            }
        }
#endregion
    }
}