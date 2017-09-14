using UnityEngine;

public class SpriteOutline : MonoBehaviour 
{
    public static Color[] colors = { new Color(0.9f, 0.94f, 0.64f, 0.75f),
                                     new Color(0.164f, 0.99f, 0.929f, 0.75f),
                                     new Color(0.964f, 0.66f, 0.984f, 0.75f),
                                     new Color(0.746f, 0.96f, 0.55f, 0.75f) };
    static int colorIndex;

    public static int OutlineSize = 5;
    float delay = 0.05f;
    float timer;

    public static Color CurrentColor { get { return colors[colorIndex]; } }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < delay) { return; }
        timer = 0;
        colorIndex = colorIndex == colors.Length - 1 ? 0 : colorIndex + 1;
    }
}