using System;
using UnityEngine;
using Necrosoft;

namespace Gunhouse
{
    public class SpaceBackground : Entity
    {
        Vector2 scaleAmount = new Vector2(-1.05f, 1.05f);
        Vector2 scaleOne = new Vector2(1.05f, 1.05f);
        float scaleSize = 1.03f;

        public int time = 0;
        Vector2 backgroundPosition = AppMain.vscreen * .5f;

        #if FIXED_16X9
        Vector2 groundPosition = AppMain.vscreen * 0.5f + new Vector2(0, -63);
        #else
        Vector2 groundPosition = AppMain.vscreen * .5f;
        #endif

        public SpaceBackground()
        {
            backgroundPosition.y -= 70;
            groundPosition.y += 250;
        }

        public override void tick()
        {
            time++;
        }

        public float positionFromTime(float start, float speed, float min, float max)
        {
            float pos = start + speed * time;
            return (pos - min) % (max - min) + min;
        }

        public override void draw()
        {
            AppMain.textures.stage_space_anchor.draw(0, new Vector2(350 * 0.5f, AppMain.vscreen.y - 125 * 0.5f),
                                                     scaleAmount, Vector4.one);

            Atlas a = AppMain.textures.stage_space;

            a.draw((int)stage_space.Sprites.background, backgroundPosition, scaleAmount, Vector4.one);

            var dragon_pos = new Vector2((float)Math.Cos(time / 1500.0f) * 800 + AppMain.vscreen.x * 0.5f,
                                         340 + (float)Math.Sin(time / 60.0f) * 3);

            bool dragon_facing = (time / 1500.0f) % (Math.PI * 2) < Math.PI;
            int frame = (time % 60) / 20;

            a.draw((int)stage_space.Sprites.animation_0 + frame, dragon_pos,
                   new Vector2(dragon_facing ? scaleSize : -scaleSize, scaleSize), Vector4.one);

            a.draw((int)stage_space.Sprites.planet_0,
                   new Vector2(960 - positionFromTime(110, 0.0025f, -400, AppMain.vscreen.x + 400), 370), scaleAmount, Vector4.one);

            a.draw((int)stage_space.Sprites.object_3,
                   new Vector2(960 - positionFromTime(110, 0.0025f, -400, AppMain.vscreen.x + 400), 370) +
                               Util.fromPolar(-time / 500.0f, 200), scaleAmount, Vector4.one);

            a.draw((int)stage_space.Sprites.planet_1,
                   new Vector2(960 - positionFromTime(363, 0.005f, -150, AppMain.vscreen.x + 150), 101), scaleAmount, Vector4.one);

            a.draw((int)stage_space.Sprites.planet_2,
                   new Vector2(960 - positionFromTime(155, 0.01f, -100, AppMain.vscreen.x + 100), 70), scaleAmount, Vector4.one);

            a.draw((int)stage_space.Sprites.planet_3,
                   new Vector2(960 - positionFromTime(46, 0.015f, -75, AppMain.vscreen.x + 75), 125), scaleAmount, Vector4.one);

            a.draw((int)stage_space.Sprites.object_0,
                   new Vector2(960 - positionFromTime(146, 0.5f, -75, AppMain.vscreen.x + 75), 300), scaleAmount, time / 50.0f, Vector4.one);

            var ship_pos = new Vector2(AppMain.vscreen.x - 535, 123) + new Vector2((float)Math.Sin(time / 500.0f) * 5,
                                       (float)Math.Cos(time / 500.0f) * 5);

            var dog_pos = ship_pos + new Vector2((float)Math.Cos(time / 250.0f) * 60, 0);

            bool facing = ((time / 250.0f) % (Math.PI * 2)) < Math.PI;

            if (facing)
            {
                a.draw((int)stage_space.Sprites.object_2, ship_pos, scaleAmount, Vector4.one);
                a.draw((int)stage_space.Sprites.object_1, dog_pos, scaleOne, Vector4.one);
            }
            else
            {
                a.draw((int)stage_space.Sprites.object_1, dog_pos, scaleAmount, Vector4.one);
                a.draw((int)stage_space.Sprites.object_2, ship_pos, scaleAmount, Vector4.one);
            }

            var ufo_pos = new Vector2((float)Math.Sin(time / 1000.0f) * 100, 208);

            a.draw((int)stage_space.Sprites.object_4, ufo_pos, scaleAmount,
                   (float)Math.Sin(time / 60.0f) / 4, Vector4.one);

            a.draw((int)stage_space.Sprites.ground, groundPosition, scaleAmount, Vector4.one);
        }
    }

    partial class PeterBoss : Target
    {
        public Vector2 origin, destination;
        public float lerp_pos, lerp_speed;
        public enum Level { LEVEL1, LEVEL2, LEVEL3 };
        public float frame;
        public State state = State.START;
        public Level dmg = Level.LEVEL1;
        public Vector2 handpos, boost;
        public int orphan_number = 0;
        public bool hit = false;
        public bool slam = false;
        public bool added_orphan = false;
        public bool mouth_fired = false;
        public int idle_mult = 120;
        public int idle_timeout = 0;
        public int time = 0;
        public float drop_damage = 0;
        public int last_missile = 0;
        public string animation;
        public int orphans_eaten = 0;
        public float summon_position;
        public Vector2 pos;

        public PeterBoss()
        {
            MetaState.end_game = true;
            position = new Vector2(700, 300);
            pos = position + new Vector2(720 - position.x, 500 - position.y);
            size = new Vector2(130, 400);
            velocity = Vector2.zero;
            hp = level1_health;
            state = State.START;
            boost = new Vector2(0, -1.0f);
        }

        static public void loadAssets()
        {
            AppMain.textures.peterboss.touch();
            AppMain.textures.peterboss.atlas.switchTexture(MetaState.wave.enemy_palette);
            AppMain.textures.peterbossbullet.touch();
            AppMain.textures.skeletonkingprojectile.touch();
        }

        public override void startDying()
        {
            if (state == State.DYING) return;

            Choom.PlayEffect(SoundAssets.BossDie2);
            state = State.DYING;
            frame = 0;
            releaseOrphans();
            Game.instance.removeBoss();
        }

        public override void damage(float by, Gun.Ammo type)
        {
            drop_damage += by;
            base.damage(by, type);
        }

        public override void tick()
        {
            base.tick();
            if (frozenThisFrame()) return;

            frame += 5;
            if (dmg == Level.LEVEL3) frame += 5;

            if (state == State.START)
            {
                if (frame > 980)
                {
                    frame = 0;
                    state = State.IDLE;
                }
            }

            if (hp < level2_health && dmg == Level.LEVEL1)
            {
                Game.instance.particle_group.add(new Pickup(position,
                                                            level2_heal_bonus * MetaState.healing_coefficient, 0));
                dmg = Level.LEVEL2;
                idle_mult /= 2;
            }

            if (hp < level3_health && dmg == Level.LEVEL2)
            {
                Game.instance.particle_group.add(new Pickup(position,
                                                            level3_heal_bonus * MetaState.healing_coefficient, 0));
                dmg = Level.LEVEL3;
                idle_mult /= 2;
            }

            if (state == State.IDLE)
            {
                if (--idle_timeout <= 0)
                {
                    frame = 0;
                    switch (dmg)
                    {
                        case Level.LEVEL1:
                            state = level1_actions[(int)Util.rng.NextFloat(0, level1_actions.Length)];
                            break;
                        case Level.LEVEL2:
                            state = level2_actions[(int)Util.rng.NextFloat(0, level2_actions.Length)];
                            break;
                        case Level.LEVEL3:
                            state = level3_actions[(int)Util.rng.NextFloat(0, level3_actions.Length)];
                            break;
                        default: state = State.IDLE; break;
                    }

                    switch (state)
                    {
                        case State.LASER: Choom.PlayEffect(SoundAssets.HootLaser); break;
                        case State.RAISE: summon_position = 0; break;
                    }
                }
            }

            if (state == State.MISSILES)
            {
                if (frame > 100 && frame < 600)
                {

                    int frames_per_missile = (500) / missiles_per_burst;
                    int next_missile = last_missile + frames_per_missile;
                    if (frame - 100 > next_missile)
                    {

                        var missile = new PeterMissile(pos + new Vector2(130, -250));
                        missile.peter = this;
                        Game.instance.enemy_bullet_group.add(missile);
                        last_missile += frames_per_missile;
                    }
                }
                if (frame > 980)
                {
                    frame = 0;
                    idle_timeout = (int)(Util.rng.NextFloat(idle_time_min, idle_time_max) * idle_mult);
                    state = State.IDLE;
                    last_missile = 0;
                }
            }

            if (state == State.SLAM)
            {
                if (frame > 300 && !slam)
                {
                    Game.instance.damageTargetPiece(PeterBoss.slam_damage);

                    Choom.PlayEffect(SoundAssets.Explosion[Util.rng.Next(SoundAssets.Explosion.Length)]);
                    Game.instance.enemy_bullet_group.add(new Explosion(pos + new Vector2(-340, -20), Vector2.zero,
                    30, 0, Gun.Ammo.NONE));
                    slam = true;
                }
                if (frame > 400)
                {
                    slam = false;
                    frame = 0;
                    state = State.IDLE;
                    idle_timeout = (int)(Util.rng.NextFloat(idle_time_min, idle_time_max) * idle_mult);
                }
            }

            if (state == State.RAISE)
            {
                if (frame > 960)
                {
                    frame = 0;
                    state = State.CAST;
                }
            }

            if (state == State.LOWER)
            {
                frame -= 10;
                if (dmg == Level.LEVEL3) frame -= 5;
                if (frame < 250)
                {
                    frame = 0;
                    state = State.IDLE;
                }
            }

            if (state == State.SHOOT)
            {
                if (frame >= 390 && !mouth_fired)
                {
                    mouth_fired = true;
                    Game.instance.enemy_bullet_group.add(
                      new PeterBossBullet(pos + new Vector2(-140, -215)));
                }
                if (frame >= 700)
                {
                    frame = 0;
                    mouth_fired = false;
                    state = State.IDLE;
                    idle_timeout = (int)(Util.rng.NextFloat(idle_time_min, idle_time_max) * idle_mult);
                }
            }

            if (state == State.CAST)
            {
                frame += 5;
                summon_position += 0.0025f;
                if (summon_position >= 1)
                {
                    state = State.LOWER;
                    frame = 550;
                    Game.instance.house.damage(2);
                }
            }


            if (state == State.DYING)
            {
                //frame += 5;
                if (frame > 1400) finishDying();
            }

            //      if(state == State.GRAB)
            //      {
            //        if(frame >= 700 && !added_orphan)
            //        {
            //          added_orphan = true;
            //          int n_grabs = Util.rng.Next(min_orphans_grabbed, max_orphans_grabbed);
            //          if(drop_damage > orphan_drop_damage)
            //          {
            //            for(int i=0; i<n_grabs; i++)
            //            {
            //              var o = new Orphan(new Vector2(111+Util.rng.Next(-40, 40), -95));
            //              Game.instance.orphan_group.add(o);
            //              o.enterWorld(position+o.position);
            //            }
            //          }
            //          else
            //          {
            //            Game.instance.house.damage(n_grabs);
            //            AppMain.sounds.blockland.play();
            //            AppMain.sounds.orphantake.play();
            //            for(int i=0; i<n_grabs; i++)
            //              held_orphans.Add(new Orphan(new Vector2(111+Util.rng.Next(-40, 40), -95)));
            //          }
            //        }
            //        if(frame >= 1000)
            //        {
            //          frame = 0;
            //          idle_timeout = (int)(Util.rng.NextFloat(idle_time_min, idle_time_max)*60);
            //          state = State.IDLE;
            //        }
            //      }

            if (state == State.LASER)
            {
                if (frame > 750 || Shield.existing_shield != null)
                {
                    frame = 0;
                    idle_timeout = (int)(Util.rng.NextFloat(idle_time_min, idle_time_max) * idle_mult);
                    state = State.IDLE;
                }
                if (frame >= 300 && frame <= 600)
                    Game.instance.damageTargetPiece(laser_damage);
            }
        }

        public Vector2 bubblePosition(int orphan)
        {
            float interp = Util.smoothStep(summon_position);
            if (orphan == 0)
                return new Vector2(225, 350) * (1 - interp) +
                  interp * (pos + new Vector2(-25, -130));
            return new Vector2(150, 200) * (1 - interp) +
              interp * (pos + new Vector2(-25, -140));
        }

        public override void draw()
        {
            string anim = "stance-normal";
            switch (dmg)
            {
                case (Level.LEVEL1):
                    if (state == State.START) anim = "entrance";
                    if (state == State.IDLE) anim = "stance-normal";
                    if (state == State.SHOOT) anim = "shoot";
                    if (state == State.SLAM) anim = "slam";
                    if (state == State.MISSILES) anim = "missilehatch";
                    if (state == State.RAISE) anim = "summon-orphan";
                    if (state == State.LOWER) anim = "summon-orphan";
                    if (state == State.CAST) anim = "summon-orphan-sparkles";
                    if (state == State.LASER) anim = "laser";
                    break;
                case (Level.LEVEL2):
                    if (state == State.IDLE) anim = "stance-damage1";
                    if (state == State.SHOOT) anim = "shoot-damage1";
                    if (state == State.SLAM) anim = "slam-damage1";
                    if (state == State.LASER) anim = "laser-damage1";
                    if (state == State.MISSILES) anim = "missilehatch-damage1";
                    if (state == State.RAISE) anim = "summon-orphan-damage1";
                    if (state == State.LOWER) anim = "summon-orphan-damage1";
                    if (state == State.CAST) anim = "summon-orphan-sparkles-damage1";
                    break;
                case (Level.LEVEL3):
                    if (state == State.IDLE) anim = "stance-damage2";
                    if (state == State.SHOOT) anim = "shoot-damage2";
                    if (state == State.SLAM) anim = "slam-damage2";
                    if (state == State.MISSILES) anim = "missilehatch-damage2";
                    if (state == State.RAISE) anim = "summon-orphan-damage2";
                    if (state == State.LOWER) anim = "summon-orphan-damage2";
                    if (state == State.CAST) anim = "summon-orphan-sparkles-damage2";
                    if (state == State.LASER) anim = "laser-damage2";
                    if (state == State.DYING) anim = "death";
                    break;
                default:
                    anim = "stance-normal";
                    break;
            }

            //AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 455),
            //  Vector2.one/(1.5f+(float)Math.Abs(position.y-320)/60),
            //  new Vector4(1, 1, 1, 0.5f));
            AppMain.textures.peterboss.draw(anim, (int)frame, pos, new Vector2(-1, 1), 0, flashColor());
            drawSubOrphans();
            //      if(state!=State.DYING)
            //      {
            //        AppMain.textures.drhootnest.draw(0, position+
            //          new Vector2(114, -25-(float)Math.Cos(frame*Math.PI/200)*5),
            //          new Vector2(-1, 1), Vector4.one);
            //        AppMain.textures.drhoot.draw("apples", (int)frame,
            //          position+new Vector2(0, (float)(Math.Sin(time/20.0f)*4)),
            //          new Vector2(-1, 1), 0, flashColor());
            //      }
            drawSubBullets();
            if (state == State.CAST)
            {
                var size = 1.0f;
                if (summon_position < 0.2f) size = summon_position * 5;
                int f = (int)frame;
                string a = "orphan-bubble";
                if (summon_position > 0.9f)
                {
                    a = "ophan-absorb";
                    f = (int)((summon_position - 0.9) * 10 * 480);
                }
                AppMain.textures.skeletonkingprojectile.draw(a, f,
                  bubblePosition(0),
                  Vector2.one * size, 0,
                  /*Game.instance.house.visibleDoorPosition()<1?new Vector4(1, 1, 1, 0.4f):*/Vector4.one);
                AppMain.textures.skeletonkingprojectile.draw(a, f,
                  bubblePosition(1),
                  Vector2.one * size, 0,
                  /*Game.instance.house.visibleDoorPosition()<1?new Vector4(1, 1, 1, 0.4f):*/Vector4.one);
            }
        }
    }

    public class PeterBossBullet : Entity
    {
        public float frame = 0;

        public PeterBossBullet(Vector2 position_)
        {
            position = position_;
            velocity = new Vector2(-PeterBoss.bullet_speed, 0);
            Choom.PlayEffect(SoundAssets.Gatling[Util.rng.Next(SoundAssets.Gatling.Length)]);
        }

        public override void tick()
        {
            frame += 5.0f;
            position += velocity;

            if (position.x < 250)
            {
                remove = true;
                Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
                Game.instance.enemy_bullet_group.add(new Explosion(position, Vector2.zero, 30, 0, Gun.Ammo.NONE));
                Game.instance.damageTargetPiece(PeterBoss.bullet_damage);
            }
        }

        public override void draw()
        {
            AppMain.textures.peterbossbullet.draw("mouth-bullet", (int)frame, position, new Vector2(-1, 1), 0, Vector4.one);
        }
    }

    public class PeterMissile : Entity
    {
        public Vector2 orientation;
        public PeterBoss peter;
        public float frame = 0;
        public float age = 0;

        public PeterMissile(Vector2 position_)
        {
            Choom.PlayEffect(SoundAssets.Gatling[Util.rng.Next(SoundAssets.Gatling.Length)]);
            position = position_;
            angle = Util.rng.NextFloat(-2 * (float)Math.PI * 8 / 6, -2 * (float)Math.PI);
            velocity = Util.fromPolar(angle, 1.0f);
        }

        public override void tick()
        {
            age += 1 / 60.0f;
            frame += 5;
            var target_position = Game.instance.targetDestination();

            var dest_velocity = target_position - position;
            var dest_modifier = dest_velocity - velocity;
            var dest_angle = Util.angle(dest_modifier);

            angle %= (float)Math.PI * 2;

            if (Math.Abs(dest_angle - angle) > Math.Abs(dest_angle + Math.PI * 2 - angle)) dest_angle += (float)Math.PI * 2;
            if (Math.Abs(dest_angle - angle) > Math.Abs(dest_angle - Math.PI * 2 - angle)) dest_angle -= (float)Math.PI * 2;

            if (angle < dest_angle) angle += PeterBoss.missile_rotation_speed;
            if (angle > dest_angle) angle -= PeterBoss.missile_rotation_speed;
            if (Math.Abs(dest_angle - angle) < PeterBoss.missile_rotation_speed)
            {
                angle = dest_angle;
                velocity += Util.fromPolar(angle, PeterBoss.missile_acceleration);
            }

            if (velocity.magnitude > PeterBoss.missile_max_speed) velocity = velocity.normalized * PeterBoss.missile_max_speed;

            position += velocity;

            if (position.x < Puzzle.grid_left + Puzzle.piece_size * 3 || age > PeterBoss.missile_lifetime)
            {
                remove = true;
                Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
                Game.instance.enemy_bullet_group.add(new Explosion(position, Vector2.zero, 20, 0, Gun.Ammo.NONE));
                if (age < PeterBoss.missile_lifetime) Game.instance.damageTargetPiece(PeterBoss.missile_damage);
            }
        }

        public override void draw()
        {
            AppMain.textures.peterbossbullet.draw("hatch-missile", (int)frame, position,
                                                  new Vector2(-0.5f * 3 / 2, 0.5f * 3 / 2) / 2, angle + (float)Math.PI, Vector4.one);
        }
    }
}
