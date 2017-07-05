using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    public class Config_Base_AssetBundle : Config_Base
    {
        //public static string Extension = ".asset";
        public static string CompileFilename(string bundleName)
        {
            return bundleName; //+ Extension;
        }
    }
}