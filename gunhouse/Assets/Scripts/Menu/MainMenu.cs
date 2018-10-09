using UnityEngine;
using UnityEngine.EventSystems;
using System.Text;
using Necrosoft.ThirdParty;
using TMPro;

namespace Gunhouse.Menu
{
    public enum MenuState { None, Splash, Title, PickADay, Pause, Store, Options, Stats, Quit,
                            Graphics, Audio, Help, Input, Credits, About, Loading, EndWave, EndGame };

    public class MainMenu : MonoBehaviour
    {
        [SerializeField] CanvasGroup fade;
        [SerializeField] MenuLoading loading;
        [SerializeField] Portraits portraits;
        [SerializeField] MenuContextButtons buttons;
        [Space(10)][SerializeField] CanvasGroup canvasDay; 
        [SerializeField] TextMeshProUGUI dayText; 
        LTDescr dayTween;

        MenuState currentMenu = MenuState.Splash;
        GameObject lastSelected;
        GameObject currentSelected;

        MenuPage[] pages;
        MenuPage activePage;
        PlayerInput input;
        public static bool ignoreFocus;
        bool ignoreEffect;
        bool ignoreSelectEffectForever;
        int clearInput;
        GameObject[] lastEffect = new GameObject[2];

        [System.NonSerialized] public bool ignore_input = true;
        [System.NonSerialized] public StringBuilder builder = new StringBuilder(170);
        [System.NonSerialized] public int SelectedWave;
        public void PortraitsHide() { portraits.Play(HashIDs.menu.Outtro); }

        void Awake()
        {
            buttons.gameObject.SetActive(false);
            input = FindObjectOfType<PlayerInput>();
            pages = GetComponentsInChildren<MenuPage>(true);

            HashIDs.GenerateAnimationHashIDs();

            fade.gameObject.SetActive(true);
            fade.alpha = 1;
        }

        void Start() { Fade(0, 0.5f); }

        void Update()
        {
            Cheats();
            ShouldFocus();

            if (ignore_input) return;

            if (input.AnyWasPressed && currentMenu == MenuState.Splash) { ClosePage(); }
            if (input.Cancel.WasPressed) { ClosePage(); }

            RefocusMenu();
        }

        void Cheats()
        {
            #if UNITY_EDITOR

            if (UnityEngine.Input.GetKeyDown(KeyCode.I)) {
                DataStorage.StartOnWave++;
                MetaState.setCoefficients(DataStorage.StartOnWave);
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.K)) {
                DataStorage.StartOnWave--;
                if (DataStorage.StartOnWave < 0) DataStorage.StartOnWave = 0;
                MetaState.setCoefficients(DataStorage.StartOnWave);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.J)) { DataStorage.Money -= 100000; }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.L)) { DataStorage.Money += 100000; }

            #endif
        }

        void ShouldFocus()
        {
            if (input.AnyIsPressed && ignoreFocus) {
                ignoreFocus = false;
                ignoreSelectEffectForever = false;
            }

            #if !UNITY_PSP2 ||!UNITY_PS4

            if (ignoreFocus) { return; }
            ignoreFocus = (UnityEngine.Input.touchCount > 0) ||
                          UnityEngine.Input.GetMouseButton(0);
            if (ignoreFocus) { ignoreSelectEffectForever = true; }

            #endif
        }

        void RefocusMenu()
        {
            /* when the menu is refocused, to prevent it jumping
                to the next item we need to clear the input. however this
                needs to happen over multiple frames because movement is
                based on IsPressed rather theen WasPressed */
            if (clearInput > 0) { input.ClearInput(); clearInput--; }

            if (!input.AnyIsPressed) { return; }
            if (EventSystem.current.currentSelectedGameObject != null) {
                if (EventSystem.current.currentSelectedGameObject.activeInHierarchy) { return; }
                if (activePage.refocusSelected == null) { return; }
                clearInput = 10;
                input.ClearInput();
                EventSystem.current.SetSelectedGameObject(activePage.refocusSelected);
            }
            else {
                if (activePage.refocusSelected == null) { return; }
                clearInput = 10;
                input.ClearInput();
                EventSystem.current.SetSelectedGameObject(activePage.refocusSelected);
            }
        }

        #region Public

        public void ClosePage() /* attached to onclick context button */
        {
            activePage.CancelPressed();
        }

        public void SetPage(MenuState pageID)
        {
            for (int i = 0; i < pages.Length; ++i) {
                if (pages[i].pageID != pageID) continue;

                pages[i].gameObject.SetActive(true);
                pages[i].Play(HashIDs.menu.Intro);

                activePage = pages[i];
            }

            currentMenu = pageID;

            /* should diplay portrait */
            if (currentMenu >= MenuState.Title &&
                currentMenu != MenuState.Pause &&
                currentMenu != MenuState.EndWave &&
                currentMenu != MenuState.EndGame &&
                currentMenu != MenuState.Loading &&
                !portraits.gameObject.activeInHierarchy && !AppMain.IsPaused) {
                portraits.gameObject.SetActive(true);
                portraits.SelectPortrait(UnityEngine.Random.Range(0, 4));
            }
        }

        public void SetActiveBackButton(bool active) { buttons.SetActiveBackButton(active); }
        public void SetActiveContextButtons(bool selectEnabled = true, bool cancelEnabled = true)
        {
            ignore_input = !(selectEnabled || cancelEnabled);
            buttons.gameObject.SetActive(selectEnabled || cancelEnabled);
            buttons.EnableButtons(selectEnabled, cancelEnabled);
            buttons.SetColor(!AppMain.IsPaused && currentMenu != MenuState.EndWave);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public void SetFocus(GameObject item, bool disableEffect = false)
        {
            if (ignoreFocus && activePage.pageID != MenuState.Store) { return; }

            ignoreEffect = item != null && !disableEffect;

            EventSystem.current.SetSelectedGameObject(null); /* clear to reset focus */

            if (item == null) {
                lastEffect[1] = null;
                return;
            }

            EventSystem.current.SetSelectedGameObject(item);
        }

        public string DayName(int wave)
        {
            return string.Format("DAY {0}, {1}", (wave / 3 + 1), wave % 3 == 0 ? "NOON" : wave % 3 == 1 ? "DUSK" : "NIGHT");
        }

        public void DisplayDayName()
        {
            canvasDay.gameObject.SetActive(true);
            canvasDay.alpha = 0;

            dayText.text = AppMain.MainMenu.DayName(MetaState.wave_number);

            dayTween = LeanTween.alphaCanvas(canvasDay, 1, 1).setLoopPingPong(1)
                                .setDelay(0.5f).setEase(LeanTweenType.easeOutQuint)
                                .setOnComplete(()=> { canvasDay.gameObject.SetActive(false); });
        }

        public void SetActiveDayName(bool active, bool cancel = false)
        {
            if (!canvasDay.gameObject.activeInHierarchy) { return; }

            if (active) {
                LeanTween.resume(dayTween.id);
            }
            else {
                if (cancel) {
                    LeanTween.cancel(dayTween.id);
                    canvasDay.gameObject.SetActive(false);
                }
                else {
                    LeanTween.pause(dayTween.id);
                }
            }
        }

        public void Fade(float finish, float time, System.Action onComplete = null)
        {
            fade.gameObject.SetActive(true);
            //fade.alpha = start;
            LeanTween.cancel(fade.gameObject);
            LeanTween.alphaCanvas(fade, finish, time).setOnComplete(() => {
                fade.gameObject.SetActive(!(fade.alpha < 0.1f));
                if (onComplete != null) { onComplete(); }
            });
        }

        public void PlaySelect()
        {
            if (ignoreEffect && !ignoreSelectEffectForever) {
                ignoreEffect = false;
                return;
            }

            if (lastEffect[0] == EventSystem.current.currentSelectedGameObject) return;
            lastEffect[0] = EventSystem.current.currentSelectedGameObject;

            if (ignoreSelectEffectForever) { return; }

            Necrosoft.Choom.PlayEffect(SoundAssets.UIConfirm);
        }

        public void PlayConfirm()
        {
            if (lastEffect[1] != null && lastEffect[1] == EventSystem.current.currentSelectedGameObject) return;
            lastEffect[1] = EventSystem.current.currentSelectedGameObject;

            Necrosoft.Choom.PlayEffect(SoundAssets.UISelect);
        }

        /* (shane) NOTE: this is dumb but I'm not going to rewrite all this code! */
        public void ClearConfirm()
        {
            lastEffect[1] = null;
        }

        #endregion
    }
}