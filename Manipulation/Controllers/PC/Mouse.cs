using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;

namespace ASL.Manipulation.Controllers.PC
{
    public class Mouse : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject selectedObject = Select();
                GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>().Focus(selectedObject);
                Debug.Log("Mouse click registered!");
            }
        }

        public GameObject Select()
        {
            Camera cam = GameObject.FindObjectOfType<Camera>();
            Vector3 mouseRay = cam.ScreenToWorldPoint(Event.current.mousePosition);
            RaycastHit hit;
            Physics.Raycast(cam.ScreenPointToRay(Event.current.mousePosition), out hit);
            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            else
            {
                return null;
            }
        }
    }
}