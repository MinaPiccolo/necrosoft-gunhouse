using UnityEngine;

namespace Gunhouse.Menu
{
    public class MenuStats : MenuPage
    {
        [SerializeField] GameObject[] arrows;
        [SerializeField] GameObject[] pages;
        int page_index = 0;
        PlayerInput input;

        protected override void Initalise() { pageID = MenuState.Stats; transitionID = MenuState.Title; }
        protected override void IntroReady()
        {
            menu.SetActiveContextButtons(true, false);
            MainMenu.SetFocus(arrows[1]);
        }

        void OnEnable()
        {
            Tracker.ScreenVisit(SCREEN_NAME.STATS);

            input = FindObjectOfType<PlayerInput>();
            SetArrows(false, true);

            for (int i = 0; i < pages.Length; ++i) { pages[i].SetActive(false); }
            pages[page_index].SetActive(true);
        }

        void Update()
        {
            if (menu.ignore_input) return;

            if (input.Left.WasPressed) { SetPage(false); }
            else if (input.Right.WasPressed) { SetPage(true);}
        }

        void SetArrows(bool left, bool right)
        {
            arrows[0].SetActive(left);
            arrows[1].SetActive(right);
        }

        public void SetPage(bool right)
        {
            int new_index = Mathf.Clamp(right ? page_index + 1 : page_index - 1, 0, pages.Length - 1); 
            if (new_index == page_index) return;

            pages[page_index].SetActive(false);
            pages[new_index].SetActive(true);
            page_index = new_index;

            SetArrows(page_index != 0, page_index != pages.Length - 1);
        }
    }
}