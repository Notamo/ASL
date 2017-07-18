using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aslBase : Photon.PunBehaviour {

    byte evCode = 101;
    bool reliable = true;

	// Use this for initialization
	virtual protected void Start () {
		
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		
	}

    // Fire an event when instantiated
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("aslBase: OnPhotonInstantiate called.");
        Debug.Log("Info: " + info);

        // Call base class method
        base.OnPhotonInstantiate(info);

        //Fire event for Master Client to catch
        //Send this' viewID as an int so that the MasterClient can request it
        //(which lets the MasterClient decide *how* to take control of the PhotonView/GameObject
        PhotonView view = this.gameObject.GetPhotonView();

        PhotonNetwork.RaiseEvent(evCode, view.viewID, reliable, null);

    }
}

