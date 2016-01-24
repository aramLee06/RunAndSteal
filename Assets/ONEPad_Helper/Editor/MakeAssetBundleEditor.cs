using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

#if UNITY_EDITOR
public class MakeAssetBundleEditor : Editor
{
    [MenuItem("ONEPAD/AssetBundle/Make Asset Bundle(Android)")]
    public static void MakeAssetBundleAndroid()
    {
        List<string> bundleLevels = GetBundleLevels();

        int bundleLevelsCount = bundleLevels.Count;
        string msg = "Build this scens.\r\n";
        for (int i = 0; i < bundleLevelsCount; i++)
        {
            msg += "\t" + Path.GetFileName(bundleLevels[i]) + "\r\n";
        }
        string assetBundleName = PlayerSettings.productName + "_bundle_android.unity3d";
        msg += "continue?\r\n*assetbundle Name is \'" + assetBundleName + "\'\r\n(in this project's root folder)";


        if (EditorUtility.DisplayDialog("ONEPad AssetBundle Maker", msg, "Yes", "No"))
        {
            BuildPipeline.BuildStreamedSceneAssetBundle(bundleLevels.ToArray(), assetBundleName, BuildTarget.Android);
        }
    }
    [MenuItem("ONEPAD/AssetBundle/Make Asset Bundle(IOS)")]
    public static void MakeAssetBundleIOS()
    {
        List<string> bundleLevels = GetBundleLevels();

        int bundleLevelsCount = bundleLevels.Count;
        string msg = "Build this scens.\r\n";
        for (int i = 0; i < bundleLevelsCount; i++)
        {
            msg += "\t" + Path.GetFileName(bundleLevels[i]) + "\r\n";
        }
        string assetBundleName = PlayerSettings.productName + "_bundle_ios.unity3d";
        msg += "continue?\r\n*assetbundle Name is \'" + assetBundleName + "\'\r\n(in this project's root folder)";


        if (EditorUtility.DisplayDialog("ONEPad AssetBundle Maker", msg, "Yes", "No"))
        {
            BuildPipeline.BuildStreamedSceneAssetBundle(bundleLevels.ToArray(), assetBundleName, BuildTarget.iOS);
        }
    }

    private static List<string> GetBundleLevels()
    {
        EditorBuildSettingsScene[] allLevels = EditorBuildSettings.scenes;
        List<string> bundleLevels = new List<string>();

        int count = allLevels.Length;
        for (int i = 0; i < count; i++)
        {
            EditorBuildSettingsScene nowScene = allLevels[i];
            if (nowScene.enabled == false)
                continue;

            string levelName = Path.GetFileName(nowScene.path);
            string gamePartyChecker = levelName.ToLower().Replace(".unity", "");
            if (gamePartyChecker == "1_login" || gamePartyChecker == "2_roomnumber")
                continue;

            bundleLevels.Add(nowScene.path);
        }

        return bundleLevels;
    }
}
#endif