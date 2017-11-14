using UnityEngine;

namespace Gunhouse.Menu
{
    [RequireComponent(typeof(Animator))]
    public abstract class MenuPage : MonoBehaviour
    {
        protected MainMenu menu;
        public MenuState pageID;
        protected MenuState transitionID;
        Animator animator;

        void Awake()
        {
            menu = GetComponentInParent<MainMenu>();
            animator = GetComponent<Animator>();
            Initalise();
        }

        public void Play(int hash) { animator.Play(hash); }

        protected abstract void Initalise();
        protected virtual void IntroReady() { menu.ignore_input = false; }
        protected virtual void OuttroFinished() { gameObject.SetActive(false); }
        protected virtual void OuttroStartNextIntro() { menu.SetPage(transitionID); }
    }
}