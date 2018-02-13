using UnityEngine;
using TMPro;

namespace Gunhouse.Menu
{
    public class OptionButton : MonoBehaviour
    {
        public StoreOption item;
        TextMeshProUGUI buttonText;
        int amount;
        [Range(0, 3)] public int index;

        public string Text { set { buttonText.text = value; } }
        public int Money { set { amount = value; } get { return amount; } }

        void Awake()
        {
            buttonText = GetComponent<TextMeshProUGUI>();
        }
    }

    public enum StoreOption { None,
                              StoreOptionEquip, StoreOptionSwap,
                              StoreOptionPurchase,
                              StoreOptionUpgrade, StoreOptionRefund,
                              StoreOptionAddHeart, StoreOptionAddHeadling, StoreOptionArmor,
                              StoreOptionRefundHeart, StoreOptionRefundHealing, StoreOptionRefundArmor,
                              StoreOptionSlot };
}
