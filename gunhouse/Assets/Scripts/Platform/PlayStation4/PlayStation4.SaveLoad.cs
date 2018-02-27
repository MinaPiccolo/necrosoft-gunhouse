using UnityEngine;

namespace Gunhouse
{
    public partial class PlayStation4
    {          
        void StartSaveLoad()
        {
            Sony.PS4.Dialog.Common.OnGotDialogResult += SaveLoadOnGotDialogResult;
            Sony.PS4.SavedGame.SaveLoad.OnSaveError += SaveLoadOnSaveError;
            Sony.PS4.SavedGame.SaveLoad.OnLoadError += SaveLoadOnLoadError;
        }

        void SaveLoadOnLoadError(Sony.PS4.SavedGame.Messages.PluginMessage msg)
        {
            if (msg.type == Sony.PS4.SavedGame.Messages.MessageType.kSavedGame_LoadCorrupted) {
                Debug.Log("Load was corrupted!");
                Sony.PS4.Dialog.Common.ShowUserMessage(Sony.PS4.Dialog.Common.UserMessageType.OK, false, "Error - Corrupt Save will be Deleted");
            }
        }

        void SaveLoadOnSaveError(Sony.PS4.SavedGame.Messages.PluginMessage msg)
        {
            if (msg.type == Sony.PS4.SavedGame.Messages.MessageType.kSavedGame_LoadCorrupted) {
                Debug.Log("Save was corrupted!");
                Sony.PS4.Dialog.Common.ShowUserMessage(Sony.PS4.Dialog.Common.UserMessageType.OK, false, "Error - Corrupt Save will be Deleted");
            }
        }
 
        void SaveLoadOnGotDialogResult(Sony.PS4.Dialog.Messages.PluginMessage msg)
        {
            Sony.PS4.Dialog.Common.CommonDialogResult dialogResult = Sony.PS4.Dialog.Common.GetResult();
            Debug.Log("Dialog result: " + dialogResult + " type " + msg.type + " Text " + msg.Text);
            //Delete SaveData
        }
    }
}
