using UnityEngine;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        static Gun.Ammo defeatWithGun = Gun.Ammo.DRAGON;    /* TaskType.DEFEAT_ENEMIES_WITH_GUN */
        static int amountToDefeat = 10;
        static int currentAmountDefeated = 0;
        static Gun.Ammo previousDefeatWithGun = Gun.Ammo.NONE;

        static Gun.Ammo ammoToUpgrade = Gun.Ammo.IGLOO;     /* TaskType.UPGRADE_GUN_TO_LEVEL */
        static int leveToUpgrade = 5;
        static Gun.Ammo previousAmmoToUpgrade = Gun.Ammo.NONE;

        static int levelToHealing = 5;                      /* TaskType.UPGRADE_HEALING_TO_LEVEL */
        static bool isLevelingHeart = false;
        static int levelToHeart;
        static int levelToArmor = 5;                        /* TaskType.UPGRADE_ARMOR_TO_LEVEL */

        static int amountToHaveInBank = 50000;              /* TaskType.HAVE_X_AMOUNT_IN_THE_BANK */

        static int amountofBlocksToMake = 10;               /* TaskType.MAKE_X_NUMBER_SIZED_BLOCKS */
        static int makeXSize = 2;
        static int makeYSize = 3;
        static int currentAmountOfBlocksMade = 0;

        static int amountOfSizedBlocksToLoadToGun = 10;     /* LOAD_GUN_WITH_X_SIZED_BLOCKS */
        static int loadGunXSize = 2;
        static int loadGunYSize = 3;
        static int currentBlocksLoadedToGun = 0;

        static int amountOfSizedBlocksToLoadToSpecial = 10; /* LOAD_SPECIAL_WITH_X_SIZED_BLOCK */
        static int loadSpecialXSize = 2;
        static int loadSpecialYSize = 3;
        static int currentBlocksLoadedToSpecial = 0;

        static int amountOfElementsMatching = 3;            /* MATCH_THE_BONUS_ELEMENT_X_TIMES_IN_A_ROW */
        static int currentElementsMatched = 0;

        static Gun.Ammo ammoGunToSend = Gun.Ammo.DRAGON;    /* SEND_OUT_THREE_GUN */
        static int amountOfGunsToSend = 3;
        static int currentAmountOfGunsSent;

        static Gun.Ammo ammoSpecialToSend = Gun.Ammo.DRAGON;/* SEND_OUT_THREE_SPECIALS */
        static int amountOfSpecialsToSend = 3;
        static int currentAmountOfSpecialsSent;

        public static int amountOfBossesToDefeat;           /* DEFEAT_X_NUMBER_OF_BOSSES */
        public static int currentBossesDefeated;

        static int daysToReach;                             /* REACH_X_DAY */

        static int[] weaponsToBeat = new int[3];            /* BEAT_A_STAGE_USING_XYZ_WEAPONS */

        static bool seeTheEnding;                           /* SEE_THE_ENDING */

        static string currentFreeTask;                      /* FREE */

        public void CheckComplete()
        {
            for (int i = 0; i < activeTasks.Length; ++i) {
                switch (possibleTasks[activeTasks[i]])
                {
                    case TaskType.DEFEAT_ENEMIES_WITH_GUN:
                    {
                        if (currentAmountDefeated >= amountToDefeat) {
                            currentAmountDefeated = 0;
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.UPGRADE_GUN_TO_LEVEL:
                    {
                        if (DataStorage.GunPower[(int)ammoToUpgrade] >= leveToUpgrade) {
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.UPGRADE_HEALING_TO_LEVEL:
                    {
                        if (DataStorage.Healing >= levelToHealing) {
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.UPGRADE_ARMOR_TO_LEVEL:
                    {
                        if (isLevelingHeart) {
                            if (DataStorage.Hearts >= levelToHeart) {
                                isLevelingHeart = false;
                                SetTaskComplete(i);
                            }
                        }
                        else if (DataStorage.Armor >= levelToArmor) {
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.HAVE_X_AMOUNT_IN_THE_BANK: {
                        if (DataStorage.Money >= amountToHaveInBank) {
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.MAKE_X_NUMBER_SIZED_BLOCKS: {
                        if (currentAmountOfBlocksMade >= amountofBlocksToMake) {
                            currentAmountOfBlocksMade = 0;
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.LOAD_GUN_WITH_X_SIZED_BLOCKS: {
                        if (currentBlocksLoadedToGun >= amountOfSizedBlocksToLoadToGun) {
                            currentBlocksLoadedToGun = 0;
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.LOAD_SPECIAL_WITH_X_SIZED_BLOCK: {
                        if (currentBlocksLoadedToSpecial >= amountOfSizedBlocksToLoadToSpecial) {
                            currentBlocksLoadedToSpecial = 0;
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.MATCH_BONUS_ELEMENT_X_TIMES_IN_A_ROW: {
                        if (currentElementsMatched >= amountOfElementsMatching) {
                            currentElementsMatched = 0;
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.SEND_OUT_THREE_GUNS: {
                        if (currentAmountOfGunsSent >= amountOfGunsToSend) {
                            currentAmountOfGunsSent = 0;
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.SEND_OUT_THREE_SPECIALS: {
                        if (currentAmountOfSpecialsSent >= amountOfSpecialsToSend) {
                            currentAmountOfSpecialsSent = 0;
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.DEFEAT_X_NUMBER_OF_BOSSES: {
                        if (currentBossesDefeated >= amountOfBossesToDefeat) {
                            currentBossesDefeated = 0;
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.REACH_X_DAY: {
                        if ((DataStorage.StartOnWave / 3 + 1) >= daysToReach) {
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.BEAT_A_STAGE_USING_XYZ_WEAPONS: {
                        int usedCorrectWeapons = 0;

                        for (int n = 0; n < DataStorage.GunEquipped.Length; ++n) {
                            if (!DataStorage.GunEquipped[n]) continue;

                            for (int m = 0; m < weaponsToBeat.Length; ++m) {
                                if (n != weaponsToBeat[m]) continue;
                                usedCorrectWeapons++;
                            }
                        }

                        if (usedCorrectWeapons >= 3) {
                            SetTaskComplete(i);
                        }
                    } break;

                    case TaskType.SEE_THE_ENDING: {
                        if (seeTheEnding) {
                            seeTheEnding = false;
                            SetTaskComplete(i);
                        }

                    } break;

                    case TaskType.FREE: { SetTaskComplete(i); } break;
                }
            }
        }

        public void UpdateText()
        {
            for (int i = 0; i < activeTasks.Length; ++i) SetTaskText(i, false);
        }

        public static void CheckAchievements()
        {
            #if UNITY_PSP2 || UNITY_PS4
            if (DataStorage.AmountOfObjectivesComplete > 74) { PlayStationVita.AwardTrophy(Achievement.LikeMothsToFlame); }
            if (DataStorage.AmountOfObjectivesComplete > 49) { PlayStationVita.AwardTrophy(Achievement.BlueSkies); }
            if (DataStorage.AmountOfObjectivesComplete > 19) { PlayStationVita.AwardTrophy(Achievement.Rambunctious); }
            if (DataStorage.AmountOfObjectivesComplete > 9) { PlayStationVita.AwardTrophy(Achievement.Misdeeds); }
            if (DataStorage.AmountOfObjectivesComplete > 2) { PlayStationVita.AwardTrophy(Achievement.ToothSome); }

            int currentDay = (DataStorage.StartOnWave / 3 + 1);
            if (currentDay > 100) { PlayStationVita.AwardTrophy(Achievement.TooManyGuns); }
            if (currentDay > 30) { PlayStationVita.AwardTrophy(Achievement.Molytrols); }
            if (currentDay > 20) { PlayStationVita.AwardTrophy(Achievement.AnotherPeter); }
            if (currentDay > 10) { PlayStationVita.AwardTrophy(Achievement.Savior); }
            if (currentDay > 5) { PlayStationVita.AwardTrophy(Achievement.HalfStep); }
            #else
            if (DataStorage.AmountOfObjectivesComplete > 74) { AppMain.menuAchievements.Award(Achievement.LikeMothsToFlame); }
            if (DataStorage.AmountOfObjectivesComplete > 49) { AppMain.menuAchievements.Award(Achievement.BlueSkies); }
            if (DataStorage.AmountOfObjectivesComplete > 19) { AppMain.menuAchievements.Award(Achievement.Rambunctious); }
            if (DataStorage.AmountOfObjectivesComplete > 9) { AppMain.menuAchievements.Award(Achievement.Misdeeds); }
            if (DataStorage.AmountOfObjectivesComplete > 2) { AppMain.menuAchievements.Award(Achievement.ToothSome); }

            int currentDay = (DataStorage.StartOnWave / 3 + 1);
            if (currentDay > 100) { AppMain.menuAchievements.Award(Achievement.TooManyGuns); }
            if (currentDay > 30) { AppMain.menuAchievements.Award(Achievement.Molytrols); }
            if (currentDay > 20) { AppMain.menuAchievements.Award(Achievement.AnotherPeter); }
            if (currentDay > 10) { AppMain.menuAchievements.Award(Achievement.Savior); }
            if (currentDay > 5) { AppMain.menuAchievements.Award(Achievement.HalfStep); }
            #endif
        }
    }
}