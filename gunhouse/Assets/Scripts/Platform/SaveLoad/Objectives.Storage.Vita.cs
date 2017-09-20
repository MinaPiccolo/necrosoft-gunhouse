#if UNITY_PSP2
using UnityEngine;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        public static void ResetValues()
        {
            defeatWithGun = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN);
            previousDefeatWithGun = defeatWithGun;
            amountToDefeat = Random.Range(5, 10);
            ammoToUpgrade = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN);
            previousAmmoToUpgrade = ammoToUpgrade;
            leveToUpgrade =DataStorage.GunPower[(int)ammoToUpgrade] + 2;
            levelToHealing = DataStorage.Healing + 2;
            levelToHeart = Mathf.Clamp(DataStorage.Hearts + 1, 0, 6);
            isLevelingHeart = false;
            amountofBlocksToMake =Random.Range(3, 10);
            makeXSize = Random.Range(2, 4);
            makeYSize = Random.Range(2, 4);
            amountOfSizedBlocksToLoadToGun = Random.Range(3, 10);
            loadGunXSize = Random.Range(2, 4);
            loadGunYSize = Random.Range(2, 4);
            amountOfSizedBlocksToLoadToSpecial = Random.Range(3, 10);
            loadSpecialXSize = Random.Range(2, 4);
            loadSpecialYSize = Random.Range(2, 4);
            currentFreeTask = "";

            int difference = 0;
            if (DataStorage.Money > 10000) {
                int targetAmount = (DataStorage.Money + 10000);
                int whole = (int)Mathf.Pow(10, Mathf.FloorToInt(Mathf.Log10(targetAmount)) - 2);
                int remainder = targetAmount % whole;
                difference = whole - remainder;
            }

            amountToHaveInBank = DataStorage.Money + 10000 + difference;
            ammoGunToSend = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING);
            ammoSpecialToSend = (Gun.Ammo)Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING);
            amountOfElementsMatching = Random.Range(2, 5);
            amountOfBossesToDefeat = Random.Range(2, 5);
            currentBossesDefeated = 0;

            int currentDay = (DataStorage.StartOnWave / 3 + 1);
            daysToReach = Mathf.Clamp(currentDay, currentDay + 1, currentDay + 4);
        }

        public static void SaveFile(PlayStationVita.SaveData data)
        {
            data.defeatWithGun = (int)defeatWithGun;
            data.previousDefeatWithGun = (int)previousDefeatWithGun;
            data.amountToDefeat = amountToDefeat;
            data.ammoToUpgrade = (int)ammoToUpgrade;
            data.previousAmmoToUpgrade = (int)previousAmmoToUpgrade;
            data.leveToUpgrade = leveToUpgrade;
            data.levelToHealing = levelToHealing;
            data.levelToHeart = levelToHeart;
            data.isLevelingHeart = isLevelingHeart ? 1 : 0;
            data.amountofBlocksToMake = amountofBlocksToMake;
            data.makeXSize = makeXSize;
            data.makeYSize = makeYSize;
            data.amountOfSizedBlocksToLoadToGun = amountOfSizedBlocksToLoadToGun;
            data.loadGunXSize = loadGunXSize;
            data.loadGunYSize = loadGunYSize;
            data.amountOfSizedBlocksToLoadToSpecial = amountOfSizedBlocksToLoadToSpecial;
            data.loadSpecialXSize = loadSpecialXSize;
            data.loadSpecialYSize = loadSpecialYSize;
            data.amountToHaveInBank = amountToHaveInBank;
            data.ammoGunToSend = (int)ammoGunToSend;
            data.ammoSpecialToSend = (int)ammoSpecialToSend;
            data.amountOfElementsMatching = amountOfElementsMatching;
            data.amountOfBossesToDefeat = amountOfBossesToDefeat;
            data.currentBossesDefeated = currentBossesDefeated;
            data.daysToReach = daysToReach;
            data.currentFreeTask = currentFreeTask;
        }

        public static void LoadFile(PlayStationVita.SaveData data)
        {
            defeatWithGun = (Gun.Ammo)data.defeatWithGun;
            previousDefeatWithGun = (Gun.Ammo)data.previousDefeatWithGun;
            amountToDefeat = data.amountToDefeat;
            ammoToUpgrade = (Gun.Ammo)data.ammoToUpgrade;
            previousAmmoToUpgrade = (Gun.Ammo)data.previousAmmoToUpgrade;
            leveToUpgrade = data.leveToUpgrade;
            levelToHealing = data.levelToHealing;
            levelToHeart = data.levelToHeart;
            isLevelingHeart = data.isLevelingHeart == 1 ? true : false;
            amountofBlocksToMake = data.amountofBlocksToMake;
            makeXSize = data.makeXSize;
            makeYSize = data.makeYSize;
            amountOfSizedBlocksToLoadToGun = data.amountOfSizedBlocksToLoadToGun;
            loadGunXSize = data.loadGunXSize;
            loadGunYSize = data.loadGunYSize;
            amountOfSizedBlocksToLoadToSpecial = data.amountOfSizedBlocksToLoadToSpecial;
            loadSpecialXSize = data.loadSpecialXSize;
            loadSpecialYSize = data.loadSpecialYSize;
            currentFreeTask = data.currentFreeTask;
            amountToHaveInBank = data.amountToHaveInBank;
            ammoGunToSend = (Gun.Ammo)data.ammoGunToSend;
            ammoSpecialToSend = (Gun.Ammo)data.ammoSpecialToSend;
            amountOfElementsMatching = data.amountOfElementsMatching;
            amountOfBossesToDefeat = data.amountOfBossesToDefeat;
            currentBossesDefeated = data.currentBossesDefeated;
            daysToReach = data.daysToReach;
        }
    }
}
#endif