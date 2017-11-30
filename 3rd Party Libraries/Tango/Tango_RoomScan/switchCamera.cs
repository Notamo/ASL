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

    int count = 0;

    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        //for each object, if object is a room or run time init object, set to marker transform
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

        if (count < 180)
        {

            SetText(Dynamic.transform.position.ToString() + " " + Dynamic.transform.rotation.eulerAngles.ToString(), Color.white);
            count++;

            if(count == 180)
            {
                SetText(" ", Color.white);
            }
        }

    }

    /// <summary>
    /// Clears the Dynamic Mesh
    /// </summary>
    public void Clear()
    {
        DynamicMesh.GetComponent<TangoDynamicMesh>().Clear();
        TangoManager.GetComponent<TangoApplication>().Tango3DRClear();
    }

    /// <summary>
    /// Sets the current Dynamic Parent Game Object to marker transform
    /// </summary>
    /// <param name="T"></param>
    public void setWorldOffset(Transform T)
    {
        if (QR == false)
        {
            Dynamic.transform.position = T.transform.position;
            Dynamic.transform.rotation = T.transform.rotation;
            QR = true;

        }
    }

    /// <summary>
    /// Toggles on/off the Dynamic Mesh
    /// </summary>
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

    public void ToggleARCamera()
    {
        if (Camera.GetComponent<Camera>().clearFlags == CameraClearFlags.Skybox)
        {
            Camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
        }
        else
        {
            Camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        }
    }

    /// <summary>
    /// Set Tango Debug Text
    /// </summary>
    /// <param name="M"></param>
    public void SetText(string s, Color c)
    {
        Te.text = s;
        Te.color = c;
    }
}
