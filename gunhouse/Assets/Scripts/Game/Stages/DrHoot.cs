using System;
using UnityEngine;
using Necrosoft;

namespace Gunhouse
{
    public class ForestBackgroundDay : Entity
    {
        Vector2 scaleAmount = new Vector2(-1.02f, 1.02f);

        public const int n_clouds = 5;
        public Vector4[] clouds = new Vector4[n_clouds];
        Vector2 groundPosition = new Vector2(AppMain.vscreen.x * 0.5f, AppMain.vscreen.y - 60);

        public ForestBackgroundDay()
        {
            for (int i = 0; i < n_clouds; i++)
            {
                clouds[i] = new Vector4(Util.rng.NextFloat(0, AppMain.vscreen.x),
                                        Util.rng.NextFloat(0, 400),
                                        Util.rng.NextFloat(0.025f, 0.1f),
                                        0);
            }
        }

        public override void tick()
        {
            for (int i = 0; i < n_clouds; i++)
            {
                clouds[i].x += clouds[i].z;
                clouds[i].y += clouds[i].w;
                if (clouds[i].x < -200) { clouds[i].x = AppMain.vscreen.x + 200; }
            }
        }

        public virtual Atlas atlas()
        {
            return AppMain.textures.stage_drhoot_noon;
        }

        public override void draw()
        {
            if (AppMain.DisplayAnchor) {
                int sprite = (int)stage_drhoot_anchors.Sprites.noon_anchor;
                if (atlas() == AppMain.textures.stage_drhoot_dusk) sprite = (int)stage_drhoot_anchors.Sprites.dusk_anchor;
                if (atlas() == AppMain.textures.stage_drhoot_night) sprite = (int)stage_drhoot_anchors.Sprites.night_anchor;

                AppMain.textures.stage_drhoot_anchors.draw(sprite,
                                                           new Vector2(390 * 0.5f, AppMain.vscreen.y - 175 * 0.5f),
                                                           scaleAmount, Vector4.one);
            }
            atlas().draw((int)stage_drhoot_noon.Sprites.background,
                         new Vector2(AppMain.vscreen.x * 0.5f, AppMain.vscreen.y * 0.5f), scaleAmount, Vector4.one);

            atlas().draw((int)stage_drhoot_noon.Sprites.platform, new Vector2(715, 410), scaleAmount, Vector4.one);
            atlas().draw((int)stage_drhoot_noon.Sprites.floor, groundPosition, scaleAmount, Vector4.one);
        }
    }

    public class ForestBackgroundDusk : ForestBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_drhoot_dusk;
        }
    }

    public class ForestBackgroundNight : ForestBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_drhoot_night;
        }
    }

    partial class DrHoot : Target
    {
        public Vector2 origin, destination;
        public float lerp_pos, lerp_speed;

        public enum State { IDLE, FLYING, LASER, GRAB, DYING };
        public float frame;
        public State state = State.FLYING;
        public Vector2 handpos, boost;
        public int orphan_number = 0;
        public bool hit = false;
        public bool added_orphan = false;
        public int idle_timeout = 0;
        public int time = 0;
        public float drop_damage = 0;

        public Vector2[] apples = new Vector2[5];
        public int n_apples = 5;
        int pallet_nest_index;

        public DrHoot()
        {
            position = new Vector2(1100, -150);
            size = new Vector2(300, 300);
            setupMove(new Vector2(630, 300));
            velocity = Vector2.zero;
            hp = health;
            state = State.FLYING;
            boost = new Vector2(0, -1.0f);

            apples[0] = new Vector2(96, -182);
            apples[1] = new Vector2(38, -221);
            apples[2] = new Vector2(0, -160);
            apples[3] = new Vector2(-74, -177);
            apples[4] = new Vector2(-99, -93);
            pallet_nest_index = Mathf.Clamp(MetaState.wave.enemy_palette, 0, 1);
        }

        public void setupMove(Vector2 to)
        {
            origin = position;
            destination = to;
            lerp_pos = 0;
            lerp_speed = approach_speed / Vector2.Distance(destination, origin);
        }

        static public void loadAssets()
        {
            AppMain.textures.drhoot.touch();
            AppMain.textures.drhoot.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
            if (state == State.DYING) { return; }

            Choom.PlayEffect(SoundAssets.BossDie1);
            state = State.DYING;
            frame = 0;
            releaseOrphans();
            Game.instance.removeBoss();
        }

        public override void damage(float by, Gun.Ammo type)
        {
            drop_damage += by;
            base.damage(by, type);

            if (n_apples > 0 && hp < max_hp * (n_apples - 1) / 5)
            {
                n_apples--;
                Game.instance.enemy_bullet_group.add(new ExplodingApple(position + apples[n_apples] + new Vector2(-20, 30)));
            }
        }

        public override void tick()
        {
            base.tick();
            if (frozenThisFrame()) { return; }

            frame += 5;

            if (state == State.FLYING)
            {
                lerp_pos += lerp_speed;
                var smooth_lerp = Util.smoothStep(lerp_pos);
                var new_position = origin * (1 - smooth_lerp) + destination * smooth_lerp;

                velocity = new_position - position;
                position = new_position;

                if (lerp_pos >= 1.0)
                {
                    state = State.IDLE;
                    frame = 0;
                    idle_timeout = (int)(Util.rng.NextFloat(idle_time_min, idle_time_max) * 60);
                }
            }

            if (state == State.IDLE)
            {
                if (--idle_timeout <= 0)
                {
                    frame = 0;
                    if (Util.rng.NextFloat() < grab_chance || Shield.existing_shield != null)
                    {
                        added_orphan = false;
                        drop_damage = 0;
                        state = State.GRAB;
                    }
                    else
                    {
                        state = State.LASER;
                        Choom.PlayEffect(SoundAssets.HootLaser);
                    }
                }
            }

            if (state == State.DYING)
            {
                frame += 5;
                if (frame > 1000) { remove = true; }
            }

            if (state == State.GRAB)
            {
                if (frame >= 700 && !added_orphan)
                {
                    added_orphan = true;
                    int n_grabs = Util.rng.Next(min_orphans_grabbed, max_orphans_grabbed);

                    if (drop_damage > orphan_drop_damage)
                    {
                        for (int i = 0; i < n_grabs; i++)
                        {
                            var o = new Orphan(new Vector2(111 + Util.rng.Next(-40, 40), -95));
                            Game.instance.orphan_group.add(o);
                            o.enterWorld(position + o.position);
                        }
                    }
                    else
                    {
                        Game.instance.house.damage(n_grabs);
                        Choom.PlayEffect(SoundAssets.BlockLand);
                        Choom.PlayEffect(SoundAssets.OrphanTake);

                        for (int i = 0; i < n_grabs; i++)
                        {
                            held_orphans.Add(new Orphan(new Vector2(111 + Util.rng.Next(-40, 40), -95)));
                        }
                    }
                }

                if (frame >= 1000)
                {
                    frame = 0;
                    idle_timeout = (int)(Util.rng.NextFloat(idle_time_min, idle_time_max) * 60);
                    state = State.IDLE;
                }
            }

            if (state == State.LASER)
            {
                if (frame > 750 || Shield.existing_shield != null)
                {
                    frame = 0;
                    idle_timeout = (int)(Util.rng.NextFloat(idle_time_min, idle_time_max) * 60);
                    state = State.IDLE;
                }
                if (frame >= 300 && frame <= 600) { Game.instance.damageTargetPiece(laser_damage); }
            }
        }

        public override void draw()
        {
            string anim = "stance";
            if (state == State.LASER) { anim = "laser"; }
            if (state == State.GRAB) { anim = "grab"; }
            if (state == State.DYING) { anim = "dead"; }

            AppMain.textures.drhoot.draw(anim, (int)frame, position + new Vector2(0, (float)(Math.Sin(time / 20.0f) * 4)),
                                         new Vector2(-1, 1), 0, flashColor());

            position.y -= (float)Math.Cos(frame * Math.PI / 200) * 5;

            drawSubOrphans();

            position.y += (float)Math.Cos(frame * Math.PI / 200) * 5;

            if (state != State.DYING)
            {
                AppMain.textures.drhootnest.draw(pallet_nest_index,
                                                 position + new Vector2(114, -25 - (float)Math.Cos(frame * Math.PI / 200) * 5),
                                                 new Vector2(-1, 1), flashColor(), 0);

                for (int i = 0; i < n_apples; i++)
                {
                    AppMain.textures.drhoot.draw("apple-alone", (int)frame,
                                                 position + apples[i] + new Vector2(-20, 30 + (float)(Math.Sin(time / 20.0f) * 4)),
                                                 new Vector2(-1, 1), 0, flashColor());
                }
            }

            drawSubBullets();
        }
    }

    public class ExplodingApple : Entity
    {
        public int frame = 0;

        public ExplodingApple(Vector2 pos)
        {
            position = pos;
        }

        public override void tick()
        {
            frame += 10;
            if (frame == 300) {
                Game.instance.particle_group.add(new Pickup(position, 100 * MetaState.healing_coefficient, 0));
                Choom.PlayEffect(SoundAssets.Explosion[Util.rng.Next(SoundAssets.Explosion.Length)]);
            }
            if (frame > 1000) { remove = true; }
        }

        public override void draw()
        {
            AppMain.textures.drhoot.draw("apple-explode", frame, position, new Vector2(-1, 1), 0, Vector4.one);
        }
    }

    partial class HootFly : Target
    {
        public enum State { NONE, FLYING, IDLE, SHOOT, ESCAPING, DYING };
        public State state = State.NONE;
        public Vector2 origin, destination;
        public float lerp_pos, lerp_speed;
        public float frame = 0;
        public int idle_timeout = 0;
        public int shoots;
        public bool fired = false;
        public int lane;

        public HootFly()
        {
            lane = Util.rng.Next(2) == 0 ? House.lane_a : House.lane_b;
            position = new Vector2(1120, lane + Util.rng.NextFloat(-25, 25));
            state = State.IDLE;
            size = new Vector2(100, 100);
            hp = health;
            shoots = Util.rng.Next(min_shoots, max_shoots);
        }

        static public void loadAssets()
        {
            AppMain.textures.drhootfly.touch();
            AppMain.textures.drhootfly.atlas.switchTexture(MetaState.wave.enemy_palette);
            AppMain.textures.drhootflybullet.touch();
            AppMain.textures.drhootflybullet.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public void setupMove(Vector2 to)
        {
            origin = position;
            destination = to;
            lerp_pos = 0;
            lerp_speed = speed / Vector2.Distance(destination, origin);
        }

        public override void draw()
        {
            string anim = "stance";
            if (state == State.SHOOT) { anim = "shoot"; }
            if (state == State.ESCAPING) { anim = "carry"; }
            if (state == State.DYING) { anim = "dead"; }

            if (state != State.DYING)
            {
                AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 480),
                                                 Vector2.one / 5, new Vector4(1, 1, 1, 0.5f));
            }

            AppMain.textures.drhootfly.draw(anim, (int)frame, position, facingScale(), 0, flashColor());
            drawSubOrphans();
            drawSubBullets();
        }

        public override void startDying()
        {
            if (state == State.DYING) { return; }
            Choom.PlayEffect(SoundAssets.EnemyDie);
            state = State.DYING;
            frame = 0;
        }

        public override void tick()
        {
            base.tick();
            if (frozenThisFrame()) return;

            if (state == State.IDLE)
            {
                frame += 15;
                velocity.y = velocity.y + gravity;
                if (--idle_timeout <= 0)
                {
                    state = State.FLYING;
                    if (--shoots > 0)
                        setupMove(new Vector2(Util.rng.NextFloat(500, 900), lane + Util.rng.NextFloat(-25, 25)));
                    else
                        setupMove(new Vector2(grab_position, lane + Util.rng.NextFloat(-25, 25)));
                }
            }

            if (state == State.FLYING)
            {
                frame += 15;
                lerp_pos += lerp_speed;
                if (lerp_pos >= 1.0f)
                {
                    position = destination;
                    velocity = Vector2.zero;
                    if (position.x == grab_position)
                    {
                        state = State.ESCAPING;
                        velocity.x = 2;
                        Choom.PlayEffect(SoundAssets.OrphanTake);
                        held_orphans.Add(new Orphan(new Vector2(35, 85)));
                        frame = 0;
                    }
                    else
                    {
                        state = State.SHOOT;
                        fired = false;
                        idle_timeout = 10;
                        frame = 400;        // 400 because firing animation cycle starts there
                    }
                }
                else
                {
                    var smooth_lerp = Util.smoothStep(lerp_pos);
                    var new_position = origin * (1 - smooth_lerp) + destination * smooth_lerp;
                    velocity = new_position - position;
                }
            }

            if (state == State.DYING)
            {
                frame += 15;
                if (frame >= 750) finishDying();
            }

            if (state == State.ESCAPING)
            {
                frame += 15;
                if (position.x > 1100)
                    remove = true;
            }

            position += velocity;

            if (state == State.SHOOT)
            {
                facing_left = true;
                frame += 15;
                if (frame > 800 && !fired)
                {
                    fired = true;
                    Game.instance.enemy_bullet_group.add(new HootFlyBullet(position + new Vector2(-30.0f, 45.0f)));
                    velocity.y = -idle_boost;
                    velocity.x = 2.0f;
                }
                if (frame > 1200)
                {
                    state = State.IDLE;
                    idle_timeout = 60;
                    frame = 0;
                }
            }
        }
    }

    public class HootFlyBullet : Entity
    {
        public enum State { FIRING, DYING };
        public State state = State.FIRING;
        public float frame = 0;
        public bool hit = false;
        public float speed;

        public HootFlyBullet(Vector2 position_)
        {
            Choom.PlayEffect(SoundAssets.Gatling[Util.rng.Next(SoundAssets.Gatling.Length)]);
            position = position_;
            speed = HootFly.bulet_speed;
            velocity = new Vector2(-speed, 0);
        }

        public override void tick()
        {
            frame += 15;
            if (state == State.FIRING)
            {
                position += velocity;
                //        velocity.y += HootMinion.weapon_gravity;
            }
            if (position.x < 250 && hit == false)
            {
                state = State.DYING;
                frame = 0;
                hit = true;
            }
            if (hit == true && frame > 399)
            {
                remove = true;
                Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
                Game.instance.damageTargetPiece(HootFly.bullet_damage);
            }
        }

        public override void draw()
        {
            string anim = "bullet";
            if (state == State.FIRING) anim = "bullet";
            if (state == State.DYING) anim = "explode";
            var color = Vector4.one;
            if (Game.instance.house.visibleDoorPosition() < 1)
                color.w = 0.4f;
            AppMain.textures.drhootflybullet.draw(anim, (int)frame, position, new Vector2(-1, 1), 0, color);
        }
    }

    partial class HootMinion : Target
    {
        public enum State { WALKING, THROWING, GRABBING, JUMPING, DYING };
        public State state = State.WALKING;
        public int frame;
        public float throwpos;
        public bool hasthrown = false;

        public HootMinion()
        {
            position = new Vector2(1000, 250);
            velocity = new Vector2(-Util.rng.NextFloat(approach_speed_min, approach_speed_max), 0);
            size = new Vector2(40, 60);
            hp = health;
            throwpos = Util.rng.NextFloat(steal_distance + 200, 900);
        }

        static public void loadAssets()
        {
            AppMain.textures.drhootminion.touch();
            AppMain.textures.drhootminion.atlas.switchTexture(MetaState.wave.enemy_palette);
            AppMain.textures.drhootminionbullet.touch();
            AppMain.textures.drhootminionbullet.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
            if (state == State.DYING) return;

            Choom.PlayEffect(SoundAssets.EnemyDie);
            velocity = new Vector2(0, 0);
            state = State.DYING;
            frame = 0;
        }

        public override void tick()
        {
            base.tick();
            if (frozenThisFrame()) return;

            velocity += new Vector2(0, gravity);
            position += velocity;
            if (position.y > 450)
            {
                position.y = 450;
                velocity.y = 0;
            }

            if (state == State.THROWING)
            {
                frame += 10;
                if (frame > 200 && hasthrown == false)
                {
                    hasthrown = true;
                    Game.instance.enemy_bullet_group.add(
                      new HootMinionBullet(position + new Vector2(-10, -10)));
                }
                if (frame > 400)
                {
                    velocity.x = -Util.rng.NextFloat(approach_speed_min, approach_speed_max);
                    state = State.WALKING;
                }
            }

            if (position.x > 1100)
            {
                remove = true;
            }

            if (state == State.DYING)
            {
                frame += 10;
                if (frame >= 1000) finishDying();
            }

            if (state == State.WALKING)
            {
                frame += (int)(1000 * Util.rng.NextFloat(run_anim_speed_min, run_anim_speed_max) / 60);
                if (position.x < steal_distance)
                {
                    position.x = steal_distance;
                    velocity.x = -velocity.x;
                    Choom.PlayEffect(SoundAssets.OrphanTake);
                    held_orphans.Add(new Orphan(new Vector2(30, -10)));
                }
            }
        }

        public override void draw()
        {
            drawSubOrphans();

            string anim = "walk";
            if (state == State.THROWING) anim = "shoot";
            if (state == State.DYING) anim = "dead";
            if (state == State.WALKING && hasthrown == true) anim = "walkgrab";
            if (state == State.WALKING && held_orphans.Count > 0) anim = "walkgrab";
            if (state != State.DYING)
                AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 490),
                  Vector2.one / 6, new Vector4(1, 1, 1, 0.5f));
            AppMain.textures.drhootminion.draw(anim, frame,
              position + new Vector2(0, 40), facingScale(), 0, flashColor());
            drawSubBullets();
        }
    }

    public class HootMinionBullet : Entity
    {
        public enum State { FIRING, DYING };
        public State state = State.FIRING;
        public float frame = 0;
        public bool hit = false;

        public HootMinionBullet(Vector2 position_)
        {
            position = position_;
            angle = Util.rng.NextFloat((float)(HootMinion.min_throw_angle * Math.PI / 180), (float)(HootMinion.max_throw_angle * Math.PI / 180));
            velocity = new Vector2((float)-Math.Cos(angle), (float)-Math.Sin(angle)) * HootMinion.throw_strength;
        }

        public override void tick()
        {
            frame += 15;
            if (state == State.FIRING)
            {
                position += velocity;
                velocity.y += HootMinion.weapon_gravity;
            }
            if (position.x < 250 && hit == false)
            {
                state = State.DYING;
                frame = 0;
                hit = true;
            }
            if (hit == true && frame > 299)
            {
                remove = true;
                Game.instance.damageTargetPiece(HootMinion.weapon_damage);
            }
        }

        public override void draw()
        {
            string anim = "bullet";
            if (state == State.FIRING) anim = "bullet";
            if (state == State.DYING) anim = "crash";
            var color = Vector4.one;
            if (Game.instance.house.visibleDoorPosition() < 1)
                color.w = 0.4f;
            AppMain.textures.drhootminionbullet.draw(anim, (int)frame, position, new Vector2(-1, 1), 0, color);
        }
    }

    partial class HootMinion2 : Target
    {
        public enum State { WALKING, THROWING, GRABBING, DYING };
        public State state = State.WALKING;
        public int frame;
        public float nextpos;
        public bool hasthrown = false;

        public HootMinion2()
        {
            position = new Vector2(1000, House.lane_b - 30);
            velocity = new Vector2(-Util.rng.NextFloat(approach_speed_min, approach_speed_max), 0);
            size = new Vector2(40, 60);
            hp = health;
            nextpos = Util.rng.NextFloat(700, 900);
        }

        static public void loadAssets()
        {
            AppMain.textures.drhootminion2.touch();
            AppMain.textures.drhootminion2.atlas.switchTexture(MetaState.wave.enemy_palette);
            AppMain.textures.drhootminionbullet.touch();
            AppMain.textures.drhootminionbullet.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
            if (state == State.DYING) return;

            Choom.PlayEffect(SoundAssets.EnemyDie);
            velocity = new Vector2(0, 0);
            state = State.DYING;
            frame = 0;
        }

        public override void tick()
        {
            base.tick();
            if (frozenThisFrame()) return;

            position += velocity;

            if (state == State.THROWING)
            {
                frame += 10;
                if (frame > 200 && hasthrown == false)
                {
                    hasthrown = true;
                    Game.instance.enemy_bullet_group.add(
                      new HootMinion2Bullet(position + new Vector2(-10, -10)));
                }
                if (frame > 400)
                {
                    nextpos = Util.rng.NextFloat(700, 900);
                    velocity.x = Math.Sign(nextpos - position.x) * Util.rng.NextFloat(approach_speed_min, approach_speed_max);
                    hasthrown = false;
                    state = State.WALKING;
                }
            }

            if (position.x > 1100)
            {
                remove = true;
            }

            if (state == State.DYING)
            {
                frame += 10;
                if (frame >= 1000) finishDying();
            }

            if (state == State.WALKING)
            {
                frame += (int)(1000 * Util.rng.NextFloat(run_anim_speed_min, run_anim_speed_max) / 60);
                if (Math.Abs(position.x - nextpos) < 1.0f)
                {
                    state = State.THROWING;
                    velocity.x = 0;
                    frame = 0;
                }
            }
        }

        public override void draw()
        {
            drawSubOrphans();

            string anim = "walk";
            if (state == State.THROWING)
            {
                anim = "shoot";
                facing_left = true;
            }
            if (state == State.DYING) anim = "die";
            if (state == State.WALKING) anim = "walk";
            if (state != State.DYING)
                AppMain.textures.shadowblob.draw(0, new Vector2(position.x, House.lane_b + 10),
                  Vector2.one / 6, new Vector4(1, 1, 1, 0.5f));
            AppMain.textures.drhootminion2.draw(anim, frame,
              position + new Vector2(0, 40), facingScale(), 0, flashColor());
            drawSubBullets();
        }
    }

    public class HootMinion2Bullet : Entity
    {
        public enum State { FIRING, DYING };
        public State state = State.FIRING;
        public float frame = 0;
        public bool hit = false;
        public int time = 0;

        public HootMinion2Bullet(Vector2 position_)
        {
            position = position_;
            angle = Util.rng.NextFloat((float)(HootMinion2.min_throw_angle * Math.PI / 180), (float)(HootMinion2.max_throw_angle * Math.PI / 180));
            velocity = new Vector2((float)-Math.Cos(angle), (float)-Math.Sin(angle)) * HootMinion2.throw_strength;
        }

        public override void tick()
        {
            frame += 15;
            if (time++ % 20 == 0) Choom.PlayEffect(SoundAssets.BoomerangShot);

            if (state == State.FIRING)
            {
                position += velocity;
                velocity.y += HootMinion2.weapon_gravity;
            }
            if (position.x < 250 && hit == false)
            {
                state = State.DYING;
                frame = 0;
                hit = true;
            }
            if (hit == true && frame > 299)
            {
                remove = true;
                Game.instance.damageTargetPiece(HootMinion2.weapon_damage);
            }
        }

        public override void draw()
        {
            string anim = "bullet";
            if (state == State.FIRING) anim = "bullet";
            if (state == State.DYING) anim = "crash";
            var color = Vector4.one;
            if (Game.instance.house.visibleDoorPosition() < 1)
                color.w = 0.4f;
            AppMain.textures.drhootminionbullet.draw(anim, (int)frame, position, new Vector2(-1, 1), 0, color);
        }
    }

    partial class HootTank : Target
    {
        public enum State { WALKING, IDLING, FIRING, DYING };
        public State state = State.WALKING;
        public float x_destination;
        public float frame = 0, idle_timeout = 0;
        public bool fired = false;

        public HootTank()
        {
            position = new Vector2(1150, 390);
            size = new Vector2(150, 200);
            hp = health;
            x_destination = Util.rng.NextFloat(walk_dest_min, walk_dest_max);
        }

        static public void loadAssets()
        {
            AppMain.textures.drhoottank.touch();
            AppMain.textures.drhoottank.atlas.switchTexture(MetaState.wave.enemy_palette);
            AppMain.textures.drhoottankbullet.touch();
            AppMain.textures.drhoottankbullet.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
            if (state == State.DYING) return;

            Choom.PlayEffect(SoundAssets.EnemyDie);
            state = State.DYING;
            frame = 0;
        }

        public override void tick()
        {
            base.tick();
            if (frozenThisFrame()) return;

            if (state == State.WALKING)
            {
                velocity.x = (float)Math.Sign(x_destination - position.x) * walk_speed;
                position += velocity;
                frame += 10;

                if (Math.Abs(position.x - x_destination) < velocity.x)
                {
                    position.x = x_destination;
                    velocity.x = 0;
                    frame = 0;
                    state = State.IDLING;
                    idle_timeout = Util.rng.NextFloat(idle_time_min, idle_time_max);
                    facing_left = true;
                }
            }

            if (state == State.IDLING)
            {
                frame += 10;
                if (--idle_timeout <= 0)
                {
                    frame = 0;
                    state = State.FIRING;
                    fired = false;
                }
            }

            if (state == State.FIRING)
            {
                frame += 15;
                if (frame >= 90 && !fired)
                {
                    if (frame > 700) fired = true;
                    if ((frame % 90) == 0)
                    {
                        Game.instance.enemy_bullet_group.add(
                          new HootTankBullet(position + new Vector2(-100, Util.rng.NextFloat(10, 15))));
                    }
                }
                if (frame >= 800)
                {
                    frame = 0;
                    state = State.WALKING;
                    x_destination = Util.rng.NextFloat(walk_dest_min, walk_dest_max);
                }
            }

            if (state == State.DYING)
            {
                frame += 10;
                if (frame >= 900) finishDying();
            }
        }

        public override void draw()
        {
            string anim = "idle";
            if (state == State.WALKING) anim = "walk";
            if (state == State.FIRING) anim = "shoot";
            if (state == State.DYING) anim = "dead";
            if (state == State.IDLING) anim = "stance";

            if (state != State.DYING)
                AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 460),
                                                 Vector2.one / 2, new Vector4(1, 1, 1, 0.5f));
            
            AppMain.textures.drhoottank.draw(anim, (int)frame,
                                             position + new Vector2(0, 75), facingScale(), 0, flashColor());
            drawSubBullets();
        }
    }

    public class HootTankBullet : Entity
    {
        public enum State { FIRING, DYING };
        public State state = State.FIRING;
        public const float damage = 1.5f;
        public float frame = 0;
        public bool hit = false;

        public HootTankBullet(Vector2 position_)
        {
            Choom.PlayEffect(SoundAssets.Gatling[Util.rng.Next(SoundAssets.Gatling.Length)], 0.5f);
            position = position_;
            velocity = new Vector2(-HootTank.bullet_speed, 0);
        }

        public override void tick()
        {
            frame += 15;
            if (state == State.FIRING)
            {
                position += velocity;
            }

            if (position.x < 250 && hit == false)
            {
                Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)], 0.5f);
                state = State.DYING;
                frame = 0;
                hit = true;
            }

            if (hit == true && frame > 149)
            {
                remove = true;
                Game.instance.damageTargetPiece(HootTank.bullet_damage);
            }
        }

        public override void draw()
        {
            string anim = "bullet";
            if (state == State.FIRING) anim = "bullet";
            if (state == State.DYING) anim = "explode";
            var color = Vector4.one;
            if (Game.instance.house.visibleDoorPosition() < 1)
                color.w = 0.4f;
            AppMain.textures.drhoottankbullet.draw(anim, (int)frame, position, new Vector2(-1, 1), 0, color);
        }
    }
}
