using System;
using UnityEngine;
using Necrosoft;

namespace Gunhouse
{
    public class DrDogBackgroundDay : Entity
    {
        Vector2 scaleAmount = new Vector2(-1.05f, 1.05f);

        public const int n_clouds = 5;
        public Vector4[] clouds = new Vector4[n_clouds];
        bool displayAnchor = false;

        public DrDogBackgroundDay()
        {
            displayAnchor = !(AppMain.top_state is MenuState);

            for (int i = 0; i < n_clouds; i++)
                clouds[i] = new Vector4(Util.rng.NextFloat(0, AppMain.vscreen.x),
                                        Util.rng.NextFloat(0, 400),
                                        Util.rng.NextFloat(0.025f, 0.1f),
                                        0);
        }

        public override void tick()
        {
            for (int i = 0; i < n_clouds; ++i)
            {
                clouds[i].x += clouds[i].z;
                clouds[i].y += clouds[i].w;
                if (clouds[i].x < -200) { clouds[i].x = AppMain.vscreen.x + 200; }
              }
        }

        public virtual Atlas atlas()
        {
            return AppMain.textures.stage_drdog_noon;
        }

        public override void draw()
        {
            if (displayAnchor) {
                int sprite = (int)stage_drdog_anchors.Sprites.noon_anchor;
                if (atlas() == AppMain.textures.stage_drdog_dusk) { sprite = (int)stage_drdog_anchors.Sprites.dusk_anchor; }
                if (atlas() == AppMain.textures.stage_drdog_night) { sprite = (int)stage_drdog_anchors.Sprites.night_anchor; }
                
                AppMain.textures.stage_drdog_anchors.draw(sprite,
                                                             new Vector2(340 * 0.5f, AppMain.vscreen.y - 70 * 0.5f),
                                                             scaleAmount, Vector4.one);
            }

            atlas().draw((int)stage_drdog_noon.Sprites.background, AppMain.vscreen * 0.5f, scaleAmount, Vector4.one);
            atlas().draw((int)stage_drdog_noon.Sprites.sun, new Vector2(850, 100), scaleAmount, Vector4.one);

            for (int i = 0; i < n_clouds; ++i) {
                atlas().draw((int)stage_drdog_noon.Sprites.cloud_2 - i % 3, clouds[i], scaleAmount, Vector4.one);
            }

            atlas().draw((int)stage_drdog_noon.Sprites.platform,
                         new Vector2(AppMain.vscreen.x * 0.5f, AppMain.vscreen.y - 396 * 0.5f - 10), scaleAmount, Vector4.one);

            #if FIXED_16X9
            atlas().draw((int)stage_drdog_noon.Sprites.ground,
                         new Vector2(AppMain.vscreen.x * 0.5f, AppMain.vscreen.y - 80), scaleAmount, Vector4.one);
            #else
            atlas().draw((int)stage_drdog_noon.Sprites.ground,
                         new Vector2(AppMain.vscreen.x * 0.5f, AppMain.vscreen.y - 45), scaleAmount, Vector4.one);
            #endif
        }
    }

    public class DrDogBackgroundDusk : DrDogBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_drdog_dusk;
        }
    }

    public class DrDogBackgroundNight : DrDogBackgroundDay
    {
        public override Atlas atlas()
        {
            return AppMain.textures.stage_drdog_night;
        }
    }

    public class DrDogMiniBullet : Entity
    {
        public float frame;
        public Vector2 dest;
        public float speed, damage;

        public DrDogMiniBullet(Vector2 position_, float speed_ = 0, float damage_ = 0)
        {
            Choom.PlayEffect(SoundAssets.Gatling[Util.rng.Next(SoundAssets.Gatling.Length)]);
            position = position_;
            speed = speed_;
            damage = damage_;
            if (speed == 0) { speed = DrDogMiniboss.bullet_speed; }
            if (damage == 0) { damage = DrDogMiniboss.bullet_damage; }
        }

        public override void tick()
        {
            dest = Game.instance.targetDestination();
            velocity = (dest - position).normalized * speed;
            angle = Util.angle(velocity);
            frame += DrDogMiniboss.bullet_anim_speed / 60.0f;
            position += velocity;

            Vector2 pos = dest - position;

            if (pos.magnitude < 20)
            {
                Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
                if (dest == Game.instance.targetDestination()) {
                    Game.instance.damageTargetPiece(DrDogMiniboss.bullet_damage);
                }
                remove = true;
            }
        }

        public override void draw()
        {
            AppMain.textures.drdogminiboss.atlas.draw((int)drdog_miniboss.Sprites.drdog_miniboss_bullet1 + (int)frame % 4,
                                                      position, new Vector2(-1, 1), (float)(angle + Math.PI), Vector4.one);
        }
    }

    public partial class DrDogMiniboss : Target
    {
        public enum State
        {
            APPROACH,
            AIMING,
            FIRING,
            WAITING,
            DYING
        };

        public State state = State.APPROACH;
        public float frame = 0;
        public float x_destination = 0;
        public float cannon_angle = 0;
        public float dest_angle = 0;
        public bool fired = false;

        public DrDogMiniboss()
        {
            position = new Vector2(1150, 380);
            x_destination = Util.rng.NextFloat(500, 800);
            size = new Vector2(120, 120);
            hp = health;
        }

        static public void loadAssets()
        {
            AppMain.textures.drdogminiboss.touch();
            AppMain.textures.drdogminiboss.atlas.switchTexture(MetaState.wave.enemy_palette);
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
            if (frozenThisFrame()) { return; }

            if (state == State.APPROACH)
            {
                velocity = new Vector2(position.x > x_destination ? - Util.rng.NextFloat(approach_speed_min, approach_speed_max) : 0, 0);
                position += velocity;
                frame += 10;
                if (velocity.x == 0) { state = State.AIMING; }
            }

            if (state == State.AIMING)
            {
                dest_angle = Util.angle(position + new Vector2(10, -58) - Game.instance.targetDestination());

                if (Math.Abs(cannon_angle - dest_angle) < aim_turn_rate * 2)
                {
                    state = State.FIRING;
                    frame = 0;
                    fired = false;
                }
                else if (cannon_angle < dest_angle) { cannon_angle += aim_turn_rate; }
                else if (cannon_angle > dest_angle) { cannon_angle -= aim_turn_rate; }
            }

            if (state == State.FIRING)
            {
                frame += 10;
                if (frame >= 300 && !fired)
                {
                    fired = true;
                    Vector2 gun_position = position + new Vector2(28, -78);

                    Game.instance.enemy_bullet_group.add(new DrDogMiniBullet(gun_position +
                                                                             (Game.instance.targetDestination() -
                                                                             gun_position).normalized * 80));
                }

                if (frame >= 450)
                {
                    state = State.WAITING;
                    frame = 0;
                }
            }

            if (state == State.WAITING)
            {
                if (++frame > attack_cooldown * 60) state = State.AIMING;
            }

            if (state == State.DYING)
            {
                frame += 10;
                if (frame >= 1000) { finishDying(); }
            }
        }

        public override void draw()
        {
            Vector2 cannon_offset = new Vector2(0, -78);
            if (state == State.FIRING && frame >= 300)
                cannon_offset.x += 18;

            if (state != State.DYING)
                AppMain.textures.shadowblob.draw(0, new Vector2(position.x - 15, 450),
                                         Vector2.one / 2, new Vector4(1, 1, 1, 0.5f));

            if (state != State.DYING)
                AppMain.textures.drdogminiboss.atlas.draw(
                    (int)drdog_miniboss.Sprites.drdog_miniboss_cannon,
                    position + cannon_offset, new Vector2(0.15f, 0.5f), facingScale(),
                    cannon_angle, flashColor());

            string anim = "walk";
            if (state == State.DYING) anim = "attack-death";
            if (state == State.FIRING) anim = "attack-body only";

            AppMain.textures.drdogminiboss.draw(anim, (int)frame,
                                          position + new Vector2(0, 80), facingScale(), 0, flashColor());
            drawSubBullets();
        }
    }

    public partial class DrDogFlyer : Target
    {
        public State state, next_state = State.GRAB;

        public enum State
        {
            NONE,
            FLYING,
            SHOOT,
            GRAB,
            DIE}

        ;

        public Vector2 origin, destination;
        public float lerp_pos, lerp_speed;
        public float frame = 0;
        public int lane;
        public const int scale = 2;

        public DrDogFlyer()
        {
            lane = Util.rng.Next(2) == 0 ? House.lane_a : House.lane_b;
            position = new Vector2(1120, pickLane());
            state = State.NONE;
            size = new Vector2(85, 85);
            hp = health;
        }

        public int pickLane()
        {
            return lane;
        }

        static public void loadAssets()
        {
            AppMain.textures.drdogflying.touch();
            AppMain.textures.drdogflying.atlas.switchTexture(MetaState.wave.enemy_palette);
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
            string anim = "idle";
            if (state == State.SHOOT) anim = "shoot";
            if (state == State.GRAB) anim = "grab";
            if (state == State.DIE) anim = "death";
            if (held_orphans.Count > 0) anim = "grab and walk";

            if (state != State.DIE)
                AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 460),
                                         Vector2.one / 4, new Vector4(1, 1, 1, 0.5f));

            AppMain.textures.drdogflying.draw(anim, (int)frame,
                                              position + new Vector2(0, 200 / scale), facingScale() / scale, 0, flashColor());
            drawSubOrphans();
            drawSubBullets();
        }

        public override void startDying()
        {
            if (state == State.DIE) return;

            Choom.PlayEffect(SoundAssets.EnemyDie);
            state = State.DIE;
            frame = 0;
        }

        public override void tick()
        {
            base.tick();
            if (frozenThisFrame()) return;

            if (state == State.NONE) {
                frame = 0;
                state = State.FLYING;
                if (Util.rng.NextDouble() < shoot_chance) next_state = State.SHOOT;
                else next_state = State.GRAB;

                if (next_state == State.GRAB)
                    setupMove(new Vector2(grab_distance, pickLane()));
                else
                    setupMove(new Vector2(Util.rng.NextFloat(500, 900), pickLane()));
            }

            if (state == State.FLYING && position != destination) {
                lerp_pos += lerp_speed;
                if (lerp_pos >= 1.0f) {
                    position = destination;
                    velocity = Vector2.zero;
                }
                else {
                    var old_smooth_lerp = Util.smoothStep(lerp_pos - lerp_speed);
                    var old_position = origin * (1 - old_smooth_lerp) + destination * old_smooth_lerp;
                    if (Vector2.Distance(position, old_position) > 0) {
                        position = old_position;
                        lerp_pos -= lerp_speed;
                        velocity = Vector2.zero;
                    }
                    else {
                        var smooth_lerp = Util.smoothStep(lerp_pos);
                        var new_position = origin * (1 - smooth_lerp) + destination * smooth_lerp;
                        velocity = new_position - position;
                    }
                }
            }

            if (state == State.FLYING) {
                frame += fly_anim_speed;

                if (Shield.existing_shield != null && destination.x < ForkGun.shield_right) {
                    frame = 0;
                    state = State.SHOOT;
                    position.x = ForkGun.shield_right;
                    destination.x = ForkGun.shield_right;
                }

                if (held_orphans.Count > 0 && position.x > 1100) {
                    remove = true;
                }

                if (frame > 800) {
                    frame -= 800;
                    if (lerp_pos >= 1.0f) {
                        state = next_state;
                        frame = 0;
                    }
                }
            }

            position += velocity;

            if (state == State.SHOOT) {
                facing_left = true;
                var new_frame = frame + shoot_anim_speed;
                if (frame <= 609 && new_frame > 609)
                    Game.instance.enemy_bullet_group.add(
                        new DrDogMiniBullet(position + new Vector2(-69 / scale, 200 / scale - 49 / scale),
                                DrDogFlyer.shot_speed, DrDogFlyer.shot_damage));
                frame = new_frame;
                if (frame > 790) state = State.NONE;
            }
            if (state == State.DIE) {
                frame += die_anim_speed;
                if (frame >= 600) finishDying();
            }
            if (state == State.GRAB) {
                frame += grab_anim_speed;
                if (Shield.existing_shield != null || position.x > grab_distance) {
                    position.x = ForkGun.shield_right;
                    setupMove(new Vector2(Util.rng.NextFloat(500, 900), pickLane()));
                    state = State.FLYING;
                }
                if (frame > 1000) {
                    state = State.FLYING;
                    setupMove(new Vector2(1200, pickLane()));
                    int grabbed = 2;

                    Choom.PlayEffect(SoundAssets.BlockLand);
                    Choom.PlayEffect(SoundAssets.OrphanTake);

                    for (int i = 0; i < grabbed; i++)
                        held_orphans.Add(new Orphan(new Vector2(250 / scale + i * 190 / scale, 50 / scale)));
                }
            }
        }
    }

    public class DrDogCockpit : Target
    {
        public DrDog drdog;
        public float frame;

        public DrDogCockpit(DrDog drdog_)
        {
            drdog = drdog_;
            size = new Vector2(100, 60);
            tick();
        }

        public override void tick()
        {
            frame += 300 / 60.0f;
            hp = 1;
            position = drdog.position + new Vector2(22, -85 * 3 / 2);
            if (drdog.hp <= 0) remove = true;
            base.tick();
        }

        public override void damage(float hp, Gun.Ammo type)
        {
            drdog.damage(hp, type);
        }

        public override void draw()
        {
            if (drdog.animation == "backmissiles launch no hatch") {
                AppMain.textures.drdog.draw("backmissiles launch hatch only",
                                    (int)drdog.frame, drdog.position + new Vector2(42, 150),
                                    drdog.facingScale(), 0, drdog.flashColor());
            }
            string animation = "cockpit normal";
            if (drdog.hp < 1200) animation = "cockpit damage1";
            if (drdog.hp < 600) animation = "cockpit damage2";
            AppMain.textures.drdog.draw(animation, (int)frame,
                                  position + new Vector2(0, 60),
                                  drdog.facingScale(), 0, drdog.flashColor());
            drawSubBullets();
        }
    }

    public partial class DrDog : Target
    {
        public State state, next_state;

        public enum State
        {
            JUMPING,
            WAITING,
            PUNCHING,
            DYING,
            EATING,
            MISSILES}

        ;

        public float frame = 0;
        public int timeout = 0;
        public SpriterSet ss = AppMain.textures.drdog;
        public int last_missile = 0;
        public string animation;
        public int orphans_eaten = 0;
        public float drop_damage = 0;

        public DrDog()
        {
            state = State.JUMPING;
            size = new Vector2(200, 200);
            velocity = new Vector2(-approach_speed, 0);
            position = new Vector2(1300, 250);
            hp = health;
            animation = "stance";
            Game.instance.enemy_group.add(new DrDogCockpit(this));
        }

        static public void loadAssets()
        {
            AppMain.textures.drdog.touch();
            AppMain.textures.drdog.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
            if (state == State.DYING) return;

            Choom.PlayEffect(SoundAssets.BossDie1);
            state = State.DYING;
            animation = "dead";
            Game.instance.removeBoss();
            velocity = Vector2.zero;
            frame = 120;
            for (int i = 0; i < orphans_eaten; i++)
                held_orphans.Add(new Orphan(Vector2.zero));
            releaseOrphans();
        }

        public override void tick()
        {
            base.tick();
            facing_left = true;
            if (frozenThisFrame()) return;

            if (animation == "eat") frame += punch_speed;
            else if (animation == "backmissiles launch no hatch") frame += 4;
            else if (animation == "dead")
                frame += die_speed;
            else frame += 500 / 60.0f;

            if (frame >= ss.animations[animation].length) {
                frame -= ss.animations[animation].length;
                if (state == State.DYING) finishDying();
            }

            if (state == State.JUMPING) {
                position += velocity;
                velocity.y += gravity;
                if (position.y > 320) {
                    position.y = 320;
                    velocity.y = 0;
                    state = State.WAITING;
                    timeout = (int)jump_wait;
                }
            }
            if (state == State.PUNCHING) {
                if (frame > 700) {
                    state = State.EATING;
                    int n_orphans = 5;
                    Choom.PlayEffect(SoundAssets.OrphanTake);
                    for (int i = 0; i < n_orphans; i++)
                        held_orphans.Add(new Orphan(new Vector2(
                            Util.rng.NextFloat(-61, 0), Util.rng.NextFloat(60 * 3 / 2, 90 * 3 / 2))));
                }
            }
            if (state == State.MISSILES) {
                if (frame > 247 && frame < 613) {
                    int frames_per_missile = (613 - 247) / missiles_per_burst;
                    int next_missile = last_missile + frames_per_missile;
                    if (frame - 247 > next_missile) {
                        var missile = new DrDogMissile(position + new Vector2(75 * 3 / 2, -85 * 3 / 2));
                        missile.drdog = this;
                        Game.instance.enemy_bullet_group.add(missile);
                        last_missile += frames_per_missile;
                    }
                }
                if (frame > 800) {
                    state = State.WAITING;
                    timeout = 100;
                    animation = "stance";
                }
            }
            if (state == State.EATING) {
                if (frame > 800) {
                    orphans_eaten += held_orphans.Count;
                    Game.instance.house.damage(held_orphans.Count);
                    held_orphans.Clear();
                }
                if (frame > 1400) {
                    state = State.WAITING;
                    timeout = 100;
                    animation = "stance";
                }
            }
            if (state == State.WAITING) {
                if (--timeout <= 0) {
                    if (position.x < punch_distance) {
                        if (Util.rng.NextFloat(0, 1) < punch_chance) {
                            state = State.PUNCHING;
                            drop_damage = 0;
                            animation = "eat";
                            frame = 0;
                        }
                        else {
                            state = State.JUMPING;
                            velocity = new Vector2(approach_speed, -jump_force);
                        }
                    }
                    else {
                        if (position.x < missile_distance &&
                        Util.rng.NextFloat(0, 1) < missile_chance) {
                            state = State.MISSILES;
                            last_missile = 0;
                            animation = "backmissiles launch no hatch";
                            frame = 0;
                        }
                        else {
                            state = State.JUMPING;
                            velocity = new Vector2(-approach_speed, -jump_force);
                        }
                    }
                }
            }
        }

        public override void damage(float by, Gun.Ammo type)
        {
            drop_damage += by;
            if (drop_damage > orphan_drop_damage && held_orphans.Count > 0)
                releaseOrphans();

            base.damage(by, type);
        }

        public override void draw()
        {
            AppMain.textures.shadowblob.draw(0, new Vector2(position.x + 20, 455),
                                       Vector2.one / (1.5f + (float)Math.Abs(position.y - 320) / 60),
                                       new Vector4(1, 1, 1, 0.5f));
            ss.draw(animation, (int)frame, position + new Vector2(25, 135),
              facingScale(), 0, flashColor());
            drawSubBullets();
            drawSubOrphans();
        }
    }

    public class DrDogMissile : Entity
    {
        public Vector2 orientation;
        public DrDog drdog;
        public float age = 0;

        public DrDogMissile(Vector2 position_)
        {
            Choom.PlayEffect(SoundAssets.Gatling[Util.rng.Next(SoundAssets.Gatling.Length)]);
            position = position_;
            angle = Util.rng.NextFloat((float)Math.PI, (float)Math.PI * 8 / 6);
            velocity = Util.fromPolar(angle, 1);
        }

        public override void tick()
        {
            age += 1 / 60.0f;

            var target_position = Game.instance.targetDestination();

            var dest_velocity = target_position - position;
            var dest_modifier = dest_velocity - velocity;
            var dest_angle = Util.angle(dest_modifier);

            angle %= (float)Math.PI * 2;
            if (Math.Abs(dest_angle - angle) > Math.Abs(dest_angle + Math.PI * 2 - angle))
                dest_angle += (float)Math.PI * 2;
            if (Math.Abs(dest_angle - angle) > Math.Abs(dest_angle - Math.PI * 2 - angle))
                dest_angle -= (float)Math.PI * 2;

            if (angle < dest_angle) angle += DrDog.missile_rotation_speed;
            if (angle > dest_angle) angle -= DrDog.missile_rotation_speed;
            if (Math.Abs(dest_angle - angle) < DrDog.missile_rotation_speed) {
                angle = dest_angle;
                velocity += Util.fromPolar(angle, DrDog.missile_acceleration);
            }

            if (velocity.magnitude > DrDog.missile_max_speed) {
                velocity = new Vector2(velocity.x, velocity.y).normalized * DrDog.missile_max_speed;
            }

            position += velocity;

            if (position.x < Puzzle.grid_left + Puzzle.piece_size * 3 ||
            age > DrDog.missile_lifetime) {
                remove = true;
                Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
                Game.instance.enemy_bullet_group.add(new Explosion(position, Vector2.zero,
                                                           20, 0, Gun.Ammo.NONE));
                if (age < DrDog.missile_lifetime)
                    Game.instance.damageTargetPiece(DrDog.missile_damage);
            }
        }

        public override void draw()
        {
            AppMain.textures.drdogbullet.draw("hatch missile", 0, position,
                                        new Vector2(-0.5f * 3 / 2, 0.5f * 3 / 2) / 2, angle + (float)Math.PI, Vector4.one);
        }
    }

    public partial class DrDogMinion2Bullet : Entity
    {
        public float frame;
        public bool explode = false;

        public DrDogMinion2Bullet(Vector2 position_)
        {
            Choom.PlayEffect(SoundAssets.LaserShot);
            position = position_;
            velocity = new Vector2(-DrDogMinion2.fire_speed, 0);
            size = new Vector2(30, 30);
        }

        public override void tick()
        {
            position += velocity;
            if (!explode && position.x < Puzzle.grid_left + Puzzle.piece_size * 3) {
                Choom.PlayEffect(SoundAssets.HitHouse[Util.rng.Next(SoundAssets.HitHouse.Length)]);
                Game.instance.damageTargetPiece(DrDogMinion2.fire_damage);
                explode = true;
                velocity = Vector2.zero;
                frame = 0;
            }
            frame += 15;
            if (explode && frame > 290) remove = true;
        }

        public override void draw()
        {
            var color = Vector4.one;
            if (Game.instance.house.visibleDoorPosition() < 1)
                color.w = 0.4f;
            AppMain.textures.drdogminion2bullet.draw(explode ? "explosion" : "bullet", (int)frame, position,
                                               Vector2.one, angle, color);
        }
    }

    public partial class DrDogMinion2 : Target
    {
        public enum State
        {
            APPROACH,
            IDLE,
            SHOOT,
            DEATH

        }

        public State state = State.APPROACH;
        public float frame;
        public float destination_x;
        public float idle_timeout;
        public bool shot;

        public DrDogMinion2()
        {
            position = new Vector2(1050, House.lane_b - 40);
            velocity = new Vector2(
                -Util.rng.NextFloat(approach_speed_min, approach_speed_max), 0);
            destination_x = Util.rng.NextFloat(destination_min, destination_max);
            size = new Vector2(60, 80);
            hp = health;
            //held_orphans.Add(new Orphan(new Vector2(0, 0)));
        }

        static public void loadAssets()
        {
            AppMain.textures.drdogminion2.touch();
            AppMain.textures.drdogminion2.atlas.switchTexture(MetaState.wave.enemy_palette);
            AppMain.textures.drdogminion2bullet.touch();
            AppMain.textures.drdogminion2bullet.atlas.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
            if (state == State.DEATH) return;
            Choom.PlayEffect(SoundAssets.EnemyDie);
            state = State.DEATH;
            frame = 0;
        }

        public override void tick()
        {
            base.tick();
            if (frozenThisFrame()) return;

            position += velocity;

            if (state == State.DEATH) {
                frame += 10;
                if (frame > 950) remove = true;
            }

            if (state == State.APPROACH) {
                frame += 10;
                if (position.x <= destination_x) {
                    state = State.IDLE;
                    velocity.x = 0;
                    idle_timeout = (int)(Util.rng.NextFloat(fire_wait_min, fire_wait_max) * 60);
                    frame = 0;
                }
            }

            if (state == State.IDLE) {
                if (frame < 320) frame += 10;
                if (--idle_timeout <= 0) {
                    state = State.SHOOT;
                    shot = false;
                }
            }

            if (state == State.SHOOT) {
                frame += 10;
                if (frame > 700 && !shot) {
                    Game.instance.enemy_bullet_group.add(
                        new DrDogMinion2Bullet(position + new Vector2(-21, 0)));
                    shot = true;
                }
                if (frame > 960) {
                    frame = 0;
                    state = State.IDLE;
                    idle_timeout = (int)(Util.rng.NextFloat(fire_wait_min, fire_wait_max) * 60);
                }
            }
        }

        public override void draw()
        {
            drawSubOrphans();
            string anim = "walk";
            if (state == State.DEATH) anim = "death";
            if (state == State.IDLE) anim = "shoot";
            if (state == State.SHOOT) anim = "shoot";
            if (state != State.DEATH)
                AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 275),
                                         Vector2.one / 6, new Vector4(1, 1, 1, 0.5f));
            AppMain.textures.drdogminion2.draw(anim, (int)frame, position + new Vector2(0, 40), facingScale(),
                                         angle, flashColor());
            drawSubBullets();
            /*bb.Add(new BitmapDrawCall(AppMain.textures.drdog.atlas.texture,
        position+origin,
        AppMain.textures.drdog.atlas.sprite_bounds[1], flashColor(), facingScale(),
        AppMain.textures.drdog.atlas.centers[1], 0));*/
            //scale.x = -scale.x;
            //AppMain.textures.drdog.draw("stance", 0, position, scale,
            //  angle, flashColor());
        }
    }

    public partial class DrDogMinion : Target
    {
        public int state;
        public float sub_frame = 0, frame_rate;
        public int frame;

        public DrDogMinion()
        {
            position = new Vector2(1050, 250);
            velocity = new Vector2(
                -Util.rng.NextFloat(approach_speed_min, approach_speed_max), 0);
            size = new Vector2(40, 60);
            hp = health;
            frame_rate = Util.rng.NextFloat(0.1f, 0.3f);
            //held_orphans.Add(new Orphan(new Vector2(0, 0)));
        }

        static public void loadAssets()
        {
            AppMain.textures.minion.touch();
            AppMain.textures.minion.switchTexture(MetaState.wave.enemy_palette);
        }

        public override void startDying()
        {
            if (state == (int)drdog_minion.Sprites.drdog_minion_death1) return;

            Choom.PlayEffect(SoundAssets.EnemyDie);
            state = (int)drdog_minion.Sprites.drdog_minion_death1;
            sub_frame = 0;
        }

        public override void tick()
        {
            base.tick();
            if (frozenThisFrame()) return;

            velocity += new Vector2(0, gravity);
            position += velocity;
            if (position.y > 450) {
                position.y = 450;
                velocity.y = 0;
            }

            if (state == (int)drdog_minion.Sprites.drdog_minion_death1) {
                sub_frame += 0.2f;
                frame = state + (int)sub_frame;
                if (sub_frame > 10) finishDying();
                return;
            }

            if (position.x > 1100) {
                remove = true;
            }

            if (position.x < steal_distance) {
                velocity.x = -velocity.x;
                Choom.PlayEffect(SoundAssets.OrphanTake);
                held_orphans.Add(new Orphan(new Vector2(20, -10)));
            }

            if (velocity.y != 0)
                state = (int)drdog_minion.Sprites.drdog_minion_jump;
            else
                state = (int)drdog_minion.Sprites.drdog_minion_walk1;

            if (state == (int)drdog_minion.Sprites.drdog_minion_walk1) {
                sub_frame += 0.1f;
                if (sub_frame >= 4) sub_frame -= 4.0f;
                frame = (int)drdog_minion.Sprites.drdog_minion_walk1 +
                (int)(sub_frame >= 3 ? 1 : sub_frame);

                if (held_orphans.Count > 0)
                    frame += (int)drdog_minion.Sprites.drdog_minion_walkg1 -
                    (int)drdog_minion.Sprites.drdog_minion_walk1;
            }
            else frame = state;
        }

        public override void draw()
        {
            drawSubOrphans();
            if (state != (int)drdog_minion.Sprites.drdog_minion_death1)
                AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 480),
                                         Vector2.one / 6, new Vector4(1, 1, 1, 0.5f));
            AppMain.textures.minion.draw(frame, position, facingScale() / 2, angle,
                                   flashColor());
            drawSubBullets();
            /*bb.Add(new BitmapDrawCall(AppMain.textures.drdog.atlas.texture,
        position+origin,
        AppMain.textures.drdog.atlas.sprite_bounds[1], flashColor(), facingScale(),
        AppMain.textures.drdog.atlas.centers[1], 0));*/
            //scale.x = -scale.x;
            //AppMain.textures.drdog.draw("stance", 0, position, scale,
            //  angle, flashColor());
        }
    }
}
