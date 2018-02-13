using UnityEngine;
using TMPro;

namespace Gunhouse.Menu
{
    public class MenuAbout : MenuPage
    {
        [SerializeField] TextMeshProUGUI text;

        protected override void Initalise() { pageID = MenuState.About; transitionID = MenuState.Options; }
        protected override void IntroReady() { menu.SetActiveContextButtons(false, true); }
    
        void OnEnable()
        {
            menu.builder.Length = 0;
            menu.builder.Append("<size=150%>ABOUT:</size>\n");
            menu.builder.AppendFormat("GUNHOUSE V{0}\n", Application.version);
            menu.builder.AppendFormat("COPYRIGHT NECROSOFT GAMES {0}\n", System.DateTime.Now.Year);
            menu.builder.Append("SUPPORT@NECROSOFTGAMES.COM\n");
            menu.builder.Append("SEE MORE AT: NECROSOFTGAMES.COM");
            text.text = menu.builder.ToString();
        }
    }
}