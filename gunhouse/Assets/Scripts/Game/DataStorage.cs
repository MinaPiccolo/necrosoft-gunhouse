using UnityEngine;
using Necrosoft;
using System.Collections.Generic;

namespace Gunhouse
{
    public static partial class DataStorage
    {
        public static class SaveKeys
        {
            public static string Version = "Version";
            public static string Money = "Money";
            public static string GunOwned = "GunOwned";
            public static string GunPower = "GunPower";
            public static string GunEquipped = "GunEquipped";
            public static string Hearts = "Hearts";
            public static string Armor = "Armor";
            public static string Healing = "Healing";
            public static string StartOnWave = "StartOnWave";
            public static string MusicVolume = "MusicVolume";
            public static string EffectVolume = "EffectVolume";
            public static string ObjectivesActive = "ObjectivesActive";
            public static string ObjectivesComplete = "ObjectivesComplete";
            public static string IgnoreSignIn = "IgnoreSignIn";
            public static string BestHardcoreScores = "BestHardcoreScores";
            public static string BlocksLoaded = "BlocksLoaded";
            public static string AmmoLoaded = "AmmoLoaded";
            public static string ShotsFired = "ShotsFired";
            public static string MatchStreak = "MatchStreak";
            public static string TimesDefeated = "TimesDefeated";
            public static string DisconcertingObjectivesSeen = "DisconcertingObjectivesSeen";
        }

        const int maxEquip = 3;
        static int version = 1;

        public const int NumberOfGuns = 10;

        public static int Money;
        public static bool[] GunOwned = new bool[NumberOfGuns];
        public static int[] GunPower = new int[NumberOfGuns];
        public static bool[] GunEquipped = new bool[NumberOfGuns];
        public static int Hearts = 2;
        public static int Armor;
        public static int Healing = 1;
        public static int StartOnWave;
        public static int AmountOfObjectivesComplete;
        public static bool IgnoreSignIn;

        public const int SCORES_TO_KEEP = 6;
        public static List<Tuple<int, int>> BestHardcoreScores = new List<Tuple<int, int>>();
        public static int[] BlocksLoaded = new int[10];
        public static int[] AmmoLoaded = new int[10];

        public static int ShotsFired, MatchStreak, TimesDefeated;

        public static int DisconcertingObjectivesSeen = 0;

        public static void Reset()
        {
            GunOwned = new bool[NumberOfGuns];
            GunPower = new int[NumberOfGuns];
            GunEquipped = new bool[NumberOfGuns];

            for (int i = 0; i < maxEquip; ++i) GunEquipped[i] = true;
            for (int i = 0; i < NumberOfGuns; ++i) {
                GunOwned[i] = i < maxEquip;
                GunPower[i] = 1;
            }
        }

        public static void Save()
        {
            PlayerPrefs.SetInt(SaveKeys.Version, version);
            PlayerPrefs.SetInt(SaveKeys.Money, Money);
            PlayerPrefs.SetInt(SaveKeys.Hearts, Hearts);
            PlayerPrefs.SetInt(SaveKeys.Armor, Armor);
            PlayerPrefs.SetInt(SaveKeys.Healing, Healing);
            PlayerPrefs.SetInt(SaveKeys.StartOnWave, StartOnWave);

            PlayerPrefsX.SetBoolArray(SaveKeys.GunOwned, GunOwned);
            PlayerPrefsX.SetIntArray(SaveKeys.GunPower, GunPower);
            PlayerPrefsX.SetBoolArray(SaveKeys.GunEquipped, GunEquipped);

            PlayerPrefs.SetFloat(SaveKeys.MusicVolume, Choom.MusicVolume);
            PlayerPrefs.SetFloat(SaveKeys.EffectVolume, Choom.EffectVolume);

            PlayerPrefsX.SetBool(SaveKeys.IgnoreSignIn, IgnoreSignIn);
            PlayerPrefsX.SetIntArray(SaveKeys.ObjectivesActive, Objectives.activeTasks);
            PlayerPrefs.SetInt(SaveKeys.ObjectivesComplete, AmountOfObjectivesComplete);

            int[] scores = new int[SCORES_TO_KEEP * 2];
            for (int i = 0; i < SCORES_TO_KEEP; i++) {
                if (i >= BestHardcoreScores.Count) break;
                scores[i * 2] = BestHardcoreScores[i].Item1;
                scores[i * 2 + 1] = BestHardcoreScores[i].Item2;
            }

            PlayerPrefsX.SetIntArray(SaveKeys.BestHardcoreScores, scores);
            PlayerPrefsX.SetIntArray(SaveKeys.BlocksLoaded, BlocksLoaded);
            PlayerPrefsX.SetIntArray(SaveKeys.AmmoLoaded, AmmoLoaded);
            PlayerPrefs.SetInt(SaveKeys.ShotsFired, ShotsFired);
            PlayerPrefs.SetInt(SaveKeys.TimesDefeated, TimesDefeated);
            PlayerPrefs.SetInt(SaveKeys.MatchStreak, MatchStreak);

            PlayerPrefs.SetInt(SaveKeys.DisconcertingObjectivesSeen, DisconcertingObjectivesSeen);
        }

        public static void Load()
        {
            Reset();

            if (!PlayerPrefs.HasKey(SaveKeys.Version)) { Save(); }

            version = PlayerPrefs.GetInt(SaveKeys.Version, 1);
            Money = PlayerPrefs.GetInt(SaveKeys.Money, 0);
            Hearts = PlayerPrefs.GetInt(SaveKeys.Hearts, 2);
            Armor = PlayerPrefs.GetInt(SaveKeys.Armor, 0);
            Healing = PlayerPrefs.GetInt(SaveKeys.Healing, 1);
            StartOnWave = PlayerPrefs.GetInt(SaveKeys.StartOnWave, 0);

            GunOwned = PlayerPrefsX.GetBoolArray(SaveKeys.GunOwned);
            GunPower = PlayerPrefsX.GetIntArray(SaveKeys.GunPower);
            GunEquipped = PlayerPrefsX.GetBoolArray(SaveKeys.GunEquipped);

            Choom.MusicVolume = PlayerPrefs.GetFloat(SaveKeys.MusicVolume, 0.75f);
            Choom.EffectVolume = PlayerPrefs.GetFloat(SaveKeys.EffectVolume, 0.75f);

            IgnoreSignIn = PlayerPrefsX.GetBool(SaveKeys.IgnoreSignIn, false);
            Objectives.activeTasks = PlayerPrefsX.GetIntArray(SaveKeys.ObjectivesActive);
            AmountOfObjectivesComplete = PlayerPrefs.GetInt(SaveKeys.ObjectivesComplete, 0);

            int[] scores = PlayerPrefsX.GetIntArray(SaveKeys.BestHardcoreScores);

            BestHardcoreScores.Clear();
            for (int i = 0; i < scores.Length / 2; i++) {
                if (scores[i * 2] > 0) {
                    BestHardcoreScores.Add(new Tuple<int, int>(scores[ i * 2], scores[i * 2 + 1]));
                }
            }

            BlocksLoaded = PlayerPrefsX.GetIntArray(SaveKeys.BlocksLoaded, 0, 10);
            AmmoLoaded = PlayerPrefsX.GetIntArray(SaveKeys.AmmoLoaded, 0, 10);
            MatchStreak = PlayerPrefs.GetInt(SaveKeys.MatchStreak);
            ShotsFired = PlayerPrefs.GetInt(SaveKeys.ShotsFired);
            TimesDefeated = PlayerPrefs.GetInt(SaveKeys.TimesDefeated);
            DisconcertingObjectivesSeen = PlayerPrefs.GetInt(SaveKeys.DisconcertingObjectivesSeen);
        }
    }
}
