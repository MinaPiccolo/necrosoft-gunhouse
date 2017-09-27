using UnityEditor;
using UnityEngine;

public class SonyEditor : MonoBehaviour
{
    [MenuItem("Sony/Set Vita NP Pass+Sig")]
    static void SetOptions()
    {
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.PSP2, "");

        // Live area and software manual settings.
        //PlayerSettings.PSVita.liveAreaPath = "Assets/Editor/SonyNPVitaPublishData/livearea_full";
        //PlayerSettings.PSVita.liveAreaTrialPath = "Assets/Editor/SonyNPVitaPublishData/livearea_trial";
        //PlayerSettings.PSVita.manualPath = "Assets/Editor/SonyNPVitaPublishData/manual";

        // Param file settings.
        //PlayerSettings.PSVita.category = PlayerSettings.PSVita.PSVitaAppCategory.Application;
        //PlayerSettings.PSVita.masterVersion = "01.00";
        //PlayerSettings.PSVita.appVersion = "01.00";
        //PlayerSettings.PSVita.contentID = "ED1633-NPXB01864_00-UNITYNPTOOLKIT00";
        //PlayerSettings.PSVita.shortTitle = "UnityNpToolkit";
        //PlayerSettings.productName = "Unity Np Toolkit Example";
        //PlayerSettings.PSVita.saveDataQuota = 10240;
        //PlayerSettings.PSVita.parentalLevel = 1;
        //PlayerSettings.PSVita.upgradable = false;
        //PlayerSettings.PSVita.tvBootMode = PlayerSettings.PSVita.PSVitaTvBootMode.Default;
        //PlayerSettings.PSVita.tvDisableEmu = false;
        //PlayerSettings.PSVita.enterButtonAssignment = PlayerSettings.PSVita.PSVitaEnterButtonAssignment.CrossButton;
        //PlayerSettings.PSVita.paramSfxPath = "Assets/Editor/SonyNPVitaPublishData/param.sfx";

        // Package settings.
        //PlayerSettings.PSVita.packagePassword = "F69ASrgax3CF3EDN93LVqLBPh71Yexui";
        //PlayerSettings.PSVita.keystoneFile = "../../psn/releases/keystone";

        // PSN Settings.
        //PlayerSettings.PSVita.npSupportGBMorGJP = true;
        //PlayerSettings.PSVita.npTitleDatPath = "Assets/Editor/SonyNPVitaPublishData/NPXB01864_00/nptitle.dat";
        //PlayerSettings.PSVita.npTrophyPackPath = "Assets/Editor/SonyNPVitaPublishData/trophies/trophy.trp";
        //PlayerSettings.PSVita.npAgeRating = 12;
        //PlayerSettings.PSVita.npCommunicationsID = "NPWR14054_00";
        PlayerSettings.PSVita.npCommsPassphrase = "0x69,0xfd,0x44,0x6d,0xdf,0x20,0xd2,0x23,\n" +
                                                  "0x06,0x61,0xbf,0x49,0x04,0x74,0x95,0x3e,\n" +
                                                  "0x31,0x97,0x31,0x2f,0x43,0x1e,0xb1,0x91,\n" +
                                                  "0x56,0x49,0x41,0x56,0xb5,0x2f,0xf3,0x8f,\n" +
                                                  "0x52,0x0b,0xdc,0x41,0x88,0xd4,0x72,0x33,\n" +
                                                  "0x90,0xff,0xec,0x66,0x02,0x96,0x02,0x40,\n" +
                                                  "0x70,0xb0,0x88,0xc9,0x78,0xbb,0x3e,0x08,\n" +
                                                  "0x7d,0xb7,0xf1,0x5f,0x7d,0x34,0xdf,0x02,\n" +
                                                  "0x4a,0x23,0x13,0xe0,0xdd,0x3b,0x64,0x10,\n" +
                                                  "0xb5,0x8d,0xb1,0x55,0x7d,0xcf,0x82,0x46,\n" +
                                                  "0x66,0x75,0x87,0x15,0x6c,0x4e,0x28,0x90,\n" +
                                                  "0x80,0x23,0x4a,0x35,0xe7,0x50,0xd9,0xb2,\n" +
                                                  "0xe8,0x7e,0xca,0xeb,0x24,0xcc,0x63,0x02,\n" +
                                                  "0x6c,0xea,0x4c,0xd7,0x88,0x21,0x0b,0x03,\n" +
                                                  "0x43,0x49,0x2e,0x0b,0xd4,0x5a,0xca,0xc6,\n" +
                                                  "0x61,0xff,0xcf,0x5f,0x1e,0x49,0x58,0x01";

        PlayerSettings.PSVita.npCommsSig = "0xb9,0xdd,0xe1,0x3b,0x01,0x00,0x00,0x00,\n" +
                                           "0x00,0x00,0x00,0x00,0x71,0x61,0x71,0x75,\n" +
                                           "0x8e,0x36,0x07,0xe9,0x60,0x13,0x08,0x00,\n" +
                                           "0x78,0x3b,0xcd,0xc5,0xd5,0x91,0xd1,0xc5,\n" +
                                           "0x43,0x45,0x2c,0x5a,0xb0,0x53,0xd3,0xe5,\n" +
                                           "0x69,0xf0,0xa5,0x10,0x5f,0x21,0x0d,0xee,\n" +
                                           "0xe1,0x5b,0xbc,0xad,0x01,0xf1,0xde,0x7d,\n" +
                                           "0x90,0x5c,0xbb,0x28,0xb0,0x59,0xc7,0xb2,\n" +
                                           "0x48,0x6f,0x8f,0xdd,0x31,0x3b,0xc6,0x00,\n" +
                                           "0x98,0x2b,0x00,0x60,0x33,0x90,0xbd,0xa5,\n" +
                                           "0x17,0x9c,0x53,0x04,0x07,0x20,0x00,0xb4,\n" +
                                           "0x4d,0x24,0x42,0xff,0x5b,0x23,0x84,0x27,\n" +
                                           "0x48,0x08,0x58,0x17,0x7b,0x0e,0xf5,0xa7,\n" +
                                           "0x10,0xa4,0xa5,0x79,0xbc,0x6d,0xc4,0x4e,\n" +
                                           "0x45,0x00,0xac,0x87,0xeb,0xa4,0x98,0xc0,\n" +
                                           "0xbb,0x04,0x99,0x20,0x39,0x36,0x6c,0x9d,\n" +
                                           "0x87,0x20,0xc8,0x5e,0x48,0x31,0x50,0x15,\n" +
                                           "0x3c,0xea,0x18,0x0e,0xdf,0xe4,0xc6,0xad,\n" +
                                           "0x83,0x92,0x52,0xd4,0x71,0x5f,0xa5,0x77,\n" +
                                           "0x44,0x0c,0x5b,0xff,0xf4,0xd2,0x05,0xc2";
    }

    [MenuItem("Sony/Set Vita Cross-Platform Trophies")]
    static void SetOptionsCpTrophies()
    {
        //SetOptions();

        // For cross-platform trophies, use a trophy pack which has the PS4 np communications ID set.
        //PlayerSettings.PSVita.npTrophyPackPath = "Assets/Editor/SonyNPVitaPublishData/trophies/trophy_psvita_ps4.trp";

        // and enable the cross-platform code in the scripts.
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.PSP2, "NPTOOLKIT_CROSS_PLATFORM_TROPHIES");
    }

    [MenuItem("Sony/Set Vita Master (Signed Trophies)")]
    static void SetOptionsMaster()
    {
        //SetOptions();

        // For master builds that will be submitted we must have a signed trophy pack.
        // Trophy packs can be signed using the Server Management Tool on SCE dev-net.
        // Trophy packs must also be signed if enabling release check mode.
        //PlayerSettings.PSVita.npTrophyPackPath = "Assets/Editor/SonyNPVitaPublishData/trophies/trophy_signed.trp";
    }
}
