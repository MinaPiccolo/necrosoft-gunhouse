using Necrosoft;

namespace Gunhouse
{
    public class MenuState : State
    {
        public MenuState(Menu.MenuState menuState = Menu.MenuState.Splash)
        {
            AppMain.MainMenu.SetPage(menuState);

            switch (menuState)
            {
            case Menu.MenuState.Credits: Choom.Play("Music/credits"); break;
            default: Choom.Play("Music/title"); break;
            }
        }
    }
}