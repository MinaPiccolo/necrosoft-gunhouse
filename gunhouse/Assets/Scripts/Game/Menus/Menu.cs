namespace Gunhouse
{
    public class MenuState : State
    {
        public MenuState(Menu.MenuState menuState, State game_state = null)
        {
            /* ============================== */
            /* this is to get around having to greate multiple
                state classes from the old menu system */

            switch (menuState)
            {
            case Menu.MenuState.EndGame:
            case Menu.MenuState.EndWave: {
                child_state = game_state;
                AppMain.tutorial.Pause(true);
                AppMain.MatchBonus.DismissAnimations();
            } break;
            case Menu.MenuState.Pause: {
                child_state = game_state;
                AppMain.IsPaused = true;
                AppMain.tutorial.Pause(true);
                AppMain.MatchBonus.PauseAnimations();
            } break;
            default: { AppMain.DisplayAnchor = false; } break;
            }

            AppMain.MainMenu.SetPage(menuState);

            /* ============================== */
            /* what music to play */
            switch (menuState)
            {
            case Menu.MenuState.EndGame:
            case Menu.MenuState.EndWave:
            case Menu.MenuState.Pause: { } break;
            default: Necrosoft.Choom.Play("Music/title"); break;
            }
        }
    }
}