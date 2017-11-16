using UnityEngine;
using System.Collections;
using TMPro;
using Necrosoft.ThirdParty;

namespace Gunhouse.Menu
{
    public class MenuLoading : MenuPage
    {
        TextMeshProUGUI text;

        protected override void Initalise() { pageID = MenuState.Loading; transitionID = MenuState.Pause; }
        protected override void OuttroStartNextIntro() { }

        protected override void OuttroFinished()
        {
            base.OuttroFinished();
        }

        protected override void IntroReady()
        {
            base.IntroReady();
            LeanTween.delayedCall(0.25f, () => { LoadGame(); });
        }

        void OnEnable()
        {
            menu.ignore_input = true;

            text = GetComponentInChildren<TextMeshProUGUI>();
            menu.builder.Length = 0;
            text.text = menu.builder.AppendFormat("<size=300%>LOADING...</size>\n{0}",
                                                  Story.tips[Util.rng.Next(Story.tips.Length)]).ToString();
        }

        void LoadGame()
        {
            AppMain.menuAchievements.HideButtons();
            AppMain.top_state.Dispose();

            if (MetaState.hardcore_mode) {
                MetaState.hardcore_score = 0;
                menu.SelectedWave = 0;
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
            }

            StartCoroutine(LoadStage());
        }

        IEnumerator LoadStage()
        {
            MetaState.reset(menu.SelectedWave);
            AppMain.top_state = new Game();
            AppMain.DisplayAnchor = true;

            yield return new WaitForEndOfFrame();

            menu.FadeInOut(true);
            menu.PortraitsHide();
            Play(HashIDs.menu.Outtro);
        }
    }
}