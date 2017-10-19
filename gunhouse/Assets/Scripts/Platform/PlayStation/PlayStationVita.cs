#if UNITY_PSP2
using Sony.NP;
using UnityEngine;
using UnityEngine.PSVita;
using UnityEngine.SceneManagement;

namespace Gunhouse
{
    public partial class PlayStationVita : MonoBehaviour
    {
        #pragma warning disable 0414
        static bool npReady = false;
        int npDelay;

        string npCommunicationID = "NPWR14054_00";
        byte[] npCommunicationPassphrase = { 0x69,0xfd,0x44,0x6d,0xdf,0x20,0xd2,0x23,
                                             0x06,0x61,0xbf,0x49,0x04,0x74,0x95,0x3e,
                                             0x31,0x97,0x31,0x2f,0x43,0x1e,0xb1,0x91,
                                             0x56,0x49,0x41,0x56,0xb5,0x2f,0xf3,0x8f,
                                             0x52,0x0b,0xdc,0x41,0x88,0xd4,0x72,0x33,
                                             0x90,0xff,0xec,0x66,0x02,0x96,0x02,0x40,
                                             0x70,0xb0,0x88,0xc9,0x78,0xbb,0x3e,0x08,
                                             0x7d,0xb7,0xf1,0x5f,0x7d,0x34,0xdf,0x02,
                                             0x4a,0x23,0x13,0xe0,0xdd,0x3b,0x64,0x10,
                                             0xb5,0x8d,0xb1,0x55,0x7d,0xcf,0x82,0x46,
                                             0x66,0x75,0x87,0x15,0x6c,0x4e,0x28,0x90,
                                             0x80,0x23,0x4a,0x35,0xe7,0x50,0xd9,0xb2,
                                             0xe8,0x7e,0xca,0xeb,0x24,0xcc,0x63,0x02,
                                             0x6c,0xea,0x4c,0xd7,0x88,0x21,0x0b,0x03,
                                             0x43,0x49,0x2e,0x0b,0xd4,0x5a,0xca,0xc6,
                                             0x61,0xff,0xcf,0x5f,0x1e,0x49,0x58,0x01 };
        byte[] npCommunicationSignature = { 0xb9,0xdd,0xe1,0x3b,0x01,0x00,0x00,0x00,
                                            0x00,0x00,0x00,0x00,0x71,0x61,0x71,0x75,
                                            0x8e,0x36,0x07,0xe9,0x60,0x13,0x08,0x00,
                                            0x78,0x3b,0xcd,0xc5,0xd5,0x91,0xd1,0xc5,
                                            0x43,0x45,0x2c,0x5a,0xb0,0x53,0xd3,0xe5,
                                            0x69,0xf0,0xa5,0x10,0x5f,0x21,0x0d,0xee,
                                            0xe1,0x5b,0xbc,0xad,0x01,0xf1,0xde,0x7d,
                                            0x90,0x5c,0xbb,0x28,0xb0,0x59,0xc7,0xb2,
                                            0x48,0x6f,0x8f,0xdd,0x31,0x3b,0xc6,0x00,
                                            0x98,0x2b,0x00,0x60,0x33,0x90,0xbd,0xa5,
                                            0x17,0x9c,0x53,0x04,0x07,0x20,0x00,0xb4,
                                            0x4d,0x24,0x42,0xff,0x5b,0x23,0x84,0x27,
                                            0x48,0x08,0x58,0x17,0x7b,0x0e,0xf5,0xa7,
                                            0x10,0xa4,0xa5,0x79,0xbc,0x6d,0xc4,0x4e,
                                            0x45,0x00,0xac,0x87,0xeb,0xa4,0x98,0xc0,
                                            0xbb,0x04,0x99,0x20,0x39,0x36,0x6c,0x9d,
                                            0x87,0x20,0xc8,0x5e,0x48,0x31,0x50,0x15,
                                            0x3c,0xea,0x18,0x0e,0xdf,0xe4,0xc6,0xad,
                                            0x83,0x92,0x52,0xd4,0x71,0x5f,0xa5,0x77,
                                            0x44,0x0c,0x5b,0xff,0xf4,0xd2,0x05,0xc2 };

        #pragma warning restore 0414

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            PSVitaVideoPlayer.TransferMemToMonoHeap();

            #if !UNITY_EDITOR

            Main.OnNPInitialized += OnInitializedNP;
            Main.Initialize(Main.kNpToolkitCreate_DoNotInitializeTrophies | Main.kNpToolkitCreate_NoRanking);
            Main.RegisterCommsID(NpServiceType.Trophy, npCommunicationID, npCommunicationPassphrase, npCommunicationSignature);

            ////string commID = BuildWindow.trophyCommId;
            //string region = GetRegion();
            //ErrorHandler(Main.RegisterCommsID(NpServiceType.Trophy, GetCommID() + "\0", GetTrophyCommPass(region), GetTrophyCommSig(region)));

            StartSaveLoad();
            StartTrophy();
            StartDialog();
            #else
            SceneManager.LoadScene((int)SceneIndex.Main);
            #endif
        }

        void Update()
        {
            #if !UNITY_EDITOR
            Sony.NP.Main.Update();
            UpdateSaveLoad();
            UpdateDialog();
            #else
            #endif
        }

        void OnInitializedNP(Messages.PluginMessage msg)
        {
            npReady = true;

            /* For some reason np ready is called twice. I can't find a reason why
                in the documentation. If this function is called the first time it
                causes an error and is ignored. */
            npDelay++;
            if (npDelay < 2) return; 
            LoadFile();
        }
    }
}
#endif