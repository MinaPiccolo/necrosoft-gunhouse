using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gunhouse.Menu
{
    public class MenuPickADay : MenuPage
    {
        [SerializeField] GameObject lastSelected;
        [SerializeField] GameObject[] arrows;
        [Space(10)] [SerializeField] Image dayImage;
        [SerializeField] TextMeshProUGUI dayText;

        PlayerInput input;
        int selected_day;
        int max_day;

        protected override void Initalise() { pageID = MenuState.PickADay; transitionID = MenuState.Title; }
        protected override void OuttroStartNextIntro()
        {
            base.OuttroStartNextIntro();

            if (transitionID == MenuState.Loading) {
                menu.PortraitOrder(2);
                menu.FadeInOut(false, 0.5f);
            }

            transitionID = MenuState.Title; /* reset it for next time */
        }
    
        protected override void IntroReady()
        {
            menu.SetActiveContextButtons(true, true);
            MainMenu.SetFocus(lastSelected);
        }

        protected override void OuttroFinished()
        {
            /* record last selected item for if the player returns */
            lastSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            MainMenu.SetFocus(null);

            base.OuttroFinished();
        }

        void OnEnable()
        {
            input = FindObjectOfType<PlayerInput>();

            max_day = DataStorage.StartOnWave;
            selected_day = max_day;

            SetArrows(selected_day != 0, selected_day != max_day);
            UpdateWaveInfo();
        }

        void Update()
        {
            if (menu.ignore_input) return;

            if (input.Left.WasPressed) { ChangeWave(false); }
            else if (input.Right.WasPressed) { ChangeWave(true);}
        }

        public void ChangeWave(bool right)
        {
            int new_index = Mathf.Clamp(right ? selected_day + 1 : selected_day - 1, 0, max_day); 
            if (new_index == selected_day) return;
            selected_day = new_index;

            SetArrows(selected_day != 0, selected_day != max_day);
            UpdateWaveInfo();
        }

        void SetArrows(bool left, bool right)
        {
            arrows[0].SetActive(left);
            arrows[1].SetActive(right);
        }

        void UpdateWaveInfo()
        {
            dayText.text = menu.DayName(selected_day);
        }

        public void StartPlay()
        {
            transitionID = MenuState.Loading;
            Play(HashIDs.menu.Outtro);
            menu.SetActiveContextButtons(false);

            menu.SelectedWave = selected_day;
            MetaState.hardcore_mode = false;
        }
    }
}