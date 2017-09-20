#if UNITY_WEBGL
using Necrosoft;

namespace Gunhouse
{
    public static partial class DataStorage
    {
        public static void SaveOptions()
        {
            Platform.SaveData(SaveKeys.MusicVolume, ((int)(Choom.MusicVolume * 100)).ToString());
            Platform.SaveData(SaveKeys.EffectVolume, ((int)(Choom.EffectVolume * 100)).ToString());
        }

        public static void SaveStore()
        {
            Platform.SaveData(SaveKeys.Money, Money.ToString());
            Platform.SaveData(SaveKeys.Hearts, Hearts.ToString());
            Platform.SaveData(SaveKeys.Armor, Armor.ToString());
            Platform.SaveData(SaveKeys.Healing, Healing.ToString());

            for (int i = 0; i < GunOwned.Length; ++i) {
                Platform.SaveData(SaveKeys.GunOwned + i.ToString(), GunOwned[i].ToString());
            }
            for (int i = 0; i < GunPower.Length; ++i) {
                Platform.SaveData(SaveKeys.GunPower + i.ToString(), GunPower[i].ToString());
            }
            for (int i = 0; i < GunEquipped.Length; ++i) {
                Platform.SaveData(SaveKeys.GunEquipped + i.ToString(), GunEquipped[i].ToString());
            }
        }

        public static void SaveEndWave()
        {
            Platform.SaveData(SaveKeys.Money, Money.ToString());

            for (int i = 0; i < Objectives.activeTasks.Length; ++i) {
                Platform.SaveData(SaveKeys.ObjectivesActive + i.ToString(), Objectives.activeTasks[i].ToString());
            }
            Platform.SaveData(SaveKeys.ObjectivesComplete, AmountOfObjectivesComplete.ToString());

            for (int i = 0; i < BlocksLoaded.Length; ++i) {
                Platform.SaveData(SaveKeys.BlocksLoaded + i.ToString(), BlocksLoaded[i].ToString());
            }
            for (int i = 0; i < AmmoLoaded.Length; ++i) {
                Platform.SaveData(SaveKeys.AmmoLoaded + i.ToString(), AmmoLoaded[i].ToString());
            }

            Platform.SaveData(SaveKeys.ShotsFired, ShotsFired.ToString());
            Platform.SaveData(SaveKeys.TimesDefeated, TimesDefeated.ToString());
            Platform.SaveData(SaveKeys.MatchStreak, MatchStreak.ToString());

            Platform.SaveData(SaveKeys.DisconcertingObjectivesSeen, DisconcertingObjectivesSeen.ToString());
        }

        public static void SaveHardcore()
        {
            int[] scores = new int[SCORES_TO_KEEP * 2];
            for (int i = 0; i < SCORES_TO_KEEP; i++) {
                if (i >= BestHardcoreScores.Count) break;
                scores[i * 2] = BestHardcoreScores[i].Item1;
                scores[i * 2 + 1] = BestHardcoreScores[i].Item2;
            }

            for (int i = 0; i < scores.Length; ++i) {
                Platform.SaveData(SaveKeys.BestHardcoreScores + i.ToString(), scores[i].ToString());
            }
        }

        public static void LoadRemote()
        {
            Reset();

            Money = Platform.LoadData(SaveKeys.Money, 0);
            Hearts = Platform.LoadData(SaveKeys.Hearts, 2);
            Armor = Platform.LoadData(SaveKeys.Armor, 0);
            Healing = Platform.LoadData(SaveKeys.Healing, 1);
            StartOnWave = Platform.LoadData(SaveKeys.StartOnWave, 0);

            for (int i = 0; i < GunOwned.Length; ++i) {
                GunOwned[i] = Platform.LoadData(SaveKeys.GunOwned + i.ToString(), i < maxEquip);
            }
            for (int i = 0; i < GunPower.Length; ++i) {
                GunPower[i] = Platform.LoadData(SaveKeys.GunPower + i.ToString(), 1);
            }
            for (int i = 0; i < GunEquipped.Length; ++i) {
                GunEquipped[i] = Platform.LoadData(SaveKeys.GunEquipped + i.ToString(), i < maxEquip);
            }

            Choom.MusicVolume = ((float)Platform.LoadData(SaveKeys.MusicVolume, 75) / (float)100);
            Choom.EffectVolume = ((float)Platform.LoadData(SaveKeys.EffectVolume, 75) / (float)100);

            for (int i = 0; i < Objectives.activeTasks.Length; ++i) {
                Objectives.activeTasks[i] = Platform.LoadData(SaveKeys.ObjectivesActive + i.ToString(), 0);
            }
            AmountOfObjectivesComplete = Platform.LoadData(SaveKeys.ObjectivesComplete, 0);

            int[] scores = new int[5];
            for (int i = 0; i < scores.Length; ++i) {
                scores[i] = Platform.LoadData(SaveKeys.BestHardcoreScores + i.ToString(), 0);
            }
            BestHardcoreScores.Clear();
            for (int i = 0; i < scores.Length / 2; i++) {
                if (scores[i * 2] > 0) {
                    BestHardcoreScores.Add(new Tuple<int, int>(scores[ i * 2], scores[i * 2 + 1]));
                }
            }

            for (int i = 0; i < BlocksLoaded.Length; ++i) {
                BlocksLoaded[i] = Platform.LoadData(SaveKeys.BlocksLoaded + i.ToString(), 0);
            }
            for (int i = 0; i < AmmoLoaded.Length; ++i) {
                AmmoLoaded[i] = Platform.LoadData(SaveKeys.AmmoLoaded + i.ToString(), 0);
            }
            MatchStreak = Platform.LoadData(SaveKeys.MatchStreak, 0);
            ShotsFired = Platform.LoadData(SaveKeys.ShotsFired, 0);
            TimesDefeated = Platform.LoadData(SaveKeys.TimesDefeated, 0);
            DisconcertingObjectivesSeen = Platform.LoadData(SaveKeys.DisconcertingObjectivesSeen, 0);
        }
    }
}
#endif