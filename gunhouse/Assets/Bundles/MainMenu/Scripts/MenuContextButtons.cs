using UnityEngine;
using UnityEngine.UI;

namespace Gunhouse.Menu
{
    public class MenuContextButtons : MonoBehaviour
    {
        [SerializeField] GameObject[] buttons;
        [SerializeField] Image[] images;

        [SerializeField] ControllerButtonImages[] controllerImages;
        Controllers controller;
        enum Controllers { PlayStation, Switch, Xbox };

        void Awake()
        {
            controller = Controllers.PlayStation;
            #if UNITY_SWITCH
            controller = Controllers.Switch;
            #endif

            images[0].sprite = controllerImages[(int)controller].Down;
            images[1].sprite = controllerImages[(int)controller].Right;
        }

        public void EnableButtons(bool selectEnabled)
        {
            buttons[0].SetActive(selectEnabled);
            buttons[1].SetActive(true);
        }

        [System.Serializable]
        public class ControllerButtonImages
        {
            public Sprite Down;
            public Sprite Right;
            public Sprite Up;
            public Sprite Left;
        }
    }
}