using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Gunhouse
{
    public class Downloader : MonoBehaviour
    {
        [SerializeField] Slider progressBar;
        public static AssetBundle Bundle;

        #if BUNDLED

        #if !UNITY_WEBGL
        void Start() { LoadBundle(); }
        #endif

        public void LoadBundle()
        {
            #if UNITY_PS4
            string bundleName = "PS4/bundled";
            #elif UNITY_TVOS
            string bundleName = "tvOS/bundled";
            #elif UNITY_IOS
            string bundleName = "iOS/bundled";
            #elif UNITY_ANDROID
            string bundleName = "Android/bundled";
            #elif UNITY_WEBGL
            string bundleName = "WebGL/bundled";
            #endif

            StartCoroutine(LoadAssetBundle(System.IO.Path.Combine(Application.streamingAssetsPath, bundleName)));
        }

        IEnumerator LoadAssetBundle(string bundleName)
        {
            #if UNITY_IOS || UNITY_TVOS || UNITY_ANDROID || UNITY_PS4

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundleName);

            while (!request.isDone) {
                if (progressBar != null) { progressBar.value = request.progress; }

                Debug.Log((int)(100 * request.progress) + "%");

                yield return null;
            }

            Bundle = request.assetBundle;
            SceneManager.LoadSceneAsync((int)SceneIndex.Main);

            #elif UNITY_WEBGL

            WWW request = WWW.LoadFromCacheOrDownload(bundleName, 1);

            while (!request.isDone) {
                if (progressBar != null) { progressBar.value = request.progress; }
                yield return null;
            }

            Bundle = request.assetBundle;
            SceneManager.LoadSceneAsync((int)SceneIndex.Main);
            request.Dispose();

            #endif
        }

        #endif
    }
}
