using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tango;
using UnityEngine.UI;

public class switchCamera : UnityEngine.MonoBehaviour
{
    public GameObject Static;
    public GameObject DynamicMesh;
    public GameObject TangoManager;
    public GameObject Camera;
    public GameObject Dynamic;
    private bool QR = false;
    private bool CameraToggle = true;
    public Text Te;

    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject G in GameObject.FindObjectsOfType<GameObject>())
        {
            if (G != Static && !G.transform.parent && G.tag != "Anchor")
            {
                
                //G.transform.position = Dynamic.transform.position;
                if (G.tag == "Room")
                {
                    GameObject D = new GameObject();
                    G.transform.SetParent(D.transform);
                    G.transform.position = Dynamic.transform.position;
                    G.transform.rotation = Dynamic.transform.rotation;
                    G.transform.SetParent(Dynamic.transform);
                    Destroy(D);
                }
                else
                {
                    G.transform.SetParent(Dynamic.transform);
                }

            }
        }

        //Dynamic.transform.Rotate(0, .5f, 0);

        //Te.text = Dynamic.transform.position + " " + Dynamic.transform.rotation + " ";
        //Te.text += GameObject.FindGameObjectWithTag("Room").transform.position;
    }

    public void Clear()
    {
        DynamicMesh.GetComponent<TangoDynamicMesh>().Clear();
        TangoManager.GetComponent<TangoApplication>().Tango3DRClear();
    }

    public void setWorldOffset(Transform T)
    {
        if (QR == false)
        {
            Dynamic.transform.position = T.transform.position;
            Dynamic.transform.rotation = T.transform.rotation;
            QR = true;
            //Te.text = Dynamic.transform.position + " " /*+ T.transform.rotation + " "*/;
        }
    }

    public void ToggleCamera()
    {
        if (DynamicMesh.GetActive() == true)
        {
            Clear();
            DynamicMesh.SetActive(false);
        }
        else
        {
            DynamicMesh.SetActive(true);
        }
    }

    public void SetText(Matrix4x4 M)
    {
        Te.text = M.ToString();
    }
}
