using UnityEngine;
using Sony.NP;
using Necrosoft;

namespace Gunhouse
{
    public partial class PlayStation4 : MonoBehaviour
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

            Core.EmptyResponse response = new Core.EmptyResponse();
            Trophies.RegisterTrophyPack(request, response);
        }

        public static void AwardTrophy(int trophyIndex)
        {
            if (!trophiesAvailable) {
                TrophiesRegister();     /* I don't think this is even possible right now */
                return;
            }

            Trophies.UnlockTrophyRequest request = new Trophies.UnlockTrophyRequest();
            request.TrophyId = trophyIndex;
            request.UserId = loggedInUser.userId;

            Core.EmptyResponse response = new Core.EmptyResponse();
            Trophies.UnlockTrophy(request, response);
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
            request.RetrieveTrophyPackSummaryIcon = true;
            request.UserId = loggedInUser.userId;
                
            Trophies.TrophyPackSummaryResponse summaryResponse = new Trophies.TrophyPackSummaryResponse();
            Trophies.GetTrophyPackSummary(request, summaryResponse);
        }

        void OutputGetTrophyPackSummary(Trophies.TrophyPackSummaryResponse response)
        {
            if (response == null) return;
            if (response.Locked) return;

            Console.Log("G: " + response.StaticConfiguration.NumGroups +
                        " T: " + response.StaticConfiguration.NumTrophies +
                        " P: " + response.StaticConfiguration.NumPlatinum +
                        " G: " + response.StaticConfiguration.NumGold +
                        " S: " + response.StaticConfiguration.NumSilver +
                        " B: " + response.StaticConfiguration.NumBronze +
                        "\n" + response.StaticConfiguration.Title +
                        " - " + response.StaticConfiguration.Description);

            Console.Log("User Progress: T: " + response.UserProgress.UnlockedTrophies +
                        " P: " + response.UserProgress.UnlockedPlatinum +
                        " G: " + response.UserProgress.UnlockedGold +
                        " S: " + response.UserProgress.UnlockedSilver +
                        " B: " + response.UserProgress.UnlockedBronze +
                        " %: " + response.UserProgress.ProgressPercentage);
        }
    }
}