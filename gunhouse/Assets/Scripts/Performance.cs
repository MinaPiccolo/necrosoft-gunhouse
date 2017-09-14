using UnityEngine;

namespace Gunhouse
{
    public class Performance : MonoBehaviour
    {
        #if UNITY_EDITOR
        float deltaTime = 0.0f;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = -1;
        }

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(25, 25, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 25;
            style.normal.textColor = new Color(1, 0.1f, 0.1f, 1);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("MS: {0:0.0} - FPS: {1:0.}", msec, fps);
            GUI.Label(rect, text, style);
        }
        #else
        void Start()
        {
            Application.targetFrameRate = 60;
        }
        #endif
    }
}