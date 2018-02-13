using System.Text;
using UnityEngine;

namespace Gunhouse
{
    public static class Text
    {
        //public static void Draw(Vector2 position, string text, Vector2 scale, Vector4 color)
        //{
        //    Draw(position, text, scale, color, color, 0, false);
        //}

        //public static void Draw(Vector2 position, string text,
        //                        Vector2 scale, Vector4 color,
        //                        Vector4 alt_color, int amount, bool from_end = false)
        //{
        //    scale.x = -scale.x;
        //    var origin = position;

        //    int swap_index = from_end ? text.Length - amount : amount;

        //    for (int i = 0; i < text.Length; ++i)
        //    {
        //        if (text[i] == '\n') {
        //            position.x = origin.x;
        //            position.y += 44 * scale.y;
        //        }

        //        int index = text[i] - ' ';
        //        if (index >= 0 && index < 96) {
        //            float y_pos = AppMain.textures.font.sprites[index].tlmargin.y;
        //            float x_pos = AppMain.textures.font.sprites[index].tlmargin.x;
        //            float x_adv = AppMain.textures.font.sprites[index].brmargin.x;

        //            AppMain.textures.font.draw(index, position +
        //                                       new Vector2(-5.0f + x_pos * scale.x, 5.0f + y_pos * scale.y),
        //                                       new Vector2(scale.x * 0.9f, scale.y * 1.0f),
        //                                       0, (from_end ? i > swap_index : i < swap_index) ? alt_color : color);

        //            position.x += x_adv * scale.x * 0.9f;
        //        }
        //    }
        //}
    }
}
