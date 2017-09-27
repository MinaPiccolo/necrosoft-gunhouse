using UnityEngine;
using System.Collections.Generic;
using Necrosoft;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        public static List<int> previousTasks = new List<int>();
        public static Queue<int> taskHistory = new Queue<int>();
        int taskHistoryMax = 5;

        int[] days = new int[]      {  3 * 3, 5 * 3, 6 * 3, 7 * 3, 8 * 3 };
        int[] taskRange = new int[] { 10,     2,     1,     3,     2 };
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(400);

        void RequestTask(int index)
        {
            DataStorage.AmountOfObjectivesComplete++;
            CheckAchievements();

            int taskCount = 10;
            for (int i = 1; i < days.Length; ++i) {
                if (DataStorage.StartOnWave >= days[i]) {
                    taskCount += taskRange[i];
                    continue;
                }

                CreateTask(index, FreshRandom(0, taskCount));
                return;
            }

            CreateTask(index, FreshRandom(0, possibleTasks.Length - 1));
        }

        int FreshRandom(int min, int max)
        {
            int number = Random.Range(min, max);

            while (previousTasks.Contains(number) || taskHistory.Contains(number) ||
                   (possibleTasks[number] == TaskType.FREE && CheckForTask(previousTasks, TaskType.FREE))) {
                number = Random.Range(min, max);
            }

            return number;
        }

        bool CheckForTask(List<int> list, TaskType task)
        {
            for (int i = 0; i < list.Count; i++) if (possibleTasks[list[i]] == task) return true;
            return false;
        }

        void CreateTask(int i, int taskIndex)
        {
            previousTasks.Remove(activeTasks[i]);
            activeTasks[i] = taskIndex;
            previousTasks.Add(activeTasks[i]);

            UpdateRequestHistory(i);
            SetTaskDetails(i);
            SetTaskText(i);
        }

        void UpdateRequestHistory(int index)
        {
            taskHistory.Enqueue(activeTasks[index]);
            while (taskHistory.Count >= taskHistoryMax) taskHistory.Dequeue();
        }

        void SetTaskDetails(int i, bool displaySlow = true)
        {
            switch(possibleTasks[activeTasks[i]])
            {
                case TaskType.DEFEAT_ENEMIES_WITH_GUN: {
                    defeatWithGun = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN);
                    while (defeatWithGun == previousDefeatWithGun) {
                        defeatWithGun = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN);
                    }
                    previousDefeatWithGun = defeatWithGun;

                    amountToDefeat = Random.Range(5, 10);
                } break;

                case TaskType.UPGRADE_GUN_TO_LEVEL: {
                    ammoToUpgrade = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN);
                    while (ammoToUpgrade == previousAmmoToUpgrade) {
                        ammoToUpgrade = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN);
                    }
                    previousAmmoToUpgrade = ammoToUpgrade;

                    leveToUpgrade = DataStorage.GunPower[(int)ammoToUpgrade] + 2;
                } break;

                case TaskType.UPGRADE_HEALING_TO_LEVEL: {
                    levelToHealing = DataStorage.Healing + 2;
                } break;

                case TaskType.UPGRADE_ARMOR_TO_LEVEL: {
                    if (DataStorage.Hearts < 6) {
                        levelToHeart = Mathf.Clamp(DataStorage.Hearts + 1, 0, 6);
                        isLevelingHeart = true;
                    }
                    else { 
                        levelToArmor = DataStorage.Armor + 2;
                        isLevelingHeart = false;
                    }
                } break;

                case TaskType.MAKE_X_NUMBER_SIZED_BLOCKS: {
                    amountofBlocksToMake = Random.Range(3, 10);
                    makeXSize = Random.Range(2, 4);
                    makeYSize = Random.Range(2, 4);
                } break;

                case TaskType.LOAD_GUN_WITH_X_SIZED_BLOCKS: {
                    amountOfSizedBlocksToLoadToGun = Random.Range(3, 10);
                    loadGunXSize = Random.Range(2, 4);
                    loadGunYSize = Random.Range(2, 4);
                } break;

                case TaskType.LOAD_SPECIAL_WITH_X_SIZED_BLOCK: {
                    amountOfSizedBlocksToLoadToSpecial = Random.Range(3, 10);
                    loadSpecialXSize = Random.Range(2, 4);
                    loadSpecialYSize = Random.Range(2, 4);
                } break;

                case TaskType.HAVE_X_AMOUNT_IN_THE_BANK: {
                    int difference = 0;
                    if (DataStorage.Money > 10000) {
                        int targetAmount = (DataStorage.Money + 10000);
                        int whole = (int)Mathf.Pow(10, Mathf.FloorToInt(Mathf.Log10(targetAmount)) - 2);
                        int remainder = targetAmount % whole;
                        difference = whole - remainder;
                    }
                    amountToHaveInBank = DataStorage.Money + 10000 + difference;
                } break;

                case TaskType.SEND_OUT_THREE_GUNS: {
                    ammoGunToSend = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING);
                } break;

                case TaskType.SEND_OUT_THREE_SPECIALS: {
                    ammoSpecialToSend = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING);
                } break;

                case TaskType.MATCH_BONUS_ELEMENT_X_TIMES_IN_A_ROW: {
                    amountOfElementsMatching = Random.Range(2, 5);
                } break;

                case TaskType.DEFEAT_X_NUMBER_OF_BOSSES: {
                    amountOfBossesToDefeat = Random.Range(2, 5);
                } break;

                case TaskType.REACH_X_DAY: {
                    int currentDay = (DataStorage.StartOnWave / 3 + 1);
                    daysToReach = Mathf.Clamp(currentDay, currentDay + 1, currentDay + 4);
                } break;

                case TaskType.BEAT_A_STAGE_USING_XYZ_WEAPONS: {
                    HashSet<int> generatedWeapons = new HashSet<int>();
                    weaponsToBeat[0] = Necrosoft.Math.Random.UniqueRandom((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING, generatedWeapons);
                    CheckWeaponConflict((Gun.Ammo)weaponsToBeat[0], generatedWeapons);
                    weaponsToBeat[1] = Necrosoft.Math.Random.UniqueRandom((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING, generatedWeapons);
                    CheckWeaponConflict((Gun.Ammo)weaponsToBeat[1], generatedWeapons);
                    weaponsToBeat[2] = Necrosoft.Math.Random.UniqueRandom((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING, generatedWeapons);
                } break;

                case TaskType.FREE: {
                    if((DataStorage.StartOnWave-3)/15 > DataStorage.DisconcertingObjectivesSeen &&
                       DataStorage.DisconcertingObjectivesSeen < disconcertingTasks.Length) {
                        currentFreeTask = disconcertingTasks[DataStorage.DisconcertingObjectivesSeen++];
                        if (DataStorage.DisconcertingObjectivesSeen >= 20) {
                            Tracker.AchievementUnlocked("door_alt");
                            AppMain.textures.door = new Atlas("atlases/door_alt.png",
                                                              new Vector2(192, 382),
                                                              AppMain.textures.door.texture.z_order);
                        }
                    }
                    else {
                        Tracker.AchievementStep(DataStorage.DisconcertingObjectivesSeen, "door_alt");
                        int randomRange = (DataStorage.StartOnWave / 3 + 1) > 9 ? freeTasks.Length : 4;
                        currentFreeTask = freeTasks[Random.Range(0, randomRange)];
                    }
                } break;
            }
        }

        void CheckWeaponConflict(Gun.Ammo weapon, HashSet<int> generatedWeapons)
        {
            switch (weapon)
            {
            case Gun.Ammo.DRAGON: generatedWeapons.Add((int)Gun.Ammo.FLAME); break;
            case Gun.Ammo.FLAME: generatedWeapons.Add((int)Gun.Ammo.DRAGON); break;
            case Gun.Ammo.IGLOO: generatedWeapons.Add((int)Gun.Ammo.FORK); break;
            case Gun.Ammo.FORK: generatedWeapons.Add((int)Gun.Ammo.IGLOO); break;
            case Gun.Ammo.SKULL: generatedWeapons.Add((int)Gun.Ammo.BOUNCE); break;
            case Gun.Ammo.BOUNCE: generatedWeapons.Add((int)Gun.Ammo.SKULL); break;
            case Gun.Ammo.VEGETABLE: generatedWeapons.Add((int)Gun.Ammo.BOOMERANG); break;
            case Gun.Ammo.BOOMERANG: generatedWeapons.Add((int)Gun.Ammo.VEGETABLE); break;
            case Gun.Ammo.LIGHTNING: generatedWeapons.Add((int)Gun.Ammo.SIN); break;
            case Gun.Ammo.SIN: generatedWeapons.Add((int)Gun.Ammo.LIGHTNING); break;
            }
        }

        void SetTaskText(int i, bool displaySlow = true)
        {
            stringBuilder.Length = 0;

            switch(possibleTasks[activeTasks[i]])
            {
                case TaskType.DEFEAT_ENEMIES_WITH_GUN: {
                    stringBuilder.AppendFormat("Defeat {1} enemies with the {2} Gun ({0}/{1})",
                                               currentAmountDefeated, amountToDefeat, WeaponNames[(int)defeatWithGun]);
                } break;

                case TaskType.UPGRADE_GUN_TO_LEVEL: {
                    stringBuilder.AppendFormat("Upgrade {0} Gun to level {1}", WeaponNames[(int)ammoToUpgrade], leveToUpgrade);
                } break;

                case TaskType.UPGRADE_HEALING_TO_LEVEL: {
                    stringBuilder.AppendFormat("Upgrade Healing to level {0}", levelToHealing);
                } break;

                case TaskType.UPGRADE_ARMOR_TO_LEVEL: {
                    if (isLevelingHeart) {
                        stringBuilder.AppendFormat("Upgrade Hearts to level {0}", levelToHeart);
                    }
                    else {
                        stringBuilder.AppendFormat("Upgrade Armor to level {0}", levelToArmor);
                    }
                } break;

                case TaskType.MAKE_X_NUMBER_SIZED_BLOCKS: {
                    stringBuilder.AppendFormat("Make {1} blocks of {2}x{3} size ({0}/{1})",
                                               currentAmountOfBlocksMade, amountofBlocksToMake, makeXSize, makeYSize);
                } break;

                case TaskType.LOAD_GUN_WITH_X_SIZED_BLOCKS: {
                    stringBuilder.AppendFormat("Load {1} Guns with blocks of {2}x{3} size ({0}/{1})",
                                               currentBlocksLoadedToGun, amountOfSizedBlocksToLoadToGun,
                                               loadGunXSize, loadGunYSize);
                } break;

                case TaskType.LOAD_SPECIAL_WITH_X_SIZED_BLOCK: {
                    stringBuilder.AppendFormat("Load {1} Specials with blocks of {2}x{3} size ({0}/{1})",
                                               currentBlocksLoadedToSpecial, amountOfSizedBlocksToLoadToSpecial,
                                               loadSpecialXSize, loadSpecialYSize);
                } break;

                case TaskType.HAVE_X_AMOUNT_IN_THE_BANK: {
                    stringBuilder.AppendFormat("Have ${0} in the bank", amountToHaveInBank);
                } break;

                case TaskType.SEND_OUT_THREE_GUNS: {
                    stringBuilder.AppendFormat("Fire {0} Gun {1} times in one turn",
                                               WeaponNames[(int)ammoGunToSend], amountOfGunsToSend);
                } break;

                case TaskType.SEND_OUT_THREE_SPECIALS: {
                    stringBuilder.AppendFormat("Fire {0} Special {1} times in one turn",
                                               WeaponNames[(int)ammoSpecialToSend], amountOfSpecialsToSend);
                } break;

                case TaskType.MATCH_BONUS_ELEMENT_X_TIMES_IN_A_ROW: {
                    stringBuilder.AppendFormat("Match the bonus element {1} times in a row ({0}/{1})",
                                               currentElementsMatched, amountOfElementsMatching);
                } break;

                case TaskType.DEFEAT_X_NUMBER_OF_BOSSES: {
                    stringBuilder.AppendFormat("Defeat {0} bosses! {1}/{0}",
                                                amountOfBossesToDefeat, currentBossesDefeated);
                } break;

                case TaskType.REACH_X_DAY: {
                    stringBuilder.AppendFormat("Reach day {0}", daysToReach);
                } break;

                case TaskType.BEAT_A_STAGE_USING_XYZ_WEAPONS: {
                    stringBuilder.AppendFormat("Beat a wave using {0}, {1}, and {2} weapons",
                                               WeaponNames[weaponsToBeat[0]], WeaponNames[weaponsToBeat[1]],
                                               WeaponNames[weaponsToBeat[2]]);
                } break;

                case TaskType.SEE_THE_ENDING: {
                    if ((DataStorage.StartOnWave / 3 + 1) < 10) {
                        stringBuilder.AppendFormat("See the ending");
                    }
                    else {
                        RequestTask(i);
                    }
                } break;

                case TaskType.FREE: {
                    stringBuilder.AppendFormat(currentFreeTask);
                } break;
            }

            if (displaySlow) TextBlock.Display(tasks[i], stringBuilder.ToString());
            else tasks[i].text = stringBuilder.ToString();
        }
    }
}