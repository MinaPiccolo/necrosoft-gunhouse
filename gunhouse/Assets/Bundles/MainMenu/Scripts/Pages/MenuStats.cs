using UnityEngine;
using TMPro;

namespace Gunhouse.Menu
{
    public class MenuStats : MenuPage
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI details;
        [SerializeField] TextMeshProUGUI[] textColumns;

        [SerializeField] GameObject[] arrows;
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
            UpdateText();
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
            int new_index = Mathf.Clamp(right ? page_index + 1 : page_index - 1, 0, 3 - 1); 
            if (new_index == page_index) return;
            page_index = new_index;

            SetArrows(page_index != 0, page_index != 3 - 1);
            UpdateText();
        }

        void UpdateText()
        {
            details.gameObject.SetActive(page_index < 2);
            textColumns[0].gameObject.SetActive(page_index > 1);
            textColumns[1].gameObject.SetActive(page_index > 1);

            switch (page_index)
            {
                case 0: SetBestHardcore(); break;
                case 1: SetInfo(); break;
                case 2: SetBlocksLoaded(); break;
            }
        }

        void SetBestHardcore()
        {
            menu.builder.Length = 0;
            title.text = menu.builder.Append("BEST HARDCORE SCORES").ToString();

            menu.builder.Length = 0;

            for (int i = 0; i < 5; i++) {
                int amount;
                int day;

                if (i < DataStorage.BestHardcoreScores.Count) {
                    amount = DataStorage.BestHardcoreScores[i].Item1;
                    day = DataStorage.BestHardcoreScores[i].Item2;

                    menu.builder.AppendFormat("{0}: ${1}, {2}\n", i + 1, amount, menu.DayName(day));
                }
                else {
                    menu.builder.AppendFormat("{0}: $0, DAY X\n", i + 1);
                }
            }

            details.text = menu.builder.ToString();
        }

        void SetInfo()
        {
            menu.builder.Length = 0;
            title.text = menu.builder.Append("INFO").ToString();

            menu.builder.Length = 0;

            int most = -1;
            int max = -1;
            for (int i = 0; i < 10; i++) {
                if (DataStorage.AmmoLoaded[i] > max) {
                    most = i;
                    max = DataStorage.AmmoLoaded[i];
                }
            }

            menu.builder.AppendFormat("Favorite Gun: {0}\n", (Gunhouse.Gun.Ammo)most);
            menu.builder.AppendFormat("Shots Fired: {0}\n", DataStorage.ShotsFired);
            menu.builder.AppendFormat("Times Defeated: {0}\n", DataStorage.TimesDefeated);
            menu.builder.AppendFormat("Best Match Streak: {0}", DataStorage.MatchStreak);

            details.text = menu.builder.ToString();
        }

        void SetBlocksLoaded()
        {
            menu.builder.Length = 0;
            title.text = menu.builder.Append("BLOCKS LOADED").ToString();

            int n = 0;
            for (int x = 2; x <= 3; x++) {
                menu.builder.Length = 0;
                for (int y = 2; y <= 6; y++) {
                    menu.builder.AppendFormat("{0}x{1}: {2}\n", x, y, DataStorage.BlocksLoaded[n]);
                    n++;
                }

                textColumns[x - 2].text = menu.builder.ToString();
            }
        }
    }
}