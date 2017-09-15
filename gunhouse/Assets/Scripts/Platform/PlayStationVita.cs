#if UNITY_PSP2
using UnityEngine;
using UnityEngine.PSVita;

namespace Gunhouse
{
    public class PlayStationVita : MonoBehaviour
    {
        void Start()
        {
            PSVitaVideoPlayer.TransferMemToMonoHeap();
        }
    }
}
#endif
