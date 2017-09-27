namespace Gunhouse
{
    public class MenuText
    {
        public const string PlayGunhouse = " Play Gunhouse!";
        public static string ContinueOn = "Continue on\n";
        public const string StartOnEarlierDay = " Start on\nearlier day";
        public const string HardcoreMode = " Hardcore Mode";
        public const string Shop = " Shop";
        public const string Options = "Options";
        public const string Stats = "Stats";

        /* continue on day */
        public const string PickADay = "Pick a day";

        /* stats */
        public const string StatsTitle = "Stats!";
        public const string BestHardcoreScore = "Best Hardcore Scores:";

        /* exit menu */
        public const string Exit = "Exit?";
        public const string Yes = "Yes";
        public const string No = "No";

        public static string Hearts = "Hearts: ";
        public static string HealingLevel = "\nHealing level: ";
        public static string ArmorLevel = "\nArmor level: ";
        public static string Cost = "\nCost: ";
        public static string EquippedLevel = "\nEquipped - Level: ";
        public static string OwnedLevel = "\nOwned - Level: ";
        public static string MoveX = "\n\nMove the red X to the gun you want\nto unequip, then select \"swap.\"";

#if CONTROLLER_AND_TOUCH || CONTROLLER

        public const string ReturnToTitle = "  Return\n to Title";
        public const string Start = "  Start";

        /* shop */
        public static string Purchase = "  Purchase\n -$";      // X
        public static string Upgrade = "  Upgrade\n-$";         // X
        public static string AddHeart = "  Add heart\n -$";     // X // &#x25AB;
        public static string AddArmor = "  Add armor\n -$";     // X // &#x25B5; // &#x25B3;
        public static string Refund = "  Refund Last\n   Upgrade\n  +$";
        public static string AddHealing = "  Add healing\n  -$";
        public const string Equip = "  Equip";
        public const string Swap = "  Swap";
        public const string Cancel = "  Cancel";

        /* options */
        public static string WatchCredits = "  Watch the\nCredits +$100";
#elif TOUCH
        public const string ReturnToTitle = " Return\nto Title";
        public const string Start = "Start";

        /* options */
        public static string WatchCredits = "  Watch the\nCredits +$100";

        /* shop */
        public static string Purchase = "Purchase\n -$"; // X
        public static string Upgrade = "Upgrade\n-$"; // X
        public static string AddHeart = "Add heart\n -$"; // X // &#x25AB;
        public static string AddArmor = "Add armor\n -$"; // X // &#x25B5; // &#x25B3;
        public static string Refund = "Refund Last\n  Upgrade\n  +$";
        public static string AddHealing = "Add healing\n  -$";
        public const string Equip = "Equip";
        public const string Swap = "Swap";
        public const string Cancel = "Cancel";
#else
        public const string ReturnToTitle = "B: Return\n to Title";
        public const string Start = "A: Start";

        /* options */
        public static string WatchCredits = "Y: Watch the\nCredits +$100";

        /* shop */
        public static string Purchase = "A: Purchase\n -$"; // X
        public static string Upgrade = "A: Upgrade\n-$"; // X
        public static string AddHeart = "A: Add heart\n -$"; // X // &#x25AB;
        public static string AddArmor = "A: Add armor\n -$"; // X // &#x25B5; // &#x25B3;
        public static string Refund = "Y: Refund Last\n   Upgrade\n  +$";
        public static string AddHealing = "Y: Add healing\n  -$";
        public const string Equip = "Y: Equip";
        public const string Swap = "A: Swap";
        public const string Cancel = "Y: Cancel";
#endif
    }
}