using UnityEngine;
using System.Text;

namespace Gunhouse.Menu
{
    public class OptionsBoard : MonoBehaviour
    {
        Transform board;
        StringBuilder builder = new StringBuilder(50);
        [SerializeField] OptionButton[] buttons;

        readonly int[] default_price = new int[10] { 0, 0, 0, 1500, 2000, 2500, 3500, 1500, 1000, 2500 };
        const int base_heart_price = 1200;
        const float heart_price_multiplier = 1.5f;
        const int base_armor_price = 3000;
        const float armor_price_multiplier = 1.5f;
        const int base_healing_price = 400;
        const float healing_price_multiplier = 1.5f;
        const int base_upgrade_price = 400;
        const float upgrade_price_multiplier = 1.65f;

        void Awake()
        {
            board = transform.GetChild(0);
        }

        public void SetX(float x)
        {
            board.SetX(x);
        }

        public void DisableButtons()
        {
            for (int i = 0; i < buttons.Length; ++i) { buttons[i].gameObject.SetActive(false); }
        }

        public void SetButton(int index, StoreOption item)
        {
            buttons[index].gameObject.SetActive(true);
            buttons[index].item = item;
            builder.Length = 0;

            buttons[index].Money = GetAmount(item);

            switch (item)
            {
            case StoreOption.StoreOptionSwap:
            case StoreOption.StoreOptionEquip: { builder.Append(GText.equip); } break;
            case StoreOption.StoreOptionPurchase: { builder.AppendFormat("{0} ${1}", GText.purchase, buttons[index].Money); } break;
            case StoreOption.StoreOptionUpgrade: { builder.AppendFormat("{0} ${1}", GText.upgrade, buttons[index].Money); } break;
            case StoreOption.StoreOptionRefund: { builder.AppendFormat("{0} ${1}", GText.refund, buttons[index].Money); } break;
            case StoreOption.StoreOptionAddHeart: { builder.AppendFormat("{0} ${1}", GText.add_heart, buttons[index].Money); } break;
            case StoreOption.StoreOptionAddHeadling: { builder.AppendFormat("{0} ${1}", GText.add_healing, buttons[index].Money); } break;
            case StoreOption.StoreOptionArmor: { builder.AppendFormat("{0} ${1}", GText.add_armor, buttons[index].Money); } break;
            case StoreOption.StoreOptionRefundHeart: { builder.AppendFormat("{0} ${1}", GText.refund_heart, buttons[index].Money); } break;
            case StoreOption.StoreOptionRefundHealing: { builder.AppendFormat("{0} ${1}", GText.refund_healing, buttons[index].Money); } break;
            case StoreOption.StoreOptionRefundArmor: { builder.AppendFormat("{0} ${1}", GText.refund_armor, buttons[index].Money); } break;
            case StoreOption.StoreOptionSlot: { builder.AppendFormat("{0} {1}", GText.slot, index + 1); } break;
            }

            buttons[index].Text = builder.ToString();
        }

        int GetAmount(StoreOption item)
        {
            int index = MenuStore.current_selection;

            switch (item)
            {
            case StoreOption.StoreOptionPurchase: return default_price[index];
            case StoreOption.StoreOptionUpgrade: return (int)(Mathf.Pow(upgrade_price_multiplier, DataStorage.GunPower[index] - 1) * base_upgrade_price);
            case StoreOption.StoreOptionRefund: return (int)(Mathf.Pow(upgrade_price_multiplier, DataStorage.GunPower[index] - 2) * base_upgrade_price);
            case StoreOption.StoreOptionAddHeart: return (int)(Mathf.Pow(heart_price_multiplier, DataStorage.Hearts - 1) * base_heart_price);
            case StoreOption.StoreOptionAddHeadling: return (int)(Mathf.Pow(healing_price_multiplier, DataStorage.Healing)) * base_healing_price;
            case StoreOption.StoreOptionArmor: return (int)(Mathf.Pow(armor_price_multiplier, DataStorage.Armor) * base_armor_price);
            case StoreOption.StoreOptionRefundHeart: return (int)(Mathf.Pow(heart_price_multiplier, DataStorage.Hearts - 2) * base_heart_price);
            case StoreOption.StoreOptionRefundHealing: return (int)(Mathf.Pow(healing_price_multiplier, DataStorage.Healing - 1)) * base_healing_price;
            case StoreOption.StoreOptionRefundArmor: return (int)(Mathf.Pow(armor_price_multiplier, DataStorage.Armor - 1) * base_armor_price);
            }

            return 0;
        }

        public GameObject Button(int index = 0)
        {
            return buttons[index].gameObject;
        }

        public int ButtonIndex(GameObject button)
        {
            for (int i = 0; i < buttons.Length; ++i) {
                if (button == buttons[i].gameObject) return i;
            }
            return 0;
        }
    }
}
