using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;

namespace ASL.Manipulation.Controllers.PC
{
    public class Mouse : MonoBehaviour
    {
        private ObjectInteractionManager objManager;

        public void Awake()
        {
            objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject selectedObject = Select();
                GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>().RequestOwnership(selectedObject, PhotonNetwork.player.ID);
            }
            if (Input.GetMouseButtonDown(1))
            {
                string prefabName = "Sphere";
                Vector3 position = new Vector3(0, 0, 2);
                Quaternion rotation = Quaternion.identity;
                objManager.Instantiate(prefabName, position, rotation);

                //GameObject.CreatePrimitive(PrimitiveType.Cube);

                //Debug.Log("Attempting to PUN-create object");
                //var a = gameObject.AddComponent<CreateObject>();
                //a.CreatePUNObject("Sphere");
                //a.CreatePUNObject("Sphere", new Vector3(2, 3, 4), Quaternion.identity);
                //a.CreatePUNObject("Sphere", new Vector3(0, 0, 2), Quaternion.identity);

                //Debug.Log("Pun-created object instantiated.");
            }

            //if (Input.GetMouseButtonDown(1))
            //{
            //    PhotonNetwork.Instantiate("PUNSphere", Vector3.zero, Quaternion.identity, 0);
            //}
        }

        public GameObject Select()
        {
            Camera cam = GameObject.FindObjectOfType<Camera>();
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseRay = cam.ScreenToWorldPoint(mousePos);
            RaycastHit hit;
            Physics.Raycast(cam.ScreenPointToRay(mousePos), out hit);

            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            else
            {
                GameObject camera = GameObject.Find("Main Camera");
                if(camera != null)
                {
                    return camera;
                }
                else
                {
                    Debug.LogError("Cannot find camera object. Selecting null object.");
                    return null;
                }
            }
        }
    }
}