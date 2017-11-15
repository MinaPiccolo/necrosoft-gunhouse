using System;
using UnityEngine;
using Necrosoft;

namespace Gunhouse
{
    public class SkeletonKingBackgroundDay : Entity
    {
        Vector2 scaleAmount = new Vector2(-1.05f, 1.05f);
        Vector2 scaleOne = new Vector2(1.05f, 1.05f);

        public const int n_clouds = 4;
        public Vector4[] clouds = new Vector4[n_clouds];
        Vector2 groundPosition = AppMain.vscreen * 0.5f;

        public SkeletonKingBackgroundDay()
        {
            for (int i = 0; i < n_clouds; i++) {
                clouds[i] = new Vector4(Util.rng.NextFloat(0, AppMain.vscreen.x),
                                        Util.rng.NextFloat(0, 300),
                                        Util.rng.NextFloat(0.025f, 0.1f),
                                        0);
            }

            groundPosition.y += 10;
        }

        public override void tick()
        {
            for(int i = 0; i < n_clouds; i++)
            {
                clouds[i].x += clouds[i].z;
                clouds[i].y += clouds[i].w;
                if (clouds[i].x < -200)  { clouds[i].x = AppMain.vscreen.x + 200; }
            }
        }

        public virtual Atlas atlas()
        {
            return AppMain.textures.stage_skeleton_noon;
        }

        public override void draw()
        {
            atlas().draw((int)stage_skeleton_noon.Sprites.background,
                         new Vector2(AppMain.vscreen.x * 0.5f, 394 * 0.5f), scaleOne, Vector4.one);

            if (AppMain.DisplayAnchor) {
                int sprite = (int)stage_skeleton_anchors.Sprites.noon_anchor;
                if (atlas() == AppMain.textures.stage_skeleton_dusk) sprite = (int)stage_skeleton_anchors.Sprites.dusk_anchor;
                if (atlas() == AppMain.textures.stage_skeleton_night) sprite = (int)stage_skeleton_anchors.Sprites.night_anchor;

                AppMain.textures.stage_skeleton_anchors.draw(sprite, new Vector2(290 * 0.5f, 514),
                                                             scaleAmount, Vector4.one);
            }

            for (int i = 0; i < n_clouds; i++) {
                atlas().draw((int)stage_skeleton_noon.Sprites.cloud_0 + i, (Vector2)clouds[i], scaleOne, Vector4.one);
            }

            atlas().draw((int)stage_skeleton_noon.Sprites.ground, groundPosition, scaleOne, Vector4.one);
        }
    }

    public class SkeletonKingBackgroundDusk : SkeletonKingBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_skeleton_dusk;
        }
    }

    public class SkeletonKingBackgroundNight : SkeletonKingBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_skeleton_night;
        }
    }

    public partial class SkeletonKingFlyer : Target
    {
        public enum State { IDLE, FLYING, TELEPORT, RETURN, GRABBING, GRABWALK, ATTACK, DYING };
        public State state = State.FLYING;
        public Vector2 telep_start, telep_end;
        public Vector2 origin, destination;
        public float lerp_pos, lerp_speed;
        public int idle_timer;
        public int frame;
        public bool hit_house = false;
        public int hits;
        public int lane;

        public SkeletonKingFlyer()
        {
          lane = Util.rng.Next(2)==0?House.lane_a:House.lane_b;
          telep_start = new Vector2(Util.rng.NextFloat(900, 500), lane);
          position = new Vector2(1000, telep_start.y);
          velocity = new Vector2(-1.0f, 0);
          size = new Vector2(75, 75);
          hp = health;
          hits = max_hits;
          setupMove(telep_start);
        }

        static public void loadAssets()
        {
          AppMain.textures.skeletonkingflying.touch();
                AppMain.textures.skeletonkingflying.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public void setupMove(Vector2 to)
        {
          origin = position;
          destination = to;
          lerp_pos = 0;
          lerp_speed = speed/(destination-origin).magnitude;
        }

        public override void startDying()
        {
          if(state == State.DYING) return;

          Choom.PlayEffect(SoundAssets.EnemyDie);
          velocity = new Vector2(0, 0);
          state = State.DYING;
          frame = 0;
        }

        public override void tick()
        {
          base.tick();
          if(frozenThisFrame()) return;

    //      velocity += new Vector2(0, gravity);
          position += velocity;
          frame += 15;
          if(state == State.GRABWALK)
          {
            if(position.x > 1100)
            {
              remove = true;
            }
          }
          if(state == State.FLYING)
          {
            lerp_pos += lerp_speed;
            if(lerp_pos >= 1.0f)
            {
              position = destination;
              velocity = Vector2.zero;
              frame = 0;
              if (hits > 0) {
                state = State.IDLE;
                idle_timer = 60;
              } else {
                state = State.GRABBING;
              }
            }
            else
            {
              var smooth_lerp = Util.smoothStep(lerp_pos);
              var new_position = origin*(1-smooth_lerp) + destination*smooth_lerp;
              velocity = new_position-position;
            }
          }
          if (state == State.IDLE)
          {
            if(idle_timer-- == 0)
            {
              frame = 100;
              state = State.TELEPORT;
            }
          }
          if (state == State.GRABBING)
          {
            if (frame > 700)
            {
              frame = 0;
              state = State.GRABWALK;
              Choom.PlayEffect(SoundAssets.OrphanTake);
              held_orphans.Add(new Orphan(new Vector2(-90, 30)));
              velocity.x = runaway_speed;
            }
          }
          if (state == State.TELEPORT)
          {
            if (frame > 300)
            {
              frame = 515;
              if (hit_house == false)
              {
                telep_start = position;
                telep_end = new Vector2((float) strike_distance, lane);
                position = telep_end;
                state = State.RETURN;
              } else {
                position = telep_start;
                state = State.RETURN;
              }
            }
          }
          if(state == State.RETURN)
          {
            frame -= 30;
            if (frame < 300)
            {
              if (hit_house == false)
              {
                frame = 0;
                state = State.ATTACK;
              } else {
                frame = 0;
                hit_house = false;
                state = State.FLYING;
                if (hits > 0)
                  telep_end = new Vector2(Util.rng.NextFloat(900, 500), lane);
                else
                  telep_end = new Vector2((float)steal_distance, 430);
                setupMove(telep_end);
              }
            }
          }
          if(state == State.DYING)
          {
            frame += (int)(700 * death_anim_speed / 60);
            if(frame >= 700) finishDying();
          }
          if (state == State.ATTACK)
          {
            frame += 15;
            if (frame > 600 && hit_house == false)
            {
              hit_house = true;
              Game.instance.damageTargetPiece(SkeletonKingFlyer.strike_damage);
              hits--;
            }
            if (frame > 1000)
            {
              frame = 0;
              idle_timer = 30;
              state = State.IDLE;
            }
          }
        }

        public override void draw()
        {
          drawSubOrphans();
          Vector2 flip =  new Vector2(facingScale().x * -1.0f, facingScale().y * 1.0f);
          string anim = "walk";
          if(state == State.IDLE) anim = "idle";
          if(state == State.RETURN) anim = "death";
          if(state == State.TELEPORT) anim = "death";
          if(state == State.GRABBING) anim = "grab";
          if(state == State.GRABWALK){
            anim = "grab and walk";
            flip.x = 1.0f;
          }
          var color = flashColor();
          if(state == State.ATTACK){
            anim = "attack";
            flip.x = 1.0f;
            if(Game.instance.house.visibleDoorPosition()<1)
              color.w = 0.4f;
          }
          if(state == State.DYING) anim = "death";
          else
            AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 480),
              Vector2.one/5, new Vector4(1, 1, 1, 0.5f));
          AppMain.textures.skeletonkingflying.draw(anim, frame,
            position+new Vector2(0, 55), flip/2, 0, color);
          drawSubBullets();
        }
    }

    public partial class SkeletonKingMinion : Target
    {
        public enum State { WALKING, DYING };
        public State state = State.WALKING;
        public int frame;

        public SkeletonKingMinion()
        {
          position = new Vector2(1000, 250);
          velocity = new Vector2(-Util.rng.NextFloat(approach_speed_min, approach_speed_max), 0);
          size     = new Vector2(40, 60);
          hp       = health;
        }

        static public void loadAssets()
        {
          AppMain.textures.skeletonkingminion.touch();
                AppMain.textures.skeletonkingminion.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
          if(state == State.DYING) return;

          Choom.PlayEffect(SoundAssets.EnemyDie);
          velocity = new Vector2(0, 0);
          state = State.DYING;
          frame = 0;
        }

        public override void tick()
        {
          base.tick();
          if(frozenThisFrame()) return;

          velocity += new Vector2(0, gravity);
          position += velocity;
          if(position.y > House.lane_c+20)
          {
            position.y = House.lane_c+20;
            velocity.y = 0;
          }

          if(position.x > 1100) {
            remove = true;
          }

          if(state == State.DYING)
          {
            frame += (int)(1000 * death_anim_speed / 60);
            if(frame >= 1000) finishDying();
          }
          if(state == State.WALKING)
          {
            frame += (int)(1000 * Util.rng.NextFloat(run_anim_speed_min, run_anim_speed_max) / 60);
            if(position.x < steal_distance)
            {
              position.x = steal_distance;
              velocity.x = -velocity.x;
              Choom.PlayEffect(SoundAssets.OrphanTake);
              held_orphans.Add(new Orphan(new Vector2(20, -10)));
            }
          }
        }

        public override void draw()
        {
          drawSubOrphans();

          string anim = "walk";
          if(state == State.WALKING && held_orphans.Count > 0) anim = "walk-grab";
          if(state == State.DYING) anim = "death";
          else
            AppMain.textures.shadowblob.draw(0, new Vector2(position.x, House.lane_c+50),
              Vector2.one/6, new Vector4(1, 1, 1, 0.5f));
          AppMain.textures.skeletonkingminion.draw(anim, frame,
            position+new Vector2(0, 30), facingScale()/2, 0, flashColor());
          drawSubBullets();
        }
    }

    public partial class SkeletonKingMinion2 : Target
    {
        public enum State { WALKING, DYING, SHOOTING };
        public State state = State.WALKING;
        public int frame;
        public float nextpos;
        public bool hasthrown = false;

        public SkeletonKingMinion2()
        {
          position = new Vector2(1000, House.lane_b-60);
          velocity = new Vector2(-Util.rng.NextFloat(approach_speed_min, approach_speed_max), 0);
          size     = new Vector2(40, 70);
          hp       = health;
          nextpos = Util.rng.NextFloat(700, 900);
        }

        static public void loadAssets()
        {
          AppMain.textures.skelkingminion2.touch();
                AppMain.textures.skelkingminion2.atlas.switchTexture(MetaState.wave.enemy_palette);
          AppMain.textures.skelkingminion2bullet.touch();
                AppMain.textures.skelkingminion2bullet.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
          if(state == State.DYING) return;

          Choom.PlayEffect(SoundAssets.EnemyDie);
          velocity = new Vector2(0, 0);
          state = State.DYING;
          frame = 0;
        }

        public override void tick()
        {
          base.tick();
          if(frozenThisFrame()) return;

          velocity += new Vector2(0, gravity);
          position += velocity;
          if(position.y > House.lane_b-30)
          {
            position.y = House.lane_b-30;
            velocity.y = 0;
          }

          if(position.x > 1100) {
            remove = true;
          }

          if(state == State.DYING)
          {
            frame += (int)(1000 * death_anim_speed / 60);
            if(frame >= 1000) finishDying();
          }

          if(state == State.SHOOTING)
          {
            frame += 10;
            if (frame > 200 && hasthrown == false){
              hasthrown = true;
              Game.instance.enemy_bullet_group.add(
                new SkeletonKingMinion2Bullet(position+new Vector2(-10, -10)));
            }
            if (frame > 500) {
              nextpos = Util.rng.NextFloat(700, 900);
              velocity.x = Math.Sign(nextpos-position.x)*Util.rng.NextFloat(approach_speed_min, approach_speed_max);
              hasthrown = false;
              state = State.WALKING;
            }
          }

          if(state == State.WALKING)
          {
            frame += (int)(1000 * Util.rng.NextFloat(run_anim_speed_min, run_anim_speed_max) / 60);
            if(Math.Abs(position.x - nextpos) < 1.0f)
            {
              state = State.SHOOTING;
              velocity.x = 0;
              frame = 0;
            }
          }
        }

        public override void draw()
        {
          drawSubOrphans();

          string anim = "walk";
          if(state == State.SHOOTING)
          {
            anim = "shoot";
            facing_left = true;
          }
          if(state == State.DYING) anim = "death";
          else
            AppMain.textures.shadowblob.draw(0, new Vector2(position.x, House.lane_b),
              Vector2.one/6, new Vector4(1, 1, 1, 0.5f));
          AppMain.textures.skelkingminion2.draw(anim, frame,
            position+new Vector2(0, 30), facingScale()*2/3, 0, flashColor());
          drawSubBullets();
        }
    }

    public class SkeletonKingMinion2Bullet : Entity
    {
        public enum State { FIRING, DYING };
        public State state = State.FIRING;
        public float frame = 0;
        public bool hit = false;
        public int time = 0;

        public SkeletonKingMinion2Bullet(Vector2 position_)
        {
          position = position_;
          angle = Util.rng.NextFloat((float) (SkeletonKingMinion2.min_throw_angle * Math.PI/180), (float) (SkeletonKingMinion2.max_throw_angle*Math.PI/180));
          velocity = new Vector2((float) -Math.Cos(angle), (float) -Math.Sin(angle)) * SkeletonKingMinion2.throw_strength;
        }

        public override void tick()
        {
          frame += 15;
          if(time++%20==0) {
            Choom.PlayEffect(SoundAssets.BoomerangShot);
          }
          if (state == State.FIRING)
          {
            position += velocity;
            velocity.y += SkeletonKingMinion2.weapon_gravity;
          }
          if(position.x < 250 && hit == false)
          {
            Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
            state = State.DYING;
            frame = 0;
            hit = true;
          }
          if (hit == true && frame > 299)
          {
            remove = true;
            Game.instance.damageTargetPiece(SkeletonKingMinion2.weapon_damage);
          }
        }

        public override void draw()
        {
          string anim = "bullet";
          if(state == State.FIRING) anim = "bullet";
          if(state == State.DYING ) anim = "explode";
          var color = Vector4.one;
          if(Game.instance.house.visibleDoorPosition()<1)
            color.w = 0.4f;
          AppMain.textures.skelkingminion2bullet.draw(anim, (int)frame, position, new Vector2(-1, 1)/2, 0, color);
        }
    }

    class FrostBullet : EnemyBullet
    {
        public int frame = 0;
        public bool exploding = false;
        public Vector2 dest;

        public FrostBullet(Vector2 position_)
        {
          position = position_;
          Choom.PlayEffect(SoundAssets.BoomerangSpecial);
        }

        public override void tick()
        {
          dest = Game.instance.targetDestination();
          velocity = (dest-position).normalized*SkeletonKingMiniboss.frost_speed;
          if(exploding)
          {
            frame += (int)(1000*SkeletonKingMiniboss.frost_explode_anim_speed/60);
            if(frame >= 900) remove = true;
            return;
          }

          position += velocity;
          angle = Util.angle(velocity)+(float)Math.PI;

          if((position-dest).magnitude < 30)
          {
            Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
            if(dest == Game.instance.targetDestination())
              Game.instance.damageTargetPiece(SkeletonKingMiniboss.frost_damage);
            kill();
          }
        }

        public override void kill()
        {
          if(exploding) return;

          exploding = true;
          frame = 80;
        }

        public override void draw()
        {
          AppMain.textures.skeletonkingminiboss.draw("attackfrost", frame,
            position, Vector2.one, angle, Game.instance.house.visibleDoorPosition()<1?new Vector4(1, 1, 1, 0.4f):Vector4.one);
        }
    }

    public partial class SkeletonKingMiniboss : Target
    {
        public enum State { FLY, WAIT, ATTACK, HAS_ATTACKED, DYING };
        public State state;

        public int frame = 0;
        public int timeout = 0;
        public Vector2 destination;
        public int lane;

        public SkeletonKingMiniboss()
        {
          lane = Util.rng.Next(2)==0?House.lane_a:House.lane_b;
          position = new Vector2(1100, 100);
          destination = pickDestination();
          size = new Vector2(50, 50);
          hp = health;
        }

        static public void loadAssets()
        {
                AppMain.textures.skeletonkingminiboss.touch();
                AppMain.textures.skeletonkingminiboss.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
          if(state == State.DYING) return;
          Choom.PlayEffect(SoundAssets.EnemyDie);
          state = State.DYING;
          frame = 0;
        }

        public Vector2 pickDestination()
        {
          return new Vector2(Util.rng.Next(500, 900), lane);
        }

        public override void tick()
        {
          base.tick();
          if(frozenThisFrame()) return;

          position += velocity;
          if(state != State.FLY) velocity *= air_resistance;

          if(state == State.FLY)
          {
            frame += (int)(1000 * fly_anim_speed / 60);
            if((position-destination).magnitude > destination_switch_distance)
              velocity += (destination-position).normalized*fly_acceleration;
            else
            {
              if(Util.rng.NextDouble() < continue_flying_chance)
                destination = pickDestination();
              else
              {
                state = State.WAIT;
                frame = 0;
                timeout = (int)(after_flight_wait*60);
              }
            }

            if(velocity.magnitude > fly_max_speed)
              velocity = velocity.normalized*fly_max_speed;
          }

          if(state == State.DYING)
          {
            frame += (int)(1000*die_anim_speed / 60);
            if(frame >= 800) finishDying();
          }

          if(state == State.WAIT)
          {
            frame += (int)(1000*idle_anim_speed / 60);
            frame %= 600;
            if(--timeout <= 0)
            {
              if(Util.rng.NextDouble() < attack_chance)
              {
                state = State.ATTACK;
                frame = 0;
              }
              else
              {
                state = State.FLY;
                destination = pickDestination();
                frame = 0;
              }
            }
          }

          if(state == State.ATTACK || state == State.HAS_ATTACKED)
          {
            frame += (int)(1000*attack_anim_speed/60);
            facing_left = true;
            if(state == State.ATTACK && frame > 600)
            {
              Game.instance.enemy_bullet_group.add(new FrostBullet(position+new Vector2(-17, 6)));
              state = State.HAS_ATTACKED;
            }
            if(frame > 900)
            {
              state = State.WAIT;
              frame = 0;
              timeout = 60;
            }
          }
        }

        public override void draw()
        {
          string anim = "";
          if(state == State.FLY   ) anim = "walk";
          if(state == State.WAIT  ) anim = "idle";
          if(state == State.ATTACK || state == State.HAS_ATTACKED)
            anim = "attackcharacter";
          if(state == State.DYING ) anim = "death";
          else
            AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 470),
              Vector2.one/5, new Vector4(1, 1, 1, 0.5f));
          AppMain.textures.skeletonkingminiboss.draw(anim, frame, position,
            facingScale(new Vector2(-1, 1)), 0, flashColor());
           drawSubBullets();
        }
    }

    public class SkeletonKingProjectile : EnemyBullet
    {
        public int frame = 0;

        public SkeletonKingProjectile(Vector2 position_)
        {
            Choom.PlayEffect(SoundAssets.SonicBoom);
            position = position_;
        }

        public override void tick()
        {
          frame += 15;
          position.x-=5;
          if(position.x < 300)
          {
            kill();
            Game.instance.damageTargetPiece(SkeletonKing.swing_damage);
          }
        }

        public override void kill()
        {
          Particle pop = new Particle(AppMain.textures.skeletonkingprojectile.atlas);
          pop.frame = 8;
          pop.loop = false;
          pop.loop_end = 12;
          pop.frame_speed = 0.2f;
          pop.scale = Vector2.one;
          pop.position = position;
          Game.instance.particle_manager.add(pop);
          remove = true;
        }

        public override void draw()
        {
          AppMain.textures.skeletonkingprojectile.draw("projectile", frame, position, new Vector2(-1, 1), 0,
            Game.instance.house.visibleDoorPosition()<1?new Vector4(1, 1, 1, 0.4f):Vector4.one);
        }
    }

    public partial class SkeletonKing : Target
    {
        public enum State { WALK, HAMMER_LIFT, CAST, SWING, IDLE, DYING }
        public State state = State.WALK;
        public int frame = 0, frame_direction = 1;
        public int idle_timeout = 0;
        public float summon_position;
        public int x_destination;
        public bool attacked = false;
        public float drop_damage = 0;

        public SkeletonKing()
        {
          position = new Vector2(1200, 450-170);
          hp = health;
          size = new Vector2(200, 340);
          state = State.WALK;
          x_destination = Util.rng.Next(min_distance, max_distance);
        }

        static public void loadAssets()
        {
          AppMain.textures.skeletonking.touch();
                AppMain.textures.skeletonking.atlas.switchTexture(MetaState.wave.enemy_palette);
          AppMain.textures.skeletonkingprojectile.touch();
                AppMain.textures.skeletonkingprojectile.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
          if(state == State.DYING) return;
          //position += new Vector2(0, 10);
          state = State.DYING;
          frame = 0;
          velocity = Vector2.zero;
          Game.instance.removeBoss();
          Choom.PlayEffect(SoundAssets.BossDie1);
        }

        public override void damage(float by, Gun.Ammo type)
        {
          base.damage(by, type);

          drop_damage += by;
          if(drop_damage > orphan_drop_damage && state == State.CAST)
          {
            for(int i=0; i<2; i++)
            {
              var o = new Orphan(Vector2.zero);
              Game.instance.orphan_group.add(o);
              o.enterWorld(bubblePosition(i));

              Particle pop = new Particle(AppMain.textures.skeletonkingprojectile.atlas);
              pop.frame = 5;
              pop.loop = false;
              pop.loop_end = 6;
              pop.frame_speed = 0.05f;
              pop.position = bubblePosition(i);
              Game.instance.particle_manager.add(pop);
            }
            state = State.IDLE;
            idle_timeout = Util.rng.Next(idle_time_min, idle_time_max);
          }
        }

        public override void draw()
        {
          string anim = "heartbeat-show";
          if(state == State.WALK       ) anim = "walk";
          if(state == State.HAMMER_LIFT) anim = "hammer-lift";
          if(state == State.CAST       ) anim = "cast";
          if(state == State.SWING      ) anim = "attack";
          if(state == State.DYING      ) anim = "dead";
          else
            AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 440),
              Vector2.one, new Vector4(1, 1, 1, 0.5f));
          AppMain.textures.skeletonking.draw(anim, frame,
            position+new Vector2(0, 170), facingScale(),
            0, flashColor());
          drawSubBullets();

          if(state == State.CAST)
          {
            var size = 1.0f;
            if(summon_position<0.2f) size = summon_position*5;
            int f = frame;
            string a = "orphan-bubble";
            if(summon_position>0.9f)
            {
              a = "ophan-absorb";
              f = (int)((summon_position-0.9)*10*480);
            }
            AppMain.textures.skeletonkingprojectile.draw(a, f,
              bubblePosition(0),
              Vector2.one*size, 0,
              Game.instance.house.visibleDoorPosition()<1?new Vector4(1, 1, 1, 0.4f):Vector4.one);
            AppMain.textures.skeletonkingprojectile.draw(a, f,
              bubblePosition(1),
              Vector2.one*size, 0,
              Game.instance.house.visibleDoorPosition()<1?new Vector4(1, 1, 1, 0.4f):Vector4.one);
          }
        }

        public Vector2 bubblePosition(int orphan)
        {
          float interp = Util.smoothStep(summon_position);
          if(orphan == 0)
            return new Vector2(225, 350)*(1-interp) +
              interp*(position+new Vector2(180, -190));
          return new Vector2(150, 200)*(1-interp) +
            interp*(position+new Vector2(-180, -190));
        }

        public override void tick()
        {
          base.tick();
          if(frozenThisFrame()) return;

          if(state == State.WALK)
          {
            velocity.x = Math.Sign(x_destination-position.x)*walk_speed;
            position += velocity;
            frame += 5;

            if(Math.Abs(position.x-x_destination) < walk_speed)
            {
              velocity.x = 0;
              facing_left = true;
              state = State.IDLE;
              idle_timeout = Util.rng.Next(idle_time_min, idle_time_max);
              frame = 103;
              frame_direction = 1;
            }
          }

          if(state == State.IDLE)
          {
            frame += frame_direction*5;
            if(frame < 103)
            {
              frame = 103;
              frame_direction = 1;
            }
            if(frame > 294)
            {
              frame = 294;
              frame_direction = -1;
            }
            if(--idle_timeout <= 0)
            {
              state = State.HAMMER_LIFT;
              frame = 0;
            }
          }

          if(state == State.HAMMER_LIFT)
          {
            frame += 5;
            if(frame > 480)
            {
              if(Util.rng.NextFloat() < swing_chance)
                state = State.SWING;
              else
              {
                state = State.CAST;
                summon_position = 0;
              }
              attacked = false;
              frame = 0;
            }
          }

          if(state == State.CAST)
          {
            frame += 5;
            summon_position += 0.0025f;
            if(summon_position >= 1)
            {
              state = State.WALK;
              x_destination = Util.rng.Next(min_distance, max_distance);
              frame = 0;
              Game.instance.house.damage(2);
            }
          }

          if(state == State.SWING)
          {
            frame += 5;
            if(frame > 220 && !attacked)
            {
              attacked = true;
              Game.instance.enemy_bullet_group.add(new SkeletonKingProjectile(position+new Vector2(-100, 50)));
            }
            if(frame > 530)
            {
              state = State.WALK;
              x_destination = Util.rng.Next(min_distance, max_distance);
              frame = 0;
            }
          }

          if(state == State.DYING)
          {
            frame += 10;
            if(frame >= 1000) finishDying();
          }
        }
    }
}
