using System.Collections;
using UnityEngine;

namespace Gunhouse
{
    public static class Util
    {
        public class UtilRandom : System.Random
        {
            public UtilRandom(int seed) : base(seed) { }

            public float NextFloat(float min = 0, float max = 1)
            {
                return (float)(NextDouble()*(max-min) + min);
            }
        }

        public static UtilRandom rng;

        public static float distanceToLineSegment(Vector2 p, Vector2 v, Vector2 w, out Vector2 closest)
        {
            float l2 = (v - w).sqrMagnitude;
            if (l2 == 0)
            {
                closest = v;
                return (p - closest).magnitude;
            }

            float t = Vector2.Dot(p - v, w - v) / l2;

            if (t < 0)
            {
                closest = v;
            }
            else if (t > 1)
            {
                closest = w;
            }
            else
            {
                closest = v + (w - v) * t;
            }

            return (p-closest).magnitude;
        }

        public static bool lineSegmentIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            Vector2 e = b - a, f = d - c;
            Vector2 p = new Vector2(-e.y, e.x);
            float h = Vector2.Dot(a - c, p) / Vector2.Dot(f, p);
            if (h < 0 || h == 1) { return false; }

            intersection = c + f * h;

            return true;
        }

        public static int sign(float n)
        {
            if (n > 0) return 1;
            if (n < 0) return -1;
            return 0;
        }

        public static Color colorFromTriplet(int color, float alpha=1)
        {
            return new Color(((color>>16)&0xff)/255.0f,
                             ((color>>8)&0xff)/255.0f, 
                             (color&0xff)/255.0f,
                             alpha);
        }

        public static void trace(params object[] args)
        {
            string s = "";
            foreach(object o in args)
            {
                if (o == null) s += "(null) ";
                else s += o.ToString() + " ";
            }

            while(s.Length > 16000)
            {
                Debug.Log(s.Substring(0, 16000));
                s = s.Substring(16000);
            }

            Debug.Log(s);
        }

        static Hashtable repeat_infos;

        public static T keyRepeat<T>(string name, T zero, int initial_timeout, int repeat_timeout, T current)
        {
            if (repeat_infos == null) repeat_infos = new Hashtable();

            RepeatInfo<T> r;
            if (repeat_infos.ContainsKey(name)) { r = (RepeatInfo<T>)repeat_infos[name]; }
            else { r = new RepeatInfo<T>(zero, initial_timeout, repeat_timeout); }

            T value = r.repeat(current);
            repeat_infos[name] = r;

            return value;
        }

        struct RepeatInfo<T>
        {
            T current, zero;
            int timeout, initial_timeout, repeat_timeout;

            public RepeatInfo(T zero, int initial_timeout, int repeat_timeout)
            {
                this.zero = zero;
                current = zero;
                timeout = 0;
                this.initial_timeout = initial_timeout;
                this.repeat_timeout = repeat_timeout;
            }

            public T repeat(T input)
            {
                if (input.Equals(zero))
                {
                    current = zero;
                    timeout = 0;
                    return zero;
                }

                if (!input.Equals(current))
                {
                    current = input;
                    timeout = initial_timeout;
                    return current;
                }

                if (--timeout <= 0)
                {
                    timeout = repeat_timeout;
                    return current;
                }

                return zero;
            }
        }

        public static float angle(Vector2 v)
        {
            return (float)System.Math.Atan2(v.y, v.x);
        }

        public static int clamp(int n, int min, int max)
        {
            if (n < min) { return min; }
            if (n > max) { return max; }

            return n;
        }

        public static float clamp(float n, float min, float max)
        {
            if (n < min) { return min; }
            if (n > max) { return max; }

            return n;
        }

        public static Vector2 fromPolar(float angle, float distance)
        {
            return new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * distance;
        }

        public static float smoothStep(float x)
        {
            return x * x * (3 - 2 * x);
        }
    }
}
