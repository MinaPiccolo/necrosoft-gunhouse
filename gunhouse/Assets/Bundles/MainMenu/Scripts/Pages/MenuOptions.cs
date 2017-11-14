using UnityEngine;

namespace Gunhouse.Menu
{
    public class MenuOptions : MenuPage
    {
        [SerializeField] GameObject lastSelected;

        protected override void Initalise() { pageID = MenuState.Options; transitionID = MenuState.Title; }

        protected override void IntroReady()
        {
            Tracker.ScreenVisit(SCREEN_NAME.OPTIONS);
            menu.SetActiveContextButtons(true);
            MainMenu.SetFocus(lastSelected);
        }

        protected override void OuttroStartNextIntro()
        {
            base.OuttroStartNextIntro();
            transitionID = MenuState.Title; /* reset it for next time */
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
        }
    }
}