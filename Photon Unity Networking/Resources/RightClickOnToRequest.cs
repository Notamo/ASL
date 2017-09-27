using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(PhotonView))]
public class RightClickOnToRequest : Photon.MonoBehaviour
{
    
    public void OnPressRight ()
    {

        Debug.Log("Detected Right Click!");

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Debug.Log("Attempting to give up Ownership");
            this.photonView.RPC("release", PhotonTargets.AllBuffered);
        }
        else
        {
            Debug.Log("Attempting to take Ownership");
            this.photonView.RPC("grabWithDelay", PhotonTargets.AllBuffered, 10000);
        }
        
    }

}
