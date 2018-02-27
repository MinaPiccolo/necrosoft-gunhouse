using UnityEngine;
using UnityEditor;
using System;

public class PlayStation4Defines
{
    [MenuItem("Game/Publishing/PlayStation4/Set Publishing Settings [SCEA]")]
	static void SetOptions()
	{
        string path = "Assets/Editor/PlayStation4/";

        PlayerSettings.PS4.playerPrefsSupport = true;
        PlayerSettings.PS4.SaveDataImagePath = path + "save_image.png";

        PlayerSettings.PS4.category = PlayerSettings.PS4.PS4AppCategory.Application;
		PlayerSettings.PS4.appVersion = "01.00";
		PlayerSettings.PS4.masterVersion = "01.00";
		
        PlayerSettings.PS4.contentID = "UP4221-CUSA09627_00-GUNHOUSECOOLTOWN";
		PlayerSettings.productName = "Gunhouse";
		PlayerSettings.PS4.parentalLevel = 4;
		PlayerSettings.PS4.enterButtonAssignment = PlayerSettings.PS4.PS4EnterButtonAssignment.CrossButton;
		PlayerSettings.PS4.paramSfxPath = "";   // path + "param.sfx";

        PlayerSettings.PS4.passcode = "4PwEdtzQzXpa6x5sUOspb4CBTWMG3YuR";
        PlayerSettings.PS4.BackgroundImagePath = path + "background.png";
        PlayerSettings.PS4.StartupImagePath = path + "startup.png";
        PlayerSettings.PS4.ShareFilePath = path + "shareparam.json";
        PlayerSettings.PS4.PronunciationXMLPath = path + "Gunhouse.xml";
        PlayerSettings.PS4.PronunciationSIGPath = path + "Gunhouse.sig";

		PlayerSettings.PS4.NPtitleDatPath = path + "CUSA09627_00/nptitle.dat";
		PlayerSettings.PS4.npTrophyPackPath = path + "trophy.trp";
		PlayerSettings.PS4.npAgeRating = 10;
        PlayerSettings.PS4.npTitleSecret = "0x11,0x66,0x99,0x9f,0xed,0x8d,0xf6,0x95,\n" +
                                           "0x95,0x53,0x6e,0x7e,0x26,0xeb,0x0e,0xbe,\n" +
                                           "0xac,0x01,0x59,0x6d,0xf6,0xb9,0xf2,0xca,\n" +
                                           "0x41,0xed,0x46,0x2d,0x23,0x5c,0xe8,0xb0,\n" +
                                           "0xe2,0x15,0x3f,0x16,0x16,0x6e,0xc9,0xc4,\n" +
                                           "0xd5,0x3e,0xaa,0xad,0xa1,0xd3,0x12,0x9f,\n" +
                                           "0x8e,0x33,0x7d,0xa4,0x3f,0xc3,0xf3,0x10,\n" +
                                           "0x5a,0xce,0x93,0xa3,0x9e,0x3b,0xe6,0x85,\n" +
                                           "0x51,0x38,0x39,0x7a,0xee,0x98,0x3c,0xbb,\n" +
                                           "0xe5,0x1a,0xaa,0x50,0x40,0xb4,0x86,0x35,\n" +
                                           "0xfc,0x68,0x4c,0x93,0xbc,0x2b,0xa7,0x18,\n" +
                                           "0x1b,0xe6,0xfc,0x3a,0x1d,0x17,0x0f,0x5e,\n" +
                                           "0x91,0x96,0xde,0x94,0x4a,0xbc,0x8b,0x20,\n" +
                                           "0x61,0x20,0x70,0xdb,0xa7,0xc2,0xfc,0x0f,\n" +
                                           "0xd5,0xef,0x23,0x64,0x4c,0x8e,0x19,0xe3,\n" +
                                           "0x0b,0x2f,0x9a,0x17,0x32,0x7d,0xdf,0xd4";

        ReplaceNPToolkit();
    }

    static void ReplaceNPToolkit()
    {
        // Replace the old NpToolkit module with the NpToolkit2 version
        string[] modules = PlayerSettings.PS4.includedModules;

        if (modules.Length == 0) {
            Debug.Log("The player settings modules list is empty. Please open the player settings to initialise the list and try again.");
            return;
        }

        bool alreadySet = false;
        bool changed = false;

        for (int i = modules.Length - 1; i >= 0; i--) {
            if (modules[i].IndexOf("libSceNpToolkit.prx", StringComparison.OrdinalIgnoreCase) >= 0) {
                Debug.Log("Swapped module libSceNpToolkit.prx for libSceNpToolkit2.prx");
                modules[i] = "libSceNpToolkit2.prx";
                changed = true;
            }
            else if (modules[i].IndexOf("libSceNpToolkit2.prx", StringComparison.OrdinalIgnoreCase) >= 0) {
                alreadySet = true;
            }
        }
        PlayerSettings.PS4.includedModules = modules;

        if (alreadySet == false && changed == false) {
            Debug.LogError("Unable to find libSceNpToolkit.prx or libSceNpToolkit2.prx in modules list.");
        }
        
        AssetDatabase.Refresh();
    }
}
