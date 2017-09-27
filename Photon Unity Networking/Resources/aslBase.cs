using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shorthand values for three common values of grab/release return values
public static class ASLSTATE
{
    public const int FAIL = -1;
    public const int NOW = 0;
    public const int NEXT = 1;
}

public class aslBase : Photon.PunBehaviour {

    private int SCENE_VALUE = 0;    // Hidden feature: assigning ownership to '0' makes object a scene object
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

        base.OnPhotonInstantiate(info);
        this.gameObject.GetPhotonView().TransferOwnership(SCENE_VALUE);
        
    }

    [PunRPC]
    public int grab(PhotonMessageInfo info)
    {
        int grabbed = ASLSTATE.FAIL;

        if (this.requestOwnership(info) == 0)
            grabbed = ASLSTATE.NOW;

        return grabbed;
    }

    [PunRPC]
    public int grabWithDelay(int msDelay, PhotonMessageInfo info)
    {
        int grabbed = this.requestOwnership(info);

        // If the ownsershp request _fails_, something is wrong.  Return immediately
        // Otherwise, wait the specified time to receive ownership before aborting the attempt.
        if(grabbed != ASLSTATE.FAIL)
        {
            // Return immediately if the player is granted ownership right away, as well.
            // Otherwise, ask Unity to retry the ownership request.
            if(grabbed != ASLSTATE.NOW)
            {
                StartCoroutine(retryOwnership(msDelay/1000.0f, info));
            }
        }

        return grabbed;
    }

    [PunRPC]
    public int checkOwnership(PhotonMessageInfo info)
    {
        int hasOwnership = requestors.IndexOf(info.sender);

        return hasOwnership;
    }

    // This are hacky kludge.  Kelvin requested "no coroutines", but without a coroutine,
    // any timed loop will lock the players' clients up.  This is the least coroutine I could
    // accomplish: retry requesting ownership on an object after some time has elapsed.
    private IEnumerator retryOwnership(float delay, PhotonMessageInfo info)
    {
        yield return new WaitForSeconds(delay);

        if (this.requestOwnership(info) != ASLSTATE.NOW)
        {
            this.relinquishOwnership(info);      
        }
        
        yield break;
    }

    [PunRPC]
    public int release(PhotonMessageInfo info)
    {
        return relinquishOwnership(info);
    }


    public int requestOwnership(PhotonMessageInfo info)
    {
        // Assume the caller will never get ownership
        int gotOwnership = ASLSTATE.FAIL;

        // Never allow any player to take control of an aslBase object tagged "room", "Room", or "ROOM."
        if(string.Compare(this.tag.ToUpper(), "ROOM") == 0)
        {
            return gotOwnership;
        }


        // Don't allow multiple requests; return false after first request by a player
        if (!requestors.Contains(info.sender))
        {
            // Add requestor to list of requests
            requestors.Add(info.sender);

            // If the requesting player is at the front of the line, pass ownership to that player.
            if (requestors[0] == info.sender)
            {
            
                PhotonView view = this.gameObject.GetPhotonView();
            
                try
                {
                    Debug.Log("aslBase: Requesting Ownership of " + view.viewID + " from " + view.owner.ID + " to " + info.sender.ID);
                }
                catch (System.NullReferenceException e)
                {
                    Debug.Log("aslBase: Requesting Ownership of " + view.viewID + " from <scene> to " + info.sender.ID);
                }

                // Send event; MasterClientLauncher should be the only registered handler
                this.gameObject.GetPhotonView().TransferOwnership(info.sender);

                // The player receives ownership immediately
                gotOwnership = ASLSTATE.NOW;
            }
            // If the player is not the current owner, but is in the list, return their current index in the list
            else
            {
                gotOwnership = requestors.IndexOf(info.sender);
            }
        }
        // If the player has already requested ownership, return their index in the list (may have moved up)
        else
        {
            gotOwnership = requestors.IndexOf(info.sender);
        }

        return gotOwnership;
    }

    public int relinquishOwnership(PhotonMessageInfo info)
    {
        // Assume J. Random Player can never return this object.
        int returnedOwnership = ASLSTATE.FAIL;
        
        // Only the current owner can actually return ownership
        if (requestors.Count >= 1 && requestors[0] == info.sender)
        {
            // Remove the player from list of requestors
            requestors.Remove(info.sender);

            // If nobody else wants to own this object, return it to the master client
            if (requestors.Count == 0)
            {
                this.gameObject.GetPhotonView().TransferOwnership(SCENE_VALUE);              
            }
            else
            {
                this.gameObject.GetPhotonView().TransferOwnership(requestors[0]);
            }

            
            PhotonView view = this.gameObject.GetPhotonView();
            
            try
            {
                Debug.Log("aslBase: Returning Ownership of " + view.viewID + " from " + info.sender + " to " + view.owner.ID);
            }
            catch (System.NullReferenceException e)
            {
                Debug.Log("aslBase: Returning Ownership of " + view.viewID + " from " + info.sender + " to <scene>");
            }
            
            returnedOwnership = ASLSTATE.NOW;

        }
        // But a player in the queue can relinquish their claim and remove themselves from the queue
        else
        {
            int index = requestors.IndexOf(info.sender);

            if (index >= 0)
            {
                returnedOwnership = index;
                requestors.Remove(info.sender);
            }
        }
        
        return returnedOwnership;
    }
}

