using UnityEngine;

namespace Gunhouse.Menu
{
    public class MenuOptions : MenuPage
    {
        GameObject lastSelected;

        protected override void Initalise() { pageID = MenuState.Options; transitionID = MenuState.Title; }

        protected override void IntroReady()
        {
            Tracker.ScreenVisit(SCREEN_NAME.OPTIONS);

            menu.SetActiveContextButtons();
            menu.SetFocus(lastSelected == null ? refocusSelected : lastSelected);

            MenuCredits.ScrollDelay = 2;
            MenuCredits.DisplayEnding = false;
        }

        protected override void OuttroStartNextIntro()
        {
            base.OuttroStartNextIntro();
            transitionID = MenuState.Title; /* reset it for next time */

            Platform.SaveOptions();
        }

        public override void CancelPressed()
        {
            /* record last selected item for if the player returns */
            lastSelected = transitionID == MenuState.Title ? null : UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            base.CancelPressed();
        }

        public void ChangePage(OnClickPage onclick)
        {
            transitionID = onclick.item;
            CancelPressed();
        }
    }
}