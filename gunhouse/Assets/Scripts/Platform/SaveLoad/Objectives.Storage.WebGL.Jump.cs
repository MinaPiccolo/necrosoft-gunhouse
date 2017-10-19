#if UNITY_WEBGL
using UnityEngine;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        public static void SaveRemote()
        {
            WebGLJump.SaveData(SaveKeys.DefeatWithGun, ((int)defeatWithGun).ToString());
            WebGLJump.SaveData(SaveKeys.PreviousDefeatWithGun, ((int)previousDefeatWithGun).ToString());
            WebGLJump.SaveData(SaveKeys.AmountToDefeat, amountToDefeat.ToString());

            WebGLJump.SaveData(SaveKeys.AmmoToUpgrade, ((int)ammoToUpgrade).ToString());
            WebGLJump.SaveData(SaveKeys.PreviousAmmoToUpgrade, ((int)previousAmmoToUpgrade).ToString());
            WebGLJump.SaveData(SaveKeys.LeveToUpgrade, leveToUpgrade.ToString());

            WebGLJump.SaveData(SaveKeys.LevelToHealing, levelToHealing.ToString());

            WebGLJump.SaveData(SaveKeys.LevelToHeart, levelToHeart.ToString());
            WebGLJump.SaveData(SaveKeys.IsLevelingHeart, (isLevelingHeart ? 1 : 0).ToString());

            WebGLJump.SaveData(SaveKeys.AmountofBlocksToMake, amountofBlocksToMake.ToString());
            WebGLJump.SaveData(SaveKeys.MakeXSize, makeXSize.ToString());
            WebGLJump.SaveData(SaveKeys.MakeYSize, makeYSize.ToString());

            WebGLJump.SaveData(SaveKeys.AmountOfSizedBlocksToLoadToGun, amountOfSizedBlocksToLoadToGun.ToString());
            WebGLJump.SaveData(SaveKeys.LoadGunXSize, loadGunXSize.ToString());
            WebGLJump.SaveData(SaveKeys.LoadGunYSize, loadGunYSize.ToString());

            WebGLJump.SaveData(SaveKeys.AmountOfSizedBlocksToLoadToSpecial, amountOfSizedBlocksToLoadToSpecial.ToString());
            WebGLJump.SaveData(SaveKeys.LoadSpecialXSize, loadSpecialXSize.ToString());
            WebGLJump.SaveData(SaveKeys.LoadSpecialYSize, loadSpecialYSize.ToString());

            WebGLJump.SaveData(SaveKeys.AmountToHaveInBank, amountToHaveInBank.ToString());

            WebGLJump.SaveData(SaveKeys.AmmoGunToSend, ((int)ammoGunToSend).ToString());

            WebGLJump.SaveData(SaveKeys.AmmoSpecialToSend, ((int)ammoSpecialToSend).ToString());

            WebGLJump.SaveData(SaveKeys.AmountOfElementsMatching, amountOfElementsMatching.ToString());

            WebGLJump.SaveData(SaveKeys.AmountOfBossesToDefeat, amountOfBossesToDefeat.ToString());
            WebGLJump.SaveData(SaveKeys.CurrentBossesDefeated, currentBossesDefeated.ToString());

            WebGLJump.SaveData(SaveKeys.DaysToReach, daysToReach.ToString());

            WebGLJump.SaveData(SaveKeys.CurrentFreeTask, currentFreeTask);
        }

        public static void LoadRemote()
        {
            defeatWithGun = (Gun.Ammo)WebGLJump.LoadData(SaveKeys.DefeatWithGun, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousDefeatWithGun = (Gun.Ammo)WebGLJump.LoadData(SaveKeys.PreviousDefeatWithGun, (int)defeatWithGun);
            amountToDefeat = WebGLJump.LoadData(SaveKeys.AmountToDefeat, Random.Range(5, 10));

            ammoToUpgrade = (Gun.Ammo)WebGLJump.LoadData(SaveKeys.AmmoToUpgrade, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousAmmoToUpgrade = (Gun.Ammo)WebGLJump.LoadData(SaveKeys.PreviousAmmoToUpgrade, (int)ammoToUpgrade);
            leveToUpgrade = WebGLJump.LoadData(SaveKeys.LeveToUpgrade, DataStorage.GunPower[(int)ammoToUpgrade] + 2);

            levelToHealing = WebGLJump.LoadData(SaveKeys.LevelToHealing, DataStorage.Healing + 2);

            levelToHeart = WebGLJump.LoadData(SaveKeys.LevelToHeart, Mathf.Clamp(DataStorage.Hearts + 1, 0, 6));
            isLevelingHeart = WebGLJump.LoadData(SaveKeys.IsLevelingHeart, 1) == 1 ? true : false;

            amountofBlocksToMake = WebGLJump.LoadData(SaveKeys.AmountofBlocksToMake, Random.Range(3, 10));
            makeXSize = WebGLJump.LoadData(SaveKeys.MakeXSize, Random.Range(2, 4));
            makeYSize = WebGLJump.LoadData(SaveKeys.MakeYSize, Random.Range(2, 4));

            amountOfSizedBlocksToLoadToGun = WebGLJump.LoadData(SaveKeys.AmountOfSizedBlocksToLoadToGun, Random.Range(3, 10));
            loadGunXSize = WebGLJump.LoadData(SaveKeys.LoadGunXSize, Random.Range(2, 4));
            loadGunYSize = WebGLJump.LoadData(SaveKeys.LoadGunYSize, Random.Range(2, 4));

            amountOfSizedBlocksToLoadToSpecial = WebGLJump.LoadData(SaveKeys.AmountOfSizedBlocksToLoadToSpecial, Random.Range(3, 10));
            loadSpecialXSize = WebGLJump.LoadData(SaveKeys.LoadSpecialXSize, Random.Range(2, 4));
            loadSpecialYSize = WebGLJump.LoadData(SaveKeys.LoadSpecialYSize, Random.Range(2, 4));

            currentFreeTask = WebGLJump.LoadData(SaveKeys.CurrentFreeTask, "");

            int difference = 0;
            if (DataStorage.Money > 10000) {
                int targetAmount = (DataStorage.Money + 10000);
                int whole = (int)Mathf.Pow(10, Mathf.FloorToInt(Mathf.Log10(targetAmount)) - 2);
                int remainder = targetAmount % whole;
                difference = whole - remainder;
            }
            amountToHaveInBank = WebGLJump.LoadData(SaveKeys.AmountToHaveInBank, DataStorage.Money + 10000 + difference);

            ammoGunToSend = (Gun.Ammo)WebGLJump.LoadData(SaveKeys.AmmoGunToSend, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));

            ammoSpecialToSend = (Gun.Ammo)WebGLJump.LoadData(SaveKeys.AmmoSpecialToSend, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));

            amountOfElementsMatching = WebGLJump.LoadData(SaveKeys.AmountOfElementsMatching, Random.Range(2, 5));

            amountOfBossesToDefeat = WebGLJump.LoadData(SaveKeys.AmountOfBossesToDefeat, Random.Range(2, 5));
            currentBossesDefeated = WebGLJump.LoadData(SaveKeys.CurrentBossesDefeated, 0);

            int currentDay = (DataStorage.StartOnWave / 3 + 1);
            daysToReach = WebGLJump.LoadData(SaveKeys.DaysToReach, Mathf.Clamp(currentDay, currentDay + 1, currentDay + 4));
        }
    }
}
#endif