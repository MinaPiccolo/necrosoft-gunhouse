using UnityEngine;
using UnityEngine.UI;
using Necrosoft.ThirdParty;

namespace Gunhouse
{
    public class MenuAchievements : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Button[] buttons;

        bool isSignedIn;

        float timer;

        #pragma warning disable 0414
        float checkDelay = 1;
        #pragma warning restore 0414

        bool signInFailedEffect;

        void Update()
        {
            #if PLAY_STORE || UNITY_IOS || UNITY_TVOS
            /* NOTE(shane): this check needs to be done because you can sign out from the
                google play games achievements menu and there's no callback for this.
                So instead we check ever second if we're still signed in. */
            if (!isSignedIn) return;

            timer += Time.deltaTime;
            if (timer % 60 < checkDelay) return;
            timer = 0;

            if (!Social.localUser.authenticated) {
                isSignedIn = false;
                DataStorage.IgnoreSignIn = true;
                buttons[0].gameObject.SetActive(true);
                buttons[1].gameObject.SetActive(false);
            }
            #endif
        }

        public void AutoSignIn()
        {
            #if PLAY_STORE
            GooglePlayGames.PlayGamesPlatform.Activate();
            #endif

            #if PLAY_STORE || UNITY_IOS || UNITY_TVOS
            if (DataStorage.IgnoreSignIn) return;

            ShowSignIn();
            #endif
        }

        public void ShowSignIn()
        {
            Tracker.ScreenVisit(SCREEN_NAME.SIGN_IN);
            
            #if PLAY_STORE || UNITY_IOS || UNITY_TVOS
            Social.localUser.Authenticate((bool success) => {
                isSignedIn = success;
                DataStorage.IgnoreSignIn = !success;

                if (!success) {
                    if (signInFailedEffect) return;

                    signInFailedEffect = true;
                    float shakeAmount = 20;
                    float shakePeriodTime = .25f;
                    LTDescr shakeTween = LeanTween.rotateAroundLocal(buttons[0].gameObject,
                                                                     Vector3.forward, shakeAmount, shakePeriodTime)
                                                  .setEase(LeanTweenType.easeShake).setLoopClamp().setRepeat(-1);
                    LeanTween.value(buttons[0].gameObject, shakeAmount, 0f, 2).setOnUpdate((float val) => {
                        shakeTween.setTo(Vector3.right * val);
                    }).setEase(LeanTweenType.easeOutQuad).setOnComplete(() => { signInFailedEffect = false; });

                    return;
                }

                buttons[0].gameObject.SetActive(false);
                buttons[1].gameObject.SetActive(true);
            });
            #endif
        }

        public void ShowAchievements()
        {
            Tracker.ScreenVisit(SCREEN_NAME.ACHIEVEMENTS);

            #if PLAY_STORE || UNITY_IOS || UNITY_TVOS
            if (!isSignedIn) return;

            Social.ShowAchievementsUI();
            #endif
        }

        public void DisplayButtons(float currentAlpha)
        {
            #if PLAY_STORE || UNITY_IOS || UNITY_TVOS
            canvasGroup.alpha = currentAlpha < 1 ? 0 : 1;
            if (currentAlpha < 1) LeanTween.alphaCanvas(canvasGroup, 1, 2);

            buttons[!isSignedIn ? 0 : 1].gameObject.SetActive(true);
            #endif
        }

        public void HideButtons()
        {
            #if PLAY_STORE || UNITY_IOS || UNITY_TVOS
            for (int i = 0; i < buttons.Length; ++i) buttons[i].gameObject.SetActive(false);
            #endif
        }

        public void Award(string achievementID)
        {
            Tracker.AchievementUnlocked(achievementID);

            #if PLAY_STORE || UNITY_IOS || UNITY_TVOS
            if (!isSignedIn) return;

            Social.ReportProgress(achievementID, 100, null);
            #endif
        }
    }
}