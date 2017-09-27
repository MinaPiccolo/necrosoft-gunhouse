#if UNITY_SWITCH
using UnityEngine;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        public static string SaveFile()
        {
            SaveData data = new SaveData();

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

            string objectivesSaveFileSerialized = JsonUtility.ToJson(data);

            SaveDataHandler.Save(objectivesSaveFileSerialized, "GunhouseObjectivesSave");
            return objectivesSaveFileSerialized;
        }

        public static void LoadFile()
        {
            string serializedObjectivesSaveData = "";
            SaveData data;
            if (SaveDataHandler.Load(ref serializedObjectivesSaveData, "GunhouseObjectivesSave")) {
                data = JsonUtility.FromJson<SaveData>(serializedObjectivesSaveData);
            }
            else {
                data = JsonUtility.FromJson<SaveData>(Save());
            }

            defeatWithGun = (Gun.Ammo)(data.defeatWithGun != 0 ? data.defeatWithGun : Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousDefeatWithGun = (Gun.Ammo)(data.previousDefeatWithGun != 0 ? data.previousDefeatWithGun : (int)defeatWithGun);
            amountToDefeat = data.amountToDefeat != 0 ? data.amountToDefeat : Random.Range(5, 10);
            ammoToUpgrade = (Gun.Ammo)(data.ammoToUpgrade != 0 ? data.ammoToUpgrade : Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousAmmoToUpgrade = (Gun.Ammo)(data.previousAmmoToUpgrade != 0 ? data.previousAmmoToUpgrade : (int)ammoToUpgrade);
            leveToUpgrade = data.leveToUpgrade != 0 ? data.leveToUpgrade : (DataStorage.GunPower[(int)ammoToUpgrade] + 2);
            levelToHealing = data.levelToHealing != 0 ? data.levelToHealing : (DataStorage.Healing + 2);
            levelToHeart = data.levelToHeart != 0 ? data.levelToHeart : Mathf.Clamp(DataStorage.Hearts + 1, 0, 6);
            isLevelingHeart = (data.isLevelingHeart != 0 ? data.isLevelingHeart : 1) == 1 ? true : false;
            amountofBlocksToMake = data.amountofBlocksToMake != 0 ? data.amountofBlocksToMake : Random.Range(3, 10);
            makeXSize = data.makeXSize != 0 ? data.makeXSize : Random.Range(2, 4);
            makeYSize = data.makeYSize != 0 ? data.makeYSize : Random.Range(2, 4);
            amountOfSizedBlocksToLoadToGun = data.amountOfSizedBlocksToLoadToGun != 0 ? data.amountOfSizedBlocksToLoadToGun : Random.Range(3, 10);
            loadGunXSize = data.loadGunXSize != 0 ? data.loadGunXSize : Random.Range(2, 4);
            loadGunYSize = data.loadGunYSize != 0 ? data.loadGunYSize : Random.Range(2, 4);
            amountOfSizedBlocksToLoadToSpecial = data.amountOfSizedBlocksToLoadToSpecial != 0 ? data.amountOfSizedBlocksToLoadToSpecial : Random.Range(3, 10);
            loadSpecialXSize = data.loadSpecialXSize != 0 ? data.loadSpecialXSize : Random.Range(2, 4);
            loadSpecialYSize = data.loadSpecialYSize != 0 ? data.loadSpecialYSize : Random.Range(2, 4);
            currentFreeTask = data.currentFreeTask;

            int difference = 0;
            if (DataStorage.Money > 10000) {
                int targetAmount = (DataStorage.Money + 10000);
                int whole = (int)Mathf.Pow(10, Mathf.FloorToInt(Mathf.Log10(targetAmount)) - 2);
                int remainder = targetAmount % whole;
                difference = whole - remainder;
            }

            amountToHaveInBank = data.amountToHaveInBank != 0 ? data.amountToHaveInBank : (DataStorage.Money + 10000 + difference);
            ammoGunToSend = (Gun.Ammo)(data.ammoGunToSend != 0 ? data.ammoGunToSend : Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));
            ammoSpecialToSend = (Gun.Ammo)(data.ammoSpecialToSend != 0 ? data.ammoSpecialToSend : Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));
            amountOfElementsMatching = data.amountOfElementsMatching != 0 ? data.amountOfElementsMatching : Random.Range(2, 5);
            amountOfBossesToDefeat = data.amountOfBossesToDefeat != 0 ? data.amountOfBossesToDefeat : Random.Range(2, 5);
            currentBossesDefeated = data.currentBossesDefeated;

            int currentDay = (DataStorage.StartOnWave / 3 + 1);
            daysToReach = data.daysToReach != 0 ? data.daysToReach : Mathf.Clamp(currentDay, currentDay + 1, currentDay + 4);
        }

        [System.Serializable]
        public class SaveData
        {
            public int defeatWithGun;
            public int previousDefeatWithGun;
            public int amountToDefeat;
            public int ammoToUpgrade;
            public int previousAmmoToUpgrade;
            public int leveToUpgrade;
            public int levelToHealing;
            public int levelToHeart;
            public int isLevelingHeart;
            public int amountofBlocksToMake;
            public int makeXSize;
            public int makeYSize;
            public int amountOfSizedBlocksToLoadToGun;
            public int loadGunXSize;
            public int loadGunYSize;
            public int amountOfSizedBlocksToLoadToSpecial;
            public int loadSpecialXSize;
            public int loadSpecialYSize;
            public int amountToHaveInBank;
            public int ammoGunToSend;
            public int ammoSpecialToSend;
            public int amountOfElementsMatching;
            public int amountOfBossesToDefeat;
            public int currentBossesDefeated;
            public int daysToReach;
            public string currentFreeTask;
        }
    }
}
#endif