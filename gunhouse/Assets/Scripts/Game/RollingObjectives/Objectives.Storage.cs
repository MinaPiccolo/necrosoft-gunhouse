using UnityEngine;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        public static class SaveKeys
        {
            public static string DefeatWithGun = "DefeatWithGun";                   /* DEFEAT_ENEMIES_WITH_GUN */
            public static string PreviousDefeatWithGun = "PreviousDefeatWithGun";
            public static string AmountToDefeat = "AmountToDefeat";

            public static string AmmoToUpgrade = "AmmoToUpgrade";                   /* UPGRADE_GUN_TO_LEVEL */
            public static string PreviousAmmoToUpgrade = "PreviousAmmoToUpgrade";
            public static string LeveToUpgrade = "LeveToUpgrade";

            public static string LevelToHealing = "LevelToHealing";                 /* UPGRADE_HEALING_TO_LEVEL */

            public static string LevelToHeart = "LevelToHeart";                     /* UPGRADE_ARMOR_TO_LEVEL */
            public static string IsLevelingHeart = "IsLevelingHeart";

            public static string AmountofBlocksToMake = "AmountofBlocksToMake";     /* MAKE_X_NUMBER_SIZED_BLOCKS */
            public static string MakeXSize = "MakeXSize";
            public static string MakeYSize = "MakeYSize";

            public static string AmountOfSizedBlocksToLoadToGun = "AmountOfSizedBlocksToLoadToGun"; /* LOAD_GUN_WITH_X_SIZED_BLOCKS */
            public static string LoadGunXSize = "LoadGunXSize";
            public static string LoadGunYSize = "LoadGunYSize";

            public static string AmountOfSizedBlocksToLoadToSpecial = "AmountOfSizedBlocksToLoadToSpecial"; /* LOAD_SPECIAL_WITH_X_SIZED_BLOCK */
            public static string LoadSpecialXSize = "LoadSpecialXSize";
            public static string LoadSpecialYSize = "LoadSpecialYSize";

            public static string AmountToHaveInBank = "AmountToHaveInBank";         /* HAVE_X_AMOUNT_IN_THE_BANK */

            public static string AmmoGunToSend = "AmmoGunToSend";                   /* SEND_OUT_THREE_GUNS */

            public static string AmmoSpecialToSend = "ammoSpecialToSend";           /* SEND_OUT_THREE_SPECIALS */

            public static string AmountOfElementsMatching = "AmountOfElementsMatching"; /* MATCH_BONUS_ELEMENT_X_TIMES_IN_A_ROW */

            public static string AmountOfBossesToDefeat = "AmountOfBossesToDefeat"; /* DEFEAT_X_NUMBER_OF_BOSSES */
            public static string CurrentBossesDefeated = "CurrentBossesDefeated";

            public static string DaysToReach = "DaysToReach";                       /* REACH_X_DAY */

            public static string CurrentFreeTask = "CurrentFreeTask";
        }

        public static void Save()
        {
            PlayerPrefs.SetInt(SaveKeys.DefeatWithGun, (int)defeatWithGun);
            PlayerPrefs.SetInt(SaveKeys.PreviousDefeatWithGun, (int)previousDefeatWithGun);
            PlayerPrefs.SetInt(SaveKeys.AmountToDefeat, amountToDefeat);

            PlayerPrefs.SetInt(SaveKeys.AmmoToUpgrade, (int)ammoToUpgrade);
            PlayerPrefs.SetInt(SaveKeys.PreviousAmmoToUpgrade, (int)previousAmmoToUpgrade);
            PlayerPrefs.SetInt(SaveKeys.LeveToUpgrade, leveToUpgrade);

            PlayerPrefs.SetInt(SaveKeys.LevelToHealing, levelToHealing);

            PlayerPrefs.SetInt(SaveKeys.LevelToHeart, levelToHeart);
            PlayerPrefs.SetInt(SaveKeys.IsLevelingHeart, isLevelingHeart ? 1 : 0);

            PlayerPrefs.SetInt(SaveKeys.AmountofBlocksToMake, amountofBlocksToMake);
            PlayerPrefs.SetInt(SaveKeys.MakeXSize, makeXSize);
            PlayerPrefs.SetInt(SaveKeys.MakeYSize, makeYSize);

            PlayerPrefs.SetInt(SaveKeys.AmountOfSizedBlocksToLoadToGun, amountOfSizedBlocksToLoadToGun);
            PlayerPrefs.SetInt(SaveKeys.LoadGunXSize, loadGunXSize);
            PlayerPrefs.SetInt(SaveKeys.LoadGunYSize, loadGunYSize);

            PlayerPrefs.SetInt(SaveKeys.AmountOfSizedBlocksToLoadToSpecial, amountOfSizedBlocksToLoadToSpecial);
            PlayerPrefs.SetInt(SaveKeys.LoadSpecialXSize, loadSpecialXSize);
            PlayerPrefs.SetInt(SaveKeys.LoadSpecialYSize, loadSpecialYSize);

            PlayerPrefs.SetInt(SaveKeys.AmountToHaveInBank, amountToHaveInBank);

            PlayerPrefs.SetInt(SaveKeys.AmmoGunToSend, (int)ammoGunToSend);

            PlayerPrefs.SetInt(SaveKeys.AmmoSpecialToSend, (int)ammoSpecialToSend);

            PlayerPrefs.SetInt(SaveKeys.AmountOfElementsMatching, amountOfElementsMatching);

            PlayerPrefs.SetInt(SaveKeys.AmountOfBossesToDefeat, amountOfBossesToDefeat);
            PlayerPrefs.SetInt(SaveKeys.CurrentBossesDefeated, currentBossesDefeated);

            PlayerPrefs.SetInt(SaveKeys.DaysToReach, daysToReach);

            PlayerPrefs.SetString(SaveKeys.CurrentFreeTask, currentFreeTask);
        }

        public static void Load()
        {
            defeatWithGun = (Gun.Ammo)PlayerPrefs.GetInt(SaveKeys.DefeatWithGun, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousDefeatWithGun = (Gun.Ammo)PlayerPrefs.GetInt(SaveKeys.PreviousDefeatWithGun, (int)defeatWithGun);
            amountToDefeat = PlayerPrefs.GetInt(SaveKeys.AmountToDefeat, Random.Range(5, 10));

            ammoToUpgrade = (Gun.Ammo)PlayerPrefs.GetInt(SaveKeys.AmmoToUpgrade, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousAmmoToUpgrade = (Gun.Ammo)PlayerPrefs.GetInt(SaveKeys.PreviousAmmoToUpgrade, (int)ammoToUpgrade);
            leveToUpgrade = PlayerPrefs.GetInt(SaveKeys.LeveToUpgrade, DataStorage.GunPower[(int)ammoToUpgrade] + 2);

            levelToHealing = PlayerPrefs.GetInt(SaveKeys.LevelToHealing, DataStorage.Healing + 2);

            levelToHeart = PlayerPrefs.GetInt(SaveKeys.LevelToHeart, Mathf.Clamp(DataStorage.Hearts + 1, 0, 6));
            isLevelingHeart = PlayerPrefs.GetInt(SaveKeys.IsLevelingHeart, 1) == 1 ? true : false;

            amountofBlocksToMake = PlayerPrefs.GetInt(SaveKeys.AmountofBlocksToMake, Random.Range(3, 10));
            makeXSize = PlayerPrefs.GetInt(SaveKeys.MakeXSize, Random.Range(2, 4));
            makeYSize = PlayerPrefs.GetInt(SaveKeys.MakeYSize, Random.Range(2, 4));

            amountOfSizedBlocksToLoadToGun = PlayerPrefs.GetInt(SaveKeys.AmountOfSizedBlocksToLoadToGun, Random.Range(3, 10));
            loadGunXSize = PlayerPrefs.GetInt(SaveKeys.LoadGunXSize, Random.Range(2, 4));
            loadGunYSize = PlayerPrefs.GetInt(SaveKeys.LoadGunYSize, Random.Range(2, 4));

            amountOfSizedBlocksToLoadToSpecial = PlayerPrefs.GetInt(SaveKeys.AmountOfSizedBlocksToLoadToSpecial, Random.Range(3, 10));
            loadSpecialXSize = PlayerPrefs.GetInt(SaveKeys.LoadSpecialXSize, Random.Range(2, 4));
            loadSpecialYSize = PlayerPrefs.GetInt(SaveKeys.LoadSpecialYSize, Random.Range(2, 4));

            currentFreeTask = PlayerPrefs.GetString(SaveKeys.CurrentFreeTask);

            int difference = 0;
            if (DataStorage.Money > 10000) {
                int targetAmount = (DataStorage.Money + 10000);
                int whole = (int)Mathf.Pow(10, Mathf.FloorToInt(Mathf.Log10(targetAmount)) - 2);
                int remainder = targetAmount % whole;
                difference = whole - remainder;
            }
            amountToHaveInBank = PlayerPrefs.GetInt(SaveKeys.AmountToHaveInBank, DataStorage.Money + 10000 + difference);

            ammoGunToSend = (Gun.Ammo)PlayerPrefs.GetInt(SaveKeys.AmmoGunToSend, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));

            ammoSpecialToSend = (Gun.Ammo)PlayerPrefs.GetInt(SaveKeys.AmmoSpecialToSend, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));

            amountOfElementsMatching = PlayerPrefs.GetInt(SaveKeys.AmountOfElementsMatching, Random.Range(2, 5));

            amountOfBossesToDefeat = PlayerPrefs.GetInt(SaveKeys.AmountOfBossesToDefeat, Random.Range(2, 5));
            currentBossesDefeated = PlayerPrefs.GetInt(SaveKeys.CurrentBossesDefeated);

            int currentDay = (DataStorage.StartOnWave / 3 + 1);
            daysToReach = PlayerPrefs.GetInt(SaveKeys.DaysToReach, Mathf.Clamp(currentDay, currentDay + 1, currentDay + 4));
        }
    }
}