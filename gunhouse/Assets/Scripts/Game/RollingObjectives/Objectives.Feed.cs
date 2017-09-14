using UnityEngine;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        static bool IsActiveTask(TaskType task)
        {
            for (int i = 0; i < activeTasks.Length; ++i) if (possibleTasks[activeTasks[i]] == task) return true;
            return false;
        }

        public static void DefeatedWithAmmo(Gun.Ammo ammo)
        {
            if (!IsActiveTask(TaskType.DEFEAT_ENEMIES_WITH_GUN)) return;

            if (ammo != defeatWithGun) return;
            currentAmountDefeated++;
        }

        public static void CreatedBlockOfSize(float x, float y)
        {
            if (!IsActiveTask(TaskType.MAKE_X_NUMBER_SIZED_BLOCKS)) return;

            if (makeXSize != x || makeYSize != y) return;
            currentAmountOfBlocksMade++;
        }

        public static void LoadGun(float x, float y)
        {
            if (!IsActiveTask(TaskType.LOAD_GUN_WITH_X_SIZED_BLOCKS)) return;

            if (loadGunXSize != x || loadGunYSize != y) return;
            currentBlocksLoadedToGun++;
        }

        public static void LoadSpecial(float x, float y)
        {
            if (!IsActiveTask(TaskType.LOAD_SPECIAL_WITH_X_SIZED_BLOCK)) return;

            if (loadSpecialXSize != x || loadSpecialYSize != y) return;
            currentBlocksLoadedToSpecial++;
        }

        public static void LoadBonusElement(bool bonus)
        {
            if (!IsActiveTask(TaskType.MATCH_BONUS_ELEMENT_X_TIMES_IN_A_ROW)) return;
            if (currentElementsMatched >= amountOfElementsMatching) return;

            currentElementsMatched = bonus ? currentElementsMatched + 1 : 0;
        }

        public static void FireGun(Gun.Ammo ammo)
        {
            if (!IsActiveTask(TaskType.SEND_OUT_THREE_GUNS)) return;

            if (ammo != ammoGunToSend) return;

            currentAmountOfGunsSent++;
        }

        public static void FireSpecial(Gun.Ammo ammo)
        {
            if (!IsActiveTask(TaskType.SEND_OUT_THREE_SPECIALS)) return;

            if (ammo != ammoSpecialToSend) return;

            currentAmountOfSpecialsSent++;
        }

        public static void BossDefeated()
        {
            if (!IsActiveTask(TaskType.DEFEAT_X_NUMBER_OF_BOSSES)) return;

            currentBossesDefeated++;
        }

        public static void DoorOpen()
        {
            if (IsActiveTask(TaskType.SEND_OUT_THREE_GUNS)) {
                if (currentAmountOfGunsSent < amountOfGunsToSend) {
                    currentAmountOfGunsSent = 0;
                }
            }

            if (IsActiveTask(TaskType.SEND_OUT_THREE_SPECIALS)) {
                if (currentAmountOfSpecialsSent < amountOfSpecialsToSend) {
                    currentAmountOfSpecialsSent = 0;
                }
            }
        }

        public static void SurvivedFinalStage()
        {
            if (!IsActiveTask(TaskType.SEE_THE_ENDING)) return;

            seeTheEnding = true;
        }
    }
}