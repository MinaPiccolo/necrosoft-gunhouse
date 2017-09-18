using Necrosoft;

namespace Gunhouse
{
    public static partial class DataStorage
    {
        #if UNITY_SWTICH
        public static void SaveOptions() { }
        public static void SaveStore() { }
        public static void SaveEndWave() { }
        public static void SaveHardcore() { }

        public static string SaveFile()
        {
            int[] scores = new int[SCORES_TO_KEEP * 2];
            for (int i = 0; i < SCORES_TO_KEEP; i++)
            {
                if (i >= BestHardcoreScores.Count) break;
                scores[i * 2] = BestHardcoreScores[i].Item1;
                scores[i * 2 + 1] = BestHardcoreScores[i].Item2;
            }

            SaveFile saveFile = new SaveFile();
            saveFile.version = version;
            saveFile.Money = Money;
            saveFile.Hearts = Hearts;
            saveFile.Armor = Armor;
            saveFile.Healing = Healing;
            saveFile.StartOnWave = StartOnWave;
            saveFile.GunOwned = GunOwned;
            saveFile.GunPower = GunPower;
            saveFile.GunEquipped = GunEquipped;
            saveFile.MusicVolume = Choom.MusicVolume;
            saveFile.EffectVolume = Choom.EffectVolume;
            saveFile.IgnoreSignIn = IgnoreSignIn;
            saveFile.ObjectivesActive = Objectives.activeTasks;
            saveFile.AmountOfObjectivesComplete = AmountOfObjectivesComplete;
            saveFile.BestHardcoreScores = scores;
            saveFile.BlocksLoaded = BlocksLoaded;
            saveFile.AmmoLoaded = AmmoLoaded;
            saveFile.ShotsFired = ShotsFired;
            saveFile.TimesDefeated = TimesDefeated;
            saveFile.MatchStreak = MatchStreak;
            saveFile.DisconcertingObjectivesSeen = DisconcertingObjectivesSeen;

            string saveFileSerialized = JsonUtility.ToJson(saveFile);

            SaveDataHandler.Save(saveFileSerialized, "GunhouseSave");
            return saveFileSerialized;
        }

        public static void LoadFile()
        {
            string serializedSaveData = "";
            SaveFile savedData;
            if (SaveDataHandler.Load(ref serializedSaveData, "GunhouseSave")) {
                savedData = JsonUtility.FromJson<SaveFile>(serializedSaveData);
            }
            else {
                serializedSaveData = Save();
                savedData = JsonUtility.FromJson<SaveFile>(serializedSaveData);
            }

            version = savedData.version != 0 ? savedData.version : 1;
            Money = savedData.Money;
            Hearts = savedData.Hearts != 0 ? savedData.Hearts : 2;
            Armor = savedData.Armor;
            Healing = savedData.Healing != 0 ? savedData.Healing : 1;
            StartOnWave = savedData.StartOnWave;
            GunOwned = savedData.GunOwned != null ? savedData.GunOwned : new bool[NumberOfGuns];
            GunPower = savedData.GunPower != null ? savedData.GunPower : new int[NumberOfGuns];
            GunEquipped = savedData.GunEquipped != null ? savedData.GunEquipped : new bool[NumberOfGuns];
            Choom.MusicVolume = savedData.MusicVolume != 0 ? savedData.MusicVolume : 0.75f;
            Choom.EffectVolume = savedData.EffectVolume != 0 ? savedData.EffectVolume : 0.75f;
            IgnoreSignIn = savedData.IgnoreSignIn;
            Objectives.activeTasks = savedData.ObjectivesActive != null ? savedData.ObjectivesActive : new int[3];
            AmountOfObjectivesComplete = savedData.AmountOfObjectivesComplete;

            int[] scores = savedData.BestHardcoreScores != null ? savedData.BestHardcoreScores : new int[0];
            BestHardcoreScores.Clear();
            for (int i = 0; i < scores.Length / 2; i++) {
                if (scores[i * 2] > 0) {
                    BestHardcoreScores.Add(new Tuple<int, int>(scores[i * 2], scores[i * 2 + 1]));
                }
            }

            BlocksLoaded = savedData.BlocksLoaded != null ? savedData.BlocksLoaded : new int[10];
            AmmoLoaded = savedData.AmmoLoaded != null ? savedData.AmmoLoaded : new int[10];
            MatchStreak = savedData.MatchStreak;
            ShotsFired = savedData.ShotsFired;
            TimesDefeated = savedData.TimesDefeated;
            DisconcertingObjectivesSeen = savedData.DisconcertingObjectivesSeen;
        }

        [Serializable]
        public class SaveFile
        {
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
        }
        #endif
    }
}
