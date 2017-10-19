#if UNITY_PSP2
using Sony.NP;
using UnityEngine;

namespace Gunhouse
{
    public partial class PlayStationVita : MonoBehaviour
    {
        static bool trophiesAvailable = Trophies.TrophiesAreAvailable;

        void StartTrophy()
        {
            #if UNITY_PS4
            // Note that you only need to do this on one platform, for example either call RegisterCommsID in the Vita app so that it can
            // share the PS4 service or call RegisterServiceLabel on the PS4 app so that it can share the PS Vita service. For this
            // example we will setup the service label if building on PS4 to share the service that was created for the PS Vita.

            // For PS4 we register the service label for the trophy service that matches
            // the np Comms ID of the PS Vita version of the application.
            ErrorHandler(Main.RegisterServiceLabel(NpServiceType.Trophy, 0));
            #endif

            Trophies.RegisterTrophyPack();
        }

        public static void AwardTrophy(int trophyIndex)
        {
            trophiesAvailable = Trophies.TrophiesAreAvailable;
            if (!trophiesAvailable) { return; }

            Trophies.AwardTrophy(trophyIndex);
        }
    }
}
#endif