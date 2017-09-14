using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text;
using TMPro;

namespace Necrosoft
{
    public static class TextBlock
    {
        static StringBuilder stringBuilder = new StringBuilder(400);
        static string hideStart = "<#FFFFFF00>"; //"<color=\"#FFFFFF00\">";
        static string hideEnd = "</color>";
        static float perCharSpeed = .01f;
        static float preCharCount = 2;

        public static void Display(TextMeshProUGUI text, string message) { Display(text, message, null); }
        public static void Display(TextMeshProUGUI text, string message, Action onComplete) { text.StartCoroutine(DisplayText(text, message, onComplete)); }
        static IEnumerator DisplayText(TextMeshProUGUI text, string message, Action onComplete)
        {
            int count = 0;

            for (int i = 0; i < message.Length; ++i) {
                stringBuilder.Length = 0;
                stringBuilder.Append(message.Substring(0, i + 1)).Append(hideStart)
                             .Append(message.Substring(i + 1)).Append(hideEnd);
                text.text = stringBuilder.ToString();

                count++;
                if (count < preCharCount) continue;
                count =  0;

                yield return new WaitForSeconds(perCharSpeed);
            }

            text.text = message;

            if (onComplete != null) { onComplete(); }
        }

        public static void Wipe(TextMeshProUGUI text) { Wipe(text, null); }
        public static void Wipe(TextMeshProUGUI text, Action onComplete) { text.StartCoroutine(WipeText(text, onComplete)); }
        static IEnumerator WipeText(TextMeshProUGUI text, Action onComplete)
        {
            int count = 0;
            string message = text.text;

            for (int i = message.Length - 1; i >= -1; --i) {
                stringBuilder.Length = 0;
                stringBuilder.Append(message.Substring(0, i + 1)).Append(hideStart)
                             .Append(message.Substring(i + 1)).Append(hideEnd);
                text.text = stringBuilder.ToString();

                count++;
                if (count < preCharCount) continue;
                count =  0;

                yield return new WaitForSeconds(perCharSpeed);
            }

            if (onComplete != null) { onComplete(); }
        }
    }
}