#if UNITY_SWITCH
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gunhouse
{
    public class NintendoSwitch
    {
        static void OnNotificationMessage(UnityEngine.Switch.Notification.Message message)
        {
            if (Gunhouse.Game.instance != null) {
                Gunhouse.Game.HidePauseButton = UnityEngine.Switch.Operation.mode == UnityEngine.Switch.Operation.OperationMode.Console;
            }

            if (message == UnityEngine.Switch.Notification.Message.Resume && AppMain.top_state is Game) {
                AppMain.IsPaused = true;
                AppMain.top_state = new MenuState(Menu.MenuState.Pause, AppMain.top_state);
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
            // Set notification modes
            UnityEngine.Switch.Notification.SetOperationModeChangedNotificationEnabled(true);
            UnityEngine.Switch.Notification.SetResumeNotificationEnabled(true);

            // Add notification handler
            UnityEngine.Switch.Notification.notificationMessageReceived += OnNotificationMessage;
        }
    }
}
#endif