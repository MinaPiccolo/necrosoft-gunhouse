using UnityEngine;

namespace Gunhouse.Menu
{
    public class Easing
    {
        static HSBColor aHSB = new HSBColor();
        static HSBColor bHSB = new HSBColor();
        static HSBColor easingColor = new HSBColor();

        public static Color Lerp(Color a, Color b, float t)
        {
            aHSB.FromColor(a);
            bHSB.FromColor(b);

            /*  check special case black (color.b == 0): interpolate neither hue nor saturation!
                check special case grey (color.s == 0): don't interpolate hue! */

            if (aHSB.b == 0) {
                easingColor.h = bHSB.h;
                easingColor.s = bHSB.s;
            }
            else if (bHSB.b == 0) {
                easingColor.h = aHSB.h;
                easingColor.s = aHSB.s;
            }
            else {
                if (aHSB.s == 0) {
                    easingColor.h = bHSB.h;
                }
                else if (bHSB.s == 0) {
                    easingColor.h = aHSB.h;
                }
                else {
                    // works around bug with LerpAngle
                    float angle = Mathf.LerpAngle(aHSB.h * 360f, bHSB.h * 360f, t);

                    while (angle < 0f) angle += 360f;
                    while (angle > 360f) angle -= 360f;

                    easingColor.h = angle / 360f;
                }
                easingColor.s = Mathf.Lerp(aHSB.s, bHSB.s, t);
            }

            easingColor.b = Mathf.Lerp(aHSB.b, bHSB.b, t);
            easingColor.a = Mathf.Lerp(aHSB.a, bHSB.a, t);

            //Color.HSVToRGB(easingColor.h, easingColor.s easingColor.b);

            return easingColor.ToColor();
        }
    }
}