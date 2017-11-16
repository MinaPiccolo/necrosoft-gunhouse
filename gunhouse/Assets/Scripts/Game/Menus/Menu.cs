using Necrosoft;
using Gunhouse.Menu;

namespace Gunhouse
{
    public class MenuState : State
    {
        public MenuState(Menu.MenuState menuState = Menu.MenuState.Splash, State game_state = null)
        {
            /* ============================== */
            /* this is to get around having to greate multiple
                state classes from the old menu system */

            switch (menuState)
            {
            case Menu.MenuState.Pause: {
                child_state = game_state;
                AppMain.IsPaused = true;
                AppMain.tutorial.Pause(true);
                Choom.Pause();
            } break;
            default: { AppMain.DisplayAnchor = false; } break;
            }

            AppMain.MainMenu.SetPage(menuState);

            /* ============================== */
            /* what music to play */
            switch (menuState)
            {
            case Menu.MenuState.Credits: {
                MenuCredits.ScrollDelay = 10;
                MenuCredits.DisplayEnding = true;
                Choom.Play("Music/credits"); 
            } break;
            default: Choom.Play("Music/title"); break;
            }
        }
    }
}