using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UWBNetworkingPackage
{
    public class TangoRoom : MonoBehaviour
    {

        private void OnDestroy()
        {
            TangoDatabase.TangoRoom T = TangoDatabase.GetRoomByName(this.gameObject.name);

            TangoDatabase.DeleteRoom(T);
        }
    }
}