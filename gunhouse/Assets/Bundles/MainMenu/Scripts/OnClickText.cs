using UnityEngine;
using TMPro;

namespace Gunhouse.Menu
{
    public class OnClickText : MonoBehaviour
    {
        [SerializeField] string[] textChoices;
        int current_index;

        TextMeshProUGUI text;

        void OnEnable()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        public void ChangeText()
        {
            text.text = textChoices[current_index = Mathf.Clamp(current_index++, 0, textChoices.Length - 1)];
        }
    }
}
