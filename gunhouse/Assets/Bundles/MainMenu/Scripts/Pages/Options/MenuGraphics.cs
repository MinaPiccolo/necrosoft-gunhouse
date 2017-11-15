using UnityEngine;

namespace Gunhouse.Menu
{
    public class MenuGraphics : MenuPage
    {
        [SerializeField] GameObject lastSelected;

        protected override void Initalise() { pageID = MenuState.Graphics; transitionID = MenuState.Options; }

        protected override void IntroReady()
        {
            menu.SetActiveContextButtons(true, true);
            MainMenu.SetFocus(lastSelected);
        }

        protected override void OuttroFinished()
        {
            /* record last selected item for if the player returns */
            lastSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            MainMenu.SetFocus(null);

            base.OuttroFinished();
        }

        public void ApplySettings()
        {
            Debug.Log("APPLY GRAPHIC SETTINGS");
        }

        public void ResetSettings()
        {
            Debug.Log("RESET GRAPHIC SETTINGS");
        }

        public void ChangeItem(OnClickItem item)
        {
            Debug.Log(item.item.ToString());
        }

        public void ChangeDisplay()
        {
            Screen.fullScreen = !Screen.fullScreen;

            // fullscreen, windows
        }

        public void ChangeResolution()
        {
            //1920x1080

            //Resolution[] resolutions = Screen.resolutions;
            //foreach (Resolution res in resolutions) {
            //    print(res.width + "x" + res.height);
            //}
            //Screen.SetResolution(resolutions[0].width, resolutions[0].height, true);
        
            // SetResolution
            // https://docs.unity3d.com/ScriptReference/Screen.SetResolution.html
            //https://docs.unity3d.com/ScriptReference/Resolution-refreshRate.html
        }

        public void ChangeQuality()
        {
            // QUALITY: < HIGH > Full Res, Half Res, Quarter Res and Eighth Res
            // VERY HIGH, HIGH, MEDIUM, LOW
        }

        public void ChangeVSync()
        {
            QualitySettings.vSyncCount = 1; // 0 off,
            // https://docs.unity3d.com/ScriptReference/QualitySettings-vSyncCount.html
        
            // If you set it to 1 then it will output one frame update for every screen update.
            // If you set it to 2 it will output one frame update every second screen update.

            // https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html

            //res = Screen.currentResolution;
            //if(res.refreshRate == 60)
            //    QualitySettings.vSyncCount = 1;
            //if(res.refreshRate == 120)
            //    QualitySettings.vSyncCount = 2;
            //print (QualitySettings.vSyncCount);
        }

        public void ChangeAntiAlaising()
        {
            // OFF 2x, 4x, 8x
            QualitySettings.antiAliasing = 2;
        }
    }
}