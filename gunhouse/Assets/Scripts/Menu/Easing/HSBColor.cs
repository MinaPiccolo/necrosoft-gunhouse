using UnityEngine;

namespace Gunhouse.Menu
{
    [System.Serializable]
    public struct HSBColor
    {
        public float h;
        public float s;
        public float b;
        public float a;

        public HSBColor(float h, float s, float b, float a) { this.h = h; this.s = s; this.b = b; this.a = a; }
        public HSBColor(float h, float s, float b) { this.h = h; this.s = s; this.b = b; a = 1.0f; }
        public HSBColor(Color color) { h = 0; s = 0; b = 0; a = color.a; FromColor(color); }

        public void FromColor(Color color)
        {
            h = 0; s = 0; b = 0; a = color.a;

            float color_r = color.r;
            float color_g = color.g;
            float color_b = color.b;

            float max = Mathf.Max(color_r, Mathf.Max(color_g, color_b));

            if (max <= 0) { return; }

            float min = Mathf.Min(color_r, Mathf.Min(color_g, color_b));
            float dif = max - min;

            if (max > min) {
                if (color_g == max) {
                    h = (color_b - color_r) / dif * 60f + 120f;
                }
                else if (color_b == max) {
                    h = (color_r - color_g) / dif * 60f + 240f;
                }
                else if (color_b > color_g) {
                    h = (color_g - color_b) / dif * 60f + 360f;
                }
                else {
                    h = (color_g - color_b) / dif * 60f;
                }

                if (h < 0) {
                    h = h + 360f;
                }
            }
            else {
                h = 0;
            }

            h *= 1f / 360f;
            s = (dif / max) * 1f;
            b = max;
        }
    }

    public static class HSBColorExtensions
    {
        static Color colorRGB = new Color();

        public static Color ToColor(this HSBColor colorHSB)
        {
            float r = colorHSB.b;
            float g = colorHSB.b;
            float b = colorHSB.b;

            if (colorHSB.s != 0) {
                float max = colorHSB.b;
                float dif = colorHSB.b * colorHSB.s;
                float min = colorHSB.b - dif;

                float h = colorHSB.h * 360f;

                if (h < 60f) {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                }
                else if (h < 120f) {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                }
                else if (h < 180f) {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f) {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                }
                else if (h < 300f) {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                }
                else if (h <= 360f) {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                }
                else {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }

            colorRGB.r = Mathf.Clamp01(r);
            colorRGB.g = Mathf.Clamp01(g);
            colorRGB.b = Mathf.Clamp01(b);
            colorRGB.a = colorHSB.a;

            return colorRGB;
        }
    }
}