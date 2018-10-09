using UnityEngine;
using Necrosoft;
using Necrosoft.ThirdParty;

namespace Gunhouse.Menu
{
    public class MenuEndGame : MenuPage
    {
        [SerializeField] Objectives objectives;

        protected override void Initalise() { pageID = MenuState.EndGame; transitionID = MenuState.Credits; }

        protected override void IntroReady()
        {
            Choom.Play("Music/credits");
            Choom.Pause(false);

            objectives.CheckComplete();
            Objectives.CheckAchievements();

            if (Objectives.AnyComplete) {
                LeanTween.delayedCall(4, () => {
                    menu.ignore_input = false;
                    refocusSelected.SetActive(true);
                    menu.SetFocus(refocusSelected);
                    objectives.Play(HashIDs.menu.Outtro);
                });
            }
            else {
                menu.ignore_input = false;
                refocusSelected.SetActive(true);
                menu.SetFocus(refocusSelected);
                objectives.Play(HashIDs.menu.Outtro); 
            }

            MenuCredits.DisplayEnding = true;
            MenuCredits.ScrollDelay = 3;
        }

        protected override void OuttroStartNextIntro()
        {
            AppMain.DisplayAnchor = false;
            AppMain.top_state.child_state.Dispose();
            AppMain.top_state.child_state = null;
            Game.instance = null;
            menu.ignore_input = true;

            base.OuttroStartNextIntro();
        }

        public override void CancelPressed()
        {
            menu.Fade(0, 1);
            Play(HashIDs.menu.Outtro);
        }

        void OnEnable()
        {
            animator.speed = 0;
            refocusSelected.SetActive(false);

            MetaState.end_game = false;
            AppMain.screenShake(0, 0);
            Choom.StopAllEffects();
            Choom.Pause();

            if (MetaState.wave_number + 1 > DataStorage.StartOnWave) {
                DataStorage.StartOnWave = MetaState.wave_number + 1;
            }

            Objectives.BossDefeated();
            Objectives.SurvivedFinalStage();

            Platform.SaveEndWave();
            Objectives.CheckAchievements();

            objectives.UpdateText();

            menu.Fade(0.9f, 0.5f, () => {
                objectives.Play(HashIDs.menu.Intro);
                animator.speed = 1;
            });
        }
    }
}