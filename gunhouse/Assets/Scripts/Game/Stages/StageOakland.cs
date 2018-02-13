using System;
using UnityEngine;

namespace Gunhouse
{
    public class StageOaklandBackgroundDay : Entity
    {
        Vector2 scaleAmount = new Vector2(-0.525f, 0.525f);
        Vector2 scaleAnchor = new Vector2(0.4f, 0.4f);

        public const int n_clouds = 2;
        public Vector4[] clouds = new Vector4[n_clouds];
        Vector2 groundPosition = (AppMain.vscreen * 0.5f) + new Vector2(0, -10);

        public StageOaklandBackgroundDay()
        {
            for (int i = 0; i < n_clouds; i++)
                clouds[i] = new Vector4(Util.rng.NextFloat(0, AppMain.vscreen.x),
                                        Util.rng.NextFloat(0, 200),
                                        Util.rng.NextFloat(0.025f, 0.1f),
                                        0);

            #if !UNITY_PS4 || !UNITY_PSP2
            groundPosition.y += 140;
            #endif
        }

        public override void tick()
        {
            for (int i = 0; i < n_clouds; ++i) {
                clouds[i].x += clouds[i].z;
                clouds[i].y += clouds[i].w;
                if (clouds[i].x < -200) { clouds[i].x = AppMain.vscreen.x + 200; }
            }
        }

        public virtual Atlas atlas() { return AppMain.textures.stage_oakland_noon; }

        public override void draw()
        {
            if (AppMain.DisplayAnchor) {
                int sprite = atlas() == AppMain.textures.stage_oakland_noon ?
                             (int)stage_oakland_anchors.Sprites.noon_anchor :
                             atlas() == AppMain.textures.stage_oakland_dusk ?
                             (int)stage_oakland_anchors.Sprites.dusk_anchor : 
                             (int)stage_oakland_anchors.Sprites.night_anchor;

                AppMain.textures.stage_oakland_anchors.draw(sprite,
                                                            new Vector2(140, 480),
                                                            scaleAnchor, Vector4.one);
            }

            atlas().draw((int)stage_oakland_noon.Sprites.background,
                         AppMain.vscreen * 0.5f + new Vector2(0, -100), scaleAmount, Vector4.one);

            atlas().draw((int)stage_oakland_noon.Sprites.cloud_0, clouds[0], scaleAmount, Vector4.one);
            atlas().draw((int)stage_oakland_noon.Sprites.cloud_1, clouds[1], scaleAmount, Vector4.one);

            atlas().draw((int)stage_oakland_noon.Sprites.ground, groundPosition, scaleAmount, Vector4.one);
        }
    }

    public class StageOaklandBackgroundDusk : StageOaklandBackgroundDay { public override Atlas atlas() { return AppMain.textures.stage_oakland_dusk; } }
    public class StageOaklandBackgroundNight : StageOaklandBackgroundDay { public override Atlas atlas() { return AppMain.textures.stage_oakland_night; } }
}
