using System;
using UnityEngine;
using Necrosoft;

namespace Gunhouse
{
    public class PenguinPirateBackgroundDay : Entity
    {
        Vector2 scaleAmount = new Vector2(-1.05f, 1.05f);
        Vector2 scaleOne = new Vector2(1.05f, 1.05f);

        public const int n_clouds = 3;
        public Vector2[] clouds = new Vector2[n_clouds];
        public int time = 100;
        #if FIXED_16X9
        Vector2 groundPosition = new Vector2(AppMain.vscreen.x * 0.5f, AppMain.vscreen.y - 100);
        #else
        Vector2 groundPosition = new Vector2(AppMain.vscreen.x * 0.5f, AppMain.vscreen.y - 40);
        #endif

        public PenguinPirateBackgroundDay()
        {
            for(int i = 0; i < n_clouds; i++) {
                clouds[i] = new Vector2(i * 400 + 100, 225);
            }
        }

        public override void tick()
        {
            time++;
        }

        public virtual Atlas atlas()
        {
            return AppMain.textures.stage_penguin_noon;
        }

        public override void draw()
        {
            atlas().draw((int)stage_penguin_noon.Sprites.background,
                         new Vector2(AppMain.vscreen.x * 0.5f, 444 * 0.5f), scaleOne, Vector4.one);

            for (int i = 0; i < n_clouds; i++) {
                atlas().draw((int)stage_penguin_noon.Sprites.cloud_0 + i,
                             new Vector2(clouds[i].x+(float)Math.Sin(i*1.5f+time/(100.0f+i*15))*3.0f,
                                         clouds[i].y+(float)Math.Cos(i*1.5f+time/(100.0f+i*15))*5.0f),
                             scaleOne, Vector4.one);
            }

            int n = 0;
            for(int y = 442; y <= 540; y += 20) {
                atlas().draw((int)stage_penguin_noon.Sprites.foreground,
                             new Vector2(960/2+(float)Math.Tanh(Math.Sin(time/(25.0f+(y-422)/8)))*20.0f,
                                         y    +(float)Math.Abs(Math.Cos(time/(25.0f+(y-422)/8)))*5.0f),
                             n % 2 == 0 ? scaleOne : scaleAmount, Vector4.one);
                n++;
            }

            atlas().draw((int)stage_penguin_noon.Sprites.ground, groundPosition, scaleOne, Vector4.one);

            int sprite = (int)stage_penguin_anchors.Sprites.noon_anchor;
            if (atlas() == AppMain.textures.stage_penguin_dusk)
                sprite = (int)stage_penguin_anchors.Sprites.dusk_anchor;
            if (atlas() == AppMain.textures.stage_penguin_night)
                sprite = (int)stage_penguin_anchors.Sprites.night_anchor;

            AppMain.textures.stage_penguin_anchors.draw(sprite, new Vector2(320 * 0.5f, AppMain.vscreen.y - 119 * 0.5f),
                                                        scaleAmount, Vector4.one);
        }
    }

    public class PenguinPirateBackgroundDusk : PenguinPirateBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_penguin_dusk;
        }
    }

    public class PenguinPirateBackgroundNight : PenguinPirateBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_penguin_night;
        }
    }

  partial class PenguinBoss : Target
  {
    public enum State { WALKING, IDLING, FIRING, DYING, DUDES };
    public State state = State.WALKING;
    public float frame = 0;
    public bool fired = false;
    public int idle_timeout = 0;

    public PenguinBoss()
    {
      position = new Vector2(1300, 310);
      size = new Vector2(250, 325);
      hp = health;
    }

    static public void loadAssets()
    {
      AppMain.textures.penguinboss.touch();
      AppMain.textures.penguinboss.atlas.switchTexture(MetaState.wave.enemy_palette);
      AppMain.textures.penguinbossbullet.touch();
        AppMain.textures.penguinbossbullet.switchTexture(MetaState.wave.enemy_palette);
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

      if(state == State.WALKING)
      {
        velocity.x = -walk_speed;
        position += velocity;
        frame += 10;

        if(position.x <= x_destination && frame%800 < 10)
        {
          velocity.x = 0;
          frame = 0;
          state = State.IDLING;
          idle_timeout = (int)Util.rng.NextFloat(idle_time_min, idle_time_max);
        }
      }

      if(state == State.IDLING)
      {
        frame += 10;
        if(--idle_timeout <= 0)
        {
          frame = 0;
          if(Util.rng.NextFloat() < shoot_chance)
            state = State.FIRING;
          else
            state = State.DUDES;
          fired = false;
        }
      }

      if(state == State.DUDES)
      {
        frame += 10;
        if(frame >= 500 && !fired)
        {
          fired = true;
          for(int i=0; i<3; i++)
          {
            var pm = new PenguinMinion();
            pm.position = position+new Vector2(-120, 50);
            pm.velocity = new Vector2(Util.rng.NextFloat(-1.0f, -5.0f), Util.rng.NextFloat(-1.0f, -5.0f));
            Game.instance.enemy_group.add(pm);
          }
        }
        if(frame >= 775)
        {
          frame = 0;
          state = State.IDLING;
          idle_timeout = (int)Util.rng.NextFloat(idle_time_min, idle_time_max);
        }
      }

      if(state == State.FIRING)
      {
        frame += 10;
        if(frame >= 500 && !fired)
        {
          fired = true;
          Game.instance.enemy_bullet_group.add(
            new PenguinBossBullet(position+new Vector2(-120, 50)));
        }
        if(frame >= 775)
        {
          frame = 0;
          state = State.IDLING;
          idle_timeout = (int)Util.rng.NextFloat(idle_time_min, idle_time_max);
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
      string anim = "stance";
      if(state == State.WALKING) anim = "walk";
      if(state == State.FIRING  || state == State.DUDES) anim = "shoot";
      if(state == State.DYING  ) anim = "dead";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 440),
          new Vector2(1, 0.5f), new Vector4(1, 1, 1, 0.5f));
      if(state == State.DYING || hp > max_hp*2/3)
        AppMain.textures.penguinboss.draw(anim, (int)frame,
          position+new Vector2(0, 140), facingScale(), 0, flashColor());
      else
      {
        AppMain.textures.penguinboss.draw(anim+"-behindsail", (int)frame,
          position+new Vector2(0, 140), facingScale(), 0, flashColor());
        string damage = "dmg1";
        if(hp < max_hp/3) damage = "dmg2";
        AppMain.textures.penguinboss.draw(anim+"-sail-"+damage, (int)frame,
          position+new Vector2(0, 140), facingScale(), 0, flashColor());
        AppMain.textures.penguinboss.draw(anim+"-abovesail", (int)frame,
          position+new Vector2(0, 140), facingScale(), 0, flashColor());
      }
      drawSubBullets();
    }
  }

  public class PenguinBossBullet : Entity
  {
    public float frame = 0;

    public PenguinBossBullet(Vector2 position_)
    {
      position = position_;
      velocity = new Vector2(-PenguinBoss.bullet_speed, 0);
        Choom.PlayEffect(SoundAssets.Gatling[Util.rng.Next(SoundAssets.Gatling.Length)]);
    }

    public override void tick()
    {
      frame += 0.1f;
      position += velocity;
      if(position.x < 250)
      {
        remove = true;
        Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
        Game.instance.enemy_bullet_group.add(new Explosion(position, Vector2.zero,
          30, 0, Gun.Ammo.NONE));
        Game.instance.damageTargetPiece(PenguinBoss.bullet_damage);
      }
    }

    public override void draw()
    {
      var color = Vector4.one;
      if(Game.instance.house.visibleDoorPosition()<1)
        color.w = 0.4f;
      AppMain.textures.penguinbossbullet.draw((int)frame%2, position, new Vector2(-1, 1), color);
    }
  }

  partial class PenguinTank : Target
  {
    public enum State { WALKING, IDLING, FIRING, DYING };
    public State state = State.WALKING;
    public float x_destination;
    public float frame = 0, idle_timeout = 0;
    public bool fired = false;

    public PenguinTank()
    {
      position = new Vector2(1150, 395);
      size = new Vector2(130, 130);
      hp   = health;
      x_destination = Util.rng.NextFloat(walk_dest_min, walk_dest_max);
    }

    static public void loadAssets()
    {
      AppMain.textures.penguintank.touch();
      AppMain.textures.penguintankbullet.touch();
            AppMain.textures.penguintank.atlas.switchTexture(MetaState.wave.enemy_palette);
            AppMain.textures.penguintankbullet.switchTexture(MetaState.wave.enemy_palette);
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
        frame += 10;
        if(--idle_timeout <= 0)
        {
          frame = 0;
          state = State.FIRING;
          fired = false;
        }
      }

      if(state == State.FIRING)
      {
        frame += 15;
        if(frame >= 500 && !fired)
        {
          fired = true;
          Game.instance.enemy_bullet_group.add(
            new PenguinTankBullet(position+new Vector2(-100, -20)));
        }
        if(frame >= 850)
        {
          frame = 0;
          state = State.WALKING;
          x_destination = Util.rng.NextFloat(walk_dest_min, walk_dest_max);
        }
      }

      if(state == State.DYING)
      {
        frame += 10;
        if(frame >= 1000) finishDying();
      }
    }

    public override void draw()
    {
      string anim = "idle";
      if(state == State.WALKING) anim = "walk";
      if(state == State.FIRING ) anim = "shoot";
      if(state == State.DYING  )  anim = "dead";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 450),
          Vector2.one/2, new Vector4(1, 1, 1, 0.5f));
      AppMain.textures.penguintank.draw(anim, (int)frame,
        position+new Vector2(0, 75), facingScale(), 0, flashColor());
      drawSubBullets();
    }
  }

  public class PenguinTankBullet : Entity
  {
    public Vector2 scale = Vector2.one;
    public float frame = 0;

    public PenguinTankBullet(Vector2 position_, float scale_=0)
    {
      Choom.PlayEffect(SoundAssets.Gatling[Util.rng.Next(SoundAssets.Gatling.Length)]);
      scale = new Vector2(-1, 1);
      if(scale_!=0) scale *= scale_;
      position = position_;
      velocity = new Vector2(-PenguinTank.bullet_speed, 0);
    }

    public override void tick()
    {
      frame += 0.1f;
      position += velocity;
      if(position.x < 250)
      {
        remove = true;
        Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
        Game.instance.enemy_bullet_group.add(new Explosion(position, Vector2.zero,
          30, 0, Gun.Ammo.NONE));
        Game.instance.damageTargetPiece(PenguinTank.bullet_damage);
      }
    }

    public override void draw()
    {
      var color = Vector4.one;
      if(Game.instance.house.visibleDoorPosition()<1)
        color.w = 0.4f;
      AppMain.textures.penguintankbullet.draw((int)frame%2, position, scale, color);
    }
  }

  public partial class PenguinMinion : Target
  {
    public enum State { WALKING, DYING };
    public State state = State.WALKING;
    public int frame;

    public PenguinMinion()
    {
      position = new Vector2(1000, 250);
      velocity = new Vector2(-Util.rng.NextFloat(approach_speed_min, approach_speed_max), 0);
      size     = new Vector2(40, 60);
      hp       = health;
    }

    static public void loadAssets()
    {
      AppMain.textures.penguinminion.touch();
      AppMain.textures.penguinminion.atlas.switchTexture(MetaState.wave.enemy_palette);
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
        velocity.x = Math.Sign(velocity.x)*Util.rng.NextFloat(approach_speed_min, approach_speed_max);
      }

      if(position.x > 1100)
      {
        remove = true;
      }

      if(state == State.DYING)
      {
        frame += 10;
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
      //if(position.y < 450) anim =
      if(state == State.WALKING && held_orphans.Count > 0) anim = "grabwalk";
      if(state == State.DYING  ) anim = "dead";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 490),
          Vector2.one/6, new Vector4(1, 1, 1, 0.5f));
      AppMain.textures.penguinminion.draw(anim, frame,
        position+new Vector2(0, 40), facingScale(), 0, flashColor());
      drawSubBullets();
    }
  }

  partial class PenguinFlyer2 : Target
  {
    public enum State { NONE, FLYING, IDLE, SHOOT, DYING };
    public State state = State.NONE;
    public Vector2 origin, destination;
    public float lerp_pos, lerp_speed;
    public float frame = 0;
    public int idle_timeout = 0;
    public bool fired = false;
    public int lane;

    public PenguinFlyer2()
    {
      lane = Util.rng.Next(2)==0?House.lane_a:House.lane_b;
      position = new Vector2(1120, lane+Util.rng.NextFloat(-25, 25));
      state = State.IDLE;
      size = new Vector2(75, 75);
      hp = health;
    }

    static public void loadAssets()
    {
      AppMain.textures.penguinflying2.touch();
      AppMain.textures.penguinflying2.atlas.switchTexture(MetaState.wave.enemy_palette);
      AppMain.textures.penguintankbullet.touch();
            AppMain.textures.penguintankbullet.switchTexture(MetaState.wave.enemy_palette);
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
      state = State.DYING;
      frame = 0;
    }

    public override void draw()
    {
      string anim = "flying-stance";
      if(state == State.SHOOT) anim = "shoot";
      if(state == State.DYING) anim = "death";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 480),
          Vector2.one/6, new Vector4(1, 1, 1, 0.5f));
      AppMain.textures.penguinflying2.draw(anim, (int)frame,
        position+new Vector2(0, 10), facingScale(), 0, flashColor());
      drawSubOrphans();
      drawSubBullets();
    }

    public override void tick()
    {
      base.tick();
      if(frozenThisFrame()) return;

      if(state == State.IDLE)
      {
        frame += 10;
        if(--idle_timeout <= 0)
        {
          state = State.FLYING;
          setupMove(new Vector2(Util.rng.NextFloat(500, 900), lane));
        }
      }

      if(state == State.FLYING)
      {
        frame += 10;
        lerp_pos += lerp_speed;
        if(lerp_pos >= 1.0f)
        {
          position = destination;
          velocity = Vector2.zero;
          state = State.SHOOT;
          fired = false;
          idle_timeout = 120;
          frame = 0;
        }
        else
        {
          var smooth_lerp = Util.smoothStep(lerp_pos);
          var new_position = origin*(1-smooth_lerp) + destination*smooth_lerp;
          velocity = new_position-position;
        }
      }

      if(state == State.DYING)
      {
        frame += 10;
        if(frame >= 800) finishDying();
      }

      position += velocity;

      if(state == State.SHOOT)
      {
        facing_left = true;
        frame += 10;
        if(frame > 320 && !fired)
        {
          fired = true;
          Game.instance.enemy_bullet_group.add(new PenguinTankBullet(position+new Vector2(0, 40), 0.5f));
        }
        if(frame > 600)
        {
          state = State.IDLE;
          idle_timeout = 60;
          frame = 0;
        }
      }
    }
  }

  partial class PenguinFlyer : Target
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

    public PenguinFlyer()
    {
      lane = Util.rng.Next(2)==0?House.lane_a:House.lane_b;
      position = new Vector2(1120, lane+Util.rng.NextFloat(-25, 25));
      state = State.IDLE;
      size = new Vector2(50, 50);
      hp = health;
      shoots = Util.rng.Next(min_shoots, max_shoots);
    }

    static public void loadAssets()
    {
      AppMain.textures.penguinflying.touch();
      AppMain.textures.penguinflying.atlas.switchTexture(MetaState.wave.enemy_palette);
      AppMain.textures.penguinflyingbullet.touch();
            AppMain.textures.penguinflyingbullet.switchTexture(MetaState.wave.enemy_palette);
    }

    public void setupMove(Vector2 to)
    {
      origin = position;
      destination = to;
      lerp_pos = 0;
      lerp_speed = speed/(destination-origin).magnitude;
    }

    public override void draw()
    {
      string anim = "stanceandwalk";
      if(state == State.SHOOT) anim = "shoot";
      if(state == State.ESCAPING) anim = "carry";
      if(state == State.DYING) anim = "dead";
      else
        AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 460),
          Vector2.one/6, new Vector4(1, 1, 1, 0.5f));
      AppMain.textures.penguinflying.draw(anim, (int)frame,
        position, facingScale(), 0, flashColor());
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
        if(--idle_timeout <= 0)
        {
          state = State.FLYING;
          if(--shoots > 0)
            setupMove(new Vector2(Util.rng.NextFloat(500, 900), lane));
          else
            setupMove(new Vector2(grab_position, lane));
        }
      }

      if(state == State.FLYING)
      {
        frame += 10;
        lerp_pos += lerp_speed;
        if(lerp_pos >= 1.0f)
        {
          position = destination;
          velocity = Vector2.zero;
          if(position.x == grab_position)
          {
            state = State.ESCAPING;
            velocity.x = 2;

            Choom.PlayEffect(SoundAssets.OrphanTake);
            held_orphans.Add(new Orphan(new Vector2(10, 40)));
            frame = 0;
          }
          else
          {
            state = State.SHOOT;
            fired = false;
            idle_timeout = 120;
            frame = 0;
          }
        }
        else
        {
          var smooth_lerp = Util.smoothStep(lerp_pos);
          var new_position = origin*(1-smooth_lerp) + destination*smooth_lerp;
          velocity = new_position-position;
        }
      }

      if(state == State.DYING)
      {
        frame += 10;
        if(frame >= 750) finishDying();
      }

      if(state == State.ESCAPING)
      {
        frame += 10;
        if(position.x > 1100)
          remove = true;
      }

      position += velocity;

      if(state == State.SHOOT)
      {
        facing_left = true;
        frame += 10;
        if(frame > 100 && !fired)
        {
          fired = true;
          Game.instance.enemy_bullet_group.add(new PenguinFlyerBullet(position));
        }
        if(frame > 200)
        {
          state = State.IDLE;
          idle_timeout = 60;
          frame = 0;
        }
      }
    }
  }

  public class PenguinFlyerBullet : Entity
  {
    public const float damage = 1.5f;
    public float frame = 0;

    public PenguinFlyerBullet(Vector2 position_)
    {
      Choom.PlayEffect(SoundAssets.BoomerangShot);
      position = position_;
      velocity = new Vector2(-2.5f, -0.75f);
    }

    public override void tick()
    {
      frame += 0.1f;
      position += velocity;
      velocity.y += 0.01f;
      if(position.x < 250)
      {
        Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
        remove = true;
        Game.instance.enemy_bullet_group.add(new Explosion(position, Vector2.zero,
          20, 0, Gun.Ammo.NONE));
        Game.instance.damageTargetPiece(damage);
      }
    }

    public override void draw()
    {
      var color = Vector4.one;
      if(Game.instance.house.visibleDoorPosition()<1)
        color.w = 0.4f;
      AppMain.textures.penguinflyingbullet.draw((int)frame%2, position, Vector2.one, color);
    }
  }
}
