using UnityEngine;

namespace Gunhouse.Menu
{
    public class MenuTitle : MenuPage
    {
        [SerializeField] GameObject lastSelected;

        protected override void Initalise() { pageID = MenuState.Title; transitionID = MenuState.Splash; }

        protected override void IntroReady()
        {
            Tracker.ScreenVisit(SCREEN_NAME.MAIN_MENU);
            menu.SetActiveContextButtons(true, true);
            MainMenu.SetFocus(lastSelected);
        }

        protected override void OuttroStartNextIntro()
        {
            base.OuttroStartNextIntro();
            transitionID = MenuState.Splash; /* reset it for next time */
        }

        protected override void OuttroFinished()
        {
            /* record last selected item for if the player returns */
            lastSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            MainMenu.SetFocus(null);

            base.OuttroFinished();
        }

        public void ChangePage(OnClickPage onclick)
        {
            transitionID = onclick.item;
            Play(HashIDs.menu.Outtro);
            menu.SetActiveContextButtons(false);

            if (onclick.item != MenuState.PlayGame && onclick.item != MenuState.Hardcore) return;
            menu.PortraitOrder(2);
            menu.FadeInOut(false);
        }
    }
}