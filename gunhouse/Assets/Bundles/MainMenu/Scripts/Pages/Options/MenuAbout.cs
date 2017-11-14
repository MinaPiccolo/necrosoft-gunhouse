namespace Gunhouse.Menu
{
    public class MenuAbout : MenuPage
    {
        protected override void Initalise() { pageID = MenuState.About; transitionID = MenuState.Options; }
        protected override void IntroReady() { menu.SetActiveContextButtons(true); }
    }
}