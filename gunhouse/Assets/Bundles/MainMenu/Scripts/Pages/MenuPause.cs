using UnityEngine;
using Necrosoft;

namespace Gunhouse.Menu
{
    public class MenuPause : MenuPage
    {
        [SerializeField] GameObject selected;
        [SerializeField] Objectives objectives;

        protected override void Initalise() { pageID = MenuState.Pause; transitionID = MenuState.None; }

        protected override void IntroReady()
        {
            menu.SetActiveContextButtons(true, true);
            MainMenu.SetFocus(selected);

            /* check objectives complete here update only don't complete? */
        }

        protected override void OuttroStartNextIntro()
        {
            menu.PortraitOrder(0);

            if (transitionID != MenuState.None) {
                base.OuttroStartNextIntro();
            }
        }

        protected override void OuttroFinished()
        {
            if (transitionID == MenuState.None) {
                AppMain.IsPaused = false;
                Choom.Pause(false);
                Choom.PlayEffect(SoundAssets.UIConfirm);

                State game_state = AppMain.top_state.child_state;
                AppMain.top_state.Dispose();
                AppMain.top_state = game_state;

                menu.SetActiveDayName(true);
                AppMain.tutorial.Pause(false);
            }

            transitionID = MenuState.None; /* reset it for next time */
            base.OuttroFinished();
        }

        void OnEnable()
        {
            Tracker.ScreenVisit(SCREEN_NAME.PAUSE);
            menu.PortraitOrder(2);
            objectives.UpdateText();
            menu.SetActiveDayName(false);
        }

        public void SetButton(OnClickItem item)
        {
            switch (item.item)
            {
            case MenuItem.Resume: {
                transitionID = MenuState.None;
                menu.PortraitsHide();
                menu.SetActiveContextButtons(false);
                Choom.PlayEffect(SoundAssets.UIConfirm);
            } break;
            case MenuItem.MainMenu: {
                transitionID = MenuState.Title;
            } break;
            case MenuItem.Store: {
                transitionID = MenuState.Store;
            } break;
            }

            switch (item.item)
            {
            case MenuItem.MainMenu:
            case MenuItem.Store: {
                AppMain.DisplayAnchor = false;
                AppMain.tutorial.SetLesson(Lesson.NONE);

                Choom.StopAllEffects();
                Choom.Pause(false);

                AppMain.top_state.child_state.Dispose();
                AppMain.top_state.child_state = null;
                Game.instance = null;

                Tracker.LevelQuit(MetaState.wave_number);
                Tracker.EndMode(MetaState.hardcore_mode, AppMain.MainMenu.DayName(MetaState.wave_number),
                        MetaState.hardcore_mode ? MetaState.hardcore_score : DataStorage.Money);

                menu.SetActiveDayName(false, true);
            } break;
            }

            Play(HashIDs.menu.Outtro);
        }
    }
}