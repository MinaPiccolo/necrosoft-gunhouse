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
                if ((DataStorage.StartOnWave-3)/15 > DataStorage.DisconcertingObjectivesSeen &&
                    DataStorage.DisconcertingObjectivesSeen < GText.Objectives.disconcerting_tasks.Length) {
                    currentFreeTask = GText.Objectives.disconcerting_tasks[DataStorage.DisconcertingObjectivesSeen++];
                    if (DataStorage.DisconcertingObjectivesSeen >= 20) {
                        Tracker.AchievementUnlocked("door_alt");
                        AppMain.textures.door = new Atlas("atlases/door_alt.png",
                                                          new Vector2(192, 382),
                                                          AppMain.textures.door.texture.z_order);
                    }
                }
                else {
                    Tracker.AchievementStep(DataStorage.DisconcertingObjectivesSeen, "door_alt");
                    int randomRange = (DataStorage.StartOnWave / 3 + 1) > 9 ? GText.Objectives.free_tasks.Length : 4;
                    currentFreeTask = GText.Objectives.free_tasks[Random.Range(0, randomRange)];
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
                stringBuilder.AppendFormat(GText.Objectives.defeat_enemies_with_gun,
                                           currentAmountDefeated, amountToDefeat,
                                           GText.Objectives.weapon_names[(int)defeatWithGun]);
            } break;

            case TaskType.UPGRADE_GUN_TO_LEVEL: {
                stringBuilder.AppendFormat(GText.Objectives.upgrade_gun_to_level,
                                           GText.Objectives.weapon_names[(int)ammoToUpgrade], leveToUpgrade);
            } break;

            case TaskType.UPGRADE_HEALING_TO_LEVEL: {
                stringBuilder.AppendFormat(GText.Objectives.upgrade_healing_to_level, levelToHealing);
            } break;

            case TaskType.UPGRADE_ARMOR_TO_LEVEL: {
                if (isLevelingHeart) {
                    stringBuilder.AppendFormat(GText.Objectives.upgrade_hearts_to_level, levelToHeart);
                }
                else {
                    stringBuilder.AppendFormat(GText.Objectives.upgrade_armor_to_level, levelToArmor);
                }
            } break;

            case TaskType.MAKE_X_NUMBER_SIZED_BLOCKS: {
                stringBuilder.AppendFormat(GText.Objectives.make_x_number_sized_blocks,
                                           currentAmountOfBlocksMade, amountofBlocksToMake, makeXSize, makeYSize);
            } break;

            case TaskType.LOAD_GUN_WITH_X_SIZED_BLOCKS: {
                stringBuilder.AppendFormat(GText.Objectives.load_gun_with_x_sized_blocks,
                                           currentBlocksLoadedToGun, amountOfSizedBlocksToLoadToGun,
                                           loadGunXSize, loadGunYSize);
            } break;

            case TaskType.LOAD_SPECIAL_WITH_X_SIZED_BLOCK: {
                stringBuilder.AppendFormat(GText.Objectives.load_special_with_x_sized_blocks,
                                           currentBlocksLoadedToSpecial, amountOfSizedBlocksToLoadToSpecial,
                                           loadSpecialXSize, loadSpecialYSize);
            } break;

            case TaskType.HAVE_X_AMOUNT_IN_THE_BANK: {
                stringBuilder.AppendFormat(GText.Objectives.have_x_amount_in_the_bank, amountToHaveInBank);
            } break;

            case TaskType.SEND_OUT_THREE_GUNS: {
                stringBuilder.AppendFormat(GText.Objectives.send_out_three_guns,
                                           GText.Objectives.weapon_names[(int)ammoGunToSend], amountOfGunsToSend);
            } break;

            case TaskType.SEND_OUT_THREE_SPECIALS: {
                stringBuilder.AppendFormat(GText.Objectives.send_out_three_specials,
                                           GText.Objectives.weapon_names[(int)ammoSpecialToSend], amountOfSpecialsToSend);
            } break;

            case TaskType.MATCH_BONUS_ELEMENT_X_TIMES_IN_A_ROW: {
                stringBuilder.AppendFormat(GText.Objectives.match_bonus_element,
                                           currentElementsMatched, amountOfElementsMatching);
            } break;

            case TaskType.DEFEAT_X_NUMBER_OF_BOSSES: {
                stringBuilder.AppendFormat(GText.Objectives.defeat_x_number_of_bosses,
                                           amountOfBossesToDefeat, currentBossesDefeated);
            } break;

            case TaskType.REACH_X_DAY: {
                stringBuilder.AppendFormat(GText.Objectives.reach_x_day, daysToReach);
            } break;

            case TaskType.BEAT_A_STAGE_USING_XYZ_WEAPONS: {
                stringBuilder.AppendFormat(GText.Objectives.beat_a_stage_using,
                                           GText.Objectives.weapon_names[weaponsToBeat[0]],
                                           GText.Objectives.weapon_names[weaponsToBeat[1]],
                                           GText.Objectives.weapon_names[weaponsToBeat[2]]);
            } break;

            case TaskType.SEE_THE_ENDING: {
                if ((DataStorage.StartOnWave / 3 + 1) < 10) {
                    stringBuilder.AppendFormat(GText.Objectives.see_the_ending);
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