#if UNITY_SWITCH
using UnityEngine;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        public static string SaveFile()
        {
            ObjectivesSaveFile objectivesSaveFile = new ObjectivesSaveFile();

            objectivesSaveFile.defeatWithGun = (int)defeatWithGun;
            objectivesSaveFile.previousDefeatWithGun = (int)previousDefeatWithGun;
            objectivesSaveFile.amountToDefeat = amountToDefeat;
            objectivesSaveFile.ammoToUpgrade = (int)ammoToUpgrade;
            objectivesSaveFile.previousAmmoToUpgrade = (int)previousAmmoToUpgrade;
            objectivesSaveFile.leveToUpgrade = leveToUpgrade;
            objectivesSaveFile.levelToHealing = levelToHealing;
            objectivesSaveFile.levelToHeart = levelToHeart;
            objectivesSaveFile.isLevelingHeart = isLevelingHeart ? 1 : 0;
            objectivesSaveFile.amountofBlocksToMake = amountofBlocksToMake;
            objectivesSaveFile.makeXSize = makeXSize;
            objectivesSaveFile.makeYSize = makeYSize;
            objectivesSaveFile.amountOfSizedBlocksToLoadToGun = amountOfSizedBlocksToLoadToGun;
            objectivesSaveFile.loadGunXSize = loadGunXSize;
            objectivesSaveFile.loadGunYSize = loadGunYSize;
            objectivesSaveFile.amountOfSizedBlocksToLoadToSpecial = amountOfSizedBlocksToLoadToSpecial;
            objectivesSaveFile.loadSpecialXSize = loadSpecialXSize;
            objectivesSaveFile.loadSpecialYSize = loadSpecialYSize;
            objectivesSaveFile.amountToHaveInBank = amountToHaveInBank;
            objectivesSaveFile.ammoGunToSend = (int)ammoGunToSend;
            objectivesSaveFile.ammoSpecialToSend = (int)ammoSpecialToSend;
            objectivesSaveFile.amountOfElementsMatching = amountOfElementsMatching;
            objectivesSaveFile.amountOfBossesToDefeat = amountOfBossesToDefeat;
            objectivesSaveFile.currentBossesDefeated = currentBossesDefeated;
            objectivesSaveFile.daysToReach = daysToReach;
            objectivesSaveFile.currentFreeTask = currentFreeTask;

            string objectivesSaveFileSerialized = JsonUtility.ToJson(objectivesSaveFile);

            SaveDataHandler.Save(objectivesSaveFileSerialized, "GunhouseObjectivesSave");
            return objectivesSaveFileSerialized;
        }

        public static void LoadFile()
        {
            string serializedObjectivesSaveData = "";
            ObjectivesSaveFile savedObjectivesData;
            if (SaveDataHandler.Load(ref serializedObjectivesSaveData, "GunhouseObjectivesSave")) {
                savedObjectivesData = JsonUtility.FromJson<ObjectivesSaveFile>(serializedObjectivesSaveData);
            }
            else {
                savedObjectivesData = JsonUtility.FromJson<ObjectivesSaveFile>(Save());
            }

            defeatWithGun = (Gun.Ammo)(savedObjectivesData.defeatWithGun != 0 ? savedObjectivesData.defeatWithGun : Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousDefeatWithGun = (Gun.Ammo)(savedObjectivesData.previousDefeatWithGun != 0 ? savedObjectivesData.previousDefeatWithGun : (int)defeatWithGun);
            amountToDefeat = savedObjectivesData.amountToDefeat != 0 ? savedObjectivesData.amountToDefeat : Random.Range(5, 10);
            ammoToUpgrade = (Gun.Ammo)(savedObjectivesData.ammoToUpgrade != 0 ? savedObjectivesData.ammoToUpgrade : Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.SIN));
            previousAmmoToUpgrade = (Gun.Ammo)(savedObjectivesData.previousAmmoToUpgrade != 0 ? savedObjectivesData.previousAmmoToUpgrade : (int)ammoToUpgrade);
            leveToUpgrade = savedObjectivesData.leveToUpgrade != 0 ? savedObjectivesData.leveToUpgrade : (DataStorage.GunPower[(int)ammoToUpgrade] + 2);
            levelToHealing = savedObjectivesData.levelToHealing != 0 ? savedObjectivesData.levelToHealing : (DataStorage.Healing + 2);
            levelToHeart = savedObjectivesData.levelToHeart != 0 ? savedObjectivesData.levelToHeart : Mathf.Clamp(DataStorage.Hearts + 1, 0, 6);
            isLevelingHeart = (savedObjectivesData.isLevelingHeart != 0 ? savedObjectivesData.isLevelingHeart : 1) == 1 ? true : false;
            amountofBlocksToMake = savedObjectivesData.amountofBlocksToMake != 0 ? savedObjectivesData.amountofBlocksToMake : Random.Range(3, 10);
            makeXSize = savedObjectivesData.makeXSize != 0 ? savedObjectivesData.makeXSize : Random.Range(2, 4);
            makeYSize = savedObjectivesData.makeYSize != 0 ? savedObjectivesData.makeYSize : Random.Range(2, 4);
            amountOfSizedBlocksToLoadToGun = savedObjectivesData.amountOfSizedBlocksToLoadToGun != 0 ? savedObjectivesData.amountOfSizedBlocksToLoadToGun : Random.Range(3, 10);
            loadGunXSize = savedObjectivesData.loadGunXSize != 0 ? savedObjectivesData.loadGunXSize : Random.Range(2, 4);
            loadGunYSize = savedObjectivesData.loadGunYSize != 0 ? savedObjectivesData.loadGunYSize : Random.Range(2, 4);
            amountOfSizedBlocksToLoadToSpecial = savedObjectivesData.amountOfSizedBlocksToLoadToSpecial != 0 ? savedObjectivesData.amountOfSizedBlocksToLoadToSpecial : Random.Range(3, 10);
            loadSpecialXSize = savedObjectivesData.loadSpecialXSize != 0 ? savedObjectivesData.loadSpecialXSize : Random.Range(2, 4);
            loadSpecialYSize = savedObjectivesData.loadSpecialYSize != 0 ? savedObjectivesData.loadSpecialYSize : Random.Range(2, 4);
            currentFreeTask = savedObjectivesData.currentFreeTask;

            int difference = 0;
            if (DataStorage.Money > 10000) {
                int targetAmount = (DataStorage.Money + 10000);
                int whole = (int)Mathf.Pow(10, Mathf.FloorToInt(Mathf.Log10(targetAmount)) - 2);
                int remainder = targetAmount % whole;
                difference = whole - remainder;
            }

            amountToHaveInBank = savedObjectivesData.amountToHaveInBank != 0 ? savedObjectivesData.amountToHaveInBank : (DataStorage.Money + 10000 + difference);
            ammoGunToSend = (Gun.Ammo)(savedObjectivesData.ammoGunToSend != 0 ? savedObjectivesData.ammoGunToSend : Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));
            ammoSpecialToSend = (Gun.Ammo)(savedObjectivesData.ammoSpecialToSend != 0 ? savedObjectivesData.ammoSpecialToSend : Random.Range((int)Gun.Ammo.DRAGON, (int)Gun.Ammo.GATLING));
            amountOfElementsMatching = savedObjectivesData.amountOfElementsMatching != 0 ? savedObjectivesData.amountOfElementsMatching : Random.Range(2, 5);
            amountOfBossesToDefeat = savedObjectivesData.amountOfBossesToDefeat != 0 ? savedObjectivesData.amountOfBossesToDefeat : Random.Range(2, 5);
            currentBossesDefeated = savedObjectivesData.currentBossesDefeated;

            int currentDay = (DataStorage.StartOnWave / 3 + 1);
            daysToReach = savedObjectivesData.daysToReach != 0 ? savedObjectivesData.daysToReach : Mathf.Clamp(currentDay, currentDay + 1, currentDay + 4);
        }
        
        [System.Serializable]
        public class ObjectivesSaveFile
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