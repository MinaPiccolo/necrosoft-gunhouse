using System;
using System.Collections.Generic;
using UnityEngine;

using Bounds = Necrosofty.Math.Bounds2;

namespace Gunhouse
{
    public class GHRenderer
    {
        bool reset_bw;
        public int layer;
        public GHTexture[] texture_order = new GHTexture[150];
        MatrixType matrix = new MatrixType { matrix = new float[][] { new float[Matrix.size], new float[Matrix.size], new float[Matrix.size] } };
        MaterialPropertyBlock normal_mpb = new MaterialPropertyBlock();

        public class MeshInfo
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<Color>   colors   = new List<Color>();
            public List<Vector2> uvs      = new List<Vector2>();
            public List<Vector2> topleft  = new List<Vector2>();
            public List<Vector2> sizes    = new List<Vector2>();
            public List<int>     indices  = new List<int>();
            public Mesh          mesh     = new Mesh();
            public Material material;
            public int index = 0;
        }

        public class MeshInfoPair { public MeshInfo[] m = new MeshInfo[2] { new MeshInfo(), new MeshInfo() }; }
        Dictionary<GHTexture, MeshInfoPair> meshinfos = new Dictionary<GHTexture, MeshInfoPair>();

        public GHRenderer(int layer = 0)
        {
            this.layer = layer;
            normal_mpb.SetFloat("_OutlineSize", 0);
            normal_mpb.SetColor("_OutlineColor", Color.clear);
        }

        public MeshInfoPair addTexture(GHTexture texture, int z_order)
        {
            MeshInfoPair m;

            if (meshinfos.ContainsKey(texture)) {
                m = meshinfos[texture];

                /* NOTE(shane): this is required for pallet swapped assets */
                if (texture.flag_z_order_reattach) {
                    texture.flag_z_order_reattach = false;
                    texture_order[z_order] = texture;
                }
            }
            else {
                m = new MeshInfoPair();
                meshinfos[texture] = m;
                texture_order[z_order] = texture;
            }

            return m;
        }

        public void addSprite(GHTexture texture, Vector2 tl, Vector2 tr, Vector2 br,
                              Vector2 bl, Bounds uv, Vector4 color,
                              int stamp = 0, float amp = 0.0f, bool outline = false)
        {
            if (color.w == 0) { return; }

            texture.touch();

            float z = - 10 - texture.z_order;

            MeshInfoPair mp = addTexture(texture, texture.z_order);
            MeshInfo m = outline ? mp.m[1] : mp.m[0]; // outline is m[1], normal is m[0]

            m.material = texture.material; // this is a ref not a copy but that's okay

            float highlight_amt = 0;
            if (amp >= 1) {
                highlight_amt = color.x - 0.5f;
                color = Vector4.one;
            }

            // technically, topleft and sizes should not be vertex attributes, but uniforms.
            // however, the materialpropertyblock thingie doesn't seem to be working properly for some reason,
            // and this is pre-scissor so hey
            m.topleft.Add(tl);
            m.topleft.Add(tl);
            m.topleft.Add(tl);
            m.topleft.Add(tl);

            Vector2 sizes = br - tl;
            m.sizes.Add(sizes);
            m.sizes.Add(sizes);
            m.sizes.Add(sizes);
            m.sizes.Add(sizes);

            if (texture.scissored) { doScissor(ref uv, ref tl, ref tr, ref br, ref bl, ref color); }

            m.vertices.Add(new Vector3(tl.x, tl.y, z));
            m.vertices.Add(new Vector3(tr.x, tr.y, z));
            m.vertices.Add(new Vector3(br.x, br.y, z));
            m.vertices.Add(new Vector3(bl.x, bl.y, z));

            m.uvs.Add(uv.Point00);
            m.uvs.Add(uv.Point10);
            m.uvs.Add(uv.Point11);
            m.uvs.Add(uv.Point01);

            m.colors.Add(color);
            m.colors.Add(color);
            m.colors.Add(color);
            m.colors.Add(color);

            m.indices.Add(m.index);
            m.indices.Add(m.index+2);
            m.indices.Add(m.index+1);
            m.indices.Add(m.index);
            m.indices.Add(m.index+3);
            m.indices.Add(m.index+2);

            m.index += 4;

            // add highlight polygon to replicate amplification
            if (amp >= 1) {
                addSprite(AppMain.textures.highlight.texture, tl, tr, br, bl,
                          new Bounds(new Vector2(0, 0), new Vector2(1, 1)),
                          new Vector4(1, 1, 1, highlight_amt), stamp, 0, outline);
            }
        }

        public void doScissor(ref Bounds uv, ref Vector2 tl, ref Vector2 tr,
                              ref Vector2 br, ref Vector2 bl, ref Vector4 color)
        {
            Bounds suv = new Bounds(uv.Point00, uv.Point11);
            bool needs_scissor = false;

            if (Puzzle.grid_top > tl.y) {
                float ratio = (Puzzle.grid_top - tl.y) / (bl.y - tl.y);
                if (ratio < 0.0f) { ratio = 0.0f; }

                tl.y = Puzzle.grid_top;
                tr.y = Puzzle.grid_top;
                suv.Point00.y = uv.Point00.y + ratio * (uv.Point01.y - uv.Point00.y);
                suv.Point10.y = suv.Point00.y;
                needs_scissor = true;
            }

            float left = tl.x;
            float right = tr.x;
            bool swap = false;

            if (tr.x > tl.x) {
                left = tr.x;
                right = tl.x;
                swap = true;
            }

            float size = Math.Abs(tl.x - tr.x);

            if (right < Puzzle.grid_left && left > Puzzle.grid_left) {
                float ratio = Math.Abs(((Puzzle.grid_left - right) / size) * (uv.Point10.x - uv.Point00.x));

                right = Puzzle.grid_left;

                if (swap) {
                    tl.x = right;
                    bl.x = right;
                    float clip = uv.Point00.x + ratio;
                    suv.Point00.x = clip;
                    suv.Point01.x = clip;
                }
                else {
                    tr.x = right;
                    br.x = right;
                    float clip = uv.Point10.x - ratio;
                    suv.Point10.x = clip;
                    suv.Point11.x = clip;
                }

                needs_scissor = true;
            }
            else if (left <= Puzzle.grid_left) {
                color.w = 0.0f;
            }

            if (left > Puzzle.grid_left + Puzzle.piece_size * 3 &&
                right < Puzzle.grid_left + Puzzle.piece_size * 3) {
                float ratio = Math.Abs((((left - (Puzzle.grid_left + Puzzle.piece_size * 3)) / size)) * (uv.Point10.x - uv.Point00.x));

                left = Puzzle.grid_left + Puzzle.piece_size * 3;

                if (swap) {
                    tr.x = left;
                    br.x = left;
                    float clip = uv.Point10.x - ratio;
                    suv.Point10.x = clip;
                    suv.Point11.x = clip;
                }
                else {
                    tl.x = left;
                    bl.x = left;
                    float clip = uv.Point00.x + ratio;
                    suv.Point00.x = clip;
                    suv.Point01.x = clip;
                }

                needs_scissor = true;

            }
            else if (right >= Puzzle.grid_left + Puzzle.piece_size * 3) {
                color.w = 0.0f;
            }

            if (needs_scissor) uv = suv;
        }

        public void addSprite(GHTexture texture, Vector2 position, Vector2 size,
                              Bounds uv, Vector4 color, float amp, bool outline)
        {
            addSprite(texture,
                      position + new Vector2(-size.x, -size.y), position + new Vector2(size.x, -size.y),
                      position + new Vector2(size.x, size.y), position + new Vector2(-size.x, size.y),
                      uv, color, 0, amp, outline);
        }

        public void addSprite(GHTexture texture, Vector2 position, float angle,
                              Vector2 size, Bounds uv, Vector4 color, Vector2 center)
        {
            if (angle == 0 && center == Vector2.zero) {
                addSprite(texture,
                          position + new Vector2(-size.x, -size.y), position + new Vector2(size.x, -size.y),
                          position + new Vector2(size.x, size.y), position + new Vector2(-size.x, size.y),
                          uv, color);
            }
            else {
                matrix = Matrix.Identity(matrix);
                matrix = Matrix.Translation(matrix, -center);
                matrix = Matrix.Scale(matrix, size * 2);
                matrix = Matrix.Rotate(matrix, angle);
                matrix = Matrix.Translation(matrix, position);

                addSprite(texture, new Vector2(matrix[0, 2], matrix[1, 2]),
                          new Vector2(matrix[0, 0] + matrix[0, 2], matrix[1, 0] + matrix[1, 2]),
                          new Vector2(matrix[0, 0] + matrix[0, 1] + matrix[0, 2],
                                      matrix[1, 0] + matrix[1, 1] + matrix[1, 2]),
                          new Vector2(matrix[0, 1] + matrix[0, 2], matrix[1, 1] + matrix[1, 2]),
                          uv, color);
            }
        }

        public void clearScene()
        {
            foreach (KeyValuePair<GHTexture, MeshInfoPair> p in meshinfos) {
                for (int i = 0; i < p.Value.m.Length; ++i) {
                    p.Value.m[i].index = 0;
                    p.Value.m[i].vertices.Clear();
                    p.Value.m[i].uvs.Clear();
                    p.Value.m[i].topleft.Clear();
                    p.Value.m[i].sizes.Clear();
                    p.Value.m[i].colors.Clear();
                    p.Value.m[i].indices.Clear();
                }
            }
        }

        public void blit()
        {
            for (int i = 0; i < texture_order.Length; ++i) {
                if (texture_order[i] == null) { continue; }

                MaterialPropertyBlock mpb = null;
                MeshInfoPair mi = meshinfos[texture_order[i]];

                for (int j = 1; j >= 0; --j) {
                    var m = mi.m[j];
                    if (m.vertices.Count == 0) continue;

                    m.mesh.Clear();
                    m.mesh.SetVertices(m.vertices);
                    m.mesh.SetColors(m.colors);
                    m.mesh.SetUVs(0, m.uvs);
                    m.mesh.SetUVs(1, m.topleft);
                    m.mesh.SetUVs(2, m.sizes);
                    m.mesh.SetTriangles(m.indices, 0);

                    if (j == 1) { // outline
                        mpb = new MaterialPropertyBlock();
                        mpb.SetFloat("_OutlineSize", SpriteOutline.OutlineSize);
                        mpb.SetColor("_OutlineColor", SpriteOutline.CurrentColor);
                    }
                    else {
                        mpb = normal_mpb;
                    }

                    if (Game.instance != null && !texture_order[i].hud) {
                        m.material.SetFloat("_UniBW", Game.instance.house.visibleDoorPosition() < 1.0f ? 1.0f : 0.0f);
                        m.material.SetFloat("_UniPulseAmtX", Game.instance.house.visibleDoorPosition () < 1.0f ? (float)Math.Sin(Game.instance.time / 30.0f) * 6.0f : 0.0f);
                        m.material.SetFloat("_UniPulseAmtY", Game.instance.house.visibleDoorPosition () < 1.0f ? (float)Math.Sin(Game.instance.time / 40.0f) * 6.0f : 0.0f);
                    }
                    else {
                        m.material.SetFloat("_UniBW", 0.0f);
                        m.material.SetFloat("_UniPulseAmtX", 0.0f);
                        m.material.SetFloat("_UniPulseAmtY", 0.0f);
                    }

                    Graphics.DrawMesh(m.mesh, Matrix4x4.identity, m.material, layer, null, 0, mpb);
                }
            }
        }
    }
}
