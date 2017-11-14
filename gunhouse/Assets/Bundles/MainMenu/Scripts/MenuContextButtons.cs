using UnityEngine;

namespace Gunhouse.Menu
{
    public class MenuContextButtons : MonoBehaviour
    {
        [SerializeField] GameObject[] buttons;

        public void EnableButtons()
        {
            for (int i = 0; i < buttons.Length; ++i) buttons[i].SetActive(false);
            for (int i = 0; i < 2; ++i) buttons[i].SetActive(true);
        }
    }
}