using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.UI.Mouse
{
    public class OwnershipTransfer : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CloneObject();
            }
            if (Input.GetMouseButtonDown(1))
            {
                RequestOwnership();
            }
        }

        public void CloneObject()
        {
            PhotonNetwork.Instantiate(gameObject.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
        }

        public void RequestOwnership()
        {
            if (gameObject.GetPhotonView() != null)
            {

                PhotonNetwork.RPC(gameObject.GetPhotonView(), "Grab", PhotonTargets.Others, false, null);
            }
        }
    }
}