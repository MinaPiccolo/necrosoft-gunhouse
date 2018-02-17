using UnityEngine;

namespace Gunhouse
{
    public class Platform : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            #if UNITY_ANDROID
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;
            #endif
        }

#if UNITY_PSP2 && !UNITY_EDITOR
        public static void Quit() { }
        public static void LoadPlayerData() { }
        public static void SavePlayerData() { }
        public static void SaveOptions() { PlayStationVita.SaveFile(); }
        public static void SaveStore() { PlayStationVita.SaveFile(); }
        public static void SaveHardcore() { }
        public static void SaveEndWave() { PlayStationVita.SaveFile(); }
#elif UNITY_SWITCH
        public static void Quit() { Application.Quit(); }
        public static void LoadPlayerData() { DataStorage.LoadFile(); Objectives.LoadFile(); }
        public static void SavePlayerData() { DataStorage.SaveFile(); Objectives.SaveFile(); }
        public static void SaveOptions() { DataStorage.SaveOptions(); }
        public static void SaveStore() { DataStorage.SaveStore(); }
        public static void SaveHardcore() { DataStorage.SaveHardcore(); }
        public static void SaveEndWave() { Objectives.SaveFile(); DataStorage.SaveEndWave(); }
#elif UNITY_WEBGL
        /* WebGL, Jump Platform */
        public static void Quit() { WebGLJump.Quit(); }
        public static void SavePlayerData() { }
#if UNITY_EDITOR
        public static void LoadPlayerData() { }
        public static void SaveOptions() { }
        public static void SaveStore() { }
        public static void SaveHardcore() { }
        public static void SaveEndWave() { }
#else
        public static void LoadPlayerData() { DataStorage.Load(); Objectives.Load(); }
        public static void SaveOptions() { DataStorage.SaveOptions(); }
        public static void SaveStore() { DataStorage.SaveStore(); }
        public static void SaveHardcore() { DataStorage.SaveHardcore(); }
        public static void SaveEndWave() { Objectives.SaveRemote(); DataStorage.SaveEndWave(); }
#endif
#else
        /* Almost every platform will use this */
        public static void Quit() { Application.Quit(); }
        public static void LoadPlayerData() { DataStorage.Load(); Objectives.Load(); }
        public static void SavePlayerData() { DataStorage.Save(); Objectives.Save(); }
        public static void SaveOptions() { }
        public static void SaveStore() { }
        public static void SaveHardcore() { }
        public static void SaveEndWave() { }
#endif
    }
}