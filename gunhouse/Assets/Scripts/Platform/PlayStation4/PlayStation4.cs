using Sony.NP;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            Application.targetFrameRate = 60;

            //#if !UNITY_EDITOR

            Main.OnAsyncEvent += MainOnAsyncEvent;
            InitToolkit init = new InitToolkit();
            init.contentRestrictions.DefaultAgeRestriction = ContentRestriction.NP_NO_AGE_RESTRICTION;
            init.contentRestrictions.ApplyContentRestriction = false;

            init.SetPushNotificationsFlags(PushNotificationsFlags.None);

            init.threadSettings.affinity = Affinity.AllCores;
            init.memoryPools.JsonPoolSize = 6 * 1024 * 1024;
	        init.memoryPools.SslPoolSize *= 4;

            initResult = Main.Initialize(init);
            if (!initResult.Initialized) return;    /* NpToolkit failed somehow  */
           
            // how do you know the profile that started the game? 
            // do I need to check the trophy pack registered correctly for that userID?
            // PS4Input.RefreshUsersDetails(0);
            // PS4Input.PadIsConnected(0);

            // Utility.initialUserId        what are these exactly
            // Utility.primaryUserId

            loggedInUser = PS4Input.RefreshUsersDetails(Utility.initialUserId);
            StartSaveLoad();
            StartTrophy();
        
            //#endif    
        }

        void MainOnAsyncEvent(NpCallbackEvent callbackEvent)
        {
            Debug.Log("Event: Service = (" + callbackEvent.Service + ") : API Called = (" +
                      callbackEvent.ApiCalled + ") : Request Id = (" + callbackEvent.NpRequestId +
                      ") : Calling User Id = (" + callbackEvent.UserId + ")");

            Debug.Log("Main_OnAsyncEvent Callback: " + callbackEvent.Service.ToString());

            //if (callbackEvent == null) return;

            //switch (callbackEvent.Service)
            //{
            //case ServiceTypes.Trophy: OnAsyncEventTrophy(callbackEvent); break;
            //}

            try
            {
                switch (callbackEvent.Service)
                {
                case ServiceTypes.Trophy: OnAsyncEventTrophy(callbackEvent); break;
                }
            }
            catch (NpToolkitException e)
            {
                Debug.LogError("Main_OnAsyncEvent NpToolkit Exception: " + e.ExtendedMessage + " " + e.ExtendedMessage + " " + e.StackTrace);
            }
            catch (Exception e)
            {
                Debug.LogError("Main_OnAsyncEvent General Exception: " + e.Message + " " + e.StackTrace);
            }
        }
    }
}