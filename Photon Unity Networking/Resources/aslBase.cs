using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enumeration of event codes for ASL (ASLE[num])
public enum ASLE
    {
        INSTANTIATE = 101,
        REQOWN,
        RETURNOWN
    };

public class aslBase : Photon.PunBehaviour {

    private bool RELIABLE = true;
    private List<PhotonPlayer> requestors = new List<PhotonPlayer>();

	// Use this for initialization
	virtual protected void Start () {
		
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		
	}

    // Fire an event when instantiated
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {

        this.gameObject.GetPhotonView().TransferOwnership(0);
      //  this.gameObject.GetPhotonView().TransferOwnership(PhotonNetwork.masterClient);

        //// use the event code for instantiation
        //byte evCode = (byte)ASLE.INSTANTIATE;

        //Debug.Log("aslBase: OnPhotonInstantiate called.");
        //Debug.Log("Info: " + info);

        //// Call base class method
        //base.OnPhotonInstantiate(info);

        ////Fire event for Master Client to catch
        ////Send this' viewID as an int so that the MasterClient can request it
        ////(which lets the MasterClient decide *how* to take control of the PhotonView/GameObject
        //PhotonView view = this.gameObject.GetPhotonView();

        //// Send event; MasterClientLauncher should be the only registered handler
        //PhotonNetwork.RaiseEvent(evCode, view.viewID, RELIABLE, null);
    }

    [PunRPC]
    public bool requestOwnership(PhotonMessageInfo info)
    {
        // Assume the caller did not get ownership
        bool gotOwnership = false;

        // Don't allow multiple requests; return false after first request by a player
        if (!requestors.Contains(info.sender))
        {
            // Add requestor to list of requests
            requestors.Add(info.sender);

            // If the requesting player is at the front of the line, pass ownership to that player.
            if (requestors[0] == info.sender)
            {
                // Compile info for ownership request - evCode, this' view, requesting player
                byte evCode = (byte)ASLE.REQOWN;
                PhotonView view = this.gameObject.GetPhotonView();
                object[] ownershipArgs = new object[] { view.viewID, info.sender };

                Debug.Log("aslBase: Requesting Ownership of " + view.viewID + " from " + view.owner.ID + " to " + info.sender.ID);

                // Send event; MasterClientLauncher should be the only registered handler
                PhotonNetwork.RaiseEvent(evCode, ownershipArgs, RELIABLE, null);

                gotOwnership = true;
            }
        }

        return gotOwnership;
    }

    [PunRPC]
    public bool relinquishOwnership(PhotonMessageInfo info)
    {
        bool returnedOwnership = false;
        PhotonPlayer nextPlayer;

        // Only the current owner can return ownership
        if (requestors.Count >= 1 &&  requestors[0] == info.sender)
        {
            // Remove the player from list of requestors
            requestors.Remove(info.sender);

            // If nobody else wants to own this object, return it to the master client
            if (requestors.Count == 0)
            {
                nextPlayer = PhotonNetwork.masterClient;
            }
            else
            {
                nextPlayer = requestors[0];
            }

            // Set info for ownership request - evCode, this' view, requesting player
            byte evCode = (byte)ASLE.RETURNOWN;
            PhotonView view = this.gameObject.GetPhotonView();
            object[] ownershipArgs = new object[] { view.viewID, nextPlayer };

            Debug.Log("aslBase: Returning Ownership of " + view.viewID + " from " + info.sender + " to " + nextPlayer.ID);

            // Send event; MasterClientLauncher should be the only registered handler
            PhotonNetwork.RaiseEvent(evCode, ownershipArgs, RELIABLE, null);
            
            returnedOwnership = true;

        }
        
        return returnedOwnership;
    }
}

