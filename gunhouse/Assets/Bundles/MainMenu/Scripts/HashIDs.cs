using UnityEngine;

namespace Gunhouse.Menu
{
    public class HashIDs
    {
        public static IntroOuttro menu;

        public static void GenerateAnimationHashIDs()
        {
            menu = new IntroOuttro(Animator.StringToHash("Base Layer.intro"),
                                   Animator.StringToHash("Base Layer.outtro"));
        }

        public class IntroOuttro
        {
            public int Intro, Outtro;
            public IntroOuttro(int intro, int outtro) { Intro = intro; Outtro = outtro; }
        }

        #if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptsReloaded()
        {
            GenerateAnimationHashIDs();
        }
        #endif
    }
}