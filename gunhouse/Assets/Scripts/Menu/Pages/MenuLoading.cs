using UnityEngine;
using System.Collections;
using TMPro;
using Necrosoft.ThirdParty;

namespace Gunhouse.Menu
{
    public class MenuLoading : MenuPage
    {
        TextMeshProUGUI text;

        protected override void Initalise() { pageID = MenuState.Loading; transitionID = MenuState.None; }
        protected override void OuttroStartNextIntro() { }

        protected override void OuttroFinished()
        {
            base.OuttroFinished();
        }

        protected override void IntroReady()
        {
            base.IntroReady();
            LeanTween.delayedCall(gameObject, 0.25f, LoadGame);
        }

        void OnEnable()
        {
            menu.ignore_input = true;

            text = GetComponentInChildren<TextMeshProUGUI>();
            menu.builder.Length = 0;
            text.SetText(menu.builder.AppendFormat("<size=300%>LOADING...</size>\n{0}",
                                                   GText.Story.tips[Util.rng.Next(GText.Story.tips.Length)]));

            menu.Fade(1, 0.5f);
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

            yield return new WaitForSeconds(0.2f);

            menu.PortraitsHide();
            Play(HashIDs.menu.Outtro);
            menu.Fade(0, 0.5f);
        }
    }
}