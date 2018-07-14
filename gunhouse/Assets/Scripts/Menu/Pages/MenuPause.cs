using UnityEngine;
using Necrosoft;

namespace Gunhouse.Menu
{
    public class MenuPause : MenuPage
    {
        GameObject lastSelected;
        [SerializeField] Objectives objectives;

        protected override void Initalise() { pageID = MenuState.Pause; transitionID = MenuState.None; }

        protected override void IntroReady()
        {
            menu.SetActiveContextButtons();
            menu.SetFocus(lastSelected == null ? refocusSelected : lastSelected);
        }

        protected override void OuttroStartNextIntro()
        {
            if (transitionID != MenuState.None && !AppMain.IsPaused) {
                Choom.Play("Music/title");
            }

            base.OuttroStartNextIntro();
        }

        protected override void OuttroFinished()
        {
            /* record last selected item for if the player returns */
            lastSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (transitionID == MenuState.Title || 
                transitionID == MenuState.None) { lastSelected = refocusSelected; }

            if (transitionID == MenuState.None) {
                Choom.Pause(false);

                State game_state = AppMain.top_state.child_state;
                AppMain.top_state.Dispose();
                AppMain.top_state = game_state;

                menu.SetActiveDayName(true);
                AppMain.tutorial.Pause(false);
                lastSelected = refocusSelected;
            }

            transitionID = MenuState.None; /* reset it for next time */

            base.OuttroFinished();
        }

        public override void CancelPressed()
        {
            objectives.Play(HashIDs.menu.Outtro);

            if (transitionID == MenuState.None) {
                AppMain.IsPaused = false;
                AppMain.MatchBonus.ResumeAnimations();
            }

            if (transitionID == MenuState.None ||
                transitionID == MenuState.Title) {
                menu.Fade(0, 0.25f);
            }

            base.CancelPressed();
        }

        void OnEnable()
        {
            Tracker.ScreenVisit(SCREEN_NAME.PAUSE);

            Choom.Pause();
            AppMain.screenShake(0, 0);

            objectives.Play(HashIDs.menu.Intro);
            objectives.UpdateText();

            menu.SetActiveDayName(false);
            menu.Fade(0.9f, 0.25f);
        }

        public void SetButton(OnClickItem item)
        {
            menu.SetActiveContextButtons(false, false);

            switch (item.item)
            {
            case MenuItem.Resume: { transitionID = MenuState.None; } break;
            case MenuItem.MainMenu: { transitionID = MenuState.Title; } break;
            case MenuItem.Audio: { transitionID = MenuState.Audio; } break;
            }

            switch (item.item)
            {
            case MenuItem.Audio: {
                menu.SetActiveDayName(false, true);
                Choom.Pause(false);
            } break;
            case MenuItem.MainMenu: {
                AppMain.HasWon = false;
                AppMain.DisplayAnchor = false;
                MetaState.hardcore_mode = false;

                AppMain.tutorial.SetLesson(Lesson.NONE);
                AppMain.tutorial.SetDisplay(false);
                AppMain.MatchBonus.DismissAnimations();

                Choom.StopAllEffects();
                Choom.Pause(false);

                AppMain.top_state.child_state.Dispose();
                AppMain.top_state.child_state = null;
                Game.instance = null;

                if (AppMain.IsPaused) {
                    Tracker.LevelQuit(MetaState.wave_number);
                }
                else {
                    Tracker.EndMode(MetaState.hardcore_mode, AppMain.MainMenu.DayName(MetaState.wave_number),
                            MetaState.hardcore_mode ? MetaState.hardcore_score : DataStorage.Money);
                }

                menu.SetActiveDayName(false, true);

                AppMain.IsPaused = false;
            } break;
            }
            CancelPressed();
        }
    }
}