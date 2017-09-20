using UnityEngine;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        #if UNITY_WEBGL
        public static void SaveRemote()
        {
            Platform.SaveData(SaveKeys.DefeatWithGun, ((int)defeatWithGun).ToString());
            Platform.SaveData(SaveKeys.PreviousDefeatWithGun, ((int)previousDefeatWithGun).ToString());
            Platform.SaveData(SaveKeys.AmountToDefeat, amountToDefeat.ToString());

            Platform.SaveData(SaveKeys.AmmoToUpgrade, ((int)ammoToUpgrade).ToString());
            Platform.SaveData(SaveKeys.PreviousAmmoToUpgrade, ((int)previousAmmoToUpgrade).ToString());
            Platform.SaveData(SaveKeys.LeveToUpgrade, leveToUpgrade.ToString());

            Platform.SaveData(SaveKeys.LevelToHealing, levelToHealing.ToString());

            Platform.SaveData(SaveKeys.LevelToHeart, levelToHeart.ToString());
            Platform.SaveData(SaveKeys.IsLevelingHeart, (isLevelingHeart ? 1 : 0).ToString());

            Platform.SaveData(SaveKeys.AmountofBlocksToMake, amountofBlocksToMake.ToString());
            Platform.SaveData(SaveKeys.MakeXSize, makeXSize.ToString());
            Platform.SaveData(SaveKeys.MakeYSize, makeYSize.ToString());

            Platform.SaveData(SaveKeys.AmountOfSizedBlocksToLoadToGun, amountOfSizedBlocksToLoadToGun.ToString());
            Platform.SaveData(SaveKeys.LoadGunXSize, loadGunXSize.ToString());
            Platform.SaveData(SaveKeys.LoadGunYSize, loadGunYSize.ToString());

            Platform.SaveData(SaveKeys.AmountOfSizedBlocksToLoadToSpecial, amountOfSizedBlocksToLoadToSpecial.ToString());
            Platform.SaveData(SaveKeys.LoadSpecialXSize, loadSpecialXSize.ToString());
            Platform.SaveData(SaveKeys.LoadSpecialYSize, loadSpecialYSize.ToString());

            Platform.SaveData(SaveKeys.AmountToHaveInBank, amountToHaveInBank.ToString());

            Platform.SaveData(SaveKeys.AmmoGunToSend, ((int)ammoGunToSend).ToString());

            Platform.SaveData(SaveKeys.AmmoSpecialToSend, ((int)ammoSpecialToSend).ToString());

            Platform.SaveData(SaveKeys.AmountOfElementsMatching, amountOfElementsMatching.ToString());

            Platform.SaveData(SaveKeys.AmountOfBossesToDefeat, amountOfBossesToDefeat.ToString());
            Platform.SaveData(SaveKeys.CurrentBossesDefeated, currentBossesDefeated.ToString());

            Platform.SaveData(SaveKeys.DaysToReach, daysToReach.ToString());

            Platform.SaveData(SaveKeys.CurrentFreeTask, currentFreeTask);
        }

        public static void LoadRemote()
        {
            defeatWithGun = (Gun.Ammo)Platform.LoadData(SaveKeys.DefeatWithGun, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousDefeatWithGun = (Gun.Ammo)Platform.LoadData(SaveKeys.PreviousDefeatWithGun, (int)defeatWithGun);
            amountToDefeat = Platform.LoadData(SaveKeys.AmountToDefeat, Random.Range(5, 10));

            ammoToUpgrade = (Gun.Ammo)Platform.LoadData(SaveKeys.AmmoToUpgrade, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousAmmoToUpgrade = (Gun.Ammo)Platform.LoadData(SaveKeys.PreviousAmmoToUpgrade, (int)ammoToUpgrade);
            leveToUpgrade = Platform.LoadData(SaveKeys.LeveToUpgrade, DataStorage.GunPower[(int)ammoToUpgrade] + 2);

            levelToHealing = Platform.LoadData(SaveKeys.LevelToHealing, DataStorage.Healing + 2);

            levelToHeart = Platform.LoadData(SaveKeys.LevelToHeart, Mathf.Clamp(DataStorage.Hearts + 1, 0, 6));
            isLevelingHeart = Platform.LoadData(SaveKeys.IsLevelingHeart, 1) == 1 ? true : false;

            amountofBlocksToMake = Platform.LoadData(SaveKeys.AmountofBlocksToMake, Random.Range(3, 10));
            makeXSize = Platform.LoadData(SaveKeys.MakeXSize, Random.Range(2, 4));
            makeYSize = Platform.LoadData(SaveKeys.MakeYSize, Random.Range(2, 4));

            amountOfSizedBlocksToLoadToGun = Platform.LoadData(SaveKeys.AmountOfSizedBlocksToLoadToGun, Random.Range(3, 10));
            loadGunXSize = Platform.LoadData(SaveKeys.LoadGunXSize, Random.Range(2, 4));
            loadGunYSize = Platform.LoadData(SaveKeys.LoadGunYSize, Random.Range(2, 4));

            amountOfSizedBlocksToLoadToSpecial = Platform.LoadData(SaveKeys.AmountOfSizedBlocksToLoadToSpecial, Random.Range(3, 10));
            loadSpecialXSize = Platform.LoadData(SaveKeys.LoadSpecialXSize, Random.Range(2, 4));
            loadSpecialYSize = Platform.LoadData(SaveKeys.LoadSpecialYSize, Random.Range(2, 4));

            currentFreeTask = Platform.LoadData(SaveKeys.CurrentFreeTask, "");

            int difference = 0;
            if (DataStorage.Money > 10000) {
                int targetAmount = (DataStorage.Money + 10000);
                int whole = (int)Mathf.Pow(10, Mathf.FloorToInt(Mathf.Log10(targetAmount)) - 2);
                int remainder = targetAmount % whole;
                difference = whole - remainder;
            }
            amountToHaveInBank = Platform.LoadData(SaveKeys.AmountToHaveInBank, DataStorage.Money + 10000 + difference);

            ammoGunToSend = (Gun.Ammo)Platform.LoadData(SaveKeys.AmmoGunToSend, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));

            ammoSpecialToSend = (Gun.Ammo)Platform.LoadData(SaveKeys.AmmoSpecialToSend, Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));

            amountOfElementsMatching = Platform.LoadData(SaveKeys.AmountOfElementsMatching, Random.Range(2, 5));

            amountOfBossesToDefeat = Platform.LoadData(SaveKeys.AmountOfBossesToDefeat, Random.Range(2, 5));
            currentBossesDefeated = Platform.LoadData(SaveKeys.CurrentBossesDefeated, 0);

            int currentDay = (DataStorage.StartOnWave / 3 + 1);
            daysToReach = Platform.LoadData(SaveKeys.DaysToReach, Mathf.Clamp(currentDay, currentDay + 1, currentDay + 4));
        }
        #endif
    }
}
