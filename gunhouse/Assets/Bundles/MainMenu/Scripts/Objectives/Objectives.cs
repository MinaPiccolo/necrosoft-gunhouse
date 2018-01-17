using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI[] tasks;
        [Space(10)] [SerializeField] Image[] ticks;
        [SerializeField] AnimationCurve tickCurve;
        [Space(10)] [SerializeField] TextMeshProUGUI[] cash;
        [SerializeField] AnimationCurve cashCurve;

        public static bool AnyComplete;
        Animator animator;
        bool loadOnce;

        void OnEnable()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            #if UNITY_EDITOR

            if (UnityEngine.Input.GetKeyDown(KeyCode.A)) { Play(Menu.HashIDs.menu.Outtro); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.D)) { Play(Menu.HashIDs.menu.Intro); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.E)) { gameObject.transform.GetChild(0).gameObject.SetActive(false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.D)) { gameObject.transform.GetChild(0).gameObject.SetActive(true); }

            if (UnityEngine.Input.GetKeyDown(KeyCode.W)) {
                for (int i = 0; i < activeTasks.Length; ++i) {
                    RequestTask(i);
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.S)) {
                for (int i = 0; i < activeTasks.Length; ++i) {
                    SetTaskComplete(i);
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Q)) {
                CheckComplete();
            }

            #endif
        }

        public void AnimationEventCheck()
        {
            bool firstTime = activeTasks[0] == 0 && activeTasks[1] == 0;
            for (int i = 0; i < activeTasks.Length; ++i) {
                if (firstTime) {
                    CreateTask(i, FreshRandom(0, taskRange[0]));
                }

                UpdateRequestHistory(i);
                SetTaskText(i, false);
            }
        }

        public void Play(int hash) { animator.Play(hash); }
    }
}