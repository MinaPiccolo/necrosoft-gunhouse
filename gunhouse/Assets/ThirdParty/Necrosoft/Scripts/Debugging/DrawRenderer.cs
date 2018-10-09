using UnityEngine;

namespace Necrosoft
{
    public class DrawRenderer : MonoBehaviour
    {
        //Camera debugCamera;

        void OnPostRender()
        {
            //GL.PushMatrix();
            //GL.LoadProjectionMatrix(debugCamera.projectionMatrix);
            //GL.Begin(GL.LINES);
            //// draw
            //GL.End();
            //GL.PopMatrix();
        }
    }

    public static class Draw
    {
        public static void Circle(Vector2 center, float radius) { Circle(center, radius, Color.white, false); }
        public static void Circle(Vector2 center, float radius, bool fill) { Circle(center, radius, Color.white, fill); }
        public static void Circle(Vector2 center, float radius, Color color, bool fill = false)
        {

        }

        public static void Line(Vector3 from, Vector3 to) { Line(from, to, Color.white, false); }
        public static void Line(Vector3 from, Vector3 to, bool fill) { Line(from, to, Color.white, fill); }
        public static void Line(Vector3 from, Vector3 to, Color color, bool fill = false)
        {

        }

        public static void Rectangle(Rect rectangle) { Rectangle(rectangle, Color.white); }
        public static void Rectangle(Rect rectangle, bool fill) { Rectangle(rectangle, Color.white, fill); }
        public static void Rectangle(Rect rectangle, Color color, bool fill = false)
        {
            Vector2 topLeft = new Vector2(rectangle.xMin, rectangle.yMin);
            Vector2 topRight = new Vector2(rectangle.xMax, rectangle.yMin);
            Vector2 bottomLeft = new Vector2(rectangle.xMin, rectangle.yMax);
            Vector2 bottomRight = new Vector2(rectangle.xMax, rectangle.yMax);

            Line(topLeft, topRight, color);
            Line(topRight, bottomRight, color);
            Line(bottomRight, bottomLeft, color);
            Line(topLeft, bottomLeft, color);
        }
    }
}