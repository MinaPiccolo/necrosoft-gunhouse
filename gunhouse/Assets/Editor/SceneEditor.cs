using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Necrosoft
{
    public static class MenuItems
    {
        static string scenePath = "Assets/Content/Scenes/";

        [MenuItem("Game/Clear PlayerPrefs %#d")]
        static void ClearPlayerPrefs()
        {
            Debug.Log("Deleted PlayerPrefs");
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Game/Scenes/Game")]
        static void LoadSceneMain()
        {
            EditorSceneManager.RestoreSceneManagerSetup(new SceneSetup[] {
                new SceneSetup() { path = scenePath + "Game.unity", isActive = true, isLoaded = true },
            });
        }

        [MenuItem("Game/Bundle/Create iOS")]
        static void AssetBundleCreateiOS()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/iOS", BuildAssetBundleOptions.None, BuildTarget.iOS);

        }

        [MenuItem("Game/Bundle/Create tvOS")]
        static void AssetBundleCreatetvOS()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/tvOS", BuildAssetBundleOptions.None, BuildTarget.tvOS);

        }

        [MenuItem("Game/Bundle/Create Android")]
        static void AssetBundleCreateAndroid()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
        }

        [MenuItem("Game/Bundle/Create WebGL")]
        static void AssetBundleCreateWebGL()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/WebGL", BuildAssetBundleOptions.None, BuildTarget.WebGL);
        }

        [MenuItem("Game/Bundle/Create PS4")]
        static void AssetBundleCreatePS4()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/PS4", BuildAssetBundleOptions.None, BuildTarget.PS4);
        }

        [MenuItem("Game/Bundle/Create Vita")]
        static void AssetBundleCreateVita()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/Vita", BuildAssetBundleOptions.None, BuildTarget.PSP2);
        }

        [MenuItem("Game/Bundle/Clear Cache")]
        static void AssetBundleClearCache()
        {
            if (Caching.ClearCache()) { Debug.LogWarning("Successfully cleaned all caches."); }
            else { Debug.LogWarning("Cache was in use."); }
        }
    }
}