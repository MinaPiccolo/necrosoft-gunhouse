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

            menu.SetActiveContextButtons();
            menu.SetFocus(lastSelected);
        }

        protected override void OuttroStartNextIntro()
        {
            base.OuttroStartNextIntro();

            /* record last selected item for if the player returns */
            lastSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (transitionID == MenuState.Loading) { /* only for hardcore */
                MetaState.hardcore_mode = true;
            }
            else if (transitionID == MenuState.Splash) {
                lastSelected = refocusSelected;
            }

            transitionID = MenuState.Splash; /* reset it for next time */
        }

        public override void CancelPressed()
        {
            transitionID = MenuState.Splash;
            Play(HashIDs.menu.Outtro);
            menu.PortraitsHide();
            menu.SetActiveContextButtons(false, false);
        }

        public void ChangePage(OnClickPage onclick)
        {
            transitionID = onclick.item;
            Play(HashIDs.menu.Outtro);
            menu.SetActiveContextButtons(false, false);
        }
    }
}