using UnityEngine;
using Necrosoft;

namespace Gunhouse
{
    public class MenuState : State
    {
        public MenuState()
        {
            AppMain.MainMenu.SetPage(Menu.MenuState.Splash);
            Choom.Play("Music/title");
        }
    }
}
