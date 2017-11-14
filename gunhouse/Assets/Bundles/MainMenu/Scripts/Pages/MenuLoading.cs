using UnityEngine;
using System.Collections;
using System.Text;
using TMPro;

namespace Gunhouse.Menu
{
    public class MenuLoading : MenuPage
    {
        TextMeshProUGUI text;
        StringBuilder builder  = new StringBuilder(200);
        bool loading;

        protected override void Initalise() { pageID = MenuState.Loading; transitionID = MenuState.PlayGame; }
        protected override void OuttroStartNextIntro() { }
        protected override void OuttroFinished() { base.OuttroFinished(); menu.PortraitFlipOrder(); }

        void OnEnable()
        {
            menu.ignore_input = true;
            loading = false;
            text = GetComponentInChildren<TextMeshProUGUI>();
            builder.Length = 0;
            text.text = builder.AppendFormat("<size=300%>LOADING...</size>\n{0}",
                                             Story.tips[Util.rng.Next(Story.tips.Length)]).ToString();
        }

        void Update()
        {
            if (loading) return;
            if (menu.FadeAlpha > 0.9f) { LoadGame(); loading = true;}
        }

        void LoadGame()
        {
            AppMain.menuAchievements.HideButtons();
            AppMain.top_state.Dispose();

            if (MetaState.hardcore_mode) {
                MetaState.hardcore_score = 0;
                MetaState.reset(0);
            }
            else {
                #if TRACKING
                string[] equippedNames = new string[3];
                int equip_index = 0;
                for (int i = 0; i < DataStorage.NumberOfGuns; ++i) {
                    if (!DataStorage.GunEquipped[i]) { continue; }
                    equippedNames[equip_index++] = ((Gun.Ammo)i).ToString();
                }
                Tracker.StartMode(Game.dayName(MetaState.wave_number), equippedNames, DataStorage.Money);
                #endif

                MetaState.reset();
                MetaState.reset(DataStorage.StartOnWave);
            }

            StartCoroutine(LoadStage());
        }

        IEnumerator LoadStage()
        {
            AppMain.top_state = new Game();

            yield return new WaitForEndOfFrame();

            menu.FadeInOut(true);
            menu.PortraitsHide();
            Play(HashIDs.menu.Outtro);
        }
    }
}