using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Necrosoft.ThirdParty;

namespace Gunhouse.Menu
{
    public partial class MenuStore : MenuPage
    {
        [SerializeField] GameObject cat;

        [SerializeField] MenuStoreSelector selector;
        [SerializeField] Color lockedColor;
        [SerializeField] RectTransform moneyBoard;
        [SerializeField] TextMeshProUGUI moneyText;

        [Space(10)] [SerializeField] TextMeshProUGUI itemTitle;
        [SerializeField] TextMeshProUGUI itemInfo;

        [Space(10)] [SerializeField] Button[] itemButtons;
        [SerializeField] OptionsBoard optionsBoard;

        [Space(10)] [SerializeField] Image equip_icon;
        [SerializeField] Image[] equips;

        [Header("Order the same as menuitem enum")]
        [SerializeField] Image[] buttonImages;

        GameObject lastItemSelected;
        StoreItem currentItem;

        bool moneyBoardMoving;
        int[] equipIndex = new int[3] { 0, 1, 2 };
        int heartsBeforeArmor = 6;
        public static int current_selection;

        void OnEnable()
        {
            Tracker.ScreenVisit(SCREEN_NAME.SHOP);

            /* set equips */
            int n = 0;
            for (int i = 0; i < DataStorage.GunEquipped.Length; ++i) {
                if (!DataStorage.GunEquipped[i]) continue;
                equipIndex[n] = i; n++;
                if (n >= equipIndex.Length) { break; }
            }

            SetItemInfo(StoreItem.StoreDragon);
            SetEquips();
            SetItemStatus();
            optionsBoard.gameObject.SetActive(false);
            selector.gameObject.SetActive(false);
        }

        protected override void Initalise() { pageID = MenuState.Store; transitionID = MenuState.Title; }

        protected override void IntroReady()
        {
            SetMoneyCounter();
            menu.SetActiveContextButtons();
            menu.SetActiveBackButton(true);
            menu.SetFocus(refocusSelected);
        }

        protected override void OuttroStartNextIntro()
        {
            selector.gameObject.SetActive(false);
            base.OuttroStartNextIntro();
            Platform.SaveStore();
        }

        protected override void OuttroFinished()
        {
            /* set equips */
            for (int i = 0; i < DataStorage.GunEquipped.Length; ++i) { DataStorage.GunEquipped[i] = false; }
            for (int i = 0; i < equipIndex.Length; ++i) { DataStorage.GunEquipped[equipIndex[i]] = true; }

            /* CloseItemOptions(); */
            optionsBoard.gameObject.SetActive(false);
            for (int i = 0; i < itemButtons.Length; ++i) { itemButtons[i].interactable = true; }
            menu.SetFocus(lastItemSelected);

            base.OuttroFinished();
        }

        public override void CancelPressed()
        {
            if (optionsBoard.gameObject.activeInHierarchy) { CloseItemOptions(); }
            else { base.CancelPressed(); }
        }

        public void StoreItemInfo(OnClickStoreItem onclick)
        {
            selector.gameObject.SetActive(true);
            selector.transform.SetXY(onclick.gameObject.transform.GetXY());
            selector.Play(onclick.item > StoreItem.StoreWave ? HashIDs.menu.Outtro : HashIDs.menu.Intro);

            SetItemInfo(onclick.item);
        }

        public void StoreItemSelected(OnClickStoreItem onclick)
        {
            if (onclick.item == StoreItem.StoreArmor && DataStorage.Hearts < heartsBeforeArmor) { return; }

            currentItem = onclick.item;

            optionsBoard.gameObject.SetActive(true);
            optionsBoard.SetX(onclick.gameObject.transform.GetX() +
                              (currentItem > StoreItem.StoreWave ? -500 : 500));

            ShowItemButtonOptions((int)currentItem);

            lastItemSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            menu.SetFocus(optionsBoard.Button());

            for (int i = 0; i < itemButtons.Length; ++i) { itemButtons[i].interactable = false; }
        }

        void SetItemInfo(StoreItem item)
        {
            int item_index = (int)item;

            itemTitle.text = GText.item_title[item_index];

            menu.builder.Length = 0;
            menu.builder.Append(GText.item_info[item_index]);

            switch (item)
            {
            case StoreItem.StoreHeart: {
                menu.builder.AppendFormat("\n{0}: {1}\n{2}: {3}",
                                          GText.hearts, DataStorage.Hearts,
                                          GText.healing, DataStorage.Healing);
            } break;
            case StoreItem.StoreArmor: {
                if (DataStorage.Hearts >= heartsBeforeArmor) {
                    menu.builder.AppendFormat("\n{0}: {1}", GText.armor, DataStorage.Armor);
                }
                else {
                    menu.builder.AppendFormat("\n<color=#F97797FF>{0}</color>", GText.more_hearts);
                }
            } break;
            default: {
                if (DataStorage.GunOwned[item_index]) {
                    menu.builder.AppendFormat("\n{0}: {1}", GText.level, DataStorage.GunPower[item_index]);
                }
                else {
                    menu.builder.AppendFormat("\n<color=#FFF192FF>{0}</color>", GText.purchase_to_unlock);
                }
            } break;
            }

            itemInfo.text = menu.builder.ToString();
        }

        void ShowItemButtonOptions(int index)
        {
            current_selection = index;

            optionsBoard.DisableButtons();

            if (index >= DataStorage.GunOwned.Length) { /* hearts, healing, and armor */
                if (index == DataStorage.GunOwned.Length) {  /* heart selected */
                    int button_index = 0;
                    if (DataStorage.Hearts < heartsBeforeArmor) {
                        optionsBoard.SetButton(button_index++, StoreOption.StoreOptionAddHeart);
                    }
                    optionsBoard.SetButton(button_index++, StoreOption.StoreOptionAddHeadling);

                    if (DataStorage.Hearts > 2 && DataStorage.Armor == 0) {
                        optionsBoard.SetButton(button_index++, StoreOption.StoreOptionRefundHeart);
                    }
                    if (DataStorage.Healing > 1) {
                        optionsBoard.SetButton(button_index++, StoreOption.StoreOptionRefundHealing);
                    }
                }
                else if (DataStorage.Hearts >= heartsBeforeArmor) {
                    optionsBoard.SetButton(0, StoreOption.StoreOptionArmor);
                    if (DataStorage.Armor > 0) {
                        optionsBoard.SetButton(1, StoreOption.StoreOptionRefundArmor);
                    }
                }
            }
            else {
                /* if it hasn't been purchased yet */
                if (!DataStorage.GunOwned[index]) {
                    optionsBoard.SetButton(0, StoreOption.StoreOptionPurchase);
                }
                else {
                    bool isEquipped = false;
                    for (int i = 0; i < equipIndex.Length; ++i) {
                        if (equipIndex[i] != index) continue;
                        isEquipped = true;
                        break;
                    }

                    int button_index = 0;
                    if (!isEquipped) {
                        optionsBoard.SetButton(button_index++, IsSwap(index) ?
                                               StoreOption.StoreOptionSwap : StoreOption.StoreOptionEquip);
                       
                    }
                    optionsBoard.SetButton(button_index++, StoreOption.StoreOptionUpgrade);
                    if (DataStorage.GunPower[index] > 1) {
                        optionsBoard.SetButton(button_index++, StoreOption.StoreOptionRefund);
                    }
                }
            }
        }

        void SetEquips()
        {
            for (int i = 0; i < equipIndex.Length; ++i) {
                equips[i].sprite = buttonImages[equipIndex[i]].sprite;
            }
        }

        void SetItemStatus()
        {
            /* check if the item is still locked. */
            int offensiveIndex = DataStorage.GunOwned.Length;
            for (int i = 0; i < offensiveIndex; ++i) {
                buttonImages[i].color = DataStorage.GunOwned[i] ? Color.white : lockedColor;
            }

            buttonImages[buttonImages.Length - 1].color = DataStorage.Hearts < heartsBeforeArmor ? lockedColor : Color.white;
        }

        void SetMoneyCounter()
        {
            menu.builder.Length = 0;
            menu.builder.AppendFormat("${0}", DataStorage.Money > 999999999 ? 999999999 : DataStorage.Money);
            moneyText.text = menu.builder.ToString();
        }

        bool IsSwap(int item_index)
        {
            int swap_index = item_index >= DataStorage.NumberOfGuns / 2 ?
                             item_index - DataStorage.NumberOfGuns / 2 :
                             item_index + DataStorage.NumberOfGuns / 2;
            for (int n = 0; n < equipIndex.Length; ++n) {
                if (equipIndex[n] != swap_index) continue;
                return true;
            }

            return false;
        }
    }
}