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
                switchMenu.gameObject.SetActive(false);
            }
        }

        public void UpdateLesson()
        {
            if (lessonIndex >= Lesson.DONE || lessonIndex == Lesson.START || isPaused) { return; }

            /* NOTE(shane): this is to stop a bug being caused by clicking the gun and button in the same frame. */
            if (lessonIndex == Lesson.ACTIVATE_SPECIALS && !specialStarted) { return; }

            textIndex += textIndex < GText.tutorial[((int)lessonIndex - 1)].Length -1 ? 1 : 0;
            textArea.gameObject.SetActive(true);
            tutorialText.text = GText.tutorial[((int)lessonIndex - 1)][textIndex];

            bool displaySkull = true;

            if (textIndex == GText.tutorial[((int)lessonIndex - 1)].Length -2) {
                switch(lessonIndex)
                {
                case Lesson.MAKE_BLOCKS: { cursor.gameObject.SetActive(true); cursor.Play(CursorAnimation.LeftRight); } break;
                case Lesson.LOAD_GUNS: { cursor.gameObject.SetActive(true); cursor.Play(CursorAnimation.Left); } break;
                case Lesson.LOAD_SPECIALS: { cursor.gameObject.SetActive(true); cursor.Play(CursorAnimation.Right); } break;
                }
            }
            else if (textIndex == GText.tutorial[((int)lessonIndex - 1)].Length -1) {

                switch(lessonIndex)
                {
                case Lesson.MAKE_BLOCKS: {
                    background.gameObject.SetActive(false);
                    hasFocus = false;
                    cursor.gameObject.SetActive(false);
                    displaySkull = false;
                } break;
                case Lesson.LOAD_GUNS: {
                    background.gameObject.SetActive(false);
                    hasFocus = false;
                    cursor.gameObject.SetActive(false);
                    displaySkull = false;
                } break;
                case Lesson.LOAD_SPECIALS: {
                    background.gameObject.SetActive(false);
                    hasFocus = false;
                    cursor.gameObject.SetActive(false);
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
                        tutorialText.text = GText.tutorial[((int)lessonIndex - 1)][textIndex];
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

            tutorialText.text = GText.tutorial[((int)lessonIndex - 1)][textIndex];
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
    }

    public enum Lesson { START, MAKE_BLOCKS, LOAD_GUNS, LOAD_SPECIALS,
                         COUNTDOWN, ACTIVATE_GUNS, ACTIVATE_SPECIALS,
                         DOOR_OPENS, GOODWILL, DONE, NONE };
}