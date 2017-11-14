namespace Gunhouse.Menu
{
    public class MenuInput : MenuPage
    {
        protected override void Initalise() { pageID = MenuState.Input; transitionID = MenuState.Options; }
        protected override void IntroReady() { menu.SetActiveContextButtons(true); }
    }
}