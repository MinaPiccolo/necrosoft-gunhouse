using UnityEngine;

namespace Gunhouse.Menu
{
    public class Portraits : MonoBehaviour
    {
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject[] portratis;
        Animator animator;
        public int SortOrder { get { return canvas.sortingOrder; } set { canvas.sortingOrder = value; } }

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
            canvas.sortingOrder = 0;
            for (int i = 0; i < portratis.Length; ++i) portratis[i].SetActive(false);
            gameObject.SetActive(false);
        }
    }
}