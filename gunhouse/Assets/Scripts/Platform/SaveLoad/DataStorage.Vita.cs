#if UNITY_PSP2
using Necrosoft;

namespace Gunhouse
{
    public static partial class DataStorage
    {
        public static void ResetValues()
        {
            GunOwned = new bool[NumberOfGuns];
            GunPower = new int[NumberOfGuns];
            GunEquipped = new bool[NumberOfGuns];

            for (int i = 0; i < maxEquip; ++i) GunEquipped[i] = true;
            for (int i = 0; i < NumberOfGuns; ++i) {
                GunOwned[i] = i < maxEquip;
                GunPower[i] = 1;
            }

            version = 1;
            Money = 0;
            Hearts = 2;
            Armor = 0;
            Healing = 1;
            StartOnWave = 0;
            Choom.MusicVolume = 0.75f;
            Choom.EffectVolume = 0.75f;
            IgnoreSignIn = false;
            Objectives.activeTasks = new int[3];
            AmountOfObjectivesComplete = 0;

            int[] scores = new int[0];
            BestHardcoreScores.Clear();
            for (int i = 0; i < scores.Length / 2; i++) {
                if (scores[i * 2] > 0) {
                    BestHardcoreScores.Add(new Tuple<int, int>(scores[i * 2], scores[i * 2 + 1]));
                }
            }

            BlocksLoaded = new int[10];
            AmmoLoaded = new int[10];
            MatchStreak = 0;
            ShotsFired = 0;
            TimesDefeated = 0;
            DisconcertingObjectivesSeen = 0;
        }

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
            version = data.version;
            Money = data.Money;
            Hearts = data.Hearts;
            Armor = data.Armor;
            Healing = data.Healing;
            StartOnWave = data.StartOnWave;
            GunOwned = data.GunOwned;
            GunPower = data.GunPower;
            GunEquipped = data.GunEquipped;
            Choom.MusicVolume = data.MusicVolume;
            Choom.EffectVolume = data.EffectVolume;
            IgnoreSignIn = data.IgnoreSignIn;
            Objectives.activeTasks = data.ObjectivesActive;
            AmountOfObjectivesComplete = data.AmountOfObjectivesComplete;

            int[] scores = data.BestHardcoreScores;
            BestHardcoreScores.Clear();
            for (int i = 0; i < scores.Length / 2; i++) {
                if (scores[i * 2] > 0) {
                    BestHardcoreScores.Add(new Tuple<int, int>(scores[i * 2], scores[i * 2 + 1]));
                }
            }

            BlocksLoaded = data.BlocksLoaded;
            AmmoLoaded = data.AmmoLoaded;
            MatchStreak = data.MatchStreak;
            ShotsFired = data.ShotsFired;
            TimesDefeated = data.TimesDefeated;
            DisconcertingObjectivesSeen = data.DisconcertingObjectivesSeen;
        }
    }
}
#endif