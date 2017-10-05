using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.UI.Menus.MasterClient
{
    public class SceneVariableSetter : MonoBehaviour
    {
        public bool isMasterClient = false;

        public void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}