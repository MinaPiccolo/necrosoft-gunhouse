using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace Necrosoft
{
    public class Stats : MonoBehaviour
    {
        [SerializeField] Font font;
        [SerializeField] [Range(0.1f, 1)] float size = 0.3f;
        [SerializeField] [Range(0.0f, 5.0f)] float timeScale = 1.0f;

        StringBuilder builder = new StringBuilder(100);
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect();

        int frameCount;
        float deltaTime;
        float delay = 0.2f;
        float counter = 0.2f;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnGUI()
        {
            float w = Screen.width / 2; w *= size;
            int h = (int)w / 4; h += h / 2;
            rect.x = 70;
            rect.y = (Screen.height - h) - 50;
            rect.width = w;
            rect.height = h;

            style.alignment = TextAnchor.LowerLeft;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            style.font = font;
            style.fontSize = (int)w / 10;

            DrawBox(rect, Color.black);
            rect.x += rect.width * 0.05f;
            rect.height -= rect.height * 0.05f;
            GUI.Label(rect, builder.ToString(), style);
        }

        void Update()
        {
            frameCount++;
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            UpdateText();

            if (Time.timeScale > timeScale || Time.timeScale < timeScale) { Time.timeScale = timeScale; }
        }

        void UpdateText()
        {
            counter += Time.deltaTime;
            if (counter < delay) return;
            counter = 0;

            builder.Length = 0;
            builder.AppendFormat("ms: {0:0} fps: {1:0}\n", deltaTime * 1000.0f, 1.0f / deltaTime);
            builder.AppendFormat("{0:0}/{1:0}kb\n", Profiler.GetTotalAllocatedMemoryLong() / 1000, Profiler.GetTotalReservedMemoryLong() / 1000);
            builder.AppendFormat("{0:0}", frameCount);
            //builder.AppendFormat("{0:0} v{1:0}.sky\n", Application.unityVersion, Application.version);
            //builder.Append(Application.genuine);
        }

        void DrawBox(Rect position, Color color)
        {
            Color temp = GUI.color;
            GUI.color = color;
            GUI.Box(position, "");
            GUI.color = temp;
        }
    }
}