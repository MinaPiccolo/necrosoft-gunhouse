using UnityEngine;
using TMPro;
using Necrosoft.ThirdParty;

namespace Gunhouse.Menu
{
    public class MenuPickADay : MenuPage
    {
        [SerializeField] RectTransform clockHandle;
        [SerializeField] GameObject[] arrows;
        [SerializeField] TextMeshProUGUI dayText;

        PlayerInput input;
        int selected_day;
        int max_day;
        float[] clock_angles = { -25, 180, 47 };

        int arrow_input;
        int arrow_input_last;
        public void ArrowPressed(bool right) { arrow_input = right ? 1 : -1; }
        public void ArrowReleased() { arrow_input = 0; arrow_input_last = 0; }
        public void ArrowEnter(bool right) { if (arrow_input_last == (right ? 1 : -1)) { arrow_input = right ? 1 : -1; } }
        public void ArrowExit() { if (arrow_input == 0) { return; } arrow_input_last = arrow_input; arrow_input = 0; }

        protected override void Initalise() { pageID = MenuState.PickADay; transitionID = MenuState.Title; }
        protected override void OuttroStartNextIntro()
        {
            base.OuttroStartNextIntro();

            transitionID = MenuState.Title; /* reset it for next time */
        }
    
        protected override void IntroReady()
        {
            menu.SetActiveContextButtons();
            menu.SetActiveBackButton(true);
            menu.SetFocus(refocusSelected, true);
        }

        void OnEnable()
        {
            input = FindObjectOfType<PlayerInput>();

            arrow_input = 0;
            arrow_input_last = 0;
            max_day = DataStorage.StartOnWave;
            selected_day = max_day;

            SetArrows(selected_day != 0, selected_day != max_day);
            dayText.text = menu.DayName(selected_day);
            clockHandle.eulerAngles = Vector3.forward * clock_angles[selected_day % 3];
        }

        void Update()
        {
            if (menu.ignore_input) return;

            int horitzonal = Util.keyRepeat("arrow_directional", 0, 30, 10,
                                            arrow_input == 0 ? (int)input.Move.x : arrow_input);
            switch (horitzonal) { case -1: ChangeWave(false); break; case 1: ChangeWave(true); break; }
        }

        void ChangeWave(bool right)
        {
            int new_index = Mathf.Clamp(right ? selected_day + 1 : selected_day - 1, 0, max_day); 
            if (new_index == selected_day) return;
            selected_day = new_index;

            Necrosoft.Choom.PlayEffect(SoundAssets.UIConfirm);
            SetArrows(selected_day != 0, selected_day != max_day);
            dayText.text = menu.DayName(selected_day);
            clockHandle.eulerAngles = Vector3.forward * clock_angles[selected_day % 3];
        }

        void SetArrows(bool left, bool right)
        {
            arrows[0].SetActive(left);
            arrows[1].SetActive(right);
        }

        public void StartPlay()
        {
            transitionID = MenuState.Loading;
            Play(HashIDs.menu.Outtro);
            menu.SetActiveContextButtons(false, false);

            menu.SelectedWave = selected_day;
            MetaState.hardcore_mode = false;
        }
    }
}