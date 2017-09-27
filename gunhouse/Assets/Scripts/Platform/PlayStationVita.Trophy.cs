#if UNITY_PSP2
using Sony.NP;
using UnityEngine;

namespace Gunhouse
{
    public partial class PlayStationVita : MonoBehaviour
    {
        static Trophies.GameInfo gameInfo;
        static Texture2D trophyIcon = null;
        static Texture2D trophyGroupIcon = null;
        static bool trophiesAvailable = Trophies.TrophiesAreAvailable;

        /* trophy 0 is the platinum trophy which we can't award, 
            it gets awarded automatically when all other trophies
            have been awarded. */
        int nextTrophyIndex = 1;
        bool[] trophiesAwarded = new bool[9];

        void StartTrophy()
        {
            #if UNITY_PS4
            // Note that you only need to do this on one platform, for example either call RegisterCommsID in the Vita app so that it can
            // share the PS4 service or call RegisterServiceLabel on the PS4 app so that it can share the PS Vita service. For this
            // example we will setup the service label if building on PS4 to share the service that was created for the PS Vita.

            // For PS4 we register the service label for the trophy service that matches the np Comms ID of the PS Vita version of the application.
            ErrorHandler(Main.RegisterServiceLabel(NpServiceType.Trophy, 0));
            #endif
            Trophies.RegisterTrophyPack();

            gameInfo = Trophies.GetCachedGameInfo();

            Trophies.OnGotGameInfo += OnTrophyGotGameInfo;
            Trophies.OnGotGroupInfo += OnTrophyGotGroupInfo;
            Trophies.OnGotTrophyInfo += OnTrophyGotTrophyInfo;
            Trophies.OnGotProgress += OnTrophyGotProgress;
            Trophies.OnAwardedTrophy += OnSomeEvent;
            Trophies.OnAwardTrophyFailed += OnSomeEvent;
            Trophies.OnAlreadyAwardedTrophy += OnSomeEvent;
            Trophies.OnUnlockedPlatinum += OnSomeEvent;
        }

        public void AwardTrophy(int trophyIndex)
        {
            trophiesAvailable = Trophies.TrophiesAreAvailable;
            if (!trophiesAvailable) { return; }

            ErrorCode error = Trophies.AwardTrophy(trophyIndex);
            OnScreenLog.Add("Award Trophy result: " + error.ToString());
            ResultCode code; Trophies.GetLastError(out code);
            OnScreenLog.Add("AwardTrophy error: " + code.lastError.ToString());

            //trophiesAwarded[trophyIndex] = true;
        }

        #region EventHandlers

        void OnTrophyGotGameInfo(Messages.PluginMessage msg)
        {
            gameInfo = Trophies.GetCachedGameInfo();
            DumpGameInfo();
        }

        void OnTrophyGotGroupInfo(Messages.PluginMessage msg)
        {
            OnScreenLog.Add("Got Group List!");

            Trophies.GroupDetails[] details = Trophies.GetCachedGroupDetails();
            Trophies.GroupData[] data = Trophies.GetCachedGroupData();

            OnScreenLog.Add("Groups: " + details.Length);

            for (int i = 0; i < details.Length; i++) {
                if (details[i].hasIcon && trophyGroupIcon == null) {
                    // As a test, create a texture from the first trophy group icon that we find.
                    trophyGroupIcon = new Texture2D(details[i].iconWidth, details[i].iconHeight);
                    trophyGroupIcon.LoadImage(details[i].icon);
                    OnScreenLog.Add("Found icon: " + trophyGroupIcon.width + ", " + trophyGroupIcon.height);
                }

                OnScreenLog.Add(" " + i + ": " +
                                details[i].groupId + ", " + details[i].title + ", " +
                                details[i].description + ", " + details[i].numTrophies + ", " +
                                details[i].numPlatinum + ", " + details[i].numGold + ", " +
                                details[i].numSilver + ", " + details[i].numBronze);

                OnScreenLog.Add(" " + i + ": " +
                                data[i].groupId + ", " + data[i].unlockedTrophies + ", " +
                                data[i].unlockedPlatinum + ", " + data[i].unlockedGold + ", " +
                                data[i].unlockedSilver + ", " + data[i].unlockedBronze + ", " +
                                data[i].progressPercentage + data[i].userId.ToString("X"));
            }
        }

        void OnTrophyGotTrophyInfo(Messages.PluginMessage msg)
        {
            OnScreenLog.Add("Got Trophy List!");

            Trophies.TrophyDetails[] details = Trophies.GetCachedTrophyDetails();
            Trophies.TrophyData[] data = Trophies.GetCachedTrophyData();

            OnScreenLog.Add("Trophies: " + details.Length);
            for (int i = 0; i < details.Length; i++) {
                if (data[i].hasIcon && trophyIcon == null) {
                    // As a test, create a texture from the first trophy icon that we find.
                    trophyIcon = new Texture2D(data[i].iconWidth, data[i].iconHeight);
                    trophyIcon.LoadImage(data[i].icon);
                    OnScreenLog.Add("Found icon: " + trophyIcon.width + ", " + trophyIcon.height);
                }

                OnScreenLog.Add(" " + i + ": " +
                                details[i].name + ", " + //details[i].description + ", " + 
                                details[i].trophyId + ", " + details[i].trophyGrade + ", " +
                                details[i].groupId + ", " + details[i].hidden + ", " +
                                data[i].unlocked + ", " + data[i].timestamp + ", " +
                                data[i].userId.ToString("X"));
            }
        }

        void OnTrophyGotProgress(Messages.PluginMessage msg)
        {
            Trophies.TrophyProgress progress = Trophies.GetCachedTrophyProgress();

            OnScreenLog.Add("Progress for userId: 0x" + progress.userId.ToString("X"));
            OnScreenLog.Add("progressPercentage: " + progress.progressPercentage);
            OnScreenLog.Add("unlockedTrophies: " + progress.unlockedTrophies);
            OnScreenLog.Add("unlockedPlatinum: " + progress.unlockedPlatinum);
            OnScreenLog.Add("unlockedGold: " + progress.unlockedGold);
            OnScreenLog.Add("unlockedSilver: " + progress.unlockedSilver);
            OnScreenLog.Add("unlockedBronze: " + progress.unlockedBronze);
        }

        #endregion
        
        void DumpGameInfo()
        {
            OnScreenLog.Add("title: " + gameInfo.title);
            OnScreenLog.Add("desc: " + gameInfo.description);
            OnScreenLog.Add("numTrophies: " + gameInfo.numTrophies);
            OnScreenLog.Add("numGroups: " + gameInfo.numGroups);
            OnScreenLog.Add("numBronze: " + gameInfo.numBronze);
            OnScreenLog.Add("numSilver: " + gameInfo.numSilver);
            OnScreenLog.Add("numGold: " + gameInfo.numGold);
            OnScreenLog.Add("numPlatinum: " + gameInfo.numPlatinum);
        }
    }
}
#endif