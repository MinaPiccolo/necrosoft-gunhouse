using System.Collections.Generic;
using UnityEngine;
using Gunhouse;

namespace Gunhouse
{
    public class GHTexture
    {
        public bool hud = false;
        public bool scissored = false;

        public Texture2D texture2d = null;
        public int last_touched = 0;
        public Material material;

        public int order;
        public string filename;
        public int hash;
        public bool mipmap;
        public bool resource = false;
        public int pf;

        static List<GHTexture> textures = new List<GHTexture>();
        static public int touch_ticker;

        static public bool build_cache = false;

        public int z_order;
        public bool flag_z_order_reattach;

        public GHTexture(string filename_, bool mipmap_)
        {
            order = AppMain.texture_order++;
            filename = filename_;
            hash = filename.GetHashCode();
            mipmap = mipmap_;
            textures.Add(this);
        }

        public GHTexture(GHTexture to_clone)
        {
            hud = to_clone.hud;
            scissored = to_clone.hud;
            texture2d = to_clone.texture2d;
            last_touched = to_clone.last_touched;
            filename = to_clone.filename;
            mipmap = to_clone.mipmap;
            pf = to_clone.pf;
        }

        public Texture2D texture()
        {
            string res = (resource ? filename : filename.Remove(filename.Length - 4));
            texture2d = null;

            texture2d = Resources.Load<Texture2D>(res);

            #if BUNDLED
            /* NOTE(shane): be careful with this function, touch(); can cause
                an infinate loop if the texture isn't found! */
            if (texture2d == null) { texture2d = (Texture2D)Downloader.Bundle.LoadAsset(System.IO.Path.GetFileName(res)); }
            #endif

            touch();

            return texture2d;
        }

        public void free()
        {
            texture2d = null;
            material = null;
        }

        public void touch()
        {
            last_touched = AppMain.frame;

            if (texture2d == null) { texture(); }

            if (material == null) {
                material = new Material(AppMain.shader);
                material.mainTexture = texture2d;
            }
        }

        static public void freeLRU()
        {
            for (int i = textures.Count - 1; i >= 0; --i) {
                if (textures[i].texture2d != null) { textures[i].free(); }
            }
        }
    }

    public class Textures
    {
        #region Field Properties

        #if LOADING_SCREEN
        public Atlas loading;
        #endif

        public Atlas arrow, pick_a_day;
        public Atlas ui, hud, house, housebits, elements;
        public Atlas door, pickups;

        public Atlas minion, orphan, circle, explosion, gunpoof, block, block_logo, cracks;
        public Atlas title, taptobegin;
        public Atlas font, clouds, cloudpuff;
        public Atlas gun, bullets, dot, highlight, fade;
        public Atlas store_board, store;
        public Atlas shadowblob, shadowblob2;

        public Atlas counter_money, counter_time;
        public Atlas money_guy;

        public SpriterSet cat;

        public SpriterSet lightning_match;

        public Atlas specialgun;
        public Atlas carrotgun, dragongun, penguingun, skullgun, lasergun;
        public Atlas beachballgun, boomeranggun, flamegun, lightninggun;
        public Atlas laserbullet, veggies, veggie_special, penguinbullet, dragonbullet, skullbullet, gumballs;
        public Atlas beachball;
        public SpriterSet forkspecial, lightning_strike, boomerang, flames;
        public Atlas dragon_special_derp, dragon_special_angry;
        public Atlas laserspecial;

        public Atlas stage_oakland_noon, stage_oakland_dusk, stage_oakland_night;
        public Atlas stage_skeleton_noon, stage_skeleton_dusk, stage_skeleton_night;
        public Atlas stage_penguin_noon, stage_penguin_dusk, stage_penguin_night;
        public Atlas stage_drdog_noon, stage_drdog_dusk, stage_drdog_night;
        public Atlas stage_drhoot_noon, stage_drhoot_dusk, stage_drhoot_night;
        public Atlas stage_pyramid_noon, stage_pyramid_dusk, stage_pyramid_night;
        public Atlas stage_space;
        public Atlas stage_skeleton_anchors, stage_penguin_anchors, stage_drdog_anchors,
                     stage_drhoot_anchors, stage_pyramid_anchors, stage_space_anchor, stage_oakland_anchors;

        public SpriterSet drdog, drdogbullet, drdogminiboss, drdogflying, skeletonkingflying;
        public SpriterSet skeletonking, skeletonkingminion, skeletonkingminiboss, skeletonkingprojectile;
        public SpriterSet penguintank, penguinflying, penguinminion, penguinboss;

        public SpriterSet drhoottank, drhoottankbullet, drhootminion, drhootminionbullet;
        public SpriterSet drhootfly, drhootflybullet, drhoot, drhootbranch;
        public Atlas drhootnest;

        public SpriterSet drdogminion2, drdogminion2bullet;
        public SpriterSet drhootminion2, drhootminion2bullet;
        public SpriterSet skelkingminion2, skelkingminion2bullet;
        public SpriterSet penguinflying2, penguinflying2bullet;
        public SpriterSet icecreamminion;
        public SpriterSet icecreamflyer, icecreamflyerbullet;
        public SpriterSet icecreamflyer2, icecreamflyer2bullet;
        public SpriterSet icecreamboss, icecreambossbullet;
        public SpriterSet icecreamtank, icecreamtankbullet;

        public SpriterSet peterboss, peterbossbullet;

        public Atlas penguintankbullet, penguinflyingbullet, penguinminionbullet, penguinbossbullet;

        #endregion

        public static GHTexture loadTexture(string name, int z_order, bool upd = true)
        {
            var result = new GHTexture("" + name, false);
            if (upd) { AppMain.renderer.addTexture(result, z_order); }

            return result;
        }

        public Textures()
        {
            #if LOADING_SCREEN
            loading = new Atlas("loading.png", "loading.txa.txt", 0);
            #endif

            fade = new Atlas("fade_white.png", 1);
            font = new Atlas("font.png", "font.fnt.txt", 1);
        }

        public void loadTheRest()
        {
            int z_order = 0;

            /* NOTE(shane): The order in which these are called decides the z order */

            #region txa

            stage_oakland_noon = new Atlas("stage_oakland_noon.png", "stage_oakland_noon.txa.txt", z_order++);
            stage_oakland_dusk = new Atlas("stage_oakland_dusk.png", "stage_oakland_dusk.txa.txt", z_order++);
            stage_oakland_night = new Atlas("stage_oakland_night.png", "stage_oakland_night.txa.txt", z_order++);
            stage_skeleton_noon = new Atlas("stage_skeleton_noon.png", "stage_skeleton_noon.txa.txt", z_order++);
            stage_skeleton_dusk = new Atlas("stage_skeleton_dusk.png", "stage_skeleton_dusk.txa.txt", z_order++);
            stage_skeleton_night = new Atlas("stage_skeleton_night.png", "stage_skeleton_night.txa.txt", z_order++);
            stage_penguin_noon = new Atlas("stage_penguin_noon.png", "stage_penguin_noon.txa.txt", z_order++);
            stage_penguin_dusk = new Atlas("stage_penguin_dusk.png", "stage_penguin_dusk.txa.txt", z_order++);
            stage_penguin_night = new Atlas("stage_penguin_night.png", "stage_penguin_night.txa.txt", z_order++);
            stage_drdog_noon = new Atlas("stage_drdog_noon.png", "stage_drdog_noon.txa.txt", z_order++);
            stage_drdog_dusk = new Atlas("stage_drdog_dusk.png", "stage_drdog_dusk.txa.txt", z_order++);
            stage_drdog_night = new Atlas("stage_drdog_night.png", "stage_drdog_night.txa.txt", z_order++);
            stage_drhoot_noon = new Atlas("stage_drhoot_noon.png", "stage_drhoot_noon.txa.txt", z_order++);
            stage_drhoot_dusk = new Atlas("stage_drhoot_dusk.png", "stage_drhoot_dusk.txa.txt", z_order++);
            stage_drhoot_night = new Atlas("stage_drhoot_night.png", "stage_drhoot_night.txa.txt", z_order++);
            stage_pyramid_noon = new Atlas("stage_pyramid_noon.png", "stage_pyramid_noon.txa.txt", z_order++);
            stage_pyramid_dusk = new Atlas("stage_pyramid_dusk.png", "stage_pyramid_dusk.txa.txt", z_order++);
            stage_pyramid_night = new Atlas("stage_pyramid_night.png", "stage_pyramid_night.txa.txt", z_order++);
            stage_space = new Atlas("stage_space.png", "stage_space.txa.txt", z_order++);

            forkspecial = new SpriterSet("fork-special.ssb.txt",
                                         new Atlas("fork-special.png", "fork-special.txa.txt", z_order++));

            #endregion

            shadowblob = new Atlas("shadowblob.png", new Vector2(280, 88), z_order++);
            shadowblob.texture.hud = true;

            #region txa

            peterboss = new SpriterSet("peter_boss.ssb.txt", new Atlas("peter_boss.png",
                                                                           "peter_boss-color1.png",
                                                                           "peter_boss-color2.png",
                                                                           "peter_boss-color3.png",
                                                                           "peter_boss-color4.png",
                                                                           "peter_boss.txa.txt", z_order++));

            drhootminion2 = new SpriterSet("drhoot-minion-2.ssb.txt",
                                           new Atlas("drhoot-minion-2.png",
                                                     "drhoot-minion-2-blue.png",
                                                     "drhoot-minion-2-orange.png",
                                                     "drhoot-minion-2.txa.txt", z_order++));
            drdogminion2 = new SpriterSet("drdog-minion-2.ssb.txt",
                                          new Atlas("drdog-minion-2.png",
                                                    "drdog-minion-2-blue.png",
                                                    "drdog-minion-2-green.png",
                                                    "drdog-minion-2.txa.txt", z_order++));
            skelkingminion2 = new SpriterSet("skelking-minion-2.ssb.txt",
                                             new Atlas("skelking-minion-2.png",
                                                       "skelking-minion-2-orange.png",
                                                       "skelking-minion-2-blue.png",
                                                       "skelking-minion-2.txa.txt", z_order++));

            icecreamboss = new SpriterSet("icecream-boss.ssb.txt",
                                          new Atlas("icecream-boss.png",
                                                    "icecream-boss-blue.png",
                                                    "icecream-boss-orange.png",
                                                    "icecream-boss.txa.txt", z_order++));
            penguinboss = new SpriterSet("penguin-boss.ssb.txt",
                                           new Atlas("penguin-boss.png",
                                                     "penguin-boss-blue.png",
                                                       "penguin-boss-orange.png",
                                                     "penguin-boss.txa.txt", z_order++));

            drdog = new SpriterSet("drdog.ssb.txt",
                                   new Atlas("drdog.png",
                                             "drdog-blue.png",
                                             "drdog-green.png",
                                             "drdog.txa.txt", z_order++, 8556.0f / 2048));
            skeletonking = new SpriterSet("skeletonking.ssb.txt",
                                          new Atlas("skeletonking.png",
                                                    "skeletonking.txa.txt", z_order++));
            drdogminiboss = new SpriterSet("drdog_miniboss.ssb.txt",
                                           new Atlas("drdog_miniboss.png",
                                                     "drdog_miniboss-blue.png",
                                                     "drdog_miniboss-green.png",
                                                     "drdog_miniboss.txa.txt", z_order++));
            penguintank = new SpriterSet("penguin-tank.ssb.txt",
                                         new Atlas("penguin-tank.png",
                                                   "penguin-tank-blue.png",
                                                   "penguin-tank-orange.png",
                                                   "penguin-tank.txa.txt", z_order++));
            icecreamtank = new SpriterSet("icecream-tank.ssb.txt",
                                          new Atlas("icecream-tank.png",
                                                    "icecream-tank-blue.png",
                                                    "icecream-tank-orange.png",
                                                    "icecream-tank.txa.txt", z_order++));
            drdogflying = new SpriterSet("drdog-flying.ssb.txt",
                                         new Atlas("drdog-flying.png",
                                                   "drdog-flying-blue.png",
                                                   "drdog-flying-green.png",
                                                   "drdog-flying.txa.txt", z_order++, 2.0f));
            penguinflying = new SpriterSet("penguin-flying.ssb.txt",
                                           new Atlas("penguin-flying.png",
                                                     "penguin-flying-blue.png",
                                                     "penguin-flying-orange.png",
                                                     "penguin-flying.txa.txt", z_order++));
            penguinflying2 = new SpriterSet("penguin-flying-2.ssb.txt",
                                            new Atlas("penguin-flying-2.png",
                                                      "penguin-flying-2-blue.png",
                                                      "penguin-flying-2-orange.png",
                                                      "penguin-flying-2.txa.txt", z_order++));
            icecreamflyer2 = new SpriterSet("icecream-flyer-2.ssb.txt",
                                            new Atlas("icecream-flyer-2.png",
                                                      "icecream-flyer-2-blue.png",
                                                      "icecream-flyer-2-orange.png",
                                                      "icecream-flyer-2.txa.txt", z_order++));
            drhoot = new SpriterSet("drhoot-boss.ssb.txt",
                                    new Atlas("drhoot-boss.png",
                                              "drhoot-boss-blue.png",
                                              "drhoot-boss-orange.png",
                                              "drhoot-boss.txa.txt", z_order++));
            drhoottank = new SpriterSet("drhoot-tank.ssb.txt",
                                        new Atlas("drhoot-tank.png",
                                                  "drhoot-tank-blue.png",
                                                  "drhoot-tank-orange.png",
                                                  "drhoot-tank.txa.txt", z_order++));
            drhootfly = new SpriterSet("drhoot-fly.ssb.txt",
                                       new Atlas("drhoot-fly.png",
                                                 "drhoot-fly-blue.png",
                                                 "drhoot-fly-orange.png",
                                                 "drhoot-fly.txa.txt", z_order++));
            orphan = new Atlas("orphan.png", "orphan.txa.txt", z_order++);
            drhootnest = new Atlas("drhoot_nest.png", "drhoot_nest.txa.txt", z_order++);
            icecreamflyer = new SpriterSet("icecream-flyer.ssb.txt",
                                           new Atlas("icecream-flyer.png",
                                                     "icecream-flyer-blue.png",
                                                     "icecream-flyer-orange.png",
                                                     "icecream-flyer.txa.txt", z_order++));
            minion = new Atlas("drdog_minion.png",
                               "drdog_minion-blue.png",
                               "drdog_minion-green.png",
                               "drdog_minion.txa.txt", z_order++);
            icecreamminion = new SpriterSet("ice-cream-minion.ssb.txt",
                                            new Atlas("ice-cream-minion.png",
                                                      "ice-cream-minion-blue.png",
                                                      "ice-cream-minion-orange.png",
                                                      "ice-cream-minion.txa.txt", z_order++));
            skeletonkingminion = new SpriterSet("skeletonkingminion.ssb.txt",
                                                   new Atlas("skeletonkingminion.png",
                                                          "skeletonkingminion-blue.png",
                                                          "skeletonkingminion-orange.png",
                                                             "skeletonkingminion.txa.txt", z_order++, 1012.0f / 506));
            penguinminion = new SpriterSet("penguin-minion.ssb.txt",
                                           new Atlas("penguin-minion.png",
                                                     "penguin-minion-blue.png",
                                                     "penguin-minion-orange.png",
                                                     "penguin-minion.txa.txt", z_order++));
            drhootminion = new SpriterSet("drhoot-minion.ssb.txt",
                                          new Atlas("drhoot-minion.png",
                                                    "drhoot-minion-blue.png",
                                                    "drhoot-minion-orange.png",
                                                    "drhoot-minion.txa.txt", z_order++));

            #endregion

            house = new Atlas("house.png", new Vector2(275, 527), z_order++);
            house.texture.hud = true;

            title = new Atlas("title.png", new Vector2(376, 300), z_order++);

            block = new Atlas("block.png", "block.txa.txt", z_order++);
            block.texture.hud = true;
            block.texture.scissored = true;

            block_logo = new Atlas("block_logo.png", "block_logo.txa.txt", z_order++);
            block_logo.texture.hud = true;
            block_logo.texture.scissored = true;

            highlight = new Atlas("fade_white.png", z_order++);
            highlight.texture.hud = true;

            if (DataStorage.DisconcertingObjectivesSeen >= 20) {
                door = new Atlas("door_alt.png", new Vector2(192, 382), z_order++);
            }
            else {
                door = new Atlas("door.png", new Vector2(192, 382), z_order++);
            }
            door.texture.hud = true;

            housebits = new Atlas("housebits.png", "housebits.txa.txt", z_order++);
            housebits.texture.hud = true;

            /* NOTE(shane): The way the z-ordering works means that this needs to be placed after the house. */
            stage_space_anchor = new Atlas("stage_space_anchor.png", "stage_space_anchor.txa.txt", z_order++);

            #region Guns

            carrotgun = new Atlas("gun-carrot.png", new Vector2(224, 156), z_order++);
            penguingun = new Atlas("gun-penguin.png", new Vector2(218, 148), z_order++);
            skullgun = new Atlas("gun-skull.png", new Vector2(227, 161), z_order++);
            dragongun = new Atlas("gun-dragon.png", new Vector2(266, 104), z_order++);
            lasergun = new Atlas("gun-laser.png", new Vector2(218, 131), z_order++);
            gun = new Atlas("gun-gumball.png", new Vector2(251, 156), z_order++);
            beachballgun = new Atlas("gun-beachball.png", new Vector2(184, 154), z_order++);
            boomeranggun = new Atlas("gun-boomerang.png", new Vector2(198, 155), z_order++);
            flamegun = new Atlas("gun-flame.png", new Vector2(208, 155), z_order++);
            lightninggun = new Atlas("gun-lightning.png", new Vector2(239, 154), z_order++);

            gun.sprites[0].center = new Vector2(0.25f, 0.5f);
            carrotgun.sprites[0].center = new Vector2(1.0f / 6, 0.5f);
            penguingun.sprites[0].center = new Vector2(1.0f / 6, 2.0f / 3);
            skullgun.sprites[0].center = new Vector2(1.0f / 6, 0.5f);
            dragongun.sprites[0].center = new Vector2(1.0f / 6, 0.5f);
            lasergun.sprites[0].center = new Vector2(1.0f / 6, 0.5f);
            beachballgun.sprites[0].center = new Vector2(0.25f, 2.0f / 3);
            boomeranggun.sprites[0].center = new Vector2(1.0f / 6, 0.5f);
            flamegun.sprites[0].center = new Vector2(1.0f / 6, 0.5f);
            lightninggun.sprites[0].center = new Vector2(1.0f / 6, 0.5f);

            carrotgun.texture.hud = true;
            penguingun.texture.hud = true;
            skullgun.texture.hud = true;
            dragongun.texture.hud = true;
            lasergun.texture.hud = true;
            gun.texture.hud = true;
            beachballgun.texture.hud = true;
            boomeranggun.texture.hud = true;
            flamegun.texture.hud = true;
            lightninggun.texture.hud = true;

            #endregion

            skeletonkingminiboss = new SpriterSet("skeletonkingminiboss.ssb.txt",
                                                  new Atlas("skeletonkingminiboss.png",
                                                            "skeletonkingminiboss-blue.png",
                                                            "skeletonkingminiboss-orange.png",
                                                            "skeletonkingminiboss.txa.txt", z_order++));
            skeletonkingflying = new SpriterSet("skeletonking-flyingenemy.ssb.txt",
                                                new Atlas("skeletonking-flyingenemy.png",
                                                          "skeletonking-flying-blue.png",
                                                          "skeletonking-flying-orange.png",
                                                          "skeletonking-flyingenemy.txa.txt", z_order++));

            counter_time = new Atlas("counter-time-numbers-yellow.png", new Vector2(32, 40), z_order++);
            counter_time.texture.hud = true;

            lightning_match = new SpriterSet("lightning-match.ssb.txt",
                                             new Atlas("lightning-match.png",
                                                       "lightning-match.txa.txt", z_order++));
            lightning_match.atlas.texture.hud = true;

            stage_skeleton_anchors = new Atlas("stage_skeleton_anchors.png", "stage_skeleton_anchors.txa.txt", z_order++);
            stage_penguin_anchors = new Atlas("stage_penguin_anchors.png", "stage_penguin_anchors.txa.txt", z_order++);
            stage_drdog_anchors = new Atlas("stage_drdog_anchors.png", "stage_drdog_anchors.txa.txt", z_order++);
            stage_drhoot_anchors = new Atlas("stage_drhoot_anchors.png", "stage_drhoot_anchors.txa.txt", z_order++);
            stage_pyramid_anchors = new Atlas("stage_pyramid_anchors.png", "stage_pyramid_anchors.txa.txt", z_order++);
            stage_oakland_anchors = new Atlas("stage_oakland_anchors.png", "stage_oakland_anchors.txa.txt", z_order++);

            store_board = new Atlas("store_board.png", "store_board.txa.txt", z_order++);
            hud = new Atlas("hud.png", "hud.txa.txt", z_order++);
            hud.texture.hud = true;

            elements = new Atlas("elements.png", "elements.txa.txt", z_order++);
            elements.texture.hud = true;

            cat = new SpriterSet("cat.ssb.txt",
                                 new Atlas("cat.png", "cat.txa.txt", z_order++));

            shadowblob2 = new Atlas("shadowblob.png", new Vector2(280, 88), z_order++);
            shadowblob2.texture.hud = true;

            circle = new Atlas("particle_circle.png", z_order++);
            circle.texture.hud = true;

            cloudpuff = new Atlas("cloudpuff.png", "cloudpuff.txa.txt", z_order++);
            gunpoof = new Atlas("gunpoof.png", "gunpoof.txa.txt", z_order++);

            peterbossbullet = new SpriterSet("peter_boss_bullet.ssb.txt",
                                             new Atlas("peter_boss_bullet.png",
                                                       "peter_boss_bullet.txa.txt", z_order++));

            icecreambossbullet = new SpriterSet("icecream-boss-bullet.ssb.txt",
                                                new Atlas("icecream-boss-bullet.png",
                                                          "icecream-boss-bullet-blue.png",
                                                          "icecream-boss-bullet-orange.png",
                                                          "icecream-boss-bullet.txa.txt", z_order++));
            drdogbullet = new SpriterSet("drdog-bullet.ssb.txt",
                                         new Atlas("drdog-bullet.png",
                                                   "drdog-bullet-blue.png",
                                                   "drdog-bullet-green.png",
                                                   "drdog-bullet.txa.txt", z_order++));

            drdogminion2bullet = new SpriterSet("drdog-minion-2-bullet.ssb.txt",
                                                new Atlas("drdog-minion-2-bullet.png",
                                                          "drdog-minion-2-bullet.txa.txt", z_order++));

            penguintankbullet = new Atlas("penguin-tank-bullet.png", "penguin-tank-bullet.txa.txt", z_order++);
            penguinflyingbullet = new Atlas("penguin-flying-bullet.png", "penguin-flying-bullet.txa.txt", z_order++);
            penguinflying2bullet = new SpriterSet("penguin-flying-2-bullet.ssb.txt",
                                                  new Atlas("penguin-flying-2-bullet.png",
                                                            "penguin-flying-2-bullet.txa.txt", z_order++));
            penguinbossbullet = new Atlas("penguin-boss-bullet.png", "penguin-boss-bullet.txa.txt", z_order++);
            skeletonkingprojectile = new SpriterSet("skeletonkingprojectile.ssb.txt",
                                                    new Atlas("skeletonkingprojectile.png",
                                                              "skeletonkingprojectile.txa.txt", z_order++));
            skeletonkingprojectile.atlas.texture.hud = true;
            skelkingminion2bullet = new SpriterSet("skelking-minion-2-bullet.ssb.txt",
                                                   new Atlas("skelking-minion-2-bullet.png",
                                                             "skelking-minion-2-bullet.txa.txt", z_order++));

            drhoottankbullet = new SpriterSet("drhoot-tank-bullet.ssb.txt",
                                              new Atlas("drhoot-tank-bullet.png",
                                                        "drhoot-tank-bullet.txa.txt", z_order++));

            drhootminionbullet = new SpriterSet("drhoot-minion-bullet.ssb.txt",
                                                new Atlas("drhoot-minion-bullet.png",
                                                          "drhoot-minion-bullet.txa.txt", z_order++));

            drhootflybullet = new SpriterSet("drhoot-fly-bullet.ssb.txt",
                                             new Atlas("drhoot-fly-bullet.png",
                                                        "drhoot-fly-bullet-blue.png",
                                                         "drhoot-fly-bullet-orange.png",
                                                       "drhoot-fly-bullet.txa.txt", z_order++));

            drhootminion2bullet = new SpriterSet("drhoot-minion-2-bullet.ssb.txt",
                                                 new Atlas("drhoot-minion-2-bullet.png",
                                                           "drhoot-minion-2-bullet.txa.txt", z_order++));

            icecreamflyerbullet = new SpriterSet("icecream-flyer-bullet.ssb.txt",
                                                new Atlas("icecream-flyer-bullet.png",
                                                          "icecream-flyer-bullet-blue.png",
                                                          "icecream-flyer-bullet-orange.png",
                                                          "icecream-flyer-bullet.txa.txt", z_order++));

            icecreamflyer2bullet = new SpriterSet("icecream-flyer-2-bullet.ssb.txt",
                                                  new Atlas("icecream-flyer-2-bullet.png",
                                                            "icecream-flyer-2-bullet-blue.png",
                                                            "icecream-flyer-2-bullet-orange.png",
                                                            "icecream-flyer-2-bullet.txa.txt", z_order++));

            icecreamtankbullet = new SpriterSet("icecream-tank-bullet.ssb.txt",
                                                new Atlas("icecream-tank-bullet.png",
                                                          "icecream-tank-bullet.txa.txt", z_order++));

            penguinbullet = new Atlas("penguin-bullet.png", "penguin-bullet.txa.txt", z_order++);
            dragonbullet = new Atlas("dragon-bullet.png", "dragon-bullet.txa.txt", z_order++);
            gumballs = new Atlas("gumballs.png", "gumballs.txa.txt", z_order++);
            veggies = new Atlas("veggies.png", "veggies.txa.txt", z_order++);
            laserbullet = new Atlas("laser-bullet.png", new Vector2(40, 20), z_order++);

            boomerang = new SpriterSet("boomerang.ssb.txt",
                                       new Atlas("boomerang.png", "boomerang.txa.txt", z_order++));
            flames = new SpriterSet("flame.ssb.txt",
                                    new Atlas("flame.png", "flame.txa.txt", z_order++));

            laserspecial = new Atlas("laser-special.png", "laser-special.txa.txt", z_order++);

            dragon_special_derp = new Atlas("special-dragon.png", new Vector2(482, 410), z_order++);
            dragon_special_angry = new Atlas("special-dragon-2.png", new Vector2(380, 358), z_order++);
            veggie_special = new Atlas("veggie-special.png", "veggie-special.txa.txt", z_order++);

            lightning_strike = new SpriterSet("lightning-strike.ssb.txt",
                                              new Atlas("lightning-strike.png",
                                                        "lightning-strike.txa.txt", z_order++));

            skullbullet = new Atlas("skull-bullet.png", "skull-bullet.txa.txt", z_order++);
            beachball = new Atlas("beachball.png", "beachball.txa.txt", z_order++);

            explosion = new Atlas("explosion.png", "explosion.txa.txt", z_order++);

            pickups = new Atlas("pickups.png", "pickups.txa.txt", z_order++);
            pickups.texture.hud = true;

            pick_a_day = new Atlas("pick_day_button.png", z_order++);
            pick_a_day.texture.hud = true;

            font = new Atlas("font.png", "font.fnt.txt", z_order++);
            font.texture.hud = true;

            #if LOADING_SCREEN
            loading = new Atlas("loading.png", "loading.txa.txt", z_order++);
            #endif

            money_guy = new Atlas("moneyguy.png", "moneyguy.txa.txt", z_order++);
            money_guy.texture.hud = true;

            counter_money = new Atlas("counter-money-numbers.png", new Vector2(16, 20), z_order++);
            counter_money.texture.hud = true;

            ui = new Atlas("ui.png", "ui.txa.txt", z_order++);
            ui.texture.hud = true;

            arrow = new Atlas("arrow.png", z_order++);
            arrow.texture.hud = true;

            store = new Atlas("store.png", "store.txa.txt", z_order++);

            fade = new Atlas("fade_white.png", z_order++);
            fade.texture.hud = true;
        }
    }
}
