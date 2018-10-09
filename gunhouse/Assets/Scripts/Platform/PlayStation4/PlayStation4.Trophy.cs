#if UNITY_PS4 || (UNITY_PSP2 && UNITY_EDITOR)
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
            }
        }

        void OutputRegisterTrophyPack(Core.EmptyResponse response)
        {
            if (response == null) return;
            if (response.Locked) return;

            trophiesAvailable = true;
        }
    }
}
#endif