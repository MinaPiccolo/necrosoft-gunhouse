#if UNITY_PS4 || (UNITY_PSP2 && UNITY_EDITOR)
using Sony.NP;
using UnityEngine;
using UnityEngine.PS4;
using System;

namespace Gunhouse
{
    public partial class PlayStation4 : MonoBehaviour
    {
        public InitResult initResult;
        public static PS4Input.LoggedInUser loggedInUser;

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            #if !UNITY_EDITOR

            Main.OnAsyncEvent += MainOnAsyncEvent;

            InitToolkit init = new InitToolkit();
            init.contentRestrictions.DefaultAgeRestriction = ContentRestriction.NP_NO_AGE_RESTRICTION;
            init.contentRestrictions.ApplyContentRestriction = false;
            init.SetPushNotificationsFlags(PushNotificationsFlags.None);
            init.threadSettings.affinity = Affinity.AllCores;
            init.memoryPools.JsonPoolSize = 6 * 1024 * 1024;
            init.memoryPools.SslPoolSize *= 4;

            initResult = Main.Initialize(init);
            if (!initResult.Initialized) return;

            loggedInUser = PS4Input.RefreshUsersDetails(Utility.primaryUserId);
            PS4PlayerPrefs.SetTitleStrings("Gunhouse", "Load your guns! Rain death from above!", "Save Data");            
            StartTrophy();
        
            #endif    
        }

        void MainOnAsyncEvent(NpCallbackEvent callbackEvent)
        {
            if (callbackEvent == null) return;

            switch (callbackEvent.Service)
            {
            case ServiceTypes.Trophy: OnAsyncEventTrophy(callbackEvent); break;
            }
        }
    }
}
#endif