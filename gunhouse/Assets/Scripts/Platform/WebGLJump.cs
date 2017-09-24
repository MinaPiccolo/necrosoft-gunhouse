#if UNITY_WEBGL
using UnityEngine;

namespace Gunhouse
{
    public class WebGLJump : MonoBehaviour
    {
        static JumpPlugin jumpPlugin;
        static PlayFabManager playFab;
        static bool once;

        void Awake()
        {
            jumpPlugin = GetComponent<JumpPlugin>();
            playFab = GetComponent<PlayFabManager>();
        }

        void Start()
        {
            jumpPlugin.Initialized += (object sender, System.EventArgs e) => {
                playFab.LoginToPlayFab(jumpPlugin.userId);
            };
        }

        public static void Quit()
        {
            jumpPlugin.Exit();
        }

        public static void Initialize()
        {
            if (once) return;

            DataStorage.LoadRemote();
            Objectives.LoadRemote();
            GameObject.FindObjectOfType<Downloader>().LoadBundle();

            once = true;
        }

        public static void SaveData(string key, string value)
        {
            playFab.UpdatePlayerData(key, value);
        }

        public static string LoadData(string key, string default_value)
        {
            string result = playFab.GetDataValueForKey(key);
            return result == null ? default_value : result;
        }

        public static int LoadData(string key, int default_value)
        {
            string result = playFab.GetDataValueForKey(key);
            return result == null ? default_value : int.Parse(result);
        }

        public static bool LoadData(string key, bool default_value)
        {
            string result = playFab.GetDataValueForKey(key);
            return result == null ? default_value : bool.Parse(result);
        }
    }
}
#endif
