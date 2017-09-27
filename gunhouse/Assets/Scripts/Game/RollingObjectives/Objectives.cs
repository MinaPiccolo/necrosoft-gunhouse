using UnityEngine;
using UnityEngine.UI;
using Necrosoft.ThirdParty;
using TMPro;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        [SerializeField] CanvasGroup banner;
        [SerializeField] TextMeshProUGUI[] tasks;
        [Space(10)] [SerializeField] Image[] ticks;
        [SerializeField] AnimationCurve tickCurve;
        [Space(10)] [SerializeField] TextMeshProUGUI[] cash;
        [SerializeField] AnimationCurve cashCurve;

        void Awake()
        {
            banner.alpha = 0;
            for (int i = 0; i < tasks.Length; ++i) tasks[i].richText = true;
        }

        void Start()
        {
            bool firstTime = activeTasks[0] == 0 && activeTasks[1] == 0;

            for (int i = 0; i < activeTasks.Length; ++i) {
                if (firstTime) {
                    activeTasks[i] = FreshRandom(0, taskRange[0]);
                    previousTasks.Add(activeTasks[i]);
                }

                UpdateRequestHistory(i);
                SetTaskText(i, false);
            }
        }

        void Update()
        {
            #if UNITY_EDITOR

            if (UnityEngine.Input.GetKeyDown(KeyCode.W)) for (int i = 0; i < activeTasks.Length; ++i) RequestTask(i);
            if (UnityEngine.Input.GetKeyDown(KeyCode.D)) Show();
            if (UnityEngine.Input.GetKeyDown(KeyCode.S)) for (int i = 0; i < activeTasks.Length; ++i) SetTaskComplete(i);
            if (UnityEngine.Input.GetKeyDown(KeyCode.A)) Hide();
            if (UnityEngine.Input.GetKeyDown(KeyCode.Q)) CheckComplete();

            #endif
        }

        public void Show()
        {
            UpdateText();
            LeanTween.alphaCanvas(banner, 1, 1).setOnComplete(() => { CheckComplete(); });
        }

        public void Hide()
        {
            LeanTween.alphaCanvas(banner, 0, 1);
        }
    }
}