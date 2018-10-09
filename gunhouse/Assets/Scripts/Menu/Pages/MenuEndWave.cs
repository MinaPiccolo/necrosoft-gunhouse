using UnityEngine;
using Necrosoft;
using Necrosoft.ThirdParty;
using TMPro;

namespace Gunhouse.Menu
{
    public class MenuEndWave : MenuPage
    {
        [SerializeField] GameObject[] titles;
        [SerializeField] TextMeshProUGUI topLeftText;
        [SerializeField] Objectives objectives;
        [SerializeField] OnClickItem waveResponse;

        [Space(10)] [SerializeField] GameObject[] buttons;

        protected override void Initalise() { pageID = MenuState.EndWave; transitionID = MenuState.None; }

        protected override void IntroReady()
        {
            menu.SetActiveContextButtons(true, false);
            menu.SetFocus(refocusSelected);

            objectives.Play(HashIDs.menu.Intro);
            objectives.UpdateText();
            LeanTween.delayedCall(0.3f, () => { objectives.CheckComplete(); });
        }

        protected override void OuttroStartNextIntro()
        {
            if (transitionID != MenuState.None) {
                Choom.Play("Music/title");
                base.OuttroStartNextIntro();
            }
        }

        protected override void OuttroFinished()
        {
            transitionID = MenuState.None; /* reset it for next time */
            Choom.Pause(false);

            base.OuttroFinished();
        }

        public override void CancelPressed() { }

        void OnEnable()
        {
            Tracker.ScreenVisit(AppMain.HasWon ? SCREEN_NAME.WAVE_COMPLETE : SCREEN_NAME.WAVE_FAILED);

            AppMain.screenShake(0, 0);
            Choom.StopAllEffects();
            Choom.Pause();

            if (AppMain.HasWon) {
                if (MetaState.wave_number % 3 == 2) {
                    Objectives.BossDefeated();
                }

                if (MetaState.wave_number + 1 > DataStorage.StartOnWave &&
                    !MetaState.hardcore_mode) {
                    DataStorage.StartOnWave = MetaState.wave_number + 1;
                }
            }
            else {
                DataStorage.TimesDefeated++;

                if (MetaState.hardcore_mode) {
                    Game.instance.saveHardcoreScore();
                    Platform.SaveHardcore();
                }
            }

            Platform.SaveEndWave();
            Objectives.CheckAchievements();

            titles[0].SetActive(AppMain.HasWon);
            titles[1].SetActive(!AppMain.HasWon);

            for (int i = 0; i < buttons.Length; ++i) { buttons[i].SetActive(true); }
            refocusSelected = buttons[0].gameObject;

            menu.builder.Length = 0;
            if (MetaState.hardcore_mode && AppMain.HasWon) {
                menu.builder.AppendFormat("Hardcore mode score so far: {0}", MoneyGuy.me.printed_score);
                buttons[0].SetActive(true);
                buttons[1].SetActive(false);
                buttons[2].SetActive(true);
            }
            else if (MetaState.hardcore_mode && !AppMain.HasWon) {
                menu.builder.AppendFormat("Final hardcore mode score: {0}", MoneyGuy.me.printed_score);
                refocusSelected = buttons[2].gameObject;
                buttons[0].SetActive(false);
                buttons[1].SetActive(false);
                buttons[2].SetActive(true);
            }
            else if (AppMain.HasWon && MetaState.wave_number < GText.Story.story.Length) {
                menu.builder.Append(GText.Story.story[MetaState.wave_number]);
            }
            else {
                menu.builder.Append(GText.Story.tips[Random.Range(0, GText.Story.tips.Length)]);
            }

            waveResponse.item = AppMain.HasWon ? MenuItem.NextWave : MenuItem.RetryWave;
            waveResponse.GetComponent<TextMeshProUGUI>().text = AppMain.HasWon ? "Next Wave" : "Retry Wave";

            topLeftText.text = menu.builder.ToString();

            menu.SetActiveDayName(false);
            menu.Fade(0.9f, 1);
        }

        public void SetButton(OnClickItem item)
        {
            menu.SetActiveContextButtons(false, false);

            switch (item.item)
            {
            case MenuItem.MainMenu: { transitionID = MenuState.Title; } break;
            case MenuItem.Store: { transitionID = MenuState.Store; } break;
            }

            switch (item.item)
            {
            case MenuItem.MainMenu:
            case MenuItem.Store: {
                AppMain.HasWon = false;
                AppMain.DisplayAnchor = false;
                MetaState.hardcore_mode = false;

                AppMain.tutorial.SetLesson(Lesson.NONE);
                AppMain.tutorial.SetDisplay(false);

                AppMain.top_state.child_state.Dispose();
                AppMain.top_state.child_state = null;
                Game.instance = null;

                Tracker.EndMode(MetaState.hardcore_mode, AppMain.MainMenu.DayName(MetaState.wave_number),
                                MetaState.hardcore_mode ? MetaState.hardcore_score : DataStorage.Money);

                menu.SetActiveDayName(false, true);
            } break;
            case MenuItem.RetryWave:
            case MenuItem.NextWave: {
                MetaState.resetWave(AppMain.HasWon);
                AppMain.HasWon = false;

                AppMain.top_state.Dispose();
                AppMain.top_state = new Game();
            } break;
            }

            objectives.Play(HashIDs.menu.Outtro);
            Play(HashIDs.menu.Outtro);
            menu.Fade(0, 0.3f);
        }
    }
}