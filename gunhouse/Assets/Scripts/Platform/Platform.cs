using UnityEngine;

namespace Gunhouse
{
    #if !UNITY_WEBGL

    public class Platform
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

    #else

    public class Platform : MonoBehaviour
    {
        public static void Quit() { WebGLJump.Exit(); }
        public static void LoadPlayerData() { }
        public static void SavePlayerData() { }

        public static void SaveOptions() { DataStorage.SaveOptions(); }
        public static void SaveStore() { DataStorage.SaveStore(); }
        public static void SaveHardcore() { DataStorage.SaveHardcore(); }

        public static void SaveEndWave()
        {
            Objectives.SaveRemote();
            DataStorage.SaveEndWave();
        }
    }

    #endif
}
