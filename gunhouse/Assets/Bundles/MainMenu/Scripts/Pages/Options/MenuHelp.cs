namespace Gunhouse.Menu
{
    public class MenuHelp : MenuPage
    {
        protected override void Initalise() { pageID = MenuState.Help; transitionID = MenuState.Options; }
        protected override void IntroReady() { menu.SetActiveContextButtons(true); }
    }
}