using UnityEngine;

namespace Gunhouse.Menu
{
    public class Portraits : MonoBehaviour
    {
        [SerializeField] GameObject[] portratis;
        Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Play(int hash) { animator.Play(hash); }

        public void SelectPortrait(int index)
        {
            portratis[index].gameObject.SetActive(true);
            animator.Play(HashIDs.menu.Intro);
        }

        void AnimationEventDisableMenu()
        {
            for (int i = 0; i < portratis.Length; ++i) portratis[i].SetActive(false);
            gameObject.SetActive(false);
        }
    }
}