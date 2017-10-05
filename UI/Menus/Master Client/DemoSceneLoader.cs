using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ASL.UI.Menus.MasterClient
{
    public class DemoSceneLoader : MonoBehaviour
    {
        public static string levelToLoad = "PC";

        public void Start()
        {
#if UNITY_WSA_10_0
            Toggle toggle_masterClient = GameObject.Find("MasterClientMenu").GetComponentInChildren<UnityEngine.UI.Toggle>();
            toggle_masterClient.isOn = false;
            toggle_masterClient.interactable = false;
#elif UNITY_ANDROID
            Toggle toggle_masterClient = GameObject.Find("MasterClientMenu").GetComponentInChildren<UnityEngine.UI.Toggle>();
            toggle_masterClient.isOn = false;
            toggle_masterClient.interactable = false;
#else
            Toggle toggle_masterClient = GameObject.Find("MasterClientMenu").GetComponentInChildren<UnityEngine.UI.Toggle>();
            toggle_masterClient.isOn = true;
            toggle_masterClient.interactable = true;
#endif
        }

        public void LoadScene()
        {
            bool isMasterClient = false;
            string levelToLoad = "";
#if UNITY_WSA_10_0
            levelToLoad = "Hololens";
#elif UNITY_ANDROID
            levelToLoad = "Tango";
#else
            // PC, Vive, Oculus
            if (GameObject.Find("MasterClientMenu").GetComponentInChildren<UnityEngine.UI.Toggle>().isOn)
            {
                isMasterClient = true;
            }

            levelToLoad = "PC";
#endif
            transform.gameObject.GetComponent<SceneVariableSetter>().isMasterClient = isMasterClient;
            SceneManager.LoadScene(levelToLoad);
        }
    }
}