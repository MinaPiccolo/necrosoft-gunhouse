using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WP8
//using Windows.Storage;
//using Windows.Storage.Streams;
//using System.Runtime.InteropServices.WindowsRuntime;
#endif

using Bounds = Necrosofty.Math.Bounds2;

namespace Gunhouse
{
    public class BinReader
    {
        public byte[] bytes;
        public int read_pos;

        #if UNITY_WP8
        byte[] ReadBytes(string f)
        {
            byte[] b;
            using (FileStream fs = File.OpenRead (f)) {
                int l = (int) fs.Length;
                b = new byte[l];
                fs.Read(b, 0, l);
            }
            return b;
        }
        #endif

        public BinReader(string name)
        {
            #if BUNDLED
            bytes = ((TextAsset)Downloader.Bundle.LoadAsset(Path.GetFileName(name))).bytes;
            #else
            if (name.Contains("://")) {
                //Debug.Log("Android load...");
                WWW www = new WWW(name);
                while(!www.isDone); // we shouldn't do this but the file are small...
                bytes = www.bytes;
            }
            else {
                #if UNITY_WP8
                bytes = ReadBytes(name);
                #else
                bytes = File.ReadAllBytes(name);
                #endif
            }

            #endif

            read_pos = 0;
        }

        public void skip(int n_bytes)
        {
            read_pos += n_bytes;
        }

        public int readChar()
        {
            return bytes[read_pos++];
        }

        public int readSignedChar()
        {
            return (sbyte)(bytes[read_pos++]);
        }

        public float readFloat()
        {
            read_pos += 4;

            return BitConverter.ToSingle(bytes, read_pos - 4);
        }

        public bool readBool()
        {
            return readChar() != 0;
        }

        public Vector2 readVector2()
        {
            var x = readFloat();
            var y = readFloat();

            return new Vector2(x, y);
        }

        public int readShort()
        {
            return readChar() | (readChar() << 8);
        }

        public int readInt()
        {
            return readChar() | (readChar() << 8) | (readChar() << 16) | (readChar() << 24);
        }

        public string readString()
        {
            int start = read_pos;

            while (bytes[read_pos] != 0) { read_pos++; }
            read_pos++;

            #if UNITY_WP8
            return System.Text.Encoding.UTF8.GetString(bytes, start, read_pos-start-1);
            #else
            return System.Text.Encoding.ASCII.GetString(bytes, start, read_pos-start-1);
            #endif
        }
    }

    public struct AtlasSprite
    {
        public String name;
        public Bounds bounds;
        public Vector2 center, size, tlmargin, brmargin;
    };

    public class Atlas
    {
        public int active_texture, max_textures;
        public GHTexture[] textures = new GHTexture[5];
        public GHTexture texture;
        public int n_sprites;
        public AtlasSprite[] sprites;
        public float scale_coefficient = 1;

        public static string txa_extension = ".txa.txt";
        public static string fnt_extension = ".fnt.txt";

        public Atlas(string png, Vector2 sprite_size, int z_order)
        {
            textures[0] = Textures.loadTexture(png, z_order);
            texture = textures[0];

            active_texture = 0;
            max_textures = 1;

            texture.z_order = z_order;
            for (int n = 0; n < max_textures; ++n) { textures[n].z_order = z_order; }

            Texture2D t = texture.texture();

            Vector2 dimensions = new Vector2(t.width / sprite_size.x, t.height / sprite_size.y);

            n_sprites = (int)System.Math.Ceiling(dimensions.x) * (int)System.Math.Ceiling(dimensions.y);
            sprites = new AtlasSprite[n_sprites];

            Vector2 size = new Vector2(1.0f / dimensions.x, 1.0f / dimensions.y);

            int i = 0;
            for(int y = 0; y < dimensions.y; ++y)
            {
                for(int x = 0; x < dimensions.x; ++x)
                {
                    sprites[i].center = new Vector2(0.5f, 0.5f);
                    sprites[i].size = sprite_size;
                    sprites[i].bounds = new Bounds(new Vector2(size.x * x * 1.0f, 1.0f - size.y * y * 1.0f),
                                                   new Vector2(size.x * ((x + 1) * 1.0f), 1.0f - size.y * ((y + 1) * 1.0f)));
                    sprites[i].bounds.uv_index = i;
                    i++;
                }
            }
        }

        public Atlas(string png, int z_order, float scale_coefficient_ = 1)
        {
            textures[0] = Textures.loadTexture(png, z_order);
            texture = textures[0];

            active_texture = 0;
            max_textures = 1;

            texture.z_order = z_order;
            for (int i = 0; i < max_textures; ++i) { textures[i].z_order = z_order; }

            Texture2D t = texture.texture();

            scale_coefficient = scale_coefficient_;
            n_sprites = 1;

            sprites = new AtlasSprite[1];

            sprites[0].bounds = new Bounds(new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));
            sprites[0].bounds.uv_index = 0;
            sprites[0].center = new Vector2(0.5f, 0.5f);
            sprites[0].size = new Vector2(t.width, t.height);
        }

        public Atlas(string png, string txa, int z_order, float scale_coefficient_ = 1)
        {
            textures[0] = Textures.loadTexture(png, z_order);
            texture = textures[0];

            active_texture = 0;
            max_textures = 1;

            texture.z_order = z_order;
            for (int i = 0; i < max_textures; ++i) { textures[i].z_order = z_order; }

            scale_coefficient = scale_coefficient_;

            BinReader tr = new BinReader(Path.Combine(Application.streamingAssetsPath, txa));
            if (txa.EndsWith(txa_extension)) { readTxa(tr); }
            else if (txa.EndsWith(fnt_extension)) { readFnt(tr); }
        }

        public Atlas(string def, string alt1, string alt2, string txa, int z_order,
                     float scale_coefficient_ = 1)
        {
            textures[0] = Textures.loadTexture(def, z_order);
            textures[1] = Textures.loadTexture(alt1, z_order, false);
            textures[2] = Textures.loadTexture(alt2, z_order, false);
            texture = textures[0];

            active_texture = 0;
            max_textures = 3;

            texture.z_order = z_order;
            for (int i = 0; i < max_textures; ++i) { textures[i].z_order = z_order; }

            scale_coefficient = scale_coefficient_;

            BinReader tr = new BinReader(Path.Combine(Application.streamingAssetsPath, txa));
            if (txa.EndsWith(txa_extension)) { readTxa(tr); }
            else if (txa.EndsWith(fnt_extension)) { readFnt(tr); }
        }

        public Atlas(string def, string alt1, string alt2, string alt3, string alt4,
                     string txa, int z_order, float scale_coefficient_ = 1)
        {
            textures[0] = Textures.loadTexture(def, z_order);
            textures[1] = Textures.loadTexture(alt1, z_order, false);
            textures[2] = Textures.loadTexture(alt2, z_order, false);
            textures[3] = Textures.loadTexture(alt3, z_order, false);
            textures[4] = Textures.loadTexture(alt4, z_order, false);
            texture = textures[0];

            active_texture = 0;
            max_textures = 5;

            texture.z_order = z_order;
            for (int i = 0; i < max_textures; ++i) { textures[i].z_order = z_order; }

            scale_coefficient = scale_coefficient_;

            BinReader tr = new BinReader(Path.Combine(Application.streamingAssetsPath, txa));
            if (txa.EndsWith(txa_extension)) { readTxa(tr); }
            else if (txa.EndsWith(fnt_extension)) { readFnt(tr); }
        }

        public void switchTexture(int set)
        {
            active_texture = Mathf.Clamp(set, 0, max_textures - 1);
            texture = textures[active_texture];
            texture.flag_z_order_reattach = true;
        }

        void readFnt(BinReader tr)
        {
            tr.skip(5);                     // header
            int block_size = tr.readInt();  // skip first block
            tr.skip(block_size);
            tr.skip(1);
            block_size = tr.readInt();
            tr.skip(2);                     // skip to atlas size

            float base_width = tr.readShort();
            float atlas_width = tr.readShort();
            float atlas_height = tr.readShort();

            tr.skip(block_size-8);          // skip rest of block 2
            tr.skip(1);                     // skip block 3
            block_size = tr.readInt();
            tr.skip(block_size);
            tr.skip(1);
            block_size = tr.readInt();

            n_sprites = block_size / 20;
            sprites = new AtlasSprite[n_sprites];

            for (int i = 0; i < n_sprites; ++i) {
                tr.skip(4);                     // char id
                int left = tr.readShort();
                int top = tr.readShort();
                float width = tr.readShort();
                float height = tr.readShort();

                sprites[i].size = new Vector2(width, height);
                sprites[i].bounds = new Bounds(new Vector2(left / atlas_width, 1.0f - top / atlas_height),
                                               new Vector2((left + width) / atlas_width, 1.0f - (top + height) / atlas_height));
                sprites[i].bounds.uv_index = i;

                int xoffset = tr.readShort();
                int yoffset = tr.readShort();
                int xadvance = tr.readShort();

                if (xoffset > 32767) { xoffset = xoffset - 65536; }
                if (yoffset > 32767) { yoffset = yoffset - 65536; }

                sprites[i].tlmargin = new Vector2(xoffset, yoffset);
                sprites[i].brmargin = new Vector2(xadvance, base_width);
                sprites[i].center = new Vector2(0.0f, base_width / height);

                tr.skip(2);
            }
        }

        void readTxa(BinReader tr)
        {
            float atlas_width  = tr.readInt();
            float atlas_height = tr.readInt();

            n_sprites = tr.readInt();
            sprites =  new AtlasSprite[n_sprites];

            for(int i = 0; i < n_sprites; ++i) {
                int left = tr.readInt();
                int top = tr.readInt();
                int width = tr.readInt();
                int height = tr.readInt();

                sprites[i].size = new Vector2(width, height);
                sprites[i].bounds = new Bounds(new Vector2(left / atlas_width, 1.0f - top / atlas_height),
                                               new Vector2((left + width) / atlas_width, 1.0f - (top + height) / atlas_height));
            }

            for (int i = 0; i < n_sprites; ++i) {
                sprites[i].name = tr.readString();
            }

            for(int i = 0; i < n_sprites; ++i) {
                int left_margin = tr.readInt();
                int top_margin = tr.readInt();
                int right_margin = tr.readInt();
                int bottom_margin = tr.readInt();

                int width  = (int)(sprites[i].bounds.Size.x * atlas_width);
                int height = (int)(sprites[i].bounds.Size.y * atlas_height);

                if (width == 0) { width++; }
                if (height == 0) { height++; }

                sprites[i].tlmargin = new Vector2(left_margin, top_margin);
                sprites[i].brmargin = new Vector2(right_margin, bottom_margin);
                sprites[i].center = new Vector2((left_margin + width - right_margin) / 2.0f / width,
                                                (top_margin + height - bottom_margin) / 2.0f / height);
            }
        }

        public void touch()
        {
            textures[active_texture].touch();
        }

        public void draw_outline(int sprite, Vector2 position, Vector2 scale, Vector4 color, float amp, bool outline)
        {
            touch();

            if (sprite >= sprites.Length) { return; }

            AppMain.renderer.addSprite(textures[active_texture], position,
                                       new Vector2(sprites[sprite].size.x / scale_coefficient * scale.x / 2.0f,
                                                   sprites[sprite].size.y / scale_coefficient * scale.y / 2.0f),
                                       sprites[sprite].bounds, color, amp, outline);
        }

        public void draw(int sprite, Vector2 position, Vector2 scale, Vector4 color, float amp = 0.0f)
        {
            touch();

            if (sprite >= sprites.Length) { return; }

            AppMain.renderer.addSprite(textures[active_texture], position,
                                       new Vector2(sprites[sprite].size.x / scale_coefficient * scale.x / 2.0f,
                                                   sprites[sprite].size.y / scale_coefficient * scale.y / 2.0f),
                                       sprites[sprite].bounds, color, amp, false);
        }

        public void draw(int sprite, Vector2 position, Vector2 scale, float rotation, Vector4 color)
        {
            touch();
            draw(sprite, position, sprites[sprite].center, scale, rotation, color);
        }

        public void draw(int sprite, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Vector4 color)
        {
            touch();

            AppMain.renderer.addSprite(texture, position, rotation,
                                       new Vector2((sprites[sprite].size.x / scale_coefficient) * scale.x / 2.0f,
                                                   (sprites[sprite].size.y / scale_coefficient) * scale.y / 2.0f),
                                       sprites[sprite].bounds, color, origin);
        }
    }

    public class SpriterAnimation
    {
        public SpriterKeyframe[] keyframes;
        public int length;
        public string name;

        public SpriterAnimation(BinReader br, int version)
        {
            name = br.readString();
            length = br.readInt();

            int keyframe_count = br.readInt();
            keyframes = new SpriterKeyframe[keyframe_count];

            for (int i = 0; i < keyframe_count; ++i) {
                keyframes[i] = new SpriterKeyframe(br, version);
            }
        }
    }

    public class SpriterKeyframe
    {
        public int time;
        public SpriterStamp[] stamps;

        public SpriterKeyframe(BinReader br, int version)
        {
            time = br.readInt();

            int stamp_count = br.readInt();
            stamps = new SpriterStamp[stamp_count];

            for (int i = 0; i < stamp_count; ++i) {
                stamps[i] = new SpriterStamp(br, version, i);
            }
        }
    }

    public struct SpriterStamp
    {
        public int atlas_id;
        public int z_next_frame;
        public Vector2 position, scale, origin;
        public float rotation;
        public int spin;

        public SpriterStamp(BinReader br, int version, int z_order)
        {
            atlas_id = br.readInt();

            if (version < 2) { z_next_frame = z_order; }
            else { z_next_frame = br.readInt(); }

            position = br.readVector2();

            if (version < 2) { scale = Vector2.one; }
            else { scale = br.readVector2(); }

            origin = br.readVector2();
            rotation = br.readFloat();
            spin = br.readSignedChar();
        }
    }

    public class SpriterSet
    {
        public Atlas atlas;
        public Dictionary<string, SpriterAnimation> animations;
        public int version;
        MatrixType object_camera = new MatrixType { matrix = new float[][] { new float[Matrix.size], new float[Matrix.size], new float[Matrix.size] } };//new MatrixType { matrix = new float[9] };
        MatrixType stamp_matrix = new MatrixType { matrix = new float[][] { new float[Matrix.size], new float[Matrix.size], new float[Matrix.size] } };//new MatrixType { matrix = new float[9] };

        public SpriterSet(string ssb, Atlas atlas_)
        {
            atlas = atlas_;
            BinReader br = new BinReader(Path.Combine(Application.streamingAssetsPath, ssb));

            #pragma warning disable
            string format = br.readString();
            version = br.readInt();

            int animation_count = br.readInt();
            animations = new Dictionary<string, SpriterAnimation>();

            for (int i = 0; i < animation_count; ++i) {
                SpriterAnimation a = new SpriterAnimation(br, version);
                animations[a.name] = a;
            }
        }

        public void touch()
        {
            atlas.touch();
        }

        public void draw(string animation, int frame, Vector2 position, Vector2 scale, float angle, Vector4 color)
        {
            object_camera = Matrix.Identity(object_camera);
            object_camera = Matrix.Scale(object_camera, scale);
            object_camera = Matrix.Rotate(object_camera, angle);
            object_camera = Matrix.Translation(object_camera, position);

            if(!animations.ContainsKey(animation)) { return; }

            SpriterAnimation anim = animations[animation];

            // determine which frames to interpolate between
            var frame_a = anim.keyframes[0];
            var frame_b = frame_a;
            if (anim.keyframes.Length > 1) {
                frame_b = anim.keyframes[1];
            }

            frame %= anim.length;
            for (int i = 1; i < anim.keyframes.Length; i++) {
                if (frame > anim.keyframes[i].time)
                {
                    frame_a = anim.keyframes[i];
                    if (i + 1 < anim.keyframes.Length)
                    {
                        frame_b = anim.keyframes[i + 1];
                    }
                    else
                    {
                        frame_b = anim.keyframes[0];
                    }
                }
            }

            // determine proportion of each frame
            float a_amt = 1, b_amt = 0;

            if (frame_a != frame_b)
            {
                int frame_b_time = frame_b.time;
                if (frame_b_time == 0)
                {
                    frame_b_time = anim.length;
                }

                b_amt = (float)(frame - frame_a.time) / (frame_b_time - frame_a.time);
                a_amt = 1.0f - b_amt;
            }

            // for each stamp in frame A
            for (int i = 0; i < frame_a.stamps.Length; ++i) {
                // figure out what stamp in frame B we're interpolating to
                SpriterStamp stamp_a = frame_a.stamps[i];
                SpriterStamp stamp_b = stamp_a;

                if (stamp_a.z_next_frame != -1 && frame_b.stamps.GetLength(0) > stamp_a.z_next_frame)
                {
                    stamp_b = frame_b.stamps[stamp_a.z_next_frame];
                }

                if (stamp_b.atlas_id != stamp_a.atlas_id)
                {
                    stamp_b = stamp_a;
                }

                // find stamp display transform properties
                int spin = stamp_a.spin;
                Vector2 pos = stamp_a.position * a_amt + stamp_b.position * b_amt;
                Vector2 origin = stamp_a.origin * a_amt + stamp_b.origin * b_amt;
                Vector2 stamp_scale = stamp_a.scale * a_amt + stamp_b.scale * b_amt;

                Vector2 stamp_size = atlas.sprites[stamp_a.atlas_id].tlmargin +
                                          atlas.sprites[stamp_a.atlas_id].size +
                                          atlas.sprites[stamp_a.atlas_id].brmargin;

                // find stamp rotation, correcting for spin direction
                float angle_a = stamp_a.rotation * (float)System.Math.PI / 180;
                float angle_b = stamp_b.rotation * (float)System.Math.PI / 180;

                if (spin == 1 && angle_b - angle_a < 0)
                {
                    angle_b += (float)System.Math.PI * 2;
                }

                if (spin == -1 && angle_b - angle_a > 0)
                {
                    angle_b -= (float)System.Math.PI * 2;
                }

                float rotation = angle_a * a_amt + angle_b * b_amt;

                // build stamp transform matrix
                stamp_matrix = Matrix.Identity(stamp_matrix);
                stamp_matrix = Matrix.Translation(stamp_matrix, new Vector2(-origin.x * stamp_size.x, - stamp_size.y + origin.y * stamp_size.y));
                stamp_matrix = Matrix.Scale(stamp_matrix, stamp_scale);
                stamp_matrix = Matrix.Rotate(stamp_matrix, -rotation);
                stamp_matrix = Matrix.Translation(stamp_matrix, new Vector2(pos.x, -pos.y));
                stamp_matrix = Matrix.Multiply(stamp_matrix, object_camera);

                // display!
                AppMain.renderer.addSprite(atlas.textures[atlas.active_texture],
                                           Matrix.Apply(stamp_matrix, Vector2.zero),
                                           Matrix.Apply(stamp_matrix, new Vector2(stamp_size.x, 0)),
                                           Matrix.Apply(stamp_matrix, stamp_size),
                                           Matrix.Apply(stamp_matrix, new Vector2(0, stamp_size.y)),
                                           atlas.sprites[stamp_a.atlas_id].bounds, color, 0, 0, false);
            }
        }
    }
}