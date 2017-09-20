#if UNITY_PSP2
using Necrosoft;

namespace Gunhouse
{
    public static partial class DataStorage
    {
        public static void SaveOptions() { }
        public static void SaveStore() { }
        public static void SaveEndWave() { }
        public static void SaveHardcore() { }

        public static void SaveFile(PlayStationVita.SaveData data)
        {
            int[] scores = new int[SCORES_TO_KEEP * 2];
            for (int i = 0; i < SCORES_TO_KEEP; i++) {
                if (i >= BestHardcoreScores.Count) break;
                scores[i * 2] = BestHardcoreScores[i].Item1;
                scores[i * 2 + 1] = BestHardcoreScores[i].Item2;
            }

            data.version = version;
            data.Money = Money;
            data.Hearts = Hearts;
            data.Armor = Armor;
            data.Healing = Healing;
            data.StartOnWave = StartOnWave;
            data.GunOwned = GunOwned;
            data.GunPower = GunPower;
            data.GunEquipped = GunEquipped;
            data.MusicVolume = Choom.MusicVolume;
            data.EffectVolume = Choom.EffectVolume;
            data.IgnoreSignIn = IgnoreSignIn;
            data.ObjectivesActive = Objectives.activeTasks;
            data.AmountOfObjectivesComplete = AmountOfObjectivesComplete;
            data.BestHardcoreScores = scores;
            data.BlocksLoaded = BlocksLoaded;
            data.AmmoLoaded = AmmoLoaded;
            data.ShotsFired = ShotsFired;
            data.TimesDefeated = TimesDefeated;
            data.MatchStreak = MatchStreak;
            data.DisconcertingObjectivesSeen = DisconcertingObjectivesSeen;
        }

        public static void LoadFile(PlayStationVita.SaveData data)
        {
            version = data.version != 0 ? data.version : 1;
            Money = data.Money;
            Hearts = data.Hearts != 0 ? data.Hearts : 2;
            Armor = data.Armor;
            Healing = data.Healing != 0 ? data.Healing : 1;
            StartOnWave = data.StartOnWave;
            GunOwned = data.GunOwned != null ? data.GunOwned : new bool[NumberOfGuns];
            GunPower = data.GunPower != null ? data.GunPower : new int[NumberOfGuns];
            GunEquipped = data.GunEquipped != null ? data.GunEquipped : new bool[NumberOfGuns];
            Choom.MusicVolume = data.MusicVolume != 0 ? data.MusicVolume : 0.75f;
            Choom.EffectVolume = data.EffectVolume != 0 ? data.EffectVolume : 0.75f;
            IgnoreSignIn = data.IgnoreSignIn;
            Objectives.activeTasks = data.ObjectivesActive != null ? data.ObjectivesActive : new int[3];
            AmountOfObjectivesComplete = data.AmountOfObjectivesComplete;

            int[] scores = data.BestHardcoreScores != null ? data.BestHardcoreScores : new int[0];
            BestHardcoreScores.Clear();
            for (int i = 0; i < scores.Length / 2; i++) {
                if (scores[i * 2] > 0) {
                    BestHardcoreScores.Add(new Tuple<int, int>(scores[i * 2], scores[i * 2 + 1]));
                }
            }

            BlocksLoaded = data.BlocksLoaded != null ? data.BlocksLoaded : new int[10];
            AmmoLoaded = data.AmmoLoaded != null ? data.AmmoLoaded : new int[10];
            MatchStreak = data.MatchStreak;
            ShotsFired = data.ShotsFired;
            TimesDefeated = data.TimesDefeated;
            DisconcertingObjectivesSeen = data.DisconcertingObjectivesSeen;
        }
    }
}
#endif