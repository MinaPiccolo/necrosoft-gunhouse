﻿#if UNITY_PSP2
using Sony.Vita.SavedGame;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gunhouse
{
    public partial class PlayStationVita : MonoBehaviour
    {
        static SaveLoad.SavedGameSlotParams slotParams = new SaveLoad.SavedGameSlotParams();
        const SaveLoad.ControlFlags controlFlags = SaveLoad.ControlFlags.NOSPACE_DIALOG_CONTINUABLE;
        const int slotNumber = 0;

        void StartSaveLoad()
        {
            Main.Initialise();
            Main.enableInternalLogging = true;
            Main.OnLog += OnLogSaveLoad;
            Main.OnLogWarning += OnLogWarningSaveLoad;
            Main.OnLogError += OnLogErrorSaveLoad;

            SaveLoad.Initialise();
            SaveLoad.OnGameSaved += OnSavedGameSaved;
            SaveLoad.OnGameLoaded += OnSavedGameLoaded;
            SaveLoad.OnSaveError += OnSaveError;
            SaveLoad.OnLoadError += OnLoadError;
            SaveLoad.OnLoadNoData += OnLoadNoData;

            slotParams.title = "Gunhouse";
            slotParams.subTitle = "Load your guns! Rain death from above!";
            slotParams.detail = "Save Data";
            slotParams.iconPath = Path.Combine(Application.streamingAssetsPath, "SaveIcon.png");

            DataStorage.ResetValues();
            Objectives.ResetValues();
            LoadFile();
        }
        
        public static void SaveFile()
        {
            SaveData data = new SaveData();
            DataStorage.SaveFile(data);
            Objectives.SaveFile(data);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, data);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Dispose();

            OnScreenLog.Add("SIZE!!!!: " + bytes.Length);

            OnScreenLog.Add("Saving Game...");
            OnScreenLog.Add(" icon: " + slotParams.iconPath);
            OnScreenLog.Add(" size: " + bytes.Length / 1024 + "KB");

            ErrorCode result = SaveLoad.AutoSaveGame(bytes, slotNumber, slotParams, controlFlags);
            if (result != ErrorCode.SG_OK) { OnScreenLog.Add("Save failed: " + result); }
        }

        public static void LoadFile()
        {
            ErrorCode result = SaveLoad.AutoLoadGame(slotNumber);
            if (result != ErrorCode.SG_OK) { OnScreenLog.Add("Load failed: " + result); }
        }

        #region EventHandlers

        void OnLogSaveLoad(Messages.PluginMessage msg) { OnScreenLog.Add(msg.Text); }
        void OnLogWarningSaveLoad(Messages.PluginMessage msg) { OnScreenLog.Add("WARNING: " + msg.Text); }

        void OnLogErrorSaveLoad(Messages.PluginMessage msg)
        {
            OnScreenLog.Add("ERROR: " + msg.Text);
            //SceneManager.LoadSceneAsync((int)SceneIndex.Main);
        }

        void OnSavedGameSaved(Messages.PluginMessage msg) { OnScreenLog.Add("Game Saved!"); }

        void OnSavedGameLoaded(Messages.PluginMessage msg)
        {
            OnScreenLog.Add("Game Loaded...");
            byte[] bytes = SaveLoad.GetLoadedGame();
            if (bytes == null) { SceneManager.LoadSceneAsync((int)SceneIndex.Main); return; }

            SaveData data;
            using (MemoryStream memoryStream = new MemoryStream()) {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                memoryStream.Write(bytes, 0, bytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                data = (SaveData)binaryFormatter.Deserialize(memoryStream);
            }

            DataStorage.LoadFile(data);
            Objectives.LoadFile(data);

            int quota = SaveLoad.GetQuota();
            // Get the current amount of used storage, note that this value is only valid when running from an installed package
            // and using the memory card for storage, e.g. not connected to host0:, otherwise it is always 0.
            int used = SaveLoad.GetUsedSize();
            OnScreenLog.Add("Storage used = " + used + "KB / " + quota + "KB");

            SceneManager.LoadSceneAsync((int)SceneIndex.Main);
        }

        void OnSaveError(Messages.PluginMessage msg)
        {
            ResultCode res = new ResultCode();
            SaveLoad.GetLastError(out res);
            OnScreenLog.Add("Failed to save: " + res.className + ": " + res.lastError + ", sce error 0x" + res.lastErrorSCE.ToString("X8"));
        }

        void OnLoadError(Messages.PluginMessage msg)
        {
            /* @complete: according to the TRC, when the game finds a corrupted save data,
                the player should be presented with the option of trying to reload or continue with a new game.

                The save games plugin does handle broken slot checking and displaying the
                required dialog you can test it by enabling the ""Fake Save Data Slot Broken"
                option in the Vita's debug settings.

                Sony.Vita.Dialog.Common.ShowUserMessage(Sony.Vita.Dialog.Common.EnumUserMessageType.MSG_DIALOG_BUTTON_TYPE_OK,
                                                        true, "No Save Data.\nCreate New Save Data.");
             */

            ResultCode res = new ResultCode();
            SaveLoad.GetLastError(out res);
            OnScreenLog.Add("Failed to load: " + res.className + ": " + res.lastError + ", sce error 0x" + res.lastErrorSCE.ToString("X8"));
            SceneManager.LoadSceneAsync((int)SceneIndex.Main);
        }

        void OnLoadNoData(Messages.PluginMessage msg)
        {
            OnScreenLog.Add("Nothing to load");
            SceneManager.LoadScene((int)SceneIndex.Main);
        }

        #endregion

        [System.Serializable]
        public class SaveData
        {
            #region DataStorage

            public int version;
            public int Money;
            public bool[] GunOwned;
            public int[] GunPower;
            public bool[] GunEquipped;
            public int Hearts;
            public int Armor;
            public int Healing;
            public int StartOnWave;
            public int AmountOfObjectivesComplete;
            public bool IgnoreSignIn;
            public int[] BestHardcoreScores;
            public int[] BlocksLoaded;
            public int[] AmmoLoaded;
            public int ShotsFired, MatchStreak, TimesDefeated;
            public int DisconcertingObjectivesSeen;
            public float MusicVolume;
            public float EffectVolume;
            public int[] ObjectivesActive;

            #endregion

            #region Objectives

            public int defeatWithGun;
            public int previousDefeatWithGun;
            public int amountToDefeat;
            public int ammoToUpgrade;
            public int previousAmmoToUpgrade;
            public int leveToUpgrade;
            public int levelToHealing;
            public int levelToHeart;
            public int isLevelingHeart;
            public int amountofBlocksToMake;
            public int makeXSize;
            public int makeYSize;
            public int amountOfSizedBlocksToLoadToGun;
            public int loadGunXSize;
            public int loadGunYSize;
            public int amountOfSizedBlocksToLoadToSpecial;
            public int loadSpecialXSize;
            public int loadSpecialYSize;
            public int amountToHaveInBank;
            public int ammoGunToSend;
            public int ammoSpecialToSend;
            public int amountOfElementsMatching;
            public int amountOfBossesToDefeat;
            public int currentBossesDefeated;
            public int daysToReach;
            public string currentFreeTask;

            #endregion
        }
    }
}
#endif
