using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Gunhouse.Menu
{
    public class MenuCredits : MenuPage
    {
        [SerializeField] GameObject[] gameEnding;
        [SerializeField] ScrollRect scroll;
        [SerializeField] [Range(1, 100)] float scrollSpeed = 10f;
        float scrollFactor = 100;
        int speedMultiplier = 1;
        [SerializeField] [Range(0, 5)] float scrollDelay = 2f;
        [SerializeField] [Range(0, 10)] float finishedDelay = 2f;
        PlayerInput input;

        protected override void Initalise() { pageID = MenuState.Credits; transitionID = MenuState.Options; }
        protected override void IntroReady()
        {
            StartCoroutine(BeginAutoScroll());
            menu.SetActiveContextButtons(true, false);
        }

        protected override void OuttroFinished()
        {
            StopCoroutine(BeginAutoScroll());
            scroll.verticalNormalizedPosition = 1;
            gameObject.SetActive(false);
        }

        protected override void OuttroStartNextIntro() { base.OuttroStartNextIntro(); StopAllCoroutines(); }

        void OnEnable()
        {
            input = FindObjectOfType<PlayerInput>();
        }

        void Update()
        {
            if (menu.ignore_input) return;
            IncreaseScrollSpeed(input.AnyIsPressed);
        }

        IEnumerator BeginAutoScroll()
        {
            yield return new WaitForSeconds(scrollDelay);

            while (scroll.verticalNormalizedPosition > 0) {
                scroll.verticalNormalizedPosition -= Time.deltaTime * ((scrollSpeed / scrollFactor) * speedMultiplier);
                yield return null;
            }

            yield return new WaitForSeconds(finishedDelay);

            //transitionID = MenuState.Title; // check if game over here.
            Play(HashIDs.menu.Outtro);
            menu.SetActiveContextButtons(false);
        }

        public void IncreaseScrollSpeed(bool increase) { speedMultiplier = increase ? 6 : 1; }
    }
}