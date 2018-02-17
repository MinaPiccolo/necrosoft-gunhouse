using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gunhouse.Menu
{
    public class MenuHelp : MenuPage
    {
        [SerializeField] GameObject[] arrows;
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] GameObject[] teachers;
        [SerializeField] TextMeshProUGUI bubbleText;
        [SerializeField] Image[] blocks;
        [SerializeField] HorizontalLayoutGroup blockLayout;
        [SerializeField] GameObject[] houses;

        int page_index;
        int page_count;
        PlayerInput input;

        int arrow_input;
        int arrow_input_last;
        public void ArrowPressed(bool right) { arrow_input = right ? 1 : -1; }
        public void ArrowReleased() { arrow_input = 0; arrow_input_last = 0; }
        public void ArrowEnter(bool right) { if (arrow_input_last == (right ? 1 : -1)) { arrow_input = right ? 1 : -1; } }
        public void ArrowExit() { if (arrow_input == 0) { return; } arrow_input_last = arrow_input; arrow_input = 0; }

        protected override void Initalise() { pageID = MenuState.Help; transitionID = MenuState.Options; }

        protected override void IntroReady()
        {
            menu.SetActiveContextButtons(false, true);
            menu.SetFocus(null);
        }

        public override void CancelPressed()
        {
            for (int i = 0; i < blocks.Length; ++i) { blocks[i].gameObject.SetActive(false); }
            blocks[0].transform.parent.gameObject.SetActive(false);
            for (int i = 0; i < houses.Length; ++i) { houses[i].SetActive(false); }
            houses[0].transform.parent.gameObject.SetActive(false);

            transitionID = AppMain.IsPaused ? MenuState.Pause : MenuState.Options;

            base.CancelPressed();
        }

        void OnEnable()
        {
            menu.PortraitsHide();

            input = FindObjectOfType<PlayerInput>();

            arrow_input = 0;
            arrow_input_last = 0;
            page_index = 0;
            page_count = GText.help.Length;

            teachers[0].SetActive(true);
            teachers[1].SetActive(false);

            blocks[0].transform.parent.gameObject.SetActive(false);
            for (int i = 0; i < blocks.Length; ++i) {
                blocks[i].color = Color.white;
                blocks[i].gameObject.SetActive(false);
            }

            SetArrows(false, true);
            SetInfo();
        }

        void Update()
        {
            if (menu.ignore_input) return;

            int horitzonal = Util.KeyRepeatTime("arrow_directional", 0, 30, 10,
                                                arrow_input == 0 ? (int)input.Move.x : arrow_input);

            switch (horitzonal) { case -1: SetPage(false); break; case 1: SetPage(true); break; }
        }

        void SetPage(bool right)
        {
            int new_index = Mathf.Clamp(right ? page_index + 1 : page_index - 1, 0, page_count - 1); 
            if (new_index == page_index) return;
            page_index = new_index;

            Necrosoft.Choom.PlayEffect(SoundAssets.UIConfirm);
            SetArrows(page_index != 0, page_index != page_count - 1);
            SetInfo();
        }

        void SetInfo()
        {
            if (page_index > (int)HelpOrder.TOWER_DEFENSE_TIP_2) {
                title.text = GText.help_titles[3];
            }
            else if (page_index > (int)HelpOrder.PUZZLE_TIP_6) {
                title.text = GText.help_titles[2];
            }
            else if (page_index > (int)HelpOrder.HOT_TIP) {
                title.text = GText.help_titles[1];
            }
            else {
                title.text = GText.help_titles[0];
            }

            switch (page_index)
            {
            case (int)HelpOrder.PUZZLE_TIP_0:
            case (int)HelpOrder.TOWER_DEFENSE_TIP_0:
            case (int)HelpOrder.HARDCORE_1: { teachers[0].SetActive(false); teachers[1].SetActive(true); } break;
            default: { teachers[0].SetActive(true); teachers[1].SetActive(false);} break;
            }

            bubbleText.text = GText.help[page_index];

            #region Block Displaying

            int item_enabled = page_index > (int)HelpOrder.PUZZLE_TIP_6 ? -10 : page_index - 2;
            int item_highlighted = 1;

            if (page_index > (int)HelpOrder.PUZZLE_TIP_1) { item_enabled++; }
            if (page_index == (int)HelpOrder.PUZZLE_TIP_2) { item_highlighted++; }
            if (page_index == (int)HelpOrder.PUZZLE_TIP_6) { item_highlighted = blocks.Length + 2; }

            if (item_enabled < 0) { blocks[0].transform.parent.gameObject.SetActive(false); }
            else { blocks[0].transform.parent.gameObject.SetActive(true); }

            for (int i = 0; i < blocks.Length; ++i) {
                if (i > item_enabled) { blocks[i].gameObject.SetActive(false); }
                else {
                    blocks[i].gameObject.SetActive(true);
                    blocks[i].color = i > (item_enabled - item_highlighted) ? Color.white : Color.gray;
                }
            }

            blockLayout.padding.right = blockLayout.padding.left = page_index == (int)HelpOrder.PUZZLE_TIP_1 ? 100 : 50;

            #endregion

            #region House Displaying

            switch (page_index)
            {
            case (int)HelpOrder.TOWER_DEFENSE_TIP_1: {
                houses[0].transform.parent.gameObject.SetActive(true);
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(false);
            } break;
            case (int)HelpOrder.TOWER_DEFENSE_TIP_2: {
                houses[0].transform.parent.gameObject.SetActive(true);
                houses[0].SetActive(false);
                houses[1].SetActive(false);
                houses[2].SetActive(true);
            } break;
            default: {
                for (int i = 0; i < houses.Length; ++i) { houses[i].SetActive(false); }
                houses[0].transform.parent.gameObject.SetActive(false);
            } break;
            }

            #endregion
        }

        void SetArrows(bool left, bool right)
        {
            arrows[0].SetActive(left);
            arrows[1].SetActive(right);
        }
    }

    public enum HelpOrder { HOT_TIP = 0,
                            PUZZLE_TIP_0, PUZZLE_TIP_1, PUZZLE_TIP_2, PUZZLE_TIP_3,
                            PUZZLE_TIP_4, PUZZLE_TIP_5, PUZZLE_TIP_6,
                            TOWER_DEFENSE_TIP_0, TOWER_DEFENSE_TIP_1, TOWER_DEFENSE_TIP_2,
                            HARDCORE_0, HARDCORE_1, HARDCORE_2 };
}