using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Necrosoft
{
    // https://docs.unity3d.com/ScriptReference/LogType.html
    // use StringBuilder
    // rich text "<color=red>Fatal error:</color>
    // link to object Debug.Log("Hello", gameObject);
    // https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity7.html

    public class Console : MonoBehaviour
    {
        [SerializeField] Font font;
        StringBuilder builder = new StringBuilder(1000);
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect();

        static List<string> log = new List<string>();
        static System.Object syncObject = new System.Object();

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
            //Log.messages += Message;
        }

        void OnDisable()
        {
            //Log.messages -= Message;
            Application.logMessageReceived -= HandleLog;
        }

        void OnGUI()
        {
            lock (syncObject)
            {
                float w = Screen.width * 4 / 5;
                int h = (int)w / 3; h += 5;
                rect.x = Screen.width / 5;
                rect.y = (Screen.height - h) - 20;
                rect.width = w;
                rect.height = h;

                style.alignment = TextAnchor.LowerLeft;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.white;
                style.font = font;
                style.fontSize = (int)w / 50;

                DrawBox(rect, Color.black);
                rect.x += rect.width * 0.015f;
                rect.height += 10;

                builder.Length = 0;
                for (int i = 0; i < log.Count; ++i)
                {
                    builder.Append(log[i]).AppendLine();
                }
                GUI.Label(rect, builder.ToString(), style);
            }
        }

        static private void Log(string message, Color color, bool keepNewlines = false, bool outputToConsole = false)
        {
            lock (syncObject)
            {
                if (log.Count > 30) { log.RemoveAt(0); }
                log.Add(message);
            }
        }

        static public void Log(string msg, bool keepNewlines = false)
        {
            Log(msg, Color.white, keepNewlines, true);
        }

        static public void Log(string msg, Color color)
        {
            Log(msg, color, false, true);
        }

        static public void LogWarning(string msg)
        {
            Log(msg, Color.yellow, false, false);
            System.Console.Error.WriteLine(msg);
        }

        static public void LogError(string msg)
        {
            Log(msg, Color.red, false, false);
            System.Console.Error.WriteLine(msg);
        }

        //void Message(Log.Message message)
        //{
        //    if (log.Count > 30) { log.RemoveAt(0); }
        //    log.Add(message.text);

        //    //switch (message.type)
        //    //{
        //    //    case Log.MessageType.Info: Debug.Log(message.text); break;
        //    //    case Log.MessageType.Warning: Debug.LogWarning(message.text); break;
        //    //    case Log.MessageType.Error: Debug.LogError(message.text); break;
        //    //}
        //}

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (log.Count > 30) { log.RemoveAt(0); }
            log.Add(logString);// + " trace: " + stackTrace);
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