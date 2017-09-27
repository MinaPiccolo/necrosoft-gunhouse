using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        public static int[] activeTasks = new int[3];

        string[] WeaponNames = new string[] { "Dragon", "Penguin" , "Skull", "Vegetable", "Lightning",
                                              "Flame", "Fork", "Beach Ball", "Boomerang", "Sine Wave" };

        static TaskType[] possibleTasks = {
            TaskType.DEFEAT_ENEMIES_WITH_GUN, TaskType.UPGRADE_GUN_TO_LEVEL, TaskType.UPGRADE_HEALING_TO_LEVEL, 
            TaskType.UPGRADE_ARMOR_TO_LEVEL, TaskType.MAKE_X_NUMBER_SIZED_BLOCKS, TaskType.LOAD_GUN_WITH_X_SIZED_BLOCKS,
            TaskType.LOAD_SPECIAL_WITH_X_SIZED_BLOCK, TaskType.HAVE_X_AMOUNT_IN_THE_BANK, TaskType.SEND_OUT_THREE_GUNS,
            TaskType.SEND_OUT_THREE_SPECIALS, TaskType.MATCH_BONUS_ELEMENT_X_TIMES_IN_A_ROW,

/* DAY6 */  TaskType.DEFEAT_X_NUMBER_OF_BOSSES, TaskType.FREE,

/* DAY7 */  TaskType.FREE,

/* DAY8 */  TaskType.REACH_X_DAY, TaskType.BEAT_A_STAGE_USING_XYZ_WEAPONS, TaskType.SEE_THE_ENDING,

/* DAY9*/   TaskType.FREE, TaskType.FREE,
        };

        enum TaskType { DEFEAT_ENEMIES_WITH_GUN, UPGRADE_GUN_TO_LEVEL, UPGRADE_HEALING_TO_LEVEL, UPGRADE_ARMOR_TO_LEVEL,
            MAKE_X_NUMBER_SIZED_BLOCKS, LOAD_GUN_WITH_X_SIZED_BLOCKS, LOAD_SPECIAL_WITH_X_SIZED_BLOCK,
            HAVE_X_AMOUNT_IN_THE_BANK, SEND_OUT_THREE_GUNS, SEND_OUT_THREE_SPECIALS,

            DEFEAT_X_NUMBER_OF_BOSSES, REACH_X_DAY, BEAT_A_STAGE_USING_XYZ_WEAPONS, SEE_THE_ENDING, DIVIDE_YOUR_FROGS,
            MATCH_BONUS_ELEMENT_X_TIMES_IN_A_ROW, FREE
        }

        string[] freeTasks = new string[] { "You got an easy day", "We think you are pretty good", "Have a nice day",
                                            "Be generally pretty cool", "Play Gunhouse", "Divide your frogs" };
        string[] disconcertingTasks = new string[] {
            "Discover the ancient texts.",
            "Learn of The Ritual. It can be completed.",
            "Meet The Scrivener. Pay the fee.",
            "Acquire the Fourth Talisman.",
            "Map the stars, find what's hidden.",
            "The gem reflects the South star.",
            "Find the temple of The Forgotten.",
            "Unearth the bones, cover them in lye.",
            "Gather the frankincense, myrrh, sulfur.",
            "Collect the wax of the lac beetle.",
            "Braid the rice paper using sub-dominant hand.",
            "Dip, drip, dip, drip. Breath between each.",
            "Fold the linens thirteen times, then four again.",
            "Bring what's yours and theirs to The Aviary.",
            "Spread the circle upon the ground, mist dyed red.",
            "Draw the runes in the way of The Forgotten.",
            "Prepare the candles, facing the waxing moon.",
            "Set the bones, twisted eighteen times.",
            "Enter the circle, backward, heels first.",
            "Summon Necro, destroyer of worlds. Be ye warned.",
            "Whoa hey Necro is here, you did all the stuff!"
        };
    }
}