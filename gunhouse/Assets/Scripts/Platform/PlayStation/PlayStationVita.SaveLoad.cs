#if UNITY_PSP2
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
            SaveLoad.OnGameLoaded += OnSavedGameLoaded;
            SaveLoad.OnLoadError += OnLoadError;
            SaveLoad.OnLoadNoData += OnLoadNoData;
            SaveLoad.Initialise();

            slotParams.title = "Gunhouse";
            slotParams.subTitle = "Load your guns! Rain death from above!";
            slotParams.detail = "Save Data";
            slotParams.iconPath = Path.Combine(Application.streamingAssetsPath, "SaveIcon.png");

            DataStorage.ResetValues();
            Objectives.ResetValues();
        }

        void UpdateSaveLoad() { Main.Update(); }
        
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

            SaveLoad.AutoSaveGame(bytes, slotNumber, slotParams, controlFlags);
        }

        public static void LoadFile()
        {
            SaveLoad.AutoLoadGame(slotNumber);
        }

        public static void DeleteFile()
        {
            SaveLoad.Delete(0, false);
            DataStorage.ResetValues();
            Objectives.ResetValues();
        }

        #region EventHandlers

        void OnSavedGameLoaded(Messages.PluginMessage msg)
        {
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

            SceneManager.LoadSceneAsync((int)SceneIndex.Main);
        }

        void OnLoadError(Messages.PluginMessage msg) { DialogSaveLoadError(); }

        void OnLoadNoData(Messages.PluginMessage msg) { SceneManager.LoadScene((int)SceneIndex.Main); }

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
