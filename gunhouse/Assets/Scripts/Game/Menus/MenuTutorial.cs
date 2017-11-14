using Necrosoft.ThirdParty;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gunhouse
{
    public class MenuTutorial : MonoBehaviour 
    {
        [SerializeField] Canvas canvas;
        [SerializeField] CanvasGroup group;
        [SerializeField] RectTransform background;
        [SerializeField] RectTransform textArea;
        [SerializeField] TutorialCursor cursor;
        [SerializeField] RectTransform textBorder;
        [SerializeField] TextMeshProUGUI tutorialText;
        [SerializeField] Animator skulls;

        [Space(10)] [SerializeField] Canvas switchMenu;

        [System.NonSerialized] public int repeatAmount = 2;
        [System.NonSerialized] public int blocksCreated;
        [System.NonSerialized] public int loadedGunAmount;
        [System.NonSerialized] public int loadedSpecialAmount;
        [System.NonSerialized] public bool hasFocus;
        [System.NonSerialized] public bool isReady = true;
        [System.NonSerialized] public bool activateSpecialFinished;
        [System.NonSerialized] public int loadMultiplierAmount;

        int previousTextIndex;
        int textIndex;
        LTDescr leanDelay;
        bool specialStarted;
        bool frameDelay;
        int frameCounter;
        bool isPaused;
        bool showingControllerMap;

        Lesson lessonIndex = Lesson.NONE;
        public bool MakeBlocks { get { return lessonIndex == Lesson.MAKE_BLOCKS; } }
        public bool LoadedGun { get { return lessonIndex == Lesson.LOAD_GUNS; } }
        public bool LoadedSpecial { get { return lessonIndex == Lesson.LOAD_SPECIALS; } }
        public bool Countdown { get { return (int)lessonIndex >= (int)Lesson.COUNTDOWN && lessonIndex != Lesson.DOOR_OPENS; } }
        public bool ActivatedGun { get { return lessonIndex == Lesson.ACTIVATE_GUNS; } }
        public bool ActivatedSpecial { get { return lessonIndex == Lesson.ACTIVATE_SPECIALS; } }
        public bool DoorOpen { get { return lessonIndex == Lesson.DOOR_OPENS; } }
        public bool FreezeDoor { get { return lessonIndex <= Lesson.MAKE_BLOCKS ||
                                              lessonIndex > Lesson.ACTIVATE_SPECIALS ||
                                              (lessonIndex == Lesson.ACTIVATE_SPECIALS && activateSpecialFinished); } }
        void Awake()
        {
            canvas.gameObject.SetActive(false);
        }

        void Update()
        {
            #if UNITY_SWITCH
            if (Input.Pad.Submit.WasPressed && switchMenu.gameObject.activeSelf) {
                HideSwitchControls();
            }
            #endif

            if (lessonIndex >= Lesson.DONE || isPaused) { return; }

            if (frameDelay) {
                if (frameCounter++ > 10) {
                    frameDelay = !frameDelay;
                    frameCounter = 0;
                }
                return;
            }

            if (Input.Pad.Submit.WasPressed) { UpdateLesson(); }
        }

        void UpdateTextBox(bool displaySkull = true)
        {
            skulls.gameObject.SetActive(displaySkull);
        }

        #region Public

        public void SetDisplay(bool display)
        {
            isPaused = false;

            if (display) {
                hasFocus = true;
                Tracker.TutorialStart();
                #if UNITY_SWITCH
                showingControllerMap = false;
                #else
                lessonIndex = Lesson.START;
                background.gameObject.SetActive(false);
                cursor.gameObject.SetActive(false);
                #endif
            }
            else {
                hasFocus = false;
                isReady = true;
                lessonIndex = Lesson.NONE;
                canvas.gameObject.SetActive(false);
            }
        }

        public void UpdateLesson()
        {
            if (lessonIndex >= Lesson.DONE || lessonIndex == Lesson.START || isPaused) { return; }

            /* NOTE(shane): this is to stop a bug being caused by clicking the gun and button in the same frame. */
            if (lessonIndex == Lesson.ACTIVATE_SPECIALS && !specialStarted) { return; }

            textIndex += textIndex < lessonText[((int)lessonIndex - 1)].Length -1 ? 1 : 0;
            textArea.gameObject.SetActive(true);
            tutorialText.text = lessonText[((int)lessonIndex - 1)][textIndex];

            bool displaySkull = true;

            if (textIndex == lessonText[((int)lessonIndex - 1)].Length -1) {
                switch(lessonIndex)
                {
                    case Lesson.MAKE_BLOCKS: {
                        background.gameObject.SetActive(false);
                        hasFocus = false;
                        cursor.gameObject.SetActive(true);
                        cursor.Play(CursorAnimation.LeftRight);
                        displaySkull = false;
                    } break;
                    case Lesson.LOAD_GUNS: {
                        background.gameObject.SetActive(false);
                        hasFocus = false;
                        cursor.gameObject.SetActive(true);
                        cursor.Play(CursorAnimation.Left);
                        displaySkull = false;
                    } break;
                    case Lesson.LOAD_SPECIALS: {
                        background.gameObject.SetActive(false);
                        hasFocus = false;
                        cursor.gameObject.SetActive(true);
                        cursor.Play(CursorAnimation.Right);
                        displaySkull = false;
                    } break;
                    case Lesson.COUNTDOWN:
                    case Lesson.ACTIVATE_GUNS:
                    case Lesson.ACTIVATE_SPECIALS: {
                        if (textIndex == previousTextIndex) {
                            hasFocus = false;
                            textArea.gameObject.SetActive(false);
                            background.gameObject.SetActive(false);
                            cursor.gameObject.SetActive(false);
                        }
                    } break;
                    case Lesson.DOOR_OPENS: {
                        background.gameObject.SetActive(false);
                        hasFocus = false;
                        cursor.gameObject.SetActive(false);
                        displaySkull = false;
                    } break;
                    case Lesson.GOODWILL: {
                        if (textIndex == previousTextIndex) {
                            hasFocus = false;
                            textArea.gameObject.SetActive(false);
                            background.gameObject.SetActive(false);
                            cursor.gameObject.SetActive(false);
                            Tracker.TutorialEnd();
                            lessonIndex = Lesson.DONE;
                        }
                    } break;
                }
            }

            UpdateTextBox(displaySkull);

            previousTextIndex = textIndex;
        }

        public void SetLesson(Lesson lesson)
        {
            #if UNITY_SWITCH
            if (!showingControllerMap && hasFocus) {
                switchMenu.gameObject.SetActive(true);
                showingControllerMap = true;
            }
            #endif

            if (lesson >= Lesson.DONE || lessonIndex >= lesson) return;

            lessonIndex = lesson;
            textIndex = 0;

            canvas.gameObject.SetActive(true);
            group.alpha = 1;

            switch(lessonIndex)
            {
                case Lesson.MAKE_BLOCKS:
                case Lesson.LOAD_GUNS:
                case Lesson.LOAD_SPECIALS:
                case Lesson.DOOR_OPENS: {
                    hasFocus = true;
                    background.gameObject.SetActive(true);
                    cursor.gameObject.SetActive(false);
                    textArea.gameObject.SetActive(true);
                } break;
                case Lesson.COUNTDOWN:
                case Lesson.ACTIVATE_GUNS:
                case Lesson.GOODWILL: {
                    frameDelay = true;
                    hasFocus = true;
                    background.gameObject.SetActive(true);
                    textArea.gameObject.SetActive(true);
                } break;
                case Lesson.ACTIVATE_SPECIALS: {
                    isReady = false;
                    specialStarted = false;
                    leanDelay = LeanTween.delayedCall(4, ()=> {
                        isReady = true;
                        hasFocus = true;
                        activateSpecialFinished = false;
                        specialStarted = true;
                        background.gameObject.SetActive(true);
                        textArea.gameObject.SetActive(true);
                        cursor.gameObject.SetActive(true);
                        cursor.Play(CursorAnimation.RightUpDown);
                        Game.instance.puzzle.selected_weapon.x = 0;
                        tutorialText.text = lessonText[((int)lessonIndex - 1)][textIndex];
                        UpdateTextBox();
                    });
                } break;
            }

            switch(lessonIndex)
            {
                case Lesson.MAKE_BLOCKS: { blocksCreated = 0; } break;
                case Lesson.LOAD_GUNS: {
                    loadedGunAmount = 0;
                    cursor.gameObject.SetActive(true);
                    cursor.Play(CursorAnimation.Left);
                } break;
                case Lesson.LOAD_SPECIALS: {
                    loadedSpecialAmount = 0;
                    cursor.gameObject.SetActive(true);
                    cursor.Play(CursorAnimation.Right);
                } break;
                case Lesson.COUNTDOWN: { cursor.gameObject.SetActive(false); } break;
                case Lesson.DOOR_OPENS: { loadMultiplierAmount = 0; } break;
                case Lesson.ACTIVATE_GUNS: {
                    isReady = true;
                    Game.instance.puzzle.selected_weapon.x = 1;
                    cursor.gameObject.SetActive(true);
                    cursor.Play(CursorAnimation.LeftUpDown);
                } break;
            }

            tutorialText.text = lessonText[((int)lessonIndex - 1)][textIndex];
            UpdateTextBox();
            previousTextIndex = textIndex;
        }

        public void Pause(bool pausing)
        {
            if (lessonIndex >= Lesson.DONE || lessonIndex == Lesson.START) return;

            isPaused = pausing;
            group.alpha = pausing ? 0 : 1;

            if (lessonIndex == Lesson.ACTIVATE_SPECIALS && !specialStarted) {
                if (pausing) { leanDelay.pause(); }
                else { leanDelay.resume(); }
            }
        }

        public void HideSwitchControls()
        {

            #if UNITY_SWITCH
            switchMenu.gameObject.SetActive(false);
            Tracker.TutorialStart();
            lessonIndex = Lesson.START;
            isPaused = false;
            background.gameObject.SetActive(false);
            cursor.gameObject.SetActive(false);
            #endif
        }

        #endregion

        static string[][] lessonText = {
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
    }

    public enum Lesson { START, MAKE_BLOCKS, LOAD_GUNS, LOAD_SPECIALS,
                         COUNTDOWN, ACTIVATE_GUNS, ACTIVATE_SPECIALS,
                         DOOR_OPENS, GOODWILL, DONE, NONE };
}