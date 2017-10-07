using UnityEngine;
using UnityEngine.SceneManagement;
using Necrosoft;
using Necrosoft.ThirdParty;
using TMPro;

namespace Gunhouse.Credits
{
    public class CreditsScene : MonoBehaviour
    {
        [SerializeField] RectTransform overlay;
        [HideInInspector] public new Transform transform;
        [SerializeField] RectTransform[] finalLocation;
        int finalIndex;
        [SerializeField] TextMeshProUGUI text;
        Objectives objectives;

        bool fadeOut;
        float speed = 80;
        float multiplier = 8;

        bool pressed;
        public void Pressed(bool isPressed) { pressed = isPressed; }

        [HideInInspector] public bool startScrolling;
        bool storeState;

        int endingId;
        int skipOnce;

        void Awake()
        {
            transform = GetComponent<Transform>();
            objectives = GameObject.FindObjectOfType<Objectives>();
        }

        void FixedUpdate()
        {
            if (startScrolling) {
                if (Input.Pad.Submit.WasPressed) Pressed(true);
                if (Input.Pad.Submit.WasReleased) Pressed(false);
            }
        }

        void Update()
        {
            if (!startScrolling) return;

            if (finalLocation[finalIndex].position.y >= -1) {
                if (fadeOut) { return; }

                LeanTween.color(overlay, new Color(0, 0, 0, 1), 1).setDelay(1).setOnComplete(() => {
                    ExitCredits();
                });

                fadeOut = true;

                return;
            }

            transform.TranslateY(pressed ? speed * multiplier * Time.deltaTime : speed * Time.deltaTime);
            if (finalLocation[finalIndex].position.y >= 0) {
                transform.TranslateY(-finalLocation[finalIndex].position.y);
            } 
        }

        public void Display(bool autoMove)
        {
            storeState = autoMove;
            startScrolling = autoMove;
            finalIndex = autoMove ? 1 : 0;

            overlay.gameObject.SetActive(true);

            if (autoMove) {
                finalLocation[0].gameObject.SetActive(false);
                LeanTween.color(overlay, new Color(0, 0, 0, 0), 1);
                Choom.Play("Music/credits");
                AppMain.background_fade = 1.0f;
                return;
            }

            DisplayEnding();
        }

        void DisplayEnding()
        {
            objectives.Show();
            LeanTween.delayedCall(6, () => { objectives.Hide(); });

            LeanTween.color(overlay, new Color(0, 0, 0, 0.5f), 1).setOnComplete(() => {
                Choom.Play("Music/credits");
                TextBlock.Display(text, Story.story[Story.story.Length - 1], () => {
                    endingId = LeanTween.delayedCall(15, () => {
                        skipOnce++;
                        startScrolling = true;
                        AppMain.background_fade = 1.0f;
                        LeanTween.color(overlay, new Color(0, 0, 0, 0), 1);
                        text.color = new Color(1, 1, 1, 0);
                   }).id;
                });
            });
        }

        public void Skip()
        {
            if (storeState) return;
            if (skipOnce >= 2) return;

            if (skipOnce == 0) {
                LeanTween.cancel(endingId);
                text.StopAllCoroutines();
                text.text = Story.story[Story.story.Length - 1];

                endingId = LeanTween.delayedCall(10, () => {
                    startScrolling = true;
                    AppMain.background_fade = 1.0f;
                    LeanTween.color(overlay, new Color(0, 0, 0, 0), 1);
                    text.color = new Color(1, 1, 1, 0);
               }).id;
            }
            else {
                LeanTween.cancel(endingId);
                startScrolling = true;
                AppMain.background_fade = 1.0f;
                LeanTween.color(overlay, new Color(0, 0, 0, 0), 1);
                text.color = new Color(1, 1, 1, 0);
            }

            skipOnce++;
        }

        void ExitCredits()
        {
            AppMain.top_state = storeState ? ((State)new Options()) : ((State)new TitleState(MenuOptions.Title));
            if (storeState) { Choom.Play("Music/title"); }
            SceneManager.UnloadSceneAsync((int)SceneIndex.Credits);
        }
    }
}