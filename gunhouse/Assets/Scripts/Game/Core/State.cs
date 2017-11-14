using System.Collections.Generic;
using UnityEngine;

using TouchStatus = Necrosofty.Input.TouchStatus;
using Math = System.Math;

namespace Gunhouse
{
    public struct Touch
    {
        public Vector2 position;
        public Vector2 last_position;
        public Vector2 initial_position;
        public Vector2 really_initial_position;
        public int id;

        public enum Status { UP, DOWN, MOVE }

        public Status status;
        public bool updated;
        public bool killed;

        public List<PuzzlePiece> right_group, left_group, both_groups;

        public void Dispose()
        {
            if (left_group != null)
            {
                left_group.Clear();
                left_group = null;
            }

            if (right_group != null)
            {
                right_group.Clear();
                right_group = null;
            }

            if (both_groups != null)
            {
                both_groups.Clear();
                both_groups = null;
            }
        }
    }

    public class Input
    {
        public static List<Touch> touches = new List<Touch>();

        public static string filename = null;
        public static int time = 0, playback_frame = 0;

        const int n_keys = 512, n_mouse_buttons = 7;
        public static bool[] keys, keys_down, keys_up;
        public static bool any_key_down;
        public static int last_key = -1;

        public static float[] cc =  new float[128];
        public static bool[] cc_changed = new bool[128];

        public static PlayerInput Pad;
        //public static PlayerActions Pad { get { return controllerInput.pad; } }

        public static void clearTouches()
        {
            for (int i = touches.Count - 1; i >= 0; --i) { touches[i].Dispose(); }
            touches.Clear();
        }

        public static void tick(List<Necrosofty.Input.TouchData> touches_in)
        {
            // initialize keys if needed
            if (keys == null)
            {
                keys      = new bool[n_keys];
                keys_down = new bool[n_keys];
                keys_up   = new bool[n_keys];
            }

            any_key_down = false;

            // update keys
            for(int i = 0; i<n_keys; i++)
            {
                bool prev = keys[i];
                keys[i] = UnityEngine.Input.GetKey((KeyCode)i);
                keys_down[i] =  keys[i] && !prev;
                keys_up[i] = !keys[i] &&  prev;

                if(keys_down[i])
                {
                    last_key = i;
                    if(i < 256) any_key_down = true;
                }
            }

            if (last_key != -1 && !keys[last_key]) last_key = -1;

            for(int i=0; i<cc.Length; i++)
            {
                float new_value = 0;//MidiJack.MidiMaster.GetKnob(0, i, 0.5f);
                cc_changed[i] = (new_value!=cc[i]);
                cc[i] = new_value;

//                if(cc_changed[i])
//                    Util.trace(i, " changed: ", cc[i]);
            }

            // flag to make sure we update all touches
            for(int i = 0; i < touches.Count; ++i)
            {
                Touch touch = touches[i];
                touch.updated = false;
                touches[i] = touch;
            }

            int touch_count = touches_in.Count;

            // for touches coming in
            for (int i = 0; i < touch_count; ++i)
            {
                Necrosofty.Input.TouchData touch_in = touches_in[i];

                bool handled = false;

                // if a touch with a matching id already exists, update it
                for (int j = 0; j < touches.Count; ++j)
                {
                    Touch touch = touches[j];
                    if (touch_in.ID == touch.id)
                    {
                        handled = true;
                        touch.last_position = touch.position;
                        touch.position = new Vector2(AppMain.vscreen.x - (touch_in.X + 0.5f) * AppMain.vscreen.x,
                                                     (touch_in.Y + 0.5f) * 544);
                        touch.updated = true;
                        touch.status = Touch.Status.MOVE;
                    }
                    touches[j] = touch;
                }

                // if not, add it
                if (!handled)
                {
                    Touch new_touch = new Touch();
                    new_touch.id = touch_in.ID;
                    new_touch.position = new Vector2(AppMain.vscreen.x - (touch_in.X + 0.5f) * AppMain.vscreen.x,
                                                     (touch_in.Y + 0.5f) * 544);
                    new_touch.last_position = new_touch.position;
                    new_touch.initial_position = new_touch.position;
                    new_touch.really_initial_position = new_touch.position;
                    new_touch.updated = true;
                    new_touch.status = Touch.Status.DOWN;
                    touches.Add(new_touch);
                }
            }

            // find unhandled touches and process them
            for (int i = 0; i < touches.Count; ++i)
            {
                Touch touch = touches[i];

                if (touch.updated) { continue; }

                if (touch.status != Touch.Status.UP)
                {
                    touch.status = Touch.Status.UP;
                    touches[i] = touch;

                    continue;
                }

                touches.RemoveAt(i);
                i--;
            }
        }
    }

    public class MonoInputFilter
    {
        static int output_touch_id = -1;
        static Necrosofty.Input.TouchData output_touch;

        public static List<Necrosofty.Input.TouchData> filter(List<Necrosofty.Input.TouchData> touches)
        {
            // if no existing touch, grab the first available
            if (output_touch_id == -1) {
                if (touches.Count > 0) {
                    output_touch = touches[0];
                    output_touch.Status = TouchStatus.Down;
                    output_touch_id = touches[0].ID;
                }
            }
            else { // update the existing touch
                bool found = false;
                for (int i = 0; i < touches.Count; ++i) {
                    if (touches[i].ID == output_touch_id) {
                        output_touch = touches[i];
                        found = true;
                    }
                }

                // didn't find it in the list! let's release the current touch
                if (!found) {
                    output_touch.Status = TouchStatus.Up;
                }
            }

            if (output_touch == null) { return new List<Necrosofty.Input.TouchData>(); }

            // if touch released, clear the current touch id
            if (output_touch.Status == TouchStatus.Up) {
                output_touch_id = -1;
            }

            // build the new touch "list"
            List<Necrosofty.Input.TouchData> output_touches = new List<Necrosofty.Input.TouchData>();
            if (output_touch_id != -1) {
                output_touches.Add(output_touch);
            }

            return output_touches;
        }
    }

    public class State
    {
        public List<Necrosofty.Input.TouchData> touches;
        public List<EntityGroup> entities;
        public State child_state = null;
        public bool tick_child_state = true, draw_child_state = true;
        public int title_position = Mathf.RoundToInt(AppMain.vscreen.x / 2.0f) + 200;

        public State()
        {
            entities = new List<EntityGroup>();
        }

        ~State()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (touches != null) {
                touches.Clear();
                touches = null;
            }

            if (entities != null) {
                for (int i = entities.Count - 1; i >= 0; --i) {
                    entities[i].Dispose();
                }

                entities.Clear();
                entities = null;
            }

            AppMain.reset_scene = true;
        }

        public virtual void tick()
        {
            if (tick_child_state && child_state != null) { child_state.tick(); }

            for (int i = 0; i < entities.Count; ++i) {
                if (AppMain.tutorial.hasFocus &&
                    (entities[i].id == EntityGroupID.Enemies ||
                     entities[i].id == EntityGroupID.EnemyBullets)) { continue; }

                if (entities[i].tickrate != 0 && Game.instance.time % entities[i].tickrate == 0) {
                    entities[i].tick();
                }
            }
        }

        public virtual void draw()
        {
            if (draw_child_state && child_state != null) {
                child_state.draw();
            }

            for (int i = 0; i < entities.Count; ++i) {
                if (!entities[i].autodraw) { continue; }

                entities[i].draw();
            }
        }
    }

    public class MetaState
    {
        public static int wave_number;
        public static WaveDefinition wave;

        public static float healing_coefficient;
        public static int hearts;
        public static float monster_armor_coefficient;
        public static float monster_attack_coefficient;

        public static bool end_game = false;

        public static bool hardcore_mode = false;
        public static int hardcore_score = 0;

        public static void reset(int start_wave = 0)
        {
            wave_number = start_wave;
            wave = null;
            resetWave();
            if (hardcore_mode) hearts = 2;
            else hearts = DataStorage.Hearts;
            setCoefficients(wave_number);
        }

        public static float logCurve(float level, float initial, float steepness, float amplification)
        {
            float s = 1 / (float)Math.Log((steepness + 1), 10);
            float b = (float)(Math.Log((level + s), 10) - Math.Log(s, 10));

            return initial + amplification * b;
        }

        public static void setCoefficients(int wave)
        {
            if (hardcore_mode)
            {
                healing_coefficient = logCurve(0,
                                               Difficulty.healing_upgrade_base,
                                               Difficulty.healing_upgrade_steepness,
                                               Difficulty.healing_upgrade_amplification);
            }
            else
            {
                healing_coefficient = logCurve(DataStorage.Healing,
                                               Difficulty.healing_upgrade_base,
                                               Difficulty.healing_upgrade_steepness,
                                               Difficulty.healing_upgrade_amplification);
            }

            if ((wave + 1) % 30 != 0) {
                monster_armor_coefficient = logCurve(wave,
                                                     Difficulty.monster_armor_initial,
                                                     Difficulty.monster_armor_steepness,
                                                     Difficulty.monster_armor_amplification);

                monster_attack_coefficient = logCurve(wave,
                                                      Difficulty.monster_attack_initial,
                                                      Difficulty.monster_attack_steepness,
                                                      Difficulty.monster_attack_amplification);
            }
            else {
                monster_armor_coefficient = logCurve(wave,
                                                     Difficulty.peter_armor_initial,
                                                     Difficulty.peter_armor_steepness,
                                                     Difficulty.peter_armor_amplification);

                monster_attack_coefficient = logCurve(wave,
                                                      Difficulty.peter_attack_initial,
                                                      Difficulty.peter_attack_steepness,
                                                      Difficulty.peter_attack_amplification);
            }

        }

        public static void resetWave(bool won = false)
        {
            if (won) {
                wave_number++;
            }

            setCoefficients(wave_number);

            wave = Difficulty.waves[(wave_number / 3) % Difficulty.waves.Length];
            wave.enemy_palette = wave.enemy_palette + Mathf.FloorToInt(wave_number / (3.0f * Difficulty.waves.Length));
            wave.reset();
        }
    }
}
