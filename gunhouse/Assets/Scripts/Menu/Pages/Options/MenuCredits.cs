﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Necrosoft;

namespace Gunhouse.Menu
{
    public class MenuCredits : MenuPage
    {
        [SerializeField] GameObject[] gameEnding;
        [SerializeField] ScrollRect scroll;
        [SerializeField] [Range(1, 100)] float scrollSpeed = 10f;
        float scrollFactor = 100;
        bool speedTouch;
        bool speedController;
        [SerializeField] [Range(0, 10)] float finishedDelay = 2f;
        PlayerInput input;

        public static float ScrollDelay;
        public static bool DisplayEnding;

        protected override void Initalise() { pageID = MenuState.Credits; transitionID = MenuState.Options; }
        protected override void IntroReady()
        {
            StartCoroutine(BeginAutoScroll());
            menu.SetActiveContextButtons(false, true);
        }

        protected override void OuttroFinished()
        {
            StopCoroutine(BeginAutoScroll());
            scroll.verticalNormalizedPosition = 1;
            gameObject.SetActive(false);
            DisplayEnding = false;
            transitionID = MenuState.Options;
            ScrollDelay = 0;
        }

        protected override void OuttroStartNextIntro()
        {
            if (DisplayEnding) { Choom.Play("Music/title"); }
            base.OuttroStartNextIntro();
        }

        void OnEnable()
        {
            input = FindObjectOfType<PlayerInput>();
            for (int i = 0; i < gameEnding.Length; ++i) { gameEnding[i].SetActive(DisplayEnding); }

            transitionID = DisplayEnding ? MenuState.Title : MenuState.Options;
        }

        void Update()
        {
            if (menu.ignore_input) return;
            ControllerIncreaseScrollSpeed(input.AnyIsPressed);
        }

        IEnumerator BeginAutoScroll()
        {
            yield return new WaitForSeconds(ScrollDelay);

            while (scroll.verticalNormalizedPosition > 0) {
                scroll.verticalNormalizedPosition -= Time.deltaTime * ((scrollSpeed / scrollFactor) *
                                                                       (speedController || speedTouch ? 6 : 1));
                yield return null;
            }

            yield return new WaitForSeconds(finishedDelay);

            Play(HashIDs.menu.Outtro);
            menu.SetActiveContextButtons(false, false);
        }

        public void ControllerIncreaseScrollSpeed(bool increase) { speedController = increase; }
        public void IncreaseScrollSpeed(bool increase) { speedTouch = increase; }
    }
}