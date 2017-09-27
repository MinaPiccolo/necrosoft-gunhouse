using UnityEngine;
using UnityEngine.UI;

namespace Gunhouse
{
    public class EndWaveCanvas : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Text noteText;
        [Space(10)] [SerializeField] Image endWaveMessage;
        [SerializeField] Sprite successMessage;
        [SerializeField] Sprite gameOverMessage;
        [Space(10)] [SerializeField] Button retryWaveButton;
        [SerializeField] Button nexWaveButton;
        [SerializeField] Button storeButton;
        [Space(10)] [SerializeField] RollingCanvas rollingCanvas;

        public void Display(bool won)
        {
            Tracker.ScreenVisit(won ? SCREEN_NAME.WAVE_COMPLETE : SCREEN_NAME.WAVE_FAILED);
            endWaveMessage.sprite = won ? successMessage : gameOverMessage;
            retryWaveButton.gameObject.SetActive(!won);
            nexWaveButton.gameObject.SetActive(won);
            storeButton.gameObject.SetActive(true);
        }
    }
}