using UnityEngine;
using System.Collections;

namespace Gunhouse.Menu
{
    public partial class MenuStore : MenuPage
    {
        public void CloseItemOptions()
        {
            optionsBoard.gameObject.SetActive(false);
            for (int i = 0; i < itemButtons.Length; ++i) { itemButtons[i].interactable = true; }

            MainMenu.SetFocus(lastItemSelected);
        }

        public void SelectOption(OptionButton button)
        {
            if (button.Money > DataStorage.Money) { Debug.Log("not enough money"); return; }

            GameObject selectedButton = button.gameObject; 
            StoreOption buttonOption = button.item;

            switch (buttonOption)
            {
                case StoreOption.StoreOptionEquip: {
                    optionsBoard.SetButton(0, StoreOption.StoreOptionSlot);
                    optionsBoard.SetButton(1, StoreOption.StoreOptionSlot);
                    optionsBoard.SetButton(2, StoreOption.StoreOptionSlot);
                } break;
                case StoreOption.StoreOptionSlot:
                case StoreOption.StoreOptionSwap: {
                    menu.ignore_input = true;
                    CloseItemOptions();

                    int equip_index = button.index;
                    int item_index = (int)currentItem;
                    if (buttonOption == StoreOption.StoreOptionSwap) {
                        int swap_index = item_index >= DataStorage.NumberOfGuns / 2 ?
                                         item_index - DataStorage.NumberOfGuns / 2 :
                                         item_index + DataStorage.NumberOfGuns / 2;
                        for (int n = 0; n < equipIndex.Length; ++n) {
                            if (equipIndex[n] != swap_index) continue;
                            equipIndex[n] = item_index;
                            equip_index = n;
                            break;
                        }
                    }
                    else {
                        equipIndex[equip_index] = item_index;
                    }

                    StartCoroutine(IconToEquip(item_index,
                                               buttonImages[item_index].transform.GetXY(),
                                               equips[equip_index].transform.GetXY()));

                } break;
                case StoreOption.StoreOptionPurchase: {
                    DataStorage.GunOwned[(int)currentItem] = true;
                    DataStorage.Money -= button.Money;
                    SetItemStatus();
                    CloseItemOptions();
                } break;
                case StoreOption.StoreOptionUpgrade: {
                    DataStorage.GunPower[(int)currentItem]++;
                    DataStorage.Money -= button.Money;
                    Tracker.ShopItemUpgrade(item_title[(int)currentItem], DataStorage.GunPower[(int)currentItem]);
                } break;
                case StoreOption.StoreOptionRefund: {
                    DataStorage.GunPower[(int)currentItem]--;
                    if (DataStorage.GunPower[(int)currentItem] == 1) { selectedButton = optionsBoard.Button(); }
                    DataStorage.Money += button.Money;
                    Tracker.ShopItemDowngrade(item_title[(int)currentItem], DataStorage.GunPower[(int)currentItem]);
                } break;
                case StoreOption.StoreOptionAddHeart: {
                    DataStorage.Hearts++;
                    DataStorage.Money -= button.Money;
                    SetItemStatus();
                } break;
                case StoreOption.StoreOptionAddHeadling: {
                    DataStorage.Healing++;
                    DataStorage.Money -= button.Money;
                } break;
                case StoreOption.StoreOptionArmor: {
                    DataStorage.Armor++;
                    DataStorage.Money -= button.Money;
                } break;
                case StoreOption.StoreOptionRefundHeart: {
                    DataStorage.Hearts--;
                    if (DataStorage.Hearts == 5) {
                        selectedButton = optionsBoard.Button(optionsBoard.ButtonIndex(selectedButton) + 1);
                    }
                    else if (DataStorage.Hearts == 2) {
                        selectedButton = optionsBoard.Button();
                    }
                    DataStorage.Money += button.Money;
                    SetItemStatus();
                } break;
                case StoreOption.StoreOptionRefundHealing: {
                    DataStorage.Healing--;
                    if (DataStorage.Healing == 1) { selectedButton = optionsBoard.Button(); }
                    DataStorage.Money += button.Money;
                } break;
                case StoreOption.StoreOptionRefundArmor: {
                    DataStorage.Armor--;
                    if (DataStorage.Armor == 0) { selectedButton = optionsBoard.Button(); }
                    DataStorage.Money += button.Money;
                } break;
            }

            if (optionsBoard.gameObject.activeSelf &&
                buttonOption != StoreOption.StoreOptionEquip) {
                ShowItemButtonOptions((int)currentItem);
                MainMenu.SetFocus(selectedButton);
            }

            SetItemInfo(currentItem);
            SetMoneyCounter();
        }

        IEnumerator IconToEquip(int index, Vector2 from, Vector2 to)
        {
            equip_icon.color = Color.white;
            equip_icon.sprite = buttonImages[index].sprite;
            float speed = 2000.0f;

            float step = (speed / (from - to).magnitude) * Time.deltaTime;
            float t = 0;
            while (t <= 1.0f) {
                t += step;
                equip_icon.transform.SetXY(Vector2.Lerp(from, to, t));
                yield return null;
            }

            equip_icon.transform.SetXY(to);
            equip_icon.color = Color.clear;
            SetEquips();
            menu.ignore_input = false;
        }
    }
}