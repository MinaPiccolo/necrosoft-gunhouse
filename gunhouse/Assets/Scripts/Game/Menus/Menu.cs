using Necrosoft;
using Gunhouse.Menu;

namespace Gunhouse
{
    public class MenuState : State
    {
        public MenuState(Menu.MenuState menuState = Menu.MenuState.Splash)
        {
            AppMain.MainMenu.SetPage(menuState);

            AppMain.DisplayAnchor = false;

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