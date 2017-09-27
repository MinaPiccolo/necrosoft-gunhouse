#if UNITY_WEBGL
using Necrosoft;

namespace Gunhouse
{
    public static partial class DataStorage
    {
        public static void SaveOptions()
        {
            WebGLJump.SaveData(SaveKeys.MusicVolume, ((int)(Choom.MusicVolume * 100)).ToString());
            WebGLJump.SaveData(SaveKeys.EffectVolume, ((int)(Choom.EffectVolume * 100)).ToString());
        }

        public static void SaveStore()
        {
            WebGLJump.SaveData(SaveKeys.Money, Money.ToString());
            WebGLJump.SaveData(SaveKeys.Hearts, Hearts.ToString());
            WebGLJump.SaveData(SaveKeys.Armor, Armor.ToString());
            WebGLJump.SaveData(SaveKeys.Healing, Healing.ToString());

            for (int i = 0; i < GunOwned.Length; ++i) {
                WebGLJump.SaveData(SaveKeys.GunOwned + i.ToString(), GunOwned[i].ToString());
            }
            for (int i = 0; i < GunPower.Length; ++i) {
                WebGLJump.SaveData(SaveKeys.GunPower + i.ToString(), GunPower[i].ToString());
            }
            for (int i = 0; i < GunEquipped.Length; ++i) {
                WebGLJump.SaveData(SaveKeys.GunEquipped + i.ToString(), GunEquipped[i].ToString());
            }
        }

        public static void SaveEndWave()
        {
            WebGLJump.SaveData(SaveKeys.Money, Money.ToString());

            for (int i = 0; i < Objectives.activeTasks.Length; ++i) {
                WebGLJump.SaveData(SaveKeys.ObjectivesActive + i.ToString(), Objectives.activeTasks[i].ToString());
            }
            WebGLJump.SaveData(SaveKeys.ObjectivesComplete, AmountOfObjectivesComplete.ToString());

            for (int i = 0; i < BlocksLoaded.Length; ++i) {
                WebGLJump.SaveData(SaveKeys.BlocksLoaded + i.ToString(), BlocksLoaded[i].ToString());
            }
            for (int i = 0; i < AmmoLoaded.Length; ++i) {
                WebGLJump.SaveData(SaveKeys.AmmoLoaded + i.ToString(), AmmoLoaded[i].ToString());
            }

            WebGLJump.SaveData(SaveKeys.ShotsFired, ShotsFired.ToString());
            WebGLJump.SaveData(SaveKeys.TimesDefeated, TimesDefeated.ToString());
            WebGLJump.SaveData(SaveKeys.MatchStreak, MatchStreak.ToString());

            WebGLJump.SaveData(SaveKeys.DisconcertingObjectivesSeen, DisconcertingObjectivesSeen.ToString());
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
                WebGLJump.SaveData(SaveKeys.BestHardcoreScores + i.ToString(), scores[i].ToString());
            }
        }

        public static void LoadRemote()
        {
            Reset();

            Money = WebGLJump.LoadData(SaveKeys.Money, 0);
            Hearts = WebGLJump.LoadData(SaveKeys.Hearts, 2);
            Armor = WebGLJump.LoadData(SaveKeys.Armor, 0);
            Healing = WebGLJump.LoadData(SaveKeys.Healing, 1);
            StartOnWave = WebGLJump.LoadData(SaveKeys.StartOnWave, 0);

            for (int i = 0; i < GunOwned.Length; ++i) {
                GunOwned[i] = WebGLJump.LoadData(SaveKeys.GunOwned + i.ToString(), i < maxEquip);
            }
            for (int i = 0; i < GunPower.Length; ++i) {
                GunPower[i] = WebGLJump.LoadData(SaveKeys.GunPower + i.ToString(), 1);
            }
            for (int i = 0; i < GunEquipped.Length; ++i) {
                GunEquipped[i] = WebGLJump.LoadData(SaveKeys.GunEquipped + i.ToString(), i < maxEquip);
            }

            Choom.MusicVolume = ((float)WebGLJump.LoadData(SaveKeys.MusicVolume, 75) / (float)100);
            Choom.EffectVolume = ((float)WebGLJump.LoadData(SaveKeys.EffectVolume, 75) / (float)100);

            for (int i = 0; i < Objectives.activeTasks.Length; ++i) {
                Objectives.activeTasks[i] = WebGLJump.LoadData(SaveKeys.ObjectivesActive + i.ToString(), 0);
            }
            AmountOfObjectivesComplete = WebGLJump.LoadData(SaveKeys.ObjectivesComplete, 0);

            int[] scores = new int[5];
            for (int i = 0; i < scores.Length; ++i) {
                scores[i] = WebGLJump.LoadData(SaveKeys.BestHardcoreScores + i.ToString(), 0);
            }
            BestHardcoreScores.Clear();
            for (int i = 0; i < scores.Length / 2; i++) {
                if (scores[i * 2] > 0) {
                    BestHardcoreScores.Add(new Tuple<int, int>(scores[ i * 2], scores[i * 2 + 1]));
                }
            }

            for (int i = 0; i < BlocksLoaded.Length; ++i) {
                BlocksLoaded[i] = WebGLJump.LoadData(SaveKeys.BlocksLoaded + i.ToString(), 0);
            }
            for (int i = 0; i < AmmoLoaded.Length; ++i) {
                AmmoLoaded[i] = WebGLJump.LoadData(SaveKeys.AmmoLoaded + i.ToString(), 0);
            }
            MatchStreak = WebGLJump.LoadData(SaveKeys.MatchStreak, 0);
            ShotsFired = WebGLJump.LoadData(SaveKeys.ShotsFired, 0);
            TimesDefeated = WebGLJump.LoadData(SaveKeys.TimesDefeated, 0);
            DisconcertingObjectivesSeen = WebGLJump.LoadData(SaveKeys.DisconcertingObjectivesSeen, 0);
        }
    }
}
#endif