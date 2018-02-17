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

        int arrow_input;
        int arrow_input_last;
        public void ArrowPressed(bool right) { arrow_input = right ? 1 : -1; }
        public void ArrowReleased() { arrow_input = 0; arrow_input_last = 0; }
        public void ArrowEnter(bool right) { if (arrow_input_last == (right ? 1 : -1)) { arrow_input = right ? 1 : -1; } }
        public void ArrowExit() { if (arrow_input == 0) { return; } arrow_input_last = arrow_input; arrow_input = 0; }

        protected override void Initalise() { pageID = MenuState.Stats; transitionID = MenuState.Title; }
        protected override void IntroReady()
        {
            menu.SetActiveContextButtons(false, true);
            menu.SetFocus(null);
        }

        void OnEnable()
        {
            Tracker.ScreenVisit(SCREEN_NAME.STATS);

            arrow_input = 0;
            arrow_input_last = 0;
            input = FindObjectOfType<PlayerInput>();
            page_index = 0;
            SetArrows(false, true);
            UpdateText();
        }

        void Update()
        {
            if (menu.ignore_input) return;

            int horitzonal = Util.KeyRepeatTime("arrow_directional", 0, 30, 10,
                                            arrow_input == 0 ? (int)input.Move.x : arrow_input);

            switch (horitzonal) { case -1: SetPage(false); break; case 1: SetPage(true); break; }
        }

        void SetArrows(bool left, bool right)
        {
            arrows[0].SetActive(left);
            arrows[1].SetActive(right);
        }

        void SetPage(bool right)
        {
            int new_index = Mathf.Clamp(right ? page_index + 1 : page_index - 1, 0, 3 - 1); 
            if (new_index == page_index) return;
            page_index = new_index;

            Necrosoft.Choom.PlayEffect(SoundAssets.UIConfirm);
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
            title.text = menu.builder.Append(GText.best_hardcore_scores).ToString();

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
                    menu.builder.AppendFormat("{0}: $0, {1} X\n", i + 1, GText.day);
                }
            }

            details.text = menu.builder.ToString();
        }

        void SetInfo()
        {
            menu.builder.Length = 0;
            title.text = menu.builder.Append(GText.info).ToString();

            menu.builder.Length = 0;

            int most = -1;
            int max = -1;
            for (int i = 0; i < 10; i++) {
                if (DataStorage.AmmoLoaded[i] > max) {
                    most = i;
                    max = DataStorage.AmmoLoaded[i];
                }
            }

            menu.builder.AppendFormat("{0}: {1}\n", GText.favourite_gun, (Gunhouse.Gun.Ammo)most);
            menu.builder.AppendFormat("{0}: {1}\n", GText.shots_fired, DataStorage.ShotsFired);
            menu.builder.AppendFormat("{0}: {1}\n", GText.times_defeated, DataStorage.TimesDefeated);
            menu.builder.AppendFormat("{0}: {1}", GText.best_match_streak, DataStorage.MatchStreak);

            details.text = menu.builder.ToString();
        }

        void SetBlocksLoaded()
        {
            menu.builder.Length = 0;
            title.text = menu.builder.Append(GText.blocks_loaded).ToString();

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