using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Necrosoft;

namespace Gunhouse.Menu
{
    public class MenuAudio : MenuPage
    {
        [SerializeField] Color highlightColor;
        [SerializeField] TextMeshProUGUI[] texts;
        [SerializeField] Slider[] sliders;
        bool ignoreEffect;

        protected override void Initalise() { pageID = MenuState.Audio; transitionID = MenuState.Options; }

        protected override void IntroReady()
        {
            menu.SetActiveContextButtons(false, true);
            menu.SetFocus(refocusSelected);
        }

        public override void CancelPressed()
        {
            Platform.SaveOptions();
            
            for (int i = 0; i < texts.Length; ++i) { texts[i].color = Color.white; }
            transitionID = AppMain.IsPaused ? MenuState.Pause : MenuState.Options;
            base.CancelPressed();
        }

        void OnEnable()
        {
            ignoreEffect = true;
            sliders[0].value = Choom.MusicVolume;
            sliders[1].value = Choom.EffectVolume;
        }

        public void HighlightText(bool first)
        {
            if (MainMenu.ignoreFocus) { return; }

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

            if (ignoreEffect) { ignoreEffect = false; return; }
            if (MainMenu.ignoreFocus) { return; }
            Choom.PlayEffect(SoundAssets.UISelect);
        }

        public void OnEffectPointerUp() {  Choom.PlayEffect(SoundAssets.UISelect); }
    }
}