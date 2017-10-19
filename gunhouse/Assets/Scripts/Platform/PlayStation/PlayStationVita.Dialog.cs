#if UNITY_PSP2
using Sony.Vita.Dialog;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gunhouse
{
    public partial class PlayStationVita : MonoBehaviour
    {
        void StartDialog()
        {
            Main.Initialise();
            Common.OnGotDialogResult += OnGotDialogResult;
        }

        void UpdateDialog()
        {
            Main.Update();
        }
        
        void DialogSaveLoadError()
        {
            Common.ShowUserMessage(Common.EnumUserMessageType.MSG_DIALOG_BUTTON_TYPE_OK,
                                   true,
                                   "Cannot load save data. If you continue\nyour save data will be overwritten");
        }

        #region EventHandlers

        void OnGotDialogResult(Messages.PluginMessage msg)
        {
            DeleteFile();
            SceneManager.LoadSceneAsync((int)SceneIndex.Main);
        }

        #endregion
    }
}
#endif