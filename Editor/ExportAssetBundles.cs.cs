using UnityEditor;

public class CreateAssetBundles
{
    // ERROR TESTING - THIS NEEDS TO GET PHASED INTO MENU.CS AND MENUHANDLER.CS
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/ASL/StreamingAssets/AssetBundlesPC", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles("Assets/ASL/StreamingAssets/AssetBundlesAndroid", BuildAssetBundleOptions.None, BuildTarget.Android);
        //BuildPipeline.BuildAssetBundles("Assets/ASL/StreamingAssets/AssetBundlesHololens", BuildAssetBundleOptions.None, BuildTarget.WSAPlayer);
        // Version of Hololens build in LW - BuildPipeline.BuildAssetBundles("Assets/Photon Unity Networking/Resources/AssetBundlesHololens", BuildAssetBundleOptions.None, BuildTarget.WSAPlayer);
    }
}
