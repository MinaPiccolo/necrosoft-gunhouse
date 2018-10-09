using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

/*
    The MIT License (MIT)

    Copyright (c) 2017, Nick Gravelyn

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

    Modified from https://github.com/nickgravelyn/UnityToolbag/tree/master/UnityConstants
*/

namespace Necrosoft.Editor
{
    public static class UnityConstantsGenerator
    {
        [MenuItem("Game/Generate UnityConstants.cs", false, 10000)]
        public static void Generate()
        {
            string filePath = string.Empty;
            foreach (var file in Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories)) {
                if (Path.GetFileNameWithoutExtension(file) == "UnityConstants") {
                    filePath = file;
                    break;
                }
            }

            if (string.IsNullOrEmpty(filePath)) {
                string directory = EditorUtility.OpenFolderPanel("Choose location for UnityConstants.cs", Application.dataPath, "");

                if (string.IsNullOrEmpty(directory)) return;

                filePath = Path.Combine(directory, "UnityConstants.cs");
            }

            using (var writer = new StreamWriter(filePath)) {
                writer.WriteLine("namespace UnityConstants");
                writer.WriteLine("{");

                #region Tags

                writer.WriteLine("    public static class Tags");
                writer.WriteLine("    {");
                foreach (var tag in UnityEditorInternal.InternalEditorUtility.tags) {
                    writer.WriteLine("        public const string {0} = \"{1}\";", MakeSafeForCode(tag), tag);
                }
                writer.WriteLine("    }");
                writer.WriteLine();

                #endregion

                #region SortingLayers

                writer.WriteLine("    public static class SortingLayers");
                writer.WriteLine("    {");
                foreach (var layer in SortingLayer.layers) {
                    writer.WriteLine("        public const int {0} = {1};", MakeSafeForCode(layer.name), layer.id);
                }
                writer.WriteLine("    }");
                writer.WriteLine();

                #endregion

                #region Layers

                writer.WriteLine("    public static class Layers");
                writer.WriteLine("    {");
                for (int i = 0; i < 32; i++) {
                    string layer = UnityEditorInternal.InternalEditorUtility.GetLayerName(i);
                    if (string.IsNullOrEmpty(layer)) continue;
                        
                    writer.WriteLine("        public const int {0} = {1};", MakeSafeForCode(layer), i);
                }
                writer.WriteLine("    }");
                writer.WriteLine();

                #endregion

                #region Culling Mask

                writer.WriteLine("    public static class Masks");
                writer.WriteLine("    {");

                for (int i = 0; i < 32; i++) {
                    string layer = UnityEditorInternal.InternalEditorUtility.GetLayerName(i);
                    if (string.IsNullOrEmpty(layer)) continue;
                    writer.WriteLine("        public const int {0} = 1 << {1};", MakeSafeForCode(layer), i);
                }

                writer.WriteLine("    }");
                writer.WriteLine();

                #endregion

                #region Scenes

                writer.WriteLine("    public static class Scenes");
                writer.WriteLine("    {");
                int sceneIndex = 0;
                foreach (var scene in EditorBuildSettings.scenes) {
                    if (!scene.enabled) continue;

                    writer.WriteLine("        public const int {0} = {1};", MakeSafeForCode(Path.GetFileNameWithoutExtension(scene.path)), sceneIndex);
                    sceneIndex++;
                }
                writer.WriteLine("    }");

                #endregion

                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }

        static string MakeSafeForCode(string str)
        {
            str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);
            if (char.IsDigit(str[0])) {
                str = "_" + str;
            }
            return str;
        }
    }
}
