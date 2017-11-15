using System;
using UnityEngine;
using Necrosoft;

namespace Gunhouse
{
    public class PyramidBackgroundDay : Entity
    {
        Vector2 scaleAmount = new Vector2(-1.05f, 1.05f);

        public const int n_clouds = 5;
        public Vector4[] clouds = new Vector4[n_clouds];
        #if FIXED_16X9
        Vector2 groundPosition = AppMain.vscreen * 0.5f + new Vector2(0, 20);
        #else
        Vector2 groundPosition = AppMain.vscreen * 0.5f;
        #endif

        public PyramidBackgroundDay()
        {
            for(int i = 0; i < n_clouds; i++) {
                clouds[i] = new Vector4(Util.rng.NextFloat(0, AppMain.vscreen.x),
                                        Util.rng.NextFloat(0, 300),
                                        Util.rng.NextFloat(0.025f, 0.1f),
                                        0);
            }

            groundPosition.y += 35;
        }

        public override void tick()
        {
            for (int i = 0; i < n_clouds; i++) {
                clouds[i].x += clouds[i].z;
                clouds[i].y += clouds[i].w;
                if (clouds[i].x < -200) { clouds[i].x = AppMain.vscreen.x + 200; }
            }
        }

        public virtual Atlas atlas()
        {
            return AppMain.textures.stage_pyramid_noon;
        }

        public override void draw()
        {
            if (AppMain.DisplayAnchor) {
                int sprite = (int)stage_pyramid_anchors.Sprites.noon_anchor;
                if (atlas() == AppMain.textures.stage_pyramid_dusk) sprite = (int)stage_pyramid_anchors.Sprites.dusk_anchor;
                if (atlas() == AppMain.textures.stage_pyramid_night) sprite = (int)stage_pyramid_anchors.Sprites.night_anchor;

                  AppMain.textures.stage_pyramid_anchors.draw(sprite,
                                                              new Vector2(340 * 0.5f, AppMain.vscreen.y - 95 * 0.5f),
                                                              scaleAmount, Vector4.one);
            }

            atlas().draw((int)stage_pyramid_noon.Sprites.background, AppMain.vscreen * 0.5f, scaleAmount, Vector4.one);

            for (int i = 0; i < n_clouds; i++) {
                atlas().draw((int)stage_pyramid_noon.Sprites.cloud_0 + i % 3, (Vector2)clouds[i], scaleAmount, Vector4.one);
            }

            atlas().draw((int)stage_pyramid_noon.Sprites.ground, groundPosition, scaleAmount, Vector4.one);
        }
    }

    public class PyramidBackgroundDusk : PyramidBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_pyramid_dusk;
        }
    }

    public class PyramidBackgroundNight : PyramidBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_pyramid_night;
        }
    }

  partial class IceCreamBoss : Target
  {
    public enum State { WALKING, IDLING, FIRING, EATING, DYING };
    public State state = State.WALKING;
    public float frame = 0;
    public float nextpos;
    public bool fired = false;
    public bool go_eat = false;
    public bool grab_o = false;
    public float num_orphans;
    public float num_spit;
    public int idle_timeout = 0;
    public int face1_die_frame = -1;
    public int face2_die_frame = -1;
    public float drop_damage = 0;

    public IceCreamBoss()
    {
      position = new Vector2(1300, 280);
      size = new Vector2(200, 400);
      hp = health;
      nextpos = Util.rng.NextFloat(min_pos, max_pos);
      velocity.x = Math.Sign(nextpos-position.x)*walk_speed;
    }

    static public void loadAssets()
    {
      AppMain.textures.icecreamboss.touch();
            AppMain.textures.icecreamboss.atlas.switchTexture(MetaState.wave.enemy_palette);
      AppMain.textures.icecreambossbullet.touch();
            AppMain.textures.icecreambossbullet.atlas.switchTexture(MetaState.wave.enemy_palette);
    }

    public override void damage(float by, Gun.Ammo type)
    {
      drop_damage += by;
      //Console.WriteLine("{0}/{1} {2} {3}", hp, max_hp, max_hp*1/3, max_hp*2/3);
      base.damage(by, type);
      if(hp < max_hp*2/3 && face1_die_frame==-1)
        face1_die_frame = 0;
      if(hp < max_hp*1/3 && face2_die_frame==-1)
        face2_die_frame = 0;
    }

    public override void startDying()
    {
      if(state == State.DYING) return;

      Choom.PlayEffect(SoundAssets.BossDie1);
      state = State.DYING;
      Game.instance.removeBoss();
      frame = 0;
    }

    public override void tick()
    {
      base.tick();
      if(frozenThisFrame()) return;

      if(face1_die_frame >= 0) face1_die_frame += 10;
      if(face2_die_frame >= 0) face2_die_frame += 10;

      if(state == State.WALKING)
      {
        position += velocity;
        frame += 10;

        if(Math.Abs(nextpos-position.x) < 5.0f)
        {
          velocity.x = 0;
          frame = 0;
          if(go_eat)
          {
            state = State.EATING;
            drop_damage = 0;
            num_orphans = Util.rng.NextFloat(1,max_orphans);
          } else {
            state = State.IDLING;
            idle_timeout = (int)Util.rng.NextFloat(idle_time_min, idle_time_max);
            num_spit = Util.rng.NextFloat(1,max_spit);
          }
        }
      }

      if(state == State.EATING)
      {
        frame += 5;
        if(Shield.existing_shield!=null) num_orphans = 0;
        if(num_orphans > 0) {
          if(frame > 260 && frame < 560 && !grab_o)
          {
            Choom.PlayEffect(SoundAssets.BlockLand);
            Choom.PlayEffect(SoundAssets.OrphanTake);
            grab_o = true;
            go_eat = false;
          }
          if(frame >= 560)
          {
            if(grab_o)
            {
              held_orphans.Add(new Orphan(new Vector2(-70, -10)));
              grab_o = false;
            } else {
              if(drop_damage > orphan_drop_damage)
              {
                releaseOrphans();
                frame = 0;
                go_eat = false;
                state = State.WALKING;
                nextpos = Util.rng.NextFloat(min_pos, max_pos);
                velocity.x = Math.Sign(nextpos-position.x)*walk_speed;
              } else if(frame > 660 & !go_eat)
              {
                num_orphans--;
                Game.instance.house.damage(held_orphans.Count);
                held_orphans.Clear();
                go_eat = true;
              }
              else if(frame > 860) frame = 0;
            }
          }
        } else {
          frame = 0;
          go_eat = false;
          state = State.WALKING;
          nextpos = Util.rng.NextFloat(min_pos, max_pos);
          velocity.x = Math.Sign(nextpos-position.x)*walk_speed;
        }
      }

      if(state == State.IDLING)
      {
        frame += 10;
        if(--idle_timeout <= 0)
        {
          if(Util.rng.NextFloat(0.0f, 1.0f) > prob_eating)
          {
            facing_left = true;
            state = State.FIRING;
            fired = false;
            velocity.x = 0;
          } else {
            state = State.WALKING;
            go_eat = true;
            nextpos = eat_pos;
            velocity.x = Math.Sign(nextpos-position.x)*walk_speed;
          }
          frame = 0;
        }
      }

      if(state == State.FIRING)
      {
        frame += 10;
        if(frame >= 400 && !fired)
        {
          fired = true;
          Game.instance.enemy_bullet_group.add(
            new IceCreamBossBullet(position+new Vector2(-180, 0)));
        }
        if(frame >= 775)
        {
          frame = 0;
          if(--num_spit == 0) {
            state = State.WALKING;
            nextpos = Util.rng.NextFloat(min_pos, max_pos);
            velocity.x = Math.Sign(nextpos-position.x)*walk_speed;
          } else {
            state = State.IDLING;
            idle_timeout = (int)Util.rng.NextFloat(idle_time_min, idle_time_max);
          }
        }
      }

      if(state == State.DYING)
      {
        frame += 10;
        if(frame >= 700) finishDying();
      }
    }

    public override void draw()
    {
      string anim_upper = "stance-upperlayer", anim_lower = "stance-lowerlayer";
      if(state == State.EATING ) { anim_upper = "grab-upperlayer";  anim_lower = "grab-lowerlayer";  }
      if(state == State.WALKING) { anim_upper = "walk-upperlayer";  anim_lower = "walk-lowerlayer";  }
      if(state == State.FIRING ) { anim_upper = "shoot-upperlayer"; anim_lower = "shoot-lowerlayer"; }

      if(state == State.DYING  ) { anim_upper = "die-mainbody"; anim_lower = null; }
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 470),
          new Vector2(1, 0.5f), new Vector4(1, 1, 1, 0.5f));

      if(anim_lower!=null)
        AppMain.textures.icecreamboss.draw(anim_lower, (int)frame,
          position+new Vector2(0, 190), new Vector2(-1*facingScale().x, 1*facingScale().y), 0, flashColor());

      if(face1_die_frame == -1)
        AppMain.textures.icecreamboss.draw("face1", (int)frame,
          position+new Vector2(0, 190), new Vector2(-1*facingScale().x, 1*facingScale().y), 0, flashColor());
      else if(face1_die_frame < 800)
        AppMain.textures.icecreamboss.draw("face1-die", face1_die_frame,
          position+new Vector2(0, 190), new Vector2(-1*facingScale().x, 1*facingScale().y), 0, flashColor());
      if(face2_die_frame == -1)
        AppMain.textures.icecreamboss.draw("face2", (int)frame,
          position+new Vector2(0, 190), new Vector2(-1*facingScale().x, 1*facingScale().y), 0, flashColor());
      else if(face2_die_frame < 800)
        AppMain.textures.icecreamboss.draw("face2-die", face2_die_frame,
          position+new Vector2(0, 190), new Vector2(-1*facingScale().x, 1*facingScale().y), 0, flashColor());

      AppMain.textures.icecreamboss.draw(anim_upper, (int)frame,
        position+new Vector2(0, 190), new Vector2(-1*facingScale().x, 1*facingScale().y), 0, flashColor());
      drawSubBullets();
      drawSubOrphans();
    }
  }

  public class IceCreamBossBullet : Entity
  {
    public enum State { FIRING, DYING };
    public State state = State.FIRING;
    public float frame = 0;
    public bool hit = false;
    public float speed;

    public IceCreamBossBullet(Vector2 position_)
    {
      Choom.PlayEffect(SoundAssets.BoomerangShot);
      position = position_;
      speed = IceCreamBoss.bullet_speed;
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
      if(position.x < 300 && hit == false)
      {
        Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
        state = State.DYING;
        frame = 0;
        hit = true;
      }
      if (hit == true && frame > 499)
      {
        remove = true;
        Game.instance.damageTargetPiece(IceCreamBoss.bullet_damage);
      }
    }

    public override void draw()
    {
      var scale = new Vector2(-1, 1);
      string anim = "bullet";
      if(state == State.FIRING) anim = "bullet";
      if(state == State.DYING )
      {
        scale /= 2;
        anim = "explosion";
      }
      var color = Vector4.one;
      if(Game.instance.house.visibleDoorPosition()<1)
        color.w = 0.4f;
      AppMain.textures.icecreambossbullet.draw(anim, (int)frame, position, scale, 0, color);
    }
  }

  partial class IceCreamMinion : Target
  {
    public enum State { WALKING, THROWING, GRABBING, JUMPING, DYING };
    public State state = State.WALKING;
    public int frame, max_jump;
    public float jumppos;
    public bool hasthrown = false;

    public IceCreamMinion()
    {
      position = new Vector2(1000, 450);
      velocity = new Vector2(-Util.rng.NextFloat(approach_speed_min, approach_speed_max), 0);
      size     = new Vector2(40, 60);
      hp       = health;
      max_jump = (int) Util.rng.NextFloat(1, maximum_jumps);
      jumppos = Util.rng.NextFloat(steal_distance + 200, 900);
    }

    static public void loadAssets()
    {
      AppMain.textures.icecreamminion.touch();
            AppMain.textures.icecreamminion.atlas.switchTexture(MetaState.wave.enemy_palette);
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
      if(position.y > 450)
      {
        position.y = 450;
        velocity.y = 0;
      }

      if(state == State.JUMPING)
      {
//        velocity.y += 0.5f;
        if (position.y >= 445) {
          position.y = 450;
          max_jump--;
          jumppos = Util.rng.NextFloat(steal_distance, position.x + 10);
          state = State.WALKING;
        }
      }

      if(position.x > 1100)
      {
        remove = true;
      }

      if(state == State.DYING)
      {
        frame += 10;
        if(frame >= 500) finishDying();
      }

      if(state == State.WALKING)
      {
        frame += 10; // (int)(400 * Util.rng.NextFloat(run_anim_speed_min, run_anim_speed_max) / 60);
        /*if(position.x < jumppos && max_jump > 0)
        {
          velocity.y = -jump_strength;
          state = State.JUMPING;
          frame = 101;
        }*/
        if(position.x < steal_distance)
        {
          position.x = steal_distance;
          velocity.x = -velocity.x;
          held_orphans.Add(new Orphan(new Vector2(35, -10)));
        }
      }
    }

    public override void draw()
    {
      drawSubOrphans();
      drawSubBullets();
      string anim = "walk";
      if(state == State.JUMPING  ) anim = "walk";
      if(state == State.DYING  ) anim = "death";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 490),
          Vector2.one/6, new Vector4(1, 1, 1, 0.5f));
      if(state == State.JUMPING && held_orphans.Count > 0) anim = "walk-kidnap";
      if(state == State.WALKING && held_orphans.Count > 0) anim = "walk-kidnap";
      AppMain.textures.icecreamminion.draw(anim, frame,
        position+new Vector2(0, 40), facingScale(), 0, flashColor());
    }
  }

  partial class IceCreamFlyer2 : Target
  {
    public enum State { NONE, FLYING, IDLE, SHOOT, DYING };
    public float speed = 0, decel_point = 0;
    public State state = State.NONE;
    public Vector2 origin, destination;
    public float frame = 0;
    public float b_swing = 0;
    public float idle_time = 0;
    public bool height_set = false;
    public bool fired = false;
    public int lane;

    public IceCreamFlyer2()
    {
      lane = Util.rng.Next(2)==0?House.lane_a:House.lane_b;

      position = new Vector2(1120, lane+Util.rng.NextFloat(-25, 25));
      state = State.IDLE;
      size = new Vector2(100, 170);
      hp = health;
      idle_time = Util.rng.NextFloat(min_idle, max_idle);
    }

    static public void loadAssets()
    {
      AppMain.textures.icecreamflyer2.touch();
            AppMain.textures.icecreamflyer2.atlas.switchTexture(MetaState.wave.enemy_palette);
      AppMain.textures.icecreamflyer2bullet.touch();
            AppMain.textures.icecreamflyer2bullet.atlas.switchTexture(MetaState.wave.enemy_palette);
    }

    public void setupMove(Vector2 to)
    {
      destination = to;
      origin = position;
      decel_point = 0;
      speed = 0;
      height_set = false;
    }

    public override void draw()
    {
      string anim = "stance-fly";
      if(state == State.SHOOT) anim = "shoot";
      if(state == State.DYING) anim = "death";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 450),
          Vector2.one/6, new Vector4(1, 1, 1, 0.5f));

      AppMain.textures.icecreamflyer2.draw(anim, (int)frame,
        position, facingScale(), 0, flashColor());
      drawSubBullets();
    }

    public override void startDying()
    {
      if(state == State.DYING) return;

      Choom.PlayEffect(SoundAssets.EnemyDie);
      state = State.DYING;
      frame = 0;
    }

    public override void tick()
    {
      base.tick();
      if(frozenThisFrame()) return;

      if(state == State.IDLE)
      {
        frame += 10;
        facing_left = true;
        if(--idle_time < 0) {
          state = State.FLYING;
          if(position.x < range_pos+200){
            setupMove(new Vector2(Util.rng.NextFloat(range_pos+100, 900), lane+Util.rng.NextFloat(-25, 25)));
          } else {
            setupMove(new Vector2(Util.rng.NextFloat(range_pos, position.x-50), lane+Util.rng.NextFloat(-25, 25)));
          }
//          setupMove(new Vector2(Util.rng.NextFloat(500, 900), pickLane()));
        }
      }

      if(state == State.FLYING)
      {
        frame += 10;
        if(height_set) {
          var pos = (position-origin).magnitude/(origin-destination).magnitude;
          if(pos < 0.5f)
          {
            speed += 0.3f;
            if(speed < max_speed) decel_point = pos;
            else speed = max_speed;
          }
          else
          {
            if(pos > 1-decel_point) speed -= 0.1f;
            if(speed < 1) speed = 1;
          }

          velocity = (destination-position).normalized*speed;
          position += velocity;
          if((position-destination).magnitude < 2)
          {
            position = destination;
            frame = 0;
            {
              fired = false;
              state = State.SHOOT;
            }
          }
        } else {
          var pos = position.y/origin.y;
          facing_left = true;
          if(pos < 0.5f)
          {
            speed += 0.3f;
            if(speed < max_climb_speed) decel_point = pos;
            else speed = max_climb_speed;
          }
          else
          {
            if(pos > 1-decel_point) speed -= 0.2f;
            if(speed < 1) speed = 1;
          }
          velocity = new Vector2 (0, Math.Sign(destination.y-position.y)*speed);
          position += velocity;
          if(Math.Abs(position.y-destination.y) < 2.0f)
          {
            speed = 0;
            height_set = true;
          }
        }
      }

      if(state == State.DYING)
      {
        frame += 10;
        if(frame >= 800) finishDying();
      }

      b_swing++;

      if(state == State.SHOOT)
      {
        facing_left = true;
        frame += 15;
        if(frame > 400 && !fired)
        {
          fired = true;
          Game.instance.enemy_bullet_group.add(new IceCreamFlyer2Bullet(position + new Vector2(-150.0f, 0.0f)));
        }
        if(frame > 600)
        {
          state = State.IDLE;
          frame = 0;
          idle_time = Util.rng.NextFloat(min_idle, max_idle);
        }
      }
    }
  }

  public class IceCreamFlyer2Bullet : Entity
  {
    public enum State { FIRING, DYING };
    public State state = State.FIRING;
    public float frame = 0;
    public bool hit = false;
    public float speed;

    public IceCreamFlyer2Bullet(Vector2 position_)
    {
      Choom.PlayEffect(SoundAssets.BoomerangShot);
      position = position_;
      speed = IceCreamFlyer2.bullet_speed;
      velocity = new Vector2(-speed, 0);
    }

    public override void tick()
    {
      frame += 15;
      if (state == State.FIRING)
      {
        position += velocity;
      }
      if(position.x < 250 && hit == false)
      {
        Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
        state = State.DYING;
        frame = 0;
        hit = true;
      }
      if (hit == true && frame > 399)
      {
        remove = true;
        Game.instance.damageTargetPiece(IceCreamFlyer2.bullet_damage);
      }
    }

    public override void draw()
    {
      var scale = new Vector2(-1, 1);
      string anim = "bullet";
      if(state == State.FIRING) anim = "bullet";
      if(state == State.DYING )
      {
        scale /= 2;
        anim = "explosion";
      }
      var color = Vector4.one;
      if(Game.instance.house.visibleDoorPosition()<1)
        color.w = 0.4f;
      AppMain.textures.icecreamflyer2bullet.draw(anim, (int)frame, position, scale, 0, color);
    }
  }

/*  partial class IceCreamFlyer : Target
  {
    public enum State { NONE, FLYING, IDLE, SHOOT, GRAB }
    public State state = State.NONE;

    public IceCreamFlyer()
    {
      position = new Vector2(1120, Util.rng.NextFloat(50, 400));
      hp = health;
    }

    static public void loadAssets()
    {
      AppMain.textures.icecreamflyer.touch();
      AppMain.textures.icecreamflyerbullet.touch();
    }

    public override void draw()
    {
      string anim = "idle";
      if(state == State.SHOOT) anim = "shoot";
      if(state == State.GRAB) anim = "grab";
      if(state == State.TAKE) anim = "grab-toss";
      if(state == State.READY) anim = "grab-dissolve";
      if(state == State.ESCAPE) anim = "idle";
      if(state == State.DYING) anim = "death";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 450),
          Vector2.one/6, new Vector4(1, 1, 1, 0.5f));

      AppMain.textures.icecreamflyer.draw(anim, (int)frame,
        position, facingScale(), 0, flashColor());
      drawSubOrphans();
      drawSubBullets();
    }

    public override void startDying()
    {
      if(state == State.DYING) return;
      state = State.DYING;
      frame = 0;
    }

    public int pickLane()
    {
      return Util.rng.Next(2)==0?House.lane_a:House.lane_b;
    }
  }*/

  partial class IceCreamFlyer : Target
  {
    public enum State { NONE, FLYING, IDLE, SHOOT, GRAB, TAKE, READY, ESCAPE, DYING };
    public float speed = 0, decel_point = 0;
    public State state = State.NONE;
    public Vector2 origin, destination;
    public float frame = 0;
    public float b_swing = 0;
    public int shoots;
    public bool fired = false, has_orphan = false;
    public int lane;

    public IceCreamFlyer()
    {
      lane = Util.rng.Next(2)==0?House.lane_a:House.lane_b;
      position = new Vector2(1120, lane+Util.rng.NextFloat(-25, 25));
      state = State.IDLE;
      size = new Vector2(100, 170);
      hp = health;
      shoots = Util.rng.Next(min_shoots, max_shoots);
    }

    static public void loadAssets()
    {
      AppMain.textures.icecreamflyer.touch();
            AppMain.textures.icecreamflyer.atlas.switchTexture(MetaState.wave.enemy_palette);
      AppMain.textures.icecreamflyerbullet.touch();
            AppMain.textures.icecreamflyerbullet.atlas.switchTexture(MetaState.wave.enemy_palette);
    }

    public void setupMove(Vector2 to)
    {
      destination = to;
      origin = position;
      decel_point = 0;
      speed = 0;
    }

    public override void draw()
    {
      string anim = "idle";
      if(state == State.SHOOT) anim = "shoot";
      if(state == State.GRAB) anim = "grab";
      if(state == State.TAKE) anim = "grab-toss";
      if(state == State.READY) anim = "grab-dissolve";
      if(state == State.ESCAPE) anim = "idle";
      if(state == State.DYING) anim = "death";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 450),
          Vector2.one/6, new Vector4(1, 1, 1, 0.5f));

      AppMain.textures.icecreamflyer.draw(anim, (int)frame,
        position+new Vector2(0, (float)Math.Sin(b_swing/15.0f) * 10.0f),
        facingScale(), 0, flashColor());
      drawSubOrphans();
      drawSubBullets();
    }

    public override void startDying()
    {
      if(state == State.DYING) return;

      Choom.PlayEffect(SoundAssets.EnemyDie);
      state = State.DYING;
      frame = 0;
    }

    public override void tick()
    {
      base.tick();
      if(frozenThisFrame()) return;

      if(state == State.IDLE)
      {
        frame += 10;
        state = State.FLYING;
        if(--shoots > 0) {
          setupMove(new Vector2(Util.rng.NextFloat(500, 900), lane+Util.rng.NextFloat(-25, 25)));
        } else
        {
          setupMove(new Vector2(grab_position, lane+Util.rng.NextFloat(-25, 25)));
        }
      }

      if(state == State.FLYING)
      {
        frame += 10;
        var pos = (position-origin).magnitude/(origin-destination).magnitude;
        if(pos < 0.5f)
        {
          speed += 0.1f;
          if(speed < max_speed) decel_point = pos;
          else speed = max_speed;
        }
        else
        {
          if(pos > 1-decel_point) speed -= 0.1f;
          if(speed < 1) speed = 1;
        }

        velocity = (destination-position).normalized*speed;
        position += velocity;
        if((position-destination).magnitude < 2)
        {
          position = destination;
          frame = 0;
          if(position.x == grab_position)
            state = State.GRAB;
          else
          {
            fired = false;
            state = State.SHOOT;
          }
        }
      }

      if(state == State.DYING)
      {
        frame += 10;
        if(frame >= 850) finishDying();
      }

      if(state == State.ESCAPE)
      {
        frame += 10;
        position += velocity;
        if(position.x > 1100)
          remove = true;
      }

      if(state == State.READY)
      {
        frame += 10;
        if (frame >= 500)
        {
          frame = 0;
          velocity.x = escape_speed;
          velocity.y = -escape_weight;
          state = State.ESCAPE;
        }
      }

      if(state == State.TAKE)
      {
/*        if(Shield.existing_shield!=null || position.x >= grab_position)
        {
          position.x = ForkGun.shield_right;
          setupMove(new Vector2(Util.rng.NextFloat(500, 900), pickLane()));
          state = State.FLYING;
        }*/
        frame += 10;
        if (frame >= 200 && has_orphan == false){
          held_orphans.Add(new Orphan(new Vector2(0, 36)));
          has_orphan = true;
        }
        if (frame >= 300){
          frame = 0;
          state = State.READY;
        }
      }

      if(state == State.GRAB)
      {
/*        if(Shield.existing_shield!=null || position.x >= grab_position)
        {
          position.x = ForkGun.shield_right;
          setupMove(new Vector2(Util.rng.NextFloat(500, 900), pickLane()));
          state = State.FLYING;
        }*/
        frame += 10;
        if (frame >= 800){
          state = State.TAKE;
          frame = 0;
        }
      }

      b_swing++;

      if(state == State.SHOOT)
      {
        facing_left = true;
        frame += 15;
        if(frame > 500 && !fired)
        {
          fired = true;
          Game.instance.enemy_bullet_group.add(new IceCreamFlyerBullet(position + new Vector2(-30.0f, -45.0f)));
        }
        if(frame > 700)
        {
          state = State.IDLE;
          frame = 0;
        }
      }
    }
  }

  public class IceCreamFlyerBullet : Entity
  {
    public enum State { FIRING, DYING };
    public State state = State.FIRING;
    public float frame = 0;
    public bool hit = false;
    public float speed;

    public IceCreamFlyerBullet(Vector2 position_)
    {
      Choom.PlayEffect(SoundAssets.BoomerangShot);
      position = position_;
      speed = IceCreamFlyer.bulet_speed;
      velocity = new Vector2(-speed, 0);
    }

    public override void tick()
    {
      frame += 15;
      if (state == State.FIRING) { position += velocity; }

      if(position.x < 250 && hit == false) {
        Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
        state = State.DYING;
        frame = 0;
        hit = true;
      }

      if (hit == true && frame > 399) {
        remove = true;
        Game.instance.damageTargetPiece(IceCreamFlyer.bullet_damage);
      }
    }

    public override void draw()
    {
      var scale = new Vector2(-1, 1);
      string anim = "bullet";
      if(state == State.FIRING) anim = "bullet";
      if(state == State.DYING )
      {
        scale /= 2;
        anim = "bullet-explode";
      }
      var color = Vector4.one;
      if(Game.instance.house.visibleDoorPosition()<1)
        color.w = 0.4f;
      AppMain.textures.icecreamflyerbullet.draw(anim, (int)frame, position, scale, 0, color);
    }
  }

  partial class IceCreamTank : Target
  {
    public enum State { WALKING, IDLING, FIRING, RELOADING, DYING };
    public State state = State.WALKING;
    public float x_destination;
    public float frame = 0, idle_timeout = 0;
    public bool fired = false;

    public IceCreamTank ()
    {
      position = new Vector2(1150, 380);
      size = new Vector2(100, 150);
      hp   = health;
      x_destination = Util.rng.NextFloat(walk_dest_min, walk_dest_max);
    }

    static public void loadAssets()
    {
      AppMain.textures.icecreamtank.touch();
            AppMain.textures.icecreamtank.atlas.switchTexture(MetaState.wave.enemy_palette);
      AppMain.textures.icecreamtankbullet.touch();
            AppMain.textures.icecreamtankbullet.atlas.switchTexture(MetaState.wave.enemy_palette);
    }

    public override void startDying()
    {
      if(state == State.DYING) return;

      Choom.PlayEffect(SoundAssets.EnemyDie);
      state = State.DYING;
      frame = 0;
    }

    public override void tick()
    {
      base.tick();
      if(frozenThisFrame()) return;

      if(state == State.WALKING)
      {
        velocity.x = (float)Math.Sign(x_destination-position.x)*walk_speed;
        position += velocity;
        frame += 10;

        if(Math.Abs(position.x-x_destination) < velocity.x)
        {
          position.x = x_destination;
          velocity.x = 0;
          frame = 0;
          state = State.IDLING;
          idle_timeout = Util.rng.NextFloat(idle_time_min, idle_time_max);
          facing_left = true;
        }
      }

      if(state == State.IDLING)
      {
        frame += 8;
        if(--idle_timeout <= 0)
        {
          frame = 0;
          state = State.FIRING;
          fired = false;
        }
      }

      if(state == State.RELOADING)
      {
        frame += 10;
        if(frame > 400)
        {
          frame = 0;
          state = State.WALKING;
          fired = false;
        }
      }

      if(state == State.FIRING)
      {
        frame += 10;
        if(frame >= 100 && !fired)
        {
//          if (frame > 700) fired = true;
//          if((frame % 90) == 0){
            Game.instance.enemy_bullet_group.add(
              new IceCreamTankBullet(position+new Vector2(-80, -160)));
            fired = true;
//          }
        }
        if(frame >= 400)
        {
          frame = 0;
          state = State.RELOADING;
          x_destination = Util.rng.NextFloat(walk_dest_min, walk_dest_max);
        }
      }

      if(state == State.DYING)
      {
        frame += 10;
        if(frame >= 650) finishDying();
      }
    }

    public override void draw()
    {
      string anim = "idle";
      if(state == State.WALKING) anim = "walk";
      if(state == State.FIRING ) anim = "shoot";
      if(state == State.RELOADING ) anim = "shoot-reload";
      if(state == State.DYING  )  anim = "death";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 460),
          Vector2.one*2/3, new Vector4(1, 1, 1, 0.5f));
      if(state == State.IDLING )  anim = "stance";

      AppMain.textures.icecreamtank.draw(anim, (int)frame,
        position+new Vector2(0, 80), facingScale(), 0, flashColor());
      drawSubBullets();
    }
  }

  public class IceCreamTankBullet : Entity
  {
    public enum State { FIRING, DYING };
    public State state = State.FIRING;
    public const float damage = 1.5f;
    public float frame = 0;
    public bool hit = false;

    public IceCreamTankBullet(Vector2 position_)
    {
      Choom.PlayEffect(SoundAssets.BoomerangShot);

      position = position_;
      velocity = new Vector2(-IceCreamTank.bullet_speed, -IceCreamTank.bullet_yspeed);
    }

    public override void tick()
    {
      frame += 15;
      velocity.y = velocity.y + IceCreamTank.gravity;
      if (state == State.FIRING)
      {
        position += velocity;
      }
      if(position.x < 290 && hit == false)
      {
        Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
        state = State.DYING;
        frame = 0;
        hit = true;
      }
      if (hit == true && frame > 500)
      {
        remove = true;
        Game.instance.damageTargetPiece(IceCreamTank.bullet_damage);
      }
    }

    public override void draw()
    {
      string anim = "bullet";
      var scale = new Vector2(-1, 1);
      if(state == State.FIRING) anim = "bullet";
      if(state == State.DYING )
      {
        scale /= 2;
        anim = "bullet-explode";
      }
      var color = Vector4.one;
      if(Game.instance.house.visibleDoorPosition()<1)
        color.w = 0.4f;
      AppMain.textures.icecreamtankbullet.draw(anim, (int)frame, position, scale, 0, color);
    }
  }
}
