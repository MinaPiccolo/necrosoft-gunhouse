using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPauseButtonHideShow : MonoBehaviour {
#if UNITY_SWITCH
    static void OnNotificationMessage(UnityEngine.Switch.Notification.Message message)
    {
        Gunhouse.Game.HidePauseButton = UnityEngine.Switch.Operation.mode == UnityEngine.Switch.Operation.OperationMode.Console;
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
#endif
}