using UnityEngine;
using UnityEngine.SceneManagement;
using Gunhouse.Credits;

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

            Input.controllerInput = GameObject.FindObjectOfType<PlayerInput>();

            AppMain.menuOverlay = GameObject.FindObjectOfType<MenuOverlay>();
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
            #if UNITY_ANDROID || UNITY_IOS
            if (AppMain.top_state is Game) { AppMain.top_state = new PauseState(AppMain.top_state); }
            #endif

            Platform.SavePlayerData();
        }

        void OnApplicationQuit()
        {
            Platform.SavePlayerData();
        }
        
        public static void LoadCreditsSceneAsync(bool autoMove)
        {
            instance.StartCoroutine(LoadCreditsSceneAsyncInternal(autoMove));
        }

        static System.Collections.IEnumerator LoadCreditsSceneAsyncInternal(bool autoMove)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)SceneIndex.Credits, LoadSceneMode.Additive);

            while (!asyncOperation.isDone) { yield return null; }

            /* NOTE(shane): this function is only used in two cases, end wave and end game.
                To save time I've just but this code here. */

            GameObject.FindObjectOfType<CreditsScene>().Display(autoMove);
        }

        public static bool HasAppStarted()
        {
            return instance != null;
        }
    }
}
