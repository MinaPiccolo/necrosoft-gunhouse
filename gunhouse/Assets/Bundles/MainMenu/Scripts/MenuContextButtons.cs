using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gunhouse.Menu
{
    public class MenuContextButtons : MonoBehaviour
    {
        [SerializeField] GameObject backButton;
        [SerializeField] TMP_FontAsset black;
        [SerializeField] TMP_FontAsset white;
        [SerializeField] GameObject[] buttons;
        [SerializeField] TextMeshProUGUI[] texts;
        [SerializeField] Image[] images;

        [SerializeField] ControllerButtonImages[] controllerImages;

        Controllers controller;
        enum Controllers { PlayStation = 0, Xbox, Touch, Switch, SwitchInverted };

        public void SetActiveBackButton(bool active) { backButton.SetActive(active); }

        void Awake()
        {
            controller = Controllers.PlayStation;
            images[0].sprite = controllerImages[(int)controller].Submit;
            images[1].sprite = controllerImages[(int)controller].Cancel;

            #if UNITY_SWITCH
            controller = Controllers.Switch;
            #endif
        }

        public void EnableButtons(bool selectEnabled, bool cancelEnabled)
        {
            buttons[0].SetActive(selectEnabled);
            buttons[1].SetActive(cancelEnabled);
            backButton.SetActive(!selectEnabled && cancelEnabled);
        }

        public void SetColor(bool isBlack)
        {
            if (isBlack) {
                for (int i = 0; i < texts.Length; ++i) {
                    texts[i].font = black;
                    texts[i].color = Color.black;
                }
            }
            else {
                for (int i = 0; i < texts.Length; ++i) {
                    texts[i].font = white;
                    texts[i].color = Color.white;
                }
            }

            #if UNITY_SWITCH
            images[0].sprite = controllerImages[isBlack ? (int)controller : (int)Controllers.SwitchInverted].Submit;
            images[1].sprite = controllerImages[isBlack ? (int)controller : (int)Controllers.SwitchInverted].Cancel;
            #endif
        }

        [System.Serializable]
        public class ControllerButtonImages
        {
            public Sprite Submit;
            public Sprite Cancel;
        }
    }
}