using UnityEngine;

#if TRACKING

using System.Collections.Generic;
using UnityEngine.Analytics.Experimental;

namespace Gunhouse
{
    public class Tracker : MonoBehaviour
    {
        public static void ScreenVisit(string screen) { AnalyticsEvent.ScreenVisit(screen); }

        public static void TutorialStart() { AnalyticsEvent.TutorialStart(); }
        public static void TutorialEnd() { AnalyticsEvent.TutorialComplete(); }

        public static void AchievementUnlocked(string achievement) { AnalyticsEvent.AchievementUnlocked(achievement); }
        public static void AchievementStep(int step, string achievement) { AnalyticsEvent.AchievementStep(step, achievement); }

        public static void ShopSelectedItem(string item) { AnalyticsEvent.StoreItemClick(StoreType.Soft, item); }
        public static void ShopItemPurchased(string item) { AnalyticsEvent.ItemAcquired(AcquisitionType.Soft, "shop", 1, item); }
        public static void ShopItemUpgrade(string item, int level) { AnalyticsEvent.Custom("item_upgrade", new Dictionary<string, object> { { "item", item }, { "level", level } }); }
        public static void ShopItemDowngrade(string item, int level) { AnalyticsEvent.Custom("item_downgrade", new Dictionary<string, object> { { "item", item }, { "level", level } }); }

        public static void StartMode(string wave, string[] weapons) { StartMode(wave, weapons, 0, true); }
        public static void StartMode(string wave, string[] weapons, int money, bool isHardcore = false)
        {
            AnalyticsEvent.Custom(isHardcore ? "hardcore_start" : "waves_start",
                        new Dictionary<string, object> { { "wave", wave },
                                                         { "money", money },
                                                         { "equip 1", weapons[0] },
                                                         { "equip 2", weapons[1] },
                                                         { "equip 3", weapons[2] } });
        }

        public static void EndMode(bool isHardcore, string wave, int money)
        {
            AnalyticsEvent.Custom(isHardcore ? "hardcore_end" : "waves_end",
                        new Dictionary<string, object> { { "wave", wave },
                                                         { "money", money } });
        }

        public static void LevelStart(int wave) { AnalyticsEvent.LevelStart(wave); }
        public static void LevelQuit(int wave) { AnalyticsEvent.LevelQuit(wave); }
        public static void LevelComplete(int wave) { AnalyticsEvent.LevelComplete(wave); }
        public static void LevelFailed(int wave) { AnalyticsEvent.LevelFail(wave); }
    }

    public static class SCREEN_NAME
    {
        public static string MAIN_MENU = "main_menu";
        public static string EARLIER_DAY = "earlier_day";
        public static string OPTIONS = "options";
        public static string SHOP = "shop";
        public static string STATS = "stats";
        public static string PAUSE = "pause";
        public static string WAVE_COMPLETE = "wave_complete";
        public static string WAVE_FAILED = "wave_failed";
        public static string CREDITS = "credits";
        public static string QUIT_MENU = "quit_menu";
        public static string ACHIEVEMENTS = "achievements_menu";
        public static string SIGN_IN = "achievements_sign_in";
    }
}
#else
namespace Gunhouse
{
    public class Tracker : MonoBehaviour
    {
        public static void ScreenVisit(string screen) { }

        public static void TutorialStart() { }
        public static void TutorialEnd() { }

        public static void AchievementUnlocked(string achievement) { }
        public static void AchievementStep(int step, string achievement) { }

        public static void ShopSelectedItem(string item) { }
        public static void ShopItemPurchased(string item) { }
        public static void ShopItemUpgrade(string item, int level) { }
        public static void ShopItemDowngrade(string item, int level) { }

        public static void StartMode(string wave, string[] weapons) { }
        public static void StartMode(string wave, string[] weapons, int money, bool isHardcore = false) { }
        public static void EndMode(bool isHardcore, string wave, int money) { }

        public static void LevelStart(int wave) { }
        public static void LevelQuit(int wave) { }
        public static void LevelComplete(int wave) { }
        public static void LevelFailed(int wave) { }
    }

    public static class SCREEN_NAME
    {
        public static string MAIN_MENU;
        public static string EARLIER_DAY;
        public static string OPTIONS;
        public static string SHOP;
        public static string STATS;
        public static string PAUSE;
        public static string WAVE_COMPLETE;
        public static string WAVE_FAILED;
        public static string CREDITS;
        public static string ACHIEVEMENTS;
        public static string SIGN_IN;
    }
}
#endif