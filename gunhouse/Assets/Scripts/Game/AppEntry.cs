using UnityEngine;

namespace Gunhouse
{
    public class AppEntry : MonoBehaviour
    {
        [SerializeField] Shader spriteShader;
        [SerializeField] bool cursorVisible = true;

        static AppEntry instance;

        void Awake()
        {
            Platform.LoadPlayerData();

            Cursor.visible = cursorVisible;
            instance = this;

            Input.Pad = GameObject.FindObjectOfType<PlayerInput>();

            AppMain.MatchBonus = GameObject.FindObjectOfType<MatchBonus>();
            AppMain.MainMenu = GameObject.FindObjectOfType<Menu.MainMenu>();
            AppMain.tutorial = GameObject.FindObjectOfType<MenuTutorial>();
            AppMain.menuAchievements = GameObject.FindObjectOfType<MenuAchievements>();

            //AppMain.overrideDoorTimer = overrideDoorTimer;
            //AppMain.overrideDoorOpenTime = doorOpenTime;
        }

        void Start()
        {
            AppMain.shader = spriteShader;
            AppMain.sfx = gameObject;

            /* NOTE(shane): this is a virual point system created to minic the
                PSP Vita which this game was ported from. A lot of the numbers
                were hard coded for that screen, so this is how we got around
                that without rewriting everything. */
            AppMain.vscreen = new Vector2(965, 554);

            Camera tempCamera = GetComponent<Camera>();
            AppMain.camera_pos = tempCamera.transform.position;
            AppMain.camera = tempCamera;

            Necrosofty.Input.Touch.c = tempCamera;

            AppMain.Start();
            AppMain.back = false;

            AppMain.menuAchievements.AutoSignIn();
        }

        void Update()
        {
            AppMain.GHInputUpdate();
            if (Input.Pad.Escape.WasPressed) { AppMain.back = true; }
            AppMain.draw();
        }

        void FixedUpdate()
        {
            AppMain.GHUpdate();
        }

        void OnApplicationPause()
        {
            #if UNITY_ANDROID || UNITY_IOS || UNITY_SWITCH
            if (AppMain.top_state is Game) {
                AppMain.IsPaused = true;
                AppMain.top_state = new MenuState(Menu.MenuState.Pause, AppMain.top_state);
            }
            #endif

            Platform.SavePlayerData();
        }

        void OnApplicationQuit()
        {
            Platform.SavePlayerData();
        }

        public static bool HasAppStarted()
        {
            return instance != null;
        }
    }
}
