using UnityEngine;
using UnityEngine.UI;
using Necrosoft;
using Necrosoft.ThirdParty;
using System.Collections.Generic;
using TMPro;

namespace Gunhouse
{
    public class MenuOverlay : MonoBehaviour
    {
        [SerializeField] CanvasGroup[] root;
        [SerializeField] RectTransform fade;
        [SerializeField] TextMeshProUGUI storyText;
        [SerializeField] Objectives objectives;

        [Space(10)] [SerializeField] RectTransform pauseRoot;
        [SerializeField] Button[] pauseButtons;

        [Space(10)] [SerializeField] RectTransform endWaveRoot;
        [SerializeField] Image endWaveBanner;
        [SerializeField] Sprite[] endWaveBanners;
        [SerializeField] Button[] endWaveButtons;

        [Space(10)] [SerializeField] Image leftButton;
        [SerializeField] Sprite[] leftButtons;

        bool isPaused;
        State childState;

        void Awake()
        {
            fade.gameObject.SetActive(false);
            pauseRoot.gameObject.SetActive(false);
            endWaveRoot.gameObject.SetActive(false);

            storyText.richText = true;
            storyText.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            pauseButtons[0].onClick.AddListener(() => Resume());
            pauseButtons[1].onClick.AddListener(() => MainMenu());
        }

        void OnDisable()
        {
            for (int i = 0; i < pauseButtons.Length; ++i) { pauseButtons[i].onClick.RemoveAllListeners(); }
            for (int i = 0; i < endWaveButtons.Length; ++i) { endWaveButtons[i].onClick.RemoveAllListeners(); }
        }

        public void Show(bool won)
        {
            if (won) { Tracker.LevelComplete(MetaState.wave_number); }
            else { Tracker.LevelFailed(MetaState.wave_number); }

            for (int i = 0; i < root.Length; ++i) root[i].alpha = 0;

            fade.gameObject.SetActive(true);
            endWaveRoot.gameObject.SetActive(true);

            objectives.UpdateText();

            endWaveBanner.sprite = endWaveBanners[won ? 1 : 0];

            if (MetaState.hardcore_mode) {
                leftButton.sprite = leftButtons[1];

                // no retry if lost. no shop menu button. main menu button.
                endWaveButtons[0].gameObject.SetActive(won);
                endWaveButtons[1].gameObject.SetActive(false);
                endWaveButtons[2].gameObject.SetActive(true);
            }
            else {
                leftButton.sprite = leftButtons[won ? 1 : 0];
                // next/retry button. shop menu button, no main menu button.
                endWaveButtons[0].gameObject.SetActive(true);
                endWaveButtons[1].gameObject.SetActive(true);
                endWaveButtons[2].gameObject.SetActive(false);
            }

            storyText.gameObject.SetActive(true);

            var text = "";
            if (MetaState.hardcore_mode && won) {
                text = "Hardcore mode score so far: " + MoneyGuy.me.printed_score;
            }
            else if (MetaState.hardcore_mode && !won) {
                text = "Final hardcore mode score: " + MoneyGuy.me.printed_score;
            }
            else if (won && MetaState.wave_number < Story.story.Length) {
                text = Story.story[MetaState.wave_number];
            }
            else {
                text = Story.tips[Random.Range(0, Story.tips.Length)];
            }
            /* note: shane might remove now */
#if UNITY_PS4 || UNITY_PSP2
            storyText.text = text;
#else
            TextBlock.Display(storyText, text);
#endif

            // disable buttons until fade is complete
            for (int i = 0; i < endWaveButtons.Length; ++i) { endWaveButtons[i].onClick.RemoveAllListeners(); }

            LeanTween.alphaCanvas(root[0], 1, 1);
            LeanTween.alphaCanvas(root[1], 1, 1).setOnComplete(() => {
                objectives.CheckComplete();
                endWaveButtons[0].onClick.AddListener(() => SelectWave(won));
                endWaveButtons[1].onClick.AddListener(() => SelectShop());
                endWaveButtons[2].onClick.AddListener(() => MainMenu());
            });
        }

        public Button[] GetLiveButtons()
        {
            List<Button> live_buttons = new List<Button>();
            foreach(Button b in pauseButtons) if (b.isActiveAndEnabled) live_buttons.Add(b);
            foreach(Button b in endWaveButtons) if (b.isActiveAndEnabled) live_buttons.Add(b);
            return live_buttons.ToArray();
        }

        void Hide()
        {
            fade.gameObject.SetActive(false);
            endWaveButtons[0].onClick.RemoveAllListeners();
            endWaveButtons[1].onClick.RemoveAllListeners();
            endWaveButtons[2].onClick.RemoveAllListeners();

            storyText.StopAllCoroutines();  /* NOTE(shane): Dialog Display uses coroutine. */
            storyText.gameObject.SetActive(false);

            endWaveRoot.gameObject.SetActive(false);

            for (int i = 0; i < root.Length; ++i) root[i].alpha = 0;
        }

        public void SelectShop()
        {
            AppMain.tutorial.SetLesson(Lesson.NONE);

            Tracker.EndMode(MetaState.hardcore_mode, Game.dayName(MetaState.wave_number),
                            MetaState.hardcore_mode ? MetaState.hardcore_score : DataStorage.Money);
            Hide();

            AppMain.top_state.Dispose();
            //AppMain.top_state = new Shop();

            Choom.StopAllEffects();
            Choom.Pause(false);

            Choom.PlayEffect(SoundAssets.UIConfirm);
            //AppMain.background_fade = 0;
            //AppMain.background_fade_delta = 1.0f / 120;
        }

        void SelectWave(bool won)
        {
            Hide();

            Choom.StopAllEffects();
            Choom.Pause(false);

            Choom.PlayEffect(SoundAssets.UIConfirm);

            MetaState.resetWave(won);
            AppMain.top_state.Dispose();

//#if LOADING_SCREEN
//            AppMain.top_state = new LoadState(() => { return new Game(); }, DataStorage.StartOnWave / 3 % 5);
//#else
            AppMain.top_state = new Game();
//#endif
        }

        public void Pause(State state)
        {
            isPaused = true;
            Tracker.ScreenVisit(SCREEN_NAME.PAUSE);
            AppMain.tutorial.Pause(true);

            childState = state;

            fade.gameObject.SetActive(true);
            pauseRoot.gameObject.SetActive(true);
            endWaveRoot.gameObject.SetActive(false);

            objectives.UpdateText();

            for (int i = 0; i < root.Length; ++i) root[i].alpha = 1;
        }

        public void Resume()
        {
            isPaused = false;
            Choom.Pause(false);
            Choom.PlayEffect(SoundAssets.UIConfirm);

            AppMain.top_state.Dispose();
            AppMain.top_state = childState;

            AppMain.tutorial.Pause(false);

            pauseRoot.gameObject.SetActive(false);
            fade.gameObject.SetActive(false);
            for (int i = 0; i < root.Length; ++i) root[i].alpha = 0;
        }

        void MainMenu()
        {
            AppMain.tutorial.SetLesson(Lesson.NONE);

            if (isPaused) { Tracker.LevelQuit(MetaState.wave_number); }
            Tracker.EndMode(MetaState.hardcore_mode, Game.dayName(MetaState.wave_number),
                            MetaState.hardcore_mode ? MetaState.hardcore_score : DataStorage.Money);


            Choom.StopAllEffects();
            Choom.Pause(false);

            AppMain.top_state.Dispose();
            AppMain.top_state = new MenuState();

            Game.instance = null;

            pauseRoot.gameObject.SetActive(false);
            endWaveRoot.gameObject.SetActive(false);
            fade.gameObject.SetActive(false);

            for (int i = 0; i < root.Length; ++i) root[i].alpha = 0;
        }
   }
}