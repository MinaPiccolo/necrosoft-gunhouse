using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Necrosoft;

namespace Gunhouse.Menu
{
    public class MenuAudio : MenuPage
    {
        [SerializeField] GameObject lastSelected;
        [SerializeField] Color highlightColor;
        [SerializeField] TextMeshProUGUI[] texts;
        [SerializeField] Slider[] sliders;

        protected override void Initalise() { pageID = MenuState.Audio; transitionID = MenuState.Options; }

        protected override void IntroReady()
        {
            menu.SetActiveContextButtons(true, false);
            MainMenu.SetFocus(lastSelected);
        }

        void OnEnable()
        {
            sliders[0].value = Choom.MusicVolume;
            sliders[1].value = Choom.EffectVolume;
        }

        public void HighlightText(bool first)
        {
            texts[first ? 0 : 1].color = highlightColor;
            texts[first ? 1 : 0].color = Color.white;
        }

        public void OnMusicChanged(Slider slider)
        {
            Choom.MusicVolume = Mathf.Clamp(slider.value, 0, 1);
        }

        public void OnEffectsChanged(Slider slider)
        {
            Choom.EffectVolume = Mathf.Clamp(slider.value, 0, 1);
        }
    }
}