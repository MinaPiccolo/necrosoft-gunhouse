#if UNITY_PSP2
using UnityEngine;
using UnityEngine.PSVita;
using UnityEngine.SceneManagement;

namespace Gunhouse
{
    public partial class PlayStationVita : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
 
            PSVitaVideoPlayer.TransferMemToMonoHeap();

            #if !UNITY_EDITOR
            StartSaveLoad();
            #else
            SceneManager.LoadScene((int)SceneIndex.Main);
            #endif
        }

        void OnEnable() { Application.logMessageReceived += HandleLog; }
        void OnDisable() { Application.logMessageReceived -= HandleLog; }
        void HandleLog(string logString, string stackTrace, LogType type) { OnScreenLog.Add("LOG: " + logString + " " + stackTrace); }
        
        void Update()
        {
            #if !UNITY_EDITOR
            UpdateSaveLoad();
            #else
            #endif
        }
    }
}
#endif
