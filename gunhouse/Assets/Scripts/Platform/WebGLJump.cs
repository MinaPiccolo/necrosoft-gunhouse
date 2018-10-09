#if UNITY_WEBGL && JUMP_STORE
using UnityEngine;

namespace Gunhouse
{
    public class WebGLJump : MonoBehaviour
    {
        /* NOTE: requires payfab sdk and jump WebGLTemplates
            Attach this script to a gameobject in the Download.Scene */

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
            #if !UNITY_EDITOR
            jumpPlugin.Initialized += (object sender, System.EventArgs e) => {
                playFab.LoginToPlayFab(jumpPlugin.userId);
            };
            #else
            DataStorage.Load();
            Objectives.Load();
            GameObject.FindObjectOfType<Downloader>().LoadBundle();
            #endif
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