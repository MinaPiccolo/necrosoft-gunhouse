using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gunhouse
{
    public class Platform : MonoBehaviour
    {
        public static void LoadPlayerData()
        {
            DataStorage.Load();
            Objectives.Load();
        }

        public static void SavePlayerData()
        {
            DataStorage.Save();
            Objectives.Save();
        }

        public static void Quit()
        {
            Application.Quit();
        }

        public static void SaveOptions() { }
        public static void SaveStore() { }
        public static void SaveHardcore() { }
        public static void SaveEndWave() { }
    }

    #if UNITY_WEBGL

    public class Platform : MonoBehaviour
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

        public static void LoadPlayerData()
        {
            //DataStorage.LoadRemote();
            //Objectives.LoadRemote();
        }

        public static void SaveOptions() { DataStorage.SaveOptions(); }
        public static void SaveStore() { DataStorage.SaveStore(); }
        public static void SaveHardcore() { DataStorage.SaveHardcore(); }

        public static void SaveEndWave()
        {
            Objectives.SaveRemote();
            DataStorage.SaveEndWave();
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

    #endif
}
