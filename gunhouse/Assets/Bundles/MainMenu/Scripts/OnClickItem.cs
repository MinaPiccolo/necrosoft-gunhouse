using UnityEngine;

namespace Gunhouse.Menu
{
    public enum MenuItem { None, Display, Resolution, Quality, AntiAliasing, VSync };

    public class OnClickItem : MonoBehaviour { public MenuItem item; }
}
