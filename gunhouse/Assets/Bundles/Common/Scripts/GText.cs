namespace Gunhouse
{
    public static class GText
    {
        public static string best_hardcore_scores = "BEST HARDCORE SCORES";
        public static string blocks_loaded = "BLOCKS LOADED";
        public static string info = "INFO";
        public static string day = "DAY";
        public static string favourite_gun = "Favorite Gun";
        public static string shots_fired = "Shots Fired";
        public static string times_defeated = "Times Defeated";
        public static string best_match_streak = "Best Match Streak";

        public static string[] item_title = {
            "DRAGON GUN", "PENGUIN GUN", "SKULL GUN", "VEGETABLE GUN", "LIGHTNING GUN",
            "FLAME GUN", "FORK GUN", "BEACH BALL GUN", "BOOMERANG GUN", "SINE WAVE GUN",
            "HEART", "ARMOR"
        };

        public static string[] item_info = {
            "UPGRADE TO A DRAGON FRIEND!",
            "YOUR ENEMIES WILL CHILL OUT.",
            "THESE GUYS ARE CHAMPING AT THE BIT.",
            "VEGGIES MAKE YOU A STRAIGHT SHOOTER.",
            "TESLA-STYLE CHAIN ATTACK.",
            "THE DRAGON GUN'S ENTHUSIASTIC COUSIN.",
            "DEFENSIVE, WITH A SPLIT PERSONALITY.",
            "BOUNCY, BOUNCY, BOUNCY!",
            "THERE AND BACK AGAIN.",
            "SINE ON THE LINE WHICH IS DOTTED!",
            "HEART INFO.", "ARMOR INFO."
        };

        public static string level = "LEVEL";
        public static string hearts = "HEARTS";
        public static string healing = "HEALING";
        public static string armor = "ARMOR";
        public static string more_hearts = "YOU NEED MORE HEARTS FIRST!";
        public static string purchase_to_unlock = "PURCHASE TO UNLOCK!";

        public static string equip = "EQUIP";
        public static string purchase = "PURCHASE";
        public static string upgrade = "UPGRADE";
        public static string refund = "REFUND";
        public static string add_heart = "ADD HEART";
        public static string add_healing = "ADD HEALING";
        public static string add_armor = "ADD ARMOR";
        public static string refund_heart = "REFUND HEART";
        public static string refund_healing = "REFUND HEALING";
        public static string refund_armor = "REFUND ARMOR";
        public static string slot = "SLOT";

        public static string[] help_titles = { "HOT TIPS!", "PUZZLE TIPS!", "TOWER DEFENSE TIPS!", "WHAT'S HARDCORE MODE?" };
        public static string[] help = {
            "We've got everything you ever wanted to know about getting guns and blasting baddies. Let's start by learning how to work that puzzle system!",

            "Gunhouse is all about making smaller block pieces into bigger blocks using horizontal sliding and gravity. You load these blocks into your house as ammo. Check the following diagram for an example!",
            "Here's a straight line of same-type block pieces. In the puzzle map it'll be surrounded by other blocks, but we've isolated it to show you something!",
            "If we move this piece over to the right one space, the two above will fall.",
            "Now move the bottom piece one space to the right, and watch them fall again.",
            "Our old pal gravity turns these four block pieces into a square!",
            "Voila! Big block city!! Load that left or right to make a gun or a special!",
            "As a protip: Look for tetrominoes, they're usually ripe for turning into blocks! Now let's read on for some tips on tower defense.",

            "In the tower defense phase, you should try to fire your weapons when enemies are in range, and place them at specific heights, taking into consideration their firing arcs.",
            "For example, if there are a lot of flying enemies, you may need to place your guns higher, or use something with auto-aim, like the Penguin or Flame guns.",
            "With weapons that have a delay, like the Skull gun, you may want to fire them first, since they take longer to detonate. Find your own favorite weapon combo! Now read on to learn about hardcore mode.",

            "In this mode you must play from the very beginning with the original gun set: Skull, Penguin, and Dragon. There are no upgrades allowed, and you must play as far as you can in one sitting.",
            "If you go back to the main menu, it's game over! Can you beat day 5 in one go!? It's tough! Even the devs haven't beaten day 7…",
            "That's all our tips!! Thanks for playing! Have a real fun and cool time!"
        };

        public static string[][] tutorial = {
            new string[] { "Welcome to Gunhouse! We're gonna show you how to gather ammo to load guns and defend your house from jerks!",
                           "First, you've got to take all these tiny block pieces and turn them into BIG blocks. Big blocks are ammo!",
                           "To make big blocks, drag one, two, or three rows of block pieces left or right, combining the pieces that fall.",
            #if UNITY_SWITCH
                           "Either touch and drag, or press the Left Stick or Directional Buttons left and right, hitting the Confirm Button to bank the pieces you'd like to move.",
                           "Try it for yourself! Make three big blocks! You can move one, two, or three block pieces at a time." },
            new string[] { "Good stuff. Now let's load that ammo into your guns!",
                           "Slide big blocks LEFT to load guns. Either drag blocks to the left and release, or press left on the Left Stick or Directional Buttons, then hit Confirm.",
                           "Try loading three big blocks to the LEFT as ammo for your guns." },
            new string[] { "Hooray! Next we'll talk about special attacks, which complement your guns.",
                           "Slide big blocks RIGHT to load specials. Either drag blocks right with touch, or press right on the Left Stick or Directional Buttons, and hit Confirm.",
                           "Try loading three big blocks to the RIGHT as special ammo!" },
            new string[] { "Well done! Keep in mind you have limited time to add ammo, which you see on top of the house.",
                           "Keep adding guns and specials until the ammo door closes! Usually the timer is 18 seconds, but we've doubled it for now." },
            new string[] { "Okay, it's time to defend your house, so let's shoot some guns! Each gun type has its own properties.",
                           "Either tap any of the guns on the left side of the house, or select a gun with the Left Stick or Directional buttons, and hit Confirm to activate." },
            new string[] { "Let's use those specials now! Specials usually affect a wide area.",
                           "Either tap any of your specials on the right side of the house, or select a special with the Left Stick or Directional Buttons, then hit Confirm." },
            #elif CONTROLLER_AND_TOUCH
                           "Either touch and drag, or press the left or right buttons, hitting X to confirm the pieces you'd like to move.",
                           "Try it for yourself! Make three big blocks! You can move one, two, or three block pieces at a time." },
            new string[] { "Good stuff. Now let's load that ammo into your guns!",
                           "Slide big blocks LEFT to load guns. Either drag blocks to the left and release, or press the left button, then hit X to confirm!",
                           "Try loading three big blocks to the LEFT as ammo for your guns." },
            new string[] { "Hooray! Next we'll talk about special attacks, which complement your guns.",
                           "Slide big blocks RIGHT to load specials. Either drag blocks right with touch, or press the right button, and hit X.",
                           "Try loading three big blocks to the RIGHT as special ammo!" },
            new string[] { "Well done! Keep in mind you have limited time to add ammo, which you see on top of the house.",
                           "Keep adding guns and specials until the ammo door closes! Usually the timer is 18 seconds, but we've doubled it for now." },
            new string[] { "Okay, it's time to defend your house, so let's shoot some guns! Each gun type has its own properties.",
                           "Either tap any of the guns on the left side of the house, or select a gun with the directional buttons, and hit X to activate." },
            new string[] { "Let's use those specials now! Specials usually affect a wide area.",
                           "Either tap any of your specials on the right side of the house, or select a special with the directional buttons and hit X." },
            #elif CONTROLLER
                           "Hit the left button and right button, hitting X to confirm the pieces you'd like to move.",
                           "Try it for yourself! Make three big blocks! If existing big blocks get in your way, they move just like block pieces." },
            new string[] { "Nice! Now let's load that ammo into your guns.",
                           "Slide big blocks LEFT to load guns. Try loading three big blocks to the LEFT, as ammo for your guns." },
            new string[] { "Hooray! Next we'll talk about special attacks, which complement your guns.",
                           "Slide big blocks RIGHT to load specials. Try loading three big blocks to the RIGHT as special ammo!" },
            new string[] { "Well done! Just keep in mind you have limited time to add ammo, which you see on top of the house.",
                           "Keep adding guns and specials until the ammo door closes!Usually the timer is 18 seconds, but we've doubled it for now." },
            new string[] { "It's time to defend your house, so let's shoot some guns! Each gun type has its own properties.",
                           "Just tap any of the guns on the left side of the house to activate them. You only have to tap once!" },
            new string[] { "Let's use those specials now! Specials usually affect a wide area.",
                           "Tap the specials on the right side of the house to activate them!" },
            #else // TOUCH
                           "Try it for yourself! Make three big blocks! You can move one, two, or three block pieces at a time." },
            new string[] { "Good stuff. Now let's load that ammo into your guns!",
                           "Slide big blocks LEFT to load guns. New guns replace old ones, or add more of the same ammo type to existing guns.",
                           "Try loading three big blocks to the LEFT as ammo for your guns." },
            new string[] { "Hooray! Next we'll talk about special attacks, which complement your guns.",
                           "Slide big blocks RIGHT to load specials. Try loading three big blocks to the RIGHT as special ammo!",
                           "Try loading three big blocks to the RIGHT as special ammo!" },
            new string[] { "Well done! Keep in mind you have limited time to add ammo, which you see on top of the house.",
                           "Keep adding guns and specials until the ammo door closes! Usually the timer is 18 seconds, but we've doubled it for now." },
            new string[] { "Okay, it's time to defend your house, so let's shoot some guns! Each gun type has its own properties.",
                           "Tap any of the guns on the left side of the house to fire them." },
            new string[] { "Let's use those specials now! Specials usually affect a wide area.",
                           "Tap any of your specials on the right side of the house, and off they'll go." },
            #endif
            new string[] { "The ammo door opens automatically after your attacks end.",
                           "For extra damage, load a block matching the block type that's pulsing above the house.",
                           "Try to make three bonus guns or specials by matching the pulsing block type!" },
            new string[] { "And now for some bad news. When enemies attack your house or steal your orphans, your heart meter empties.",
                           "But!! When you defeat enemies, you regain some hearts, and get money to use in the store!",
                           "Just keep at it and you'll do great! Go get 'em!" }
        };

        public static class Objectives
        {
            public static string defeat_enemies_with_gun = "Defeat {1} enemies with the {2} Gun ({0}/{1})";
            public static string upgrade_gun_to_level = "Upgrade {0} Gun to level {1}";
            public static string upgrade_hearts_to_level = "Upgrade Hearts to level {0}";
            public static string upgrade_healing_to_level = "Upgrade Healing to level {0}";
            public static string upgrade_armor_to_level = "Upgrade Armor to level {0}";
            public static string make_x_number_sized_blocks = "Make {1} blocks of {2}x{3} size ({0}/{1})";
            public static string load_gun_with_x_sized_blocks = "Load {1} Guns with blocks of {2}x{3} size ({0}/{1})";
            public static string load_special_with_x_sized_blocks = "Load {1} Specials with blocks of {2}x{3} size ({0}/{1})";
            public static string have_x_amount_in_the_bank = "Have ${0} in the bank";
            public static string send_out_three_guns = "Fire {0} Gun {1} times in one turn";
            public static string send_out_three_specials = "Fire {0} Special {1} times in one turn";

            public static string match_bonus_element = "Match the bonus element {1} times in a row ({0}/{1})";
            public static string defeat_x_number_of_bosses = "Defeat {0} bosses! {1}/{0}";
            public static string reach_x_day = "Reach day {0}";
            public static string beat_a_stage_using = "Beat a wave using {0}, {1}, and {2} weapons";
            public static string see_the_ending = "See the ending";

            public static string[] weapon_names = {
                "Dragon", "Penguin" , "Skull", "Vegetable", "Lightning",
                "Flame", "Fork", "Beach Ball", "Boomerang", "Sine Wave" };

            public static string[] free_tasks = {
                "You got an easy day", "We think you are pretty good", "Have a nice day",
                "Be generally pretty cool", "Play Gunhouse", "Divide your frogs" };

            public static string[] disconcerting_tasks = {
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
                "Whoa hey Necro is here, you did all the stuff!" };
        }

        public class Story
        {
            public static string[] tips = {
                "Protip: Match big blocks to the pulsing element to get a power bonus!",
                "Protip: Get health and money by killing enemies.",
                "Protip: New guns are automatically equipped. Make sure to curate your loadout.",
                "Protip: Spend dollar units at the company store. Ask your parents for permission.",
                "Protip: Loading tiny blocks has no effect on your guns, but gets them out of the way.",
                "Protip: The boomerang special attack stays in the lane from which it was fired.",
                "Protip: Loading a new ammo type replaces your existing gun.",
                "Protip: Figure out your favorite guns and only use those forever.",
                "Protip: Call somebody you love today. Tell them I said hi.",
                "Protip: Load guns to the left and special attacks to the right!",
                "Protip: Alien robots are jerks.",
                "Protip: The more a gun jiggles, the more powerful it is!",
                "Protip: Defend your orphans! They are so precious!",
                "Protip: I know where you live.",
                "Protip: Bet you I could beat up a wolverine.",
                "Protip: Look out for tetrominos! They are easy to turn into big blocks.",
                "Protip: Let's be friends forever!",
                "Protip: Necrosoft Games loves you.",
                "Protip: To donate money to help actual orphans, we recommend www.savethechildren.org.",
                "Protip: If you bought this game you are probably very attractive.",
                "Protip: The fork gun's shield special blocks most projectiles, but has trouble with ghosts.",
                "Protip: Beachball bullets bounce back to the height from which they were shot.",
                "Protip: Frogs aren't actually that good at math.",
                "Protip: The laser special not only locks enemies in place, it damages them when it breaks apart.",
                "Protip: Skull gun bullets only do damage when they explode.",
                "Protip: \"You live in a little house made of guns. You need many guns to fight invaders but also need to keep a roof on top of your many children\" ~Peter Molydeux",
                "Protip: Ground enemies stick to a certain path, but flying enemies go wherever they like.",
                "Protip: Hit enemies to make them drop orphans!",
                "Protip: If you're having trouble with a particular wave, remember to upgrade your guns and health!",
                "Protip: If you chew with your mouth open you are just the worst.",
                "Protip: The money collector robot is saving up your cast-off pennies to open a gluten-free bakery."
            };

            public static string[] story = {
                "Caretaker's note: I know not whence these clanging metal men come, nor the whyfors of their appearing, but they have caused the children a great deal of unease. I am not one to suffer fools.",
                "Caretaker's note: And still they persist. Never have there been such carryings-on at my doorstep. They may go with my blessing so long as they never return.",
                "Caretaker's note: Billy lost his first infant's tooth in to-day's commotion. I gently explained to him that a new one would grow back in its place, but he would not be consoled. The other children made sport of him, and were negatively affected by his ill humor. They took to misdeeds throughout the evening.",
                "Caretaker's note: A day's passing has not assuaged the metal howlers at my gate. The children have succumbed to unease, and though I am not inclined to grouse, it is a trying task managing both their emotional states and the affairs outside our walls.",
                "Caretaker's note: I have not the proclivity nor a tolerance for violence. It is why I took up as a caretaker of the young and parentless. But when a body is so thoroughly vexed, as I have been these two days past, to what other path can we turn?",
                "Caretaker's note: It appears we shall have another evening without rest. I shall remain diligent for as long as I am able, but I begin to consider the situation outside the orphanage. With so many aggressors at our doors, what of the rest of the town? Why does no-one come to our aid?",
                "Caretaker's note: As this morning dawned, I knew our situation would be unchanged, and none would come to help. It is up to me. It is a lucky thing our stores are full of provisions, that we may last out several days of isolation. I pray for a swift end to these troubles.",
                "Caretaker's note: The children have taken to naming our foes, singing limericks to mock their horrid visages. Rhyming is the devil's game, but these are desperate times, and I shall let it pass. This world is not so black and white.",
                "Caretaker's note: Through it all, daily chores must be dealt with. Victuals must be prepared, the orphanage's leaky pipes demand a constant tending to, and the beds want making. Hard times shall not remove from us our civility.",
                "Caretaker's note: I have resolved to educate the children in matters of the orphanage's upkeep and defense. Should I be unable to continue my duties, they will have to carry on in my stead. Whatever the case, we shall not waver.",
                "Caretaker's note: Round noontime, little Jillian got the devil in her, and fell ill. I worry she is like to expire. Salves and tinctures can't protect her from the terrible din out-of-doors. It's rest she's needing, above all.",
                "Caretaker's note: Our fair Jillian did not make the night through. Had I the time, had I the supplies, I might have offered her a smoother passing, but she went with a wail and a terrible violence. The others shuffle about under a sombre malaise.",
                "Caretaker's note: The constant repairs and day-to-days of the orphanage are more than I can manage. Evermoreso, the children help where they can. They grow older daily. Would that I could grant them a carefree youth.",
                "Caretaker's note: There are times when I would simply give up. But the children are my strength. I will carry on forever if needs must, for their sake if not for my own. I cannot protect their innocence, but mayhaps I can guard their lives.",
                "Caretaker's note: I miss the sunshine. I can see it from outside our windows, but that is a pale replacement for letting it beam down upon my face, in the meadows. But I mustn't leave the orphanage. I mustn't.",
                "Caretaker's note: Some of the children have taken to wearing the crockery on their heads as helmets. I remind them that we still need to cook on occasion, but they seem to be taking our conflict more seriously than their dietary needs.",
                "Caretaker's note: Watching my young charges at their posts recalls me bittersweetly to the days when they would roll about with their toys, and it was only scuffed knees or bruised feelings that elicited their tears. The challenges of to-day are far more grave.",
                "Caretaker's note: The children grow accustomed to these skirmishes, alarmingly so. I admit, there is little time to feel sorry for ourselves. We must battle on. I fear for their future.",
                "Caretaker's note: These last events rocked the children's toychest for a powerful spell. Though I bound it with twine, it did split apart into many pieces. Scattered as they were, I could not help but consider the many children we have missed to the inexplicable happenings of these dark days. Well may they rest.",
                "Caretaker's note: The long hours harry me. I started with a fright when our Timothy tried to hold my hand as I went about my duties. It frightened him so, and he would not be quieted until I gave him a biscuit as a calmative. I do confess it was the last of them.",
                "Caretaker's note: Though it has been ages since I touched the stuff, I happened upon some brandy down in the cellar as I ventured deeper into our provisions. Just a sip to calm the nerves. I haven't the stomach for more.",
                "Caretaker's note: I took ken to it the night prior, but feared to write it down, making it real. Our stores begin to run low. Though we have rationed carefully, there are too many mouths to feed. Too many are working too hard to go without nourishment. Actions must be taken.",
                "Caretaker's note: We have been isolated for so long now. We have heard nothing from the outside world, and I fear we may be all that is left of it. I must get word from the village, and we must have more supplies. I will broach the subject after our sparse meal.",
                "Caretaker's note: As agreed, I stole away in secret whilst the children kept the metal men at bay. I have divined that those we now face are only the vanguard of a much larger syndicate. I hear tell of cities falling, and it is the children they are after. The children!",
                "Caretaker's note: During last evening's sojourn, I managed to scrounge a small variety of provisions, with which to supplement our dwindling stores. This morn I distributed a parcel of sweet biscuits among my hungry charges. Such smiles they boasted!",
                "Caretaker's note: To-day I fell asleep for a spell as I kept watch. I woke with a start, worried something might have befallen the children. In truth, they had taken over my post, and put a blanket round my shoulders besides. The silly dears.",
                "Caretaker's note: From beyond the reaches of the village rubble, I can hear klaxons sounding. Someone is warning us. But who, and of what? What horrors could be worse than those we have already faced? Why this terrible din?",
                "Caretaker's note: Something stirs, out there, amid the ruins of our once-populous village. In the furthest reaches of our vision. Something massive. It comes for My Innocent Little Orphans' souls. It shall not have them. Where others have fallen, we shall resist.",
                "Caretaker's note: It feeds on us - on the children. I see that now. With each precious stolen life it grows stronger. I know not the means of stopping it. What could do this, if not a god? Gods be damned, then, and may the devil take them.",
                "Caretaker's note: It was no god. Just another empty metal shell. Our orphanage stands strong, for the time being. But what of others? Who will help them? We, who survive. We shall help. We shall write our own fable. Where evil roams, we shall resist. Sally forth, my children, to victory! ~Genevieve Severine Mercy Goppert"
            };
        }
    }
}