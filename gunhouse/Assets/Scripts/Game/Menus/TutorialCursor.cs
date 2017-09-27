using UnityEngine;
using UnityEngine.UI;

namespace Gunhouse
{
    public enum CursorAnimation { None, Left, Right, LeftRight, LeftUpDown, RightUpDown };

    public class TutorialCursor : MonoBehaviour
    {
        Animator animator;
        int[] animationHash;

        int cursorLeftRight;
        int cursorLeft;
        int cursorRight;
        int cursorLeftUpDown;
        int cursorRightUpDown;

        void Awake()
        {
            animator = GetComponent<Animator>();

            /* order according to how the CursorAnimation enum is arranged */
            animationHash = new int[] { Animator.StringToHash("Base Layer.CursorLeft"),
                                        Animator.StringToHash("Base Layer.CursorRight"),
                                        Animator.StringToHash("Base Layer.CursorLeftRight"),
                                        Animator.StringToHash("Base Layer.CursorLeftUpDown"),
                                        Animator.StringToHash("Base Layer.CursorRightUpDown") };
        }

        public void Play(CursorAnimation animation)
        {
            animator.Play(animationHash[(int)animation - 1]);
        }

        public void AnimationStarted()
        {

        }

        public void AnimationFinished()
        {

        }
    }
}