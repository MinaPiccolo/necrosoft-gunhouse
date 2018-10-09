using UnityEngine;

namespace Necrosoft
{
    [ExecuteInEditMode]
    public class SafeZone : MonoBehaviour
    {
        new Camera camera;
        Texture2D blank;

        enum TitleSafeSizeMode { Pixels, Percentage };
        [SerializeField] TitleSafeSizeMode sizeMode = TitleSafeSizeMode.Percentage;

        [SerializeField] Color innerColor = new Color(1, 1, 0, 0.75f);
        [SerializeField] Color outerColor = new Color(1, 0, 0, 0.75f);

        [SerializeField] [Range(0, 50)] int sizeX = 5;
        [SerializeField] [Range(0, 50)] int sizeY = 5;

        void OnEnable()
        {
            camera = GetComponent<Camera>();
        }

        void OnValidate()
        {
            if (sizeX < 0) { sizeX = 0; }
            if (sizeY < 0) { sizeY = 0; }

            if (sizeMode == TitleSafeSizeMode.Percentage) {
                if (sizeX > 25) { sizeX = 25; }
                if (sizeY > 25) { sizeY = 25; }
            }
        }

        void OnDestroy()
        {
            if (!blank) return;

            DestroyImmediate(blank);
            blank = null;
        }

        void OnGUI()
        {
            if (!camera) return;

            if (!blank) {
                blank = new Texture2D(1, 1);
                blank.SetPixel(0, 0, Color.white);
                blank.hideFlags = HideFlags.HideAndDontSave;
            }

            float w = camera.pixelWidth;
            float h = camera.pixelHeight;

            // Compute the actual sizes based on the size mode and our sizes
            float wMargin = 0;
            float hMargin = 0;

            switch (sizeMode)
            {
            case TitleSafeSizeMode.Percentage:
                wMargin = w * (sizeX / 100.0f);
                hMargin = h * (sizeY / 100.0f);
            break;
            case TitleSafeSizeMode.Pixels:
                // Clamp to 1/4 the screen size so we never overlap the other side
                wMargin = Mathf.Clamp(sizeX, 0, w / 4);
                hMargin = Mathf.Clamp(sizeY, 0, h / 4);
            break;
            }

            // Draw the outer region first
            GUI.color = outerColor;
            GUI.DrawTexture(new Rect(0, 0, w, hMargin), blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(0, h - hMargin, w, hMargin), blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(0, hMargin, wMargin, h - hMargin * 2), blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(w - wMargin, hMargin, wMargin, h - hMargin * 2), blank, ScaleMode.StretchToFill, true);

            // Then the inner region
            GUI.color = innerColor;
            GUI.DrawTexture(new Rect(wMargin, hMargin, w - wMargin * 2, hMargin), blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(wMargin, h - hMargin * 2, w - wMargin * 2, hMargin), blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(wMargin, hMargin * 2, wMargin, h - hMargin * 4), blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(w - wMargin * 2, hMargin * 2, wMargin, h - hMargin * 4), blank, ScaleMode.StretchToFill, true);
        }
    }
}
