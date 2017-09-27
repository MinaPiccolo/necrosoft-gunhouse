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
        public static float background_fade = 0.0f;
        public static float background_fade_delta = 1.0f / 60;
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

        public static void Start()
        {
            GHInputUpdate();
            initialize();

            int seed = new System.Random().Next();

            Util.rng = new Util.UtilRandom(seed);
            background = new DrDogBackgroundDay();
            MoneyGuy.me = new MoneyGuy();
        }

        public static void initialize()
        {
            renderer = new GHRenderer();
            textures = new Textures();

//#if LOADING_SCREEN && !BUNDLED
            //top_state = new LoadState(() => { return new TitleState(MenuOptions.Title); }, 5);
//#else
            top_state = new TitleState(MenuOptions.Title);
//#endif
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

            #if LOADING_SCREEN
            if (!(top_state is LoadState || top_state is EndGameState)) {
            #else
            if (!(top_state is EndGameState)) {
            #endif

                background_fade += background_fade_delta;
                if (background_fade < 0) {
                    background_fade = 0;
                    background_fade_delta = 0;
                }

                if (background_fade > 1) {
                    background_fade = 1;
                    background_fade_delta = 0;
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

            #if LOADING_SCREEN
            if (!(top_state is LoadState) && !(top_state is CreditState)) {
            #endif

                if (textures.stage_drdog_noon == null) { textures.loadTheRest(); }

                background.draw();

                if (background_fade < 1.0f) {
                    textures.fade.draw(0, new Vector2((960 * 0.5f),(544 * 0.5f)), new Vector2(1000, 1000),
                                       new Vector4(0, 0, 0,(1 - background_fade)));
                }

            #if LOADING_SCREEN
            }
            #endif

            top_state.draw();

            renderer.blit();
            renderer.clearScene();
        }
    }
}
