#if UNITY_PS4
using Sony.NP;
using Necrosoft;

namespace Gunhouse
{
    public partial class PlayStation4
    {
        static bool trophiesAvailable = false;

        void StartTrophy()
        {
            TrophiesRegister();
        }

        public static void TrophiesRegister()
        {
            Trophies.RegisterTrophyPackRequest request = new Trophies.RegisterTrophyPackRequest();
            request.UserId = loggedInUser.userId;
            Trophies.RegisterTrophyPack(request, new Core.EmptyResponse());
        }

        public static void AwardTrophy(int trophyIndex)
        {
            if (!trophiesAvailable) return;

            Trophies.UnlockTrophyRequest request = new Trophies.UnlockTrophyRequest();
            request.TrophyId = trophyIndex;
            request.UserId = loggedInUser.userId;
            Trophies.UnlockTrophy(request, new Core.EmptyResponse());
        }

        public void OnAsyncEventTrophy(NpCallbackEvent callbackEvent)
        {
            switch (callbackEvent.ApiCalled)
            {
            case FunctionTypes.TrophyRegisterTrophyPack: OutputRegisterTrophyPack(callbackEvent.Response as Core.EmptyResponse); break;
            case FunctionTypes.TrophyGetTrophyPackSummary: OutputGetTrophyPackSummary(callbackEvent.Response as Trophies.TrophyPackSummaryResponse); break;
            }
        }

        void OutputRegisterTrophyPack(Core.EmptyResponse response)
        {
            if (response == null) return;
            if (response.Locked) return;

            trophiesAvailable = true;

            Trophies.GetTrophyPackSummaryRequest request = new Trophies.GetTrophyPackSummaryRequest();
            request.UserId = loggedInUser.userId;
            Trophies.GetTrophyPackSummary(request, new Trophies.TrophyPackSummaryResponse());
        }

        void OutputGetTrophyPackSummary(Trophies.TrophyPackSummaryResponse response)
        {
            if (response == null) return;
            if (response.Locked) return;

            Console.Log("Available - G: " + response.StaticConfiguration.NumGroups +
                        " T: " + response.StaticConfiguration.NumTrophies +
                        " P: " + response.StaticConfiguration.NumPlatinum +
                        " G: " + response.StaticConfiguration.NumGold +
                        " S: " + response.StaticConfiguration.NumSilver +
                        " B: " + response.StaticConfiguration.NumBronze + 

                        "\nProgress  - T: " + response.UserProgress.UnlockedTrophies +
                        " P: " + response.UserProgress.UnlockedPlatinum +
                        "  G: " + response.UserProgress.UnlockedGold +
                        " S: " + response.UserProgress.UnlockedSilver +
                        " B: " + response.UserProgress.UnlockedBronze +
                         " " + response.UserProgress.ProgressPercentage + "%");
        }
    }
}
#endif