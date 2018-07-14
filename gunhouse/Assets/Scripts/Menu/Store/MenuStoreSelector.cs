using UnityEngine;
using TMPro;

namespace Gunhouse.Menu
{
    public class MenuStoreSelector : MonoBehaviour
    {
        Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Play(int hashID) { animator.Play(hashID); }
    }
}
