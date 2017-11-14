using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Gunhouse.Menu
{
    public class MenuCredits : MenuPage
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] ScrollRect scroll;
        [SerializeField] [Range(1, 100)] float scrollSpeed = 10f;
        float scrollFactor = 100;
        int speedMultiplier = 1;
        [SerializeField] [Range(0, 5)] float scrollDelay = 2f;
        PlayerInput input;

        protected override void Initalise() { pageID = MenuState.Credits; transitionID = MenuState.Options; }
        protected override void IntroReady()
        {
            StartCoroutine(BeginAutoScroll());
            menu.SetActiveContextButtons(true);
        }

        protected override void OuttroFinished()
        {
            StopCoroutine(BeginAutoScroll());
            scroll.verticalNormalizedPosition = 1;
            gameObject.SetActive(false);
        }

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
        }

        public void IncreaseScrollSpeed(bool increase) { speedMultiplier = increase ? 6 : 1; }
    }
}