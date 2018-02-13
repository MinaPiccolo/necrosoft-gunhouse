namespace Gunhouse.Menu
{
    public class MenuSplash : MenuPage
    {
        protected override void Initalise() { pageID = MenuState.Splash; transitionID = MenuState.Title; }

        public override void CancelPressed()
        {
            menu.PlayConfirm();
            menu.ignore_input = true;
            Play(HashIDs.menu.Outtro);
            menu.SetFocus(null);
        }

        protected override void IntroReady()
        {
            menu.SetFocus(gameObject);
            base.IntroReady();
        }
    }
}