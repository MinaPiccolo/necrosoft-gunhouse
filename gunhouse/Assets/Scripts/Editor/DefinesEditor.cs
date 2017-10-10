using UnityEngine;
using UnityEditor;

namespace Necrosoft
{
    public class DefinesEditor : EditorWindow
    {
        /* N3DS, PS4, PSP2, Switch, Tizen, tvOS, WebGL, XboxOne already exist */
        static string[] platforms = { "BUNDLED",
                                      "LOADING_SCREEN", "LOADING_CREDITS",
                                      "TRACKING",
                                      "CONTROLLER_AND_TOUCH", "CONTROLLER", "TOUCH",
                                      "JUMP_STORE",
                                      "MACOS_STORE", "MACOS_STANDALONE",
                                      "STEAM_PC",
                                      "PLAY_STORE" };

        static bool[] defineEnable = new bool[platforms.Length];
        Vector2 scroll;

        [MenuItem("Game/Edit Defines")]
        static void Init()
        {
            DefinesEditor window = GetWindow<DefinesEditor>(false, "Defines", true);
            window.Show();
        }

        void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll, false, false);

            GUILayout.Space(10);

            for (int i = 0; i < platforms.Length; ++i) {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(platforms[i]);

                if (defineEnable[i] != EditorGUILayout.Toggle(defineEnable[i])) {
                    defineEnable[i] = !defineEnable[i];
                    EditorPrefs.SetBool(platforms[i], defineEnable[i]);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Update Defines", GUILayout.ExpandWidth(false))) {
                string defines = "";

                for (int i = 0; i < platforms.Length; ++i) {
                    if (!defineEnable[i]) continue;
                    defines += platforms[i] + ";";
                }

                Debug.Log("Update Defines: " + defines);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);

                for (int i = 0; i < platforms.Length; ++i) {
                    defineEnable[i] = EditorPrefs.GetBool(platforms[i], false);
                }
            }

            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        void OnFocus()
        {
            for (int i = 0; i < platforms.Length; ++i) {
                defineEnable[i] = EditorPrefs.GetBool(platforms[i], false);
            }
        }
    }
}