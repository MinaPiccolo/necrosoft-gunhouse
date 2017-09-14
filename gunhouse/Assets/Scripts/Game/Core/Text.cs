using System.Text;
using UnityEngine;

namespace Gunhouse
{
    public static class Text
    {
        static StringBuilder builder = new StringBuilder(200);

        public static void Draw(Vector2 position, string text, Vector2 scale, Vector4 color)
        {
            Draw(position, text, scale, color, color, 0, false);
        }

        public static void Draw(Vector2 position, string text,
                                Vector2 scale, Vector4 color,
                                Vector4 alt_color, int amount, bool from_end = false)
        {
            scale.x = -scale.x;
            var origin = position;

            int swap_index = from_end ? text.Length - amount : amount;

            for (int i = 0; i < text.Length; ++i)
            {
                if (text[i] == '\n') {
                    position.x = origin.x;
                    position.y += 44 * scale.y;
                }

                int index = text[i] - ' ';
                if (index >= 0 && index < 96) {
                    float y_pos = AppMain.textures.font.sprites[index].tlmargin.y;
                    float x_pos = AppMain.textures.font.sprites[index].tlmargin.x;
                    float x_adv = AppMain.textures.font.sprites[index].brmargin.x;

                    AppMain.textures.font.draw(index, position +
                                               new Vector2(-5.0f + x_pos * scale.x, 5.0f + y_pos * scale.y),
                                               new Vector2(scale.x * 0.9f, scale.y * 1.0f),
                                               0, (from_end ? i > swap_index : i < swap_index) ? alt_color : color);

                    position.x += x_adv * scale.x * 0.9f;
                }
            }
        }

        public static Vector2 Size(Atlas font, string text)
        {
            float line_width = 0.0f;
            Vector2 size = new Vector2(0, 1);

            for (int i = 0; i < text.Length; ++i) {
                if (text[i] != '\n') {
                    int index = text[i] - ' ';
                    if (index >= 0 && index < 96) {
                        line_width += font.sprites[index].brmargin.x;
                    }
                    size.x = Mathf.Max(size.x, line_width);
                }
                else {
                    line_width = 0.0f;
                    size.y++;
                }
            }

            return new Vector2(size.x, size.y * 44);
        }

        public static string Wrap(string text, int width = 70)
        {
            if (text.Contains("\n")) {
                string output = "";
                string[] lines = text.Split(new char[] { '\n' });
                for (int i = 0; i < lines.Length; ++i) {
                    output += Wrap(lines[i], width) + "\n";
                }
                return output.TrimEnd(new char[] { '\n' });
            }

            int column = 0;
            builder.Length = 0;

            string[] words = text.Split(new char[] { ' ' });
            for (int i = 0; i < words.Length; ++i) {

                if (column + words[i].Length >= width) {
                    column = 0;
                    builder.Append("\n");
                }
                builder.Append(words[i]).Append(" ");
                column += words[i].Length + 1;
            }
            return builder.ToString();
        }

        public static int[] IntAsArray(int value)
        {
            /* NOTE(shane): think should be a queue in our case */
            System.Collections.Generic.Stack<int> numbers = new System.Collections.Generic.Stack<int>();

            for (; value > 0; value /= 10) numbers.Push(value % 10);

            return numbers.ToArray();
        }
    }
}
