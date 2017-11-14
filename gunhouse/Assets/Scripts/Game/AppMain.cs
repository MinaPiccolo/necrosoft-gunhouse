using UnityEngine;
using System.Collections.Generic;

namespace Gunhouse
{
    public static class AppMain
    {
        public static GHRenderer renderer;
        public static List<Necrosofty.Input.TouchData> data;
        public static GameObject audio_object;
        public static Camera camera;

        public static Entity background;
        public static Vector2 vscreen;

        public static MenuOverlay menuOverlay;
        public static MenuTutorial tutorial;
        public static MenuAchievements menuAchievements;

        public static State top_state;

        public static Textures textures;

        public static int texture_order = 0;

        public static string bgmfilename;

        public static Shader shader;
        public static int frame = 0;
        public static GameObject sfx;

        public static int shake_int = -1;
        public static int shake_max = -1;
        public static int shake_dur = 0;
        public static int shake_time = 0;

        public static bool back;
        public static bool reset_scene = false;
        public static bool wipe_textures = false;
        //public static bool update_tile = false;
        //public static string update_text = "";

        public static Vector3 camera_pos;

        public static bool overrideDoorTimer;
        public static int overrideDoorOpenTime;

        #if UNITY_IOS && !UNITY_TVOS
        public static bool game_pad_active = false;
        #else
        public static bool game_pad_active = true;
        #endif

        public static Menu.MainMenu MainMenu;

        public static void Start()
        {
            GHInputUpdate();
            initialize();

            int seed = new System.Random().Next();

            Util.rng = new Util.UtilRandom(seed);
            background = new DrDogBackgroundDay();
            MoneyGuy.me = new MoneyGuy();
            AppMain.MainMenu.FadeInOut(true);
        }

        public static void initialize()
        {
            renderer = new GHRenderer();
            textures = new Textures();
            top_state = new MenuState();
        }

        public static void screenShake(int amount, int length)
        {
            shake_int = (int)(amount * .15f);
            if (shake_int > shake_max) { shake_max = shake_int; }
            shake_dur = length;
            shake_time = length;
        }

        public static void GHInputUpdate()
        {
            data = Necrosofty.Input.Touch.GetData(0);
            data = MonoInputFilter.filter(data);
        }

        public static void GHUpdate()
        {
            Input.tick(data);
            for (int i = 0; i < Input.touches.Count; ++i) {
                Touch t = Input.touches[i];
                Input.touches[i] = t;
            }

            if (Game.instance == null ||
                Game.instance.house.door_position == 1.0f) {
                background.tick();
            }
            top_state.tick();

            frame++;

            if (shake_time > 0) {
                Vector3 noise = new Vector3(Util.rng.NextFloat(0, shake_int) - (shake_int * 0.5f),
                                            Util.rng.NextFloat(0, shake_int) - (shake_int * 0.5f), 0.0f);
                camera.transform.position = camera_pos + noise;
                shake_time--;

                if (shake_time == 0) {
                    camera.transform.position = camera_pos;
                }
            }
        }

        static public long last_ram = 0;

        public static void draw()
        {
            if (reset_scene) {
                //Resources.UnloadUnusedAssets();
                reset_scene = false;
            }

            if (wipe_textures) {
                GHTexture.freeLRU();
                //Resources.UnloadUnusedAssets();
                System.GC.Collect();
                wipe_textures = false;

                return;
            }

            if (textures.stage_drdog_noon == null) { textures.loadTheRest(); }
            background.draw();

            top_state.draw();

            renderer.blit();
            renderer.clearScene();
        }
    }
}
