using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Text;

namespace Gunhouse.Menu
{
    public enum MenuState { None, Splash, Title, PlayGame, Hardcore, Store, Options, Stats, Quit,
                            Graphics, Audio, Help, Input, Credits, About, Loading };

    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Image fade;
        [SerializeField] MenuLoading loading;
        [SerializeField] Portraits portraits;
        [SerializeField] MenuContextButtons buttons;

        MenuState currentMenu = MenuState.Splash;
        GameObject lastSelected;
        GameObject currentSelected;

        MenuPage[] pages;
        MenuPage activePage;
        PlayerInput input;
        [System.NonSerialized] public bool ignore_input = true;
        [System.NonSerialized] public StringBuilder builder = new StringBuilder(170);
        public float FadeAlpha { get { return fade.color.a; } }

        void Awake()
        {
            buttons.gameObject.SetActive(false);
            input = FindObjectOfType<PlayerInput>();
            pages = GetComponentsInChildren<MenuPage>(true);

            HashIDs.GenerateAnimationHashIDs();
        }

        void Update()
        {
            Cheats();

            if (ignore_input) return;

            ExitMenu();
        }

        void LateUpdate()
        {
            if (ignore_input) return;

            currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null && lastSelected != null && lastSelected.activeSelf) {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }

        public void SetPage(MenuState pageID)
        {
            for (int i = 0; i < pages.Length; ++i) {
                if (pages[i].pageID != pageID) continue;

                pages[i].gameObject.SetActive(true);
                pages[i].Play(HashIDs.menu.Intro);

                activePage = pages[i];
                currentMenu = pageID;
            }

            if (currentMenu == MenuState.Title && !portraits.gameObject.activeSelf) {
                portraits.gameObject.SetActive(true);
                portraits.SelectPortrait(Random.Range(0, 4));
            }

            if (currentMenu != MenuState.Loading) {
                if (pageID == MenuState.PlayGame || pageID == MenuState.Hardcore) {
                    currentMenu = pageID;
                    MetaState.hardcore_mode = pageID == MenuState.Hardcore;
                    loading.gameObject.SetActive(true);
                    loading.Play(HashIDs.menu.Intro);
                }
            }
        }

        public void SetActiveContextButtons(bool enable, bool selectEnabled = true)
        {
            ignore_input = !enable;
            buttons.gameObject.SetActive(enable);
            buttons.EnableButtons(selectEnabled);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public void DismissSplash()
        {
            ignore_input = true;
            activePage.Play(HashIDs.menu.Outtro);
        }

        public static void SetFocus(GameObject item)
        {
            EventSystem.current.SetSelectedGameObject(null); /* clear to reset focus */
            if (item == null) return;
            EventSystem.current.SetSelectedGameObject(item);
        }

        void ExitMenu()
        {
            if (input.AnyWasPressed && currentMenu == MenuState.Splash) { DismissSplash(); }

            if (!input.Cancel.WasPressed) return;

            ignore_input = true;

            activePage.Play(HashIDs.menu.Outtro);
            buttons.gameObject.SetActive(false);
            if (currentMenu == MenuState.Title) { PortraitsHide(); }
        }

        public void PortraitOrder(int index) { portraits.SortOrder = index; }
        public void PortraitsHide() { portraits.Play(HashIDs.menu.Outtro); }
    
        public void FadeInOut(bool fadeIn, float time = 1.0f)
        {
            StartCoroutine(FadeInOut(fadeIn ? 1 : 0, fadeIn ? 0 : 1, time));
        }

        IEnumerator FadeInOut(float from, float to, float time)
        {
            fade.gameObject.SetActive(true);
            Color color = from < 1 ? Color.clear : Color.black;
            fade.color = color;

            float t = 0.0f;
            float rate = 1.0f / time;

            while (t < 1.0) {
                t += Time.deltaTime * rate;
                color.a = Mathf.Lerp(from, to, t);
                fade.color = color;

                yield return null;
            }

            fade.color = color;
            if (fade.color.a < 0.1f) { fade.gameObject.SetActive(false); }
        }

        void Cheats()
        {
            #if UNITY_EDITOR

            int key = Util.keyRepeat("level select", 0, 20, 5, Input.last_key);

            if (key == (int)KeyCode.I) {
                DataStorage.StartOnWave++;
                MetaState.setCoefficients(DataStorage.StartOnWave);
            }

            if (key == (int)KeyCode.K) {
                DataStorage.StartOnWave--;
                if (DataStorage.StartOnWave < 0) DataStorage.StartOnWave = 0;
                MetaState.setCoefficients(DataStorage.StartOnWave);
            }

            if (key == (int)KeyCode.J) { DataStorage.Money -= 100000; }
            if (key == (int)KeyCode.L) { DataStorage.Money += 100000; }

            #endif
        }

        public string DayName(int wave)
        {
            builder.Length = 0;
            builder.AppendFormat("DAY {0}, {1}", (wave / 3 + 1),
                                 wave % 3 == 0 ? "NOON" : wave % 3 == 1 ? "DUSK" : "NIGHT");

            return AppMain.MainMenu.builder.ToString();
        }
    }
}