using System;
using System.Collections.Generic;
using UnityEngine;
using Necrosoft;

namespace Gunhouse
{
    public class Gun : Entity
    {
        /* NOTE(shane): unfortunately the order of this enum matters because of how the store upgrades work */
        public enum Ammo { DRAGON = 0, IGLOO, SKULL, VEGETABLE, LIGHTNING, FLAME, FORK, BOUNCE, BOOMERANG, SIN, GATLING, NONE };

        public EntityGroup targets = null, bullets = null;
        public Target current_target = null;
        public int gun_index = 0;

        public Vector2 aim_at = Vector2.zero;
        public Vector2 aim_vector = new Vector2(1, 0);
        public float desired_angle = 0;

        public float fire_timeout = 0;
        public Ammo ammo = Ammo.GATLING;
        public int ammo_ct = 0;
        public int upgrade = 0;
        public bool is_destroyed = false;
        public int destroy = 0;

        public List<Entity> reticle_targets;
        public int time = 0;

        public float fire_rate, aim_error, turn_speed;

        public bool firing = true;
        public bool selected = false;

        public Gun(Vector2 origin, EntityGroup targets_, EntityGroup bullets_)
        {
            position = origin;
            targets = targets_;
            bullets = bullets_;
            fire_timeout = 0;
            reticle_targets = new List<Entity>();
        }

        public override void tick()
        {
            // Gatling guns don't shoot!
            if (ammo == Ammo.GATLING) { return; }

            time++;

            if (is_destroyed) {
                destroy--;
                if (destroy < 0) {
                    is_destroyed = false;
                    feed(Ammo.GATLING, 0);
                    firing = false;
                }
            }

            reticle_targets.Clear();

            // remove dead targets from aim list
            for (int i = 0; i < reticle_targets.Count; i++) {
                if (reticle_targets [i].remove || (reticle_targets [i] as Target).hp <= 0) {
                    reticle_targets.RemoveAt(i--);
                }
            }

            /* find aim vector for current target */
            if (reticle_targets.Count != 0) {
                current_target = (Target)reticle_targets[0];
            }
            else {
                current_target = targets.findClosest (position, ammo, 100);
            }

            if (current_target != null) {
                aim_at = current_target.position;
            }
            else {
                aim_at = Vector2.zero;
            }

            if (aim_at == Vector2.zero) {
                aim_vector = new Vector2 (1, 0);
                desired_angle = 0;
            }
            else {
                if (ammo == Ammo.SKULL) {
                    aim_vector = (aim_at - (position + new Vector2 (0, 30)));
                }
                else {
                    aim_vector = (aim_at - position);
                }

                aim_vector.Normalize();
                desired_angle = (float)Math.Atan2 (aim_vector.y, aim_vector.x);

                if (ammo == Ammo.DRAGON) {
                    desired_angle -= DragonGun.aim_lead * (aim_at - position).magnitude;
                }
            }

            /* fire at current angle */
            angle = Math.Abs (angle - desired_angle) < turn_speed ? desired_angle : angle;
            angle += angle < desired_angle ? turn_speed : 0;
            angle -= angle > desired_angle ? turn_speed : 0;

            // hard lock angle unless veggie gun
            if (ammo != Ammo.IGLOO && ammo != Ammo.FLAME) {
                angle = 0;
                current_target = null;
            }

            int boosted_upgrade = upgrade;

            if (--fire_timeout <= 0 && (firing || ammo == Ammo.GATLING)) {

                var noisy_angle = angle + Util.rng.NextFloat(-aim_error, aim_error);
                var noisy_aim = new Vector2((float)Math.Cos(noisy_angle), (float)Math.Sin(noisy_angle));
                fire_timeout = (int)(60 / fire_rate);

                if (ammo == Ammo.SIN) {
                    Choom.PlayEffect(SoundAssets.MathShot);
                    Game.instance.bullet_group.add(new SinBullet(position + new Vector2(150, 0), new Vector2 (2, 0),
                                                                 Game.instance.enemy_group, null, boosted_upgrade));
                }

                if (ammo == Ammo.BOUNCE) {
                    Choom.PlayEffect(SoundAssets.BeachBallShot);
                    Game.instance.bullet_group.add(new BeachBall(position + new Vector2 (105, -10), new Vector2 (2, 0),
                                                                 Game.instance.enemy_group, null, boosted_upgrade));
                }

                if (ammo == Ammo.FORK) {
                    Choom.PlayEffect(SoundAssets.ForkShot);
                    Forker.bullet(position + new Vector2(130, -20), boosted_upgrade);
                }

                if (ammo == Ammo.BOOMERANG) {
                    Boomeranger.bullet(position + new Vector2(135, 10), boosted_upgrade, gun_index);
                }

                if (ammo == Ammo.FLAME) {
                    Flamer.emitFlame(position + (noisy_aim * 75 + new Vector2(noisy_aim.y, -noisy_aim.x) * -15) * 3 / 2,
                                     noisy_aim, boosted_upgrade, false);
                }

                if (ammo == Ammo.SKULL) {
                    float aim_angle = Util.angle (noisy_aim);
                    float half_arc = SkullGun.fire_arc / 2.0f;
                    for (float a = aim_angle - half_arc; a <= aim_angle + half_arc; a +=
                         SkullGun.fire_arc / SkullGun.skulls_per_shot) {
                        Skuller.addBullet(position + noisy_aim * 100 * 1.5f + new Vector2(noisy_aim.y, -noisy_aim.x) * -30,
                                          Util.fromPolar(a, 1), boosted_upgrade, Game.instance.enemy_group,
                                          reticle_targets.Count > 0 ? reticle_targets [0] as Target : null);
                    }

                    Choom.PlayEffect(SoundAssets.SkullShot);
                }

                if (ammo == Ammo.IGLOO) {
                    Penguiner.penguin(position + noisy_aim * 95 * 1.5f + new Vector2(noisy_aim.y, -noisy_aim.x) * 10,
                                      noisy_aim * IglooGun.velocity, false, boosted_upgrade, targetList());
                    Choom.PlayEffect(SoundAssets.IceShot);
                }

                if (ammo == Ammo.LIGHTNING) {
                    Choom.PlayEffect(SoundAssets.LightningShot);
                    Lightning.lightningFrom(this, boosted_upgrade);
                }

                if (ammo == Ammo.VEGETABLE) {
                    Choom.PlayEffect(SoundAssets.VegShot);

                    DataStorage.ShotsFired++;
                    //Util.trace(DataStorage.ShotsFired);

                    int type = Util.rng.Next(6);
                    Particle vb = new Particle(AppMain.textures.veggies);
                    vb.frame = type;
                    vb.position = position + noisy_aim * 105 * 1.5f + new Vector2(noisy_aim.y, -noisy_aim.x) * 5;
                    vb.velocity = noisy_aim * VegetableGun.velocity;
                    vb.angle = Util.angle(noisy_aim);
                    vb.collides_with = targetList();

                    var s = VegetableGun.size + boosted_upgrade * VegetableGun.size_upgrade;
                    vb.scale = new Vector2(s / 8.0f, s / 8.0f);
                    vb.ground_at = 480;
                    vb.drawable_size = 100;
                    vb.collide_behavior = (ref Particle p, Entity e) => {
                        p.remove = true;
                        if (e != null) {
                            (e as Target).damage(VegetableGun.damage + boosted_upgrade * VegetableGun.damage_upgrade,
                                                 Gun.Ammo.VEGETABLE);
                        }

                        Particle splat = new Particle (AppMain.textures.veggies);
                        splat.frame = type / 2 * 4 + 8;
                        splat.frame_speed = 0.1f;
                        splat.loop_end = (int)splat.frame + 2;
                        splat.loop = false;
                        splat.position = p.position;
                        splat.velocity = e != null ? new Vector2 (e.velocity.x, 0) : Vector2.zero;
                        splat.angle = p.angle;
                        splat.scale = p.scale / 2;
                        Game.instance.bullet_manager.add(splat);
                    };

                    Game.instance.bullet_manager.add(vb);
                }

                if (ammo == Ammo.DRAGON) {
                    DragonGun.fireBullet(position, noisy_aim, boosted_upgrade, targetList());
                }

                ammo_ct--;
                if (ammo_ct <= 0 && ammo != Ammo.GATLING) {
                    stopFiring();
                    destroyGun();
                }
            }
        }

        public override void draw()
        {
            Vector2 size = new Vector2(1.0f, 1.0f) * 3 / 4;

            if (is_destroyed) {
                AppMain.textures.gunpoof.draw((int)7 - Mathf.FloorToInt(destroy / 5.0f),
                                              position + new Vector2(50.0f, 0), size * .5f, angle,
                                              Vector4.one);

                if (destroy < 30) return;
            }

            var vibrate = Util.fromPolar(Util.rng.NextFloat((float)Math.PI * 2), Util.rng.NextFloat(0, upgrade)) / 4;
            if (firing) vibrate /= 4;

            position += vibrate;

            bool selected = Game.instance.house.isDoorClosed &&
                            Game.instance.puzzle.selected_weapon == new Vector2(1, gun_index);

            var color = Vector4.one;// Gun.color(selected);

            if (selected && fireableExists() && AppMain.game_pad_active) {
                AppMain.textures.ui_game.draw((int)ui_game.Sprites.arrow,
                                              position - new Vector2(40 + Mathf.Abs(Mathf.Sin(AppMain.frame / 30f)) * 16, 0),
                                              Vector2.one, 0, Vector4.one);
            }

            int gun_id = -1;

            switch (ammo)
            {
            case Ammo.VEGETABLE: gun_id = (int)guns.Sprites.vegetable; break;
            case Ammo.IGLOO: gun_id = (int)guns.Sprites.ice; break;
            case Ammo.DRAGON: gun_id = (int)guns.Sprites.dragon; break;
            case Ammo.SKULL: gun_id = (int)guns.Sprites.skull; break;
            case Ammo.LIGHTNING: gun_id = (int)guns.Sprites.lightning; break;
            case Ammo.SIN: gun_id = (int)guns.Sprites.laser; break;
            case Ammo.FORK: gun_id = (int)guns.Sprites.gumball; break;
            case Ammo.BOUNCE: gun_id = (int)guns.Sprites.beachball; break;
            case Ammo.BOOMERANG: gun_id = (int)guns.Sprites.boomerang; break;
            case Ammo.FLAME: gun_id = (int)guns.Sprites.flame; break;
            }

            if (gun_id != -1) {
                AppMain.textures.house_guns.draw(gun_id, position, size, angle, color);
            }

            position -= vibrate;
        }

        public void startFiring()
        {
            if (firing) { return; }

            if (ammo == Ammo.BOOMERANG) ammo_ct = (int)(BoomerangGun.ammo + upgrade * BoomerangGun.ammo_upgrade);
            if (ammo == Ammo.BOUNCE) ammo_ct = (int)(BeachBallGun.ammo + upgrade * BeachBallGun.ammo_upgrade);
            if (ammo == Ammo.DRAGON) ammo_ct = (int)(DragonGun.ammo + upgrade * DragonGun.ammo_upgrade);
            if (ammo == Ammo.FLAME) ammo_ct = (int)(FlameGun.ammo + upgrade * FlameGun.ammo_upgrade);
            if (ammo == Ammo.FORK) ammo_ct = (int)(ForkGun.ammo + upgrade * ForkGun.ammo_upgrade);
            if (ammo == Ammo.IGLOO) ammo_ct = (int)(IglooGun.ammo + upgrade * IglooGun.ammo_upgrade);
            if (ammo == Ammo.LIGHTNING) ammo_ct = (int)(LightningGun.ammo + upgrade * LightningGun.ammo_upgrade);
            if (ammo == Ammo.SIN) ammo_ct = (int)(SinGun.ammo + upgrade * SinGun.ammo_upgrade);
            if (ammo == Ammo.SKULL) ammo_ct = (int)(SkullGun.ammo + upgrade * SkullGun.ammo_upgrade);
            if (ammo == Ammo.VEGETABLE) ammo_ct = (int)(VegetableGun.ammo + upgrade * VegetableGun.ammo_upgrade);

            firing = true;
            fire_rate = ammo_ct / Puzzle.gun_burst_length;

            if (ammo == Ammo.FLAME) { Choom.PlayEffect(SoundAssets.FlameShot, true); }
        }

        public void stopFiring()
        {
            if (ammo != Ammo.GATLING) { firing = false; }
            if (ammo == Ammo.FLAME) { Choom.StopEffect(SoundAssets.FlameShot); }
        }

        public void destroyGun()
        {
            is_destroyed = true;
            destroy = 39;
        }

        PuzzlePiece choosePiece()
        {
            for (;;)
            {
                PuzzlePiece p = Game.instance.puzzle.pieceAt(new Vector2(Util.rng.Next(3), gun_index * 2 + Util.rng.Next(1)));

                if (p != null) { return p; }
            }
        }

        public void feed(Ammo type, int amt)
        {
            if (ammo == type) { upgrade += amt - 1; }
            else { upgrade = amt - 1; }

            if (upgrade > Puzzle.max_upgrade_level) { upgrade = Puzzle.max_upgrade_level; }

            firing = false;

            if (upgrade > Puzzle.max_upgrade_level) { upgrade = Puzzle.max_upgrade_level; }

            ammo = type;
            angle = 0;

            if (ammo == Ammo.GATLING) { ammo_ct = 0; }

            if (ammo == Ammo.SKULL)
            {
                fire_rate = SkullGun.fire_rate;
                turn_speed = SkullGun.turn_speed;
                aim_error = SkullGun.aim_error;
            }
            if (ammo == Ammo.FLAME)
                turn_speed = FlameGun.turn_speed;
            if (ammo == Ammo.DRAGON)
            {
                fire_rate = DragonGun.fire_rate;
                turn_speed = DragonGun.turn_speed;
                aim_error = DragonGun.aim_error;
            }
            if (ammo == Ammo.IGLOO)
            {
                fire_rate = IglooGun.fire_rate;
                turn_speed = IglooGun.turn_speed;
                aim_error = IglooGun.aim_error;
            }
            if (ammo == Ammo.VEGETABLE)
            {
                fire_rate = VegetableGun.fire_rate;
                turn_speed = VegetableGun.turn_speed;
                aim_error = VegetableGun.aim_error;
            }

            if (ammo_ct != 0) fire_rate = Math.Max(fire_rate, ammo_ct / Puzzle.attack_round_length);
        }

        public static bool fireableExists()
        {
            foreach(Entity e in Game.instance.gun_group.entities) {
                Gun g = e as Gun;
                if (g == null) continue;
                if (g.upgrade > 0) return true;
            }

            foreach(var s in Game.instance.house.special_attacks)
                if (s.Item2 > 0) return true;
            
            return false;
        }

        public List<Entity> targetList()
        {
            if (reticle_targets.Count > 0) { return reticle_targets; }
            return targets.entities;
        }

        public static float UpgradeMultiplier(Gun.Ammo ammo)
        {
            if (MetaState.hardcore_mode) {
                return MetaState.logCurve(1,
                                          Difficulty.gun_upgrade_base,
                                          Difficulty.gun_upgrade_steepness,
                                          Difficulty.gun_upgrade_amplification) / MetaState.monster_armor_coefficient;
            }

            for (int i = 0; i < (int)Gun.Ammo.NONE; ++i) {
                if ((Gun.Ammo)i != ammo) { continue; }

                return MetaState.logCurve(DataStorage.GunPower[i],
                                          Difficulty.gun_upgrade_base,
                                          Difficulty.gun_upgrade_steepness,
                                          Difficulty.gun_upgrade_amplification) / MetaState.monster_armor_coefficient;
            }

            return 1.0f;
        }
    }

    public class Bullet : Entity
    {
        public EntityGroup targets = null;
        public Target target = null;
        public int damage = 8;
        public float gravity = 0;
        public float ground = 420;

        public Bullet(Vector2 position_, Vector2 velocity_, EntityGroup targets_, Target target_ = null)
        {
            position = position_;
            velocity = velocity_;
            targets = targets_;
            target = target_;
            size = new Vector2 (5, 5);
            angle = (float)Math.Atan2(velocity.y, velocity.x);
            DataStorage.ShotsFired++;
            //Util.trace(DataStorage.ShotsFired);
        }

        public override void Dispose()
        {
            base.Dispose();

            targets = null;
            target = null;
        }

        public virtual void hit(Target t = null) { }

        public override void tick()
        {
            position += velocity;
            velocity.y += gravity;

            Entity t;
            if (targets.findCollision(this, out t)) {
                if (target == null || target == t) {
                    remove = true;
                    hit((Target)t);
                }
            }

            if (position.y > ground && velocity.y > 0) {
                remove = true;
                hit(null);
            }

            if (position.x > 1000 && velocity.x > 0 ||
                position.x < -50 && velocity.x < 0 ||
                position.y > 600 && velocity.y > 0 ||
                position.y < -50 && velocity.y < 0) {
                remove = true;
            }
        }

        public static void explosion(Vector2 position, Gun.Ammo type, Vector2 velocity,
                                     float size, int damage, EntityGroup targets, bool hidden = false)
        {
            for (int i = 0; i < Game.instance.enemy_group.entities.Count; ++i) {
                Target t = (Target)Game.instance.enemy_group.entities[i];

                if ((t.position - position).magnitude < size + t.size.magnitude) {
                    t.damage(damage, type);
                }
            }

            Choom.PlayEffect(SoundAssets.Explosion[Util.rng.Next(SoundAssets.Explosion.Length)]);

            if (hidden) { return; }

            if (type == Gun.Ammo.DRAGON) {
                Particle db = new Particle(AppMain.textures.gun_dragon);
                db.frame = 1;
                db.frame_speed = 0.15f;
                db.position = position;
                db.velocity = velocity;
                db.scale = new Vector2(size, size) / 96;
                db.origin = new Vector2(1.0f / 2, 3.0f / 4);
                db.loop_end = (int)gun_dragon.Sprites.special_0;
                Game.instance.bullet_manager.add(db);
            }
            else if (type == Gun.Ammo.SKULL) {
                Particle db = new Particle(AppMain.textures.skullbullet);
                db.frame = 4;
                db.frame_speed = 0.15f;
                db.position = position;
                db.velocity = velocity;
                db.scale = new Vector2(size, size) / 128;
                db.origin = new Vector2(1.0f / 2, 3.0f / 4);
                Game.instance.bullet_manager.add(db);
            }
            else {
                Particle p = new Particle(AppMain.textures.explosion);
                p.position = position;
                p.velocity = velocity;
                p.scale = new Vector2(size, size) / 32;
                p.frame_speed = 8.0f / 60;
                Game.instance.bullet_manager.add(p);
            }
        }

        public override void draw()
        {
            AppMain.textures.bullets.draw(0, position, Vector2.one, angle, Vector4.one);
        }
    }

    public class BeachBall : Bullet
    {
        public int upgrade = 0;
        public int sprite;
        public float spin;
        public Target[] hit_targets = new Target[10];
        public int next_hit_index = 0;
        public int ticks_since_last_hit = 0;
        Vector2 scale = new Vector2(0.5f, 0.5f);
        Vector2 scaleInvertX = new Vector2(-0.5f, 0.5f);

        public BeachBall(Vector2 position_, Vector2 velocity_, EntityGroup targets_, Target target_, int upgrade_)
                        : base (position_, velocity_, targets_, target_)
        {
            upgrade = upgrade_;
            ground = 600;
            int s = BeachBallGun.size + upgrade * BeachBallGun.size_upgrade;
            size = new Vector2(s, s);
            sprite = Util.rng.Next(4);
            gravity = BeachBallGun.gravity;
            randomSpin();
        }

        public override void Dispose()
        {
            base.Dispose();

            for (int i = 0; i < 10; i++) { hit_targets[i] = null; }
        }

        public void randomSpin()
        {
            spin = Util.rng.NextFloat(-0.1f, 0.1f);
        }

        public override void hit(Target t)
        {
            if (t == null) { return; }

            bool skip_hit = false;
            for (int i = 0; i < 10; i++) {
                if (t == hit_targets[i]) { skip_hit = true; }
            }

            if (!skip_hit) {
                ticks_since_last_hit = 0;
                hit_targets[next_hit_index] = t;
                next_hit_index = Util.clamp(next_hit_index + 1, 0, 9);

                t.damage(BeachBallGun.damage + BeachBallGun.damage_upgrade * upgrade, Gun.Ammo.BOUNCE);

                if ((velocity.y < 0 && position.y > t.position.y) || (velocity.y > 0 && position.y < t.position.y)) {
                    Choom.PlayEffect(SoundAssets.BeachBallBounce);
                    AppMain.screenShake(upgrade * 5, 20);
                    velocity.y = -velocity.y;
                    position.y += velocity.y;
                }
            }
            remove = false;
        }

        public override void tick()
        {
            base.tick();

            if (position.y > 450) {
                position.y = 450;
                velocity.y = -velocity.y;

                Choom.PlayEffect(SoundAssets.BeachBallBounce);
                randomSpin();
            }

            angle += spin;
            ticks_since_last_hit++;
            if (ticks_since_last_hit > 60) {
                for (int i = 0; i < 10; i++) { hit_targets [i] = null; }
            }
        }

        public override void draw()
        {
            AppMain.textures.gun_beachball.draw((int)gun_beachball.Sprites.bullet_blue + sprite,
                position, scale / 56 * size.x, angle, Vector4.one);

            AppMain.textures.gun_beachball.draw((int)gun_beachball.Sprites.bullet_shine,
                position, scaleInvertX / 56 * size.x, 0, Vector4.one);

            //AppMain.textures.beachball.draw((int)beachball.Sprites.gun_beachball_bullet1 + sprite,
            //                                position, Vector2.one / 56 * size.x, angle, Vector4.one);
            
            //AppMain.textures.beachball.draw((int)beachball.Sprites.gun_beachball_bullet_highlite,
                                            //position, new Vector2 (-1, 1) / 56 * size.x, 0, Vector4.one);
        }
    }

    public class SinBullet : Bullet
    {
        public int upgrade = 0;
        public float offset = 0;
        public float phase = 0;
        public float offset_cap = 0.0f;
        Vector2 scale = new Vector2(-0.5f, 0.5f);

        public SinBullet (Vector2 position_, Vector2 velocity_, EntityGroup targets_,
                          Target target_, int upgrade_) : base (position_, velocity_, targets_, target_)
        {
            position = position_;
            velocity = velocity_;
            upgrade = upgrade_;
            int s = SinGun.size + upgrade * SinGun.size_upgrade;
            size = new Vector2 (s, s);
        }

        public override void tick()
        {
            base.tick ();
            position.y -= offset;
            position += velocity;
            float last_offset = offset;

            offset = (float)Math.Sin(position.x / SinGun.wavelength + phase) *
            (SinGun.vertical_range + upgrade * SinGun.vertical_range_upgrade) *
            offset_cap;
            angle = Util.angle(new Vector2 (velocity.x, offset) - new Vector2(0, last_offset));
            position.y += offset;
            if (position.x > 1000) remove = true;

            offset_cap = 0.05f + 0.95f * offset_cap;
        }

        public override void hit (Target t)
        {
            if (t != null) {
                t.damage(SinGun.damage + SinGun.damage_upgrade * upgrade, Gun.Ammo.SIN);
                //remove = true;
            }
        }

        public override void draw ()
        {
            AppMain.textures.gun_sin.draw((int)gun_sin.Sprites.bullet,
                                          position, scale * size.x / 20, angle, Vector4.one);

            //AppMain.textures.laserbullet.draw(0, position, new Vector2(-1, 1) * size.x / 20, angle, Vector4.one);
        }
    }

    public class SkullBullet : Bullet
    {
        public Target attached_to = null;
        public int explosion_timeout = 0;
        public int upgrade = 0;
        public float frame = 0;
        public bool special = false;

        public SkullBullet (bool special_, Vector2 position, Vector2 velocity_, int upgrade_, EntityGroup targets_, Target target_ = null)
                            : base (position, velocity_, targets_, target_)
        {
            special = special_;
            upgrade = upgrade_;
            ground = 450;

            velocity = special ? new Vector2(velocity_.x, velocity_.y).normalized *
                                 (Util.rng.NextFloat (SkullGun.special_min_velocity, SkullGun.special_max_velocity)) :
                                 velocity_.normalized * SkullGun.velocity;

            float s = special ? SkullGun.special_skull_size + SkullGun.special_skull_size_upgrade * upgrade :
                                SkullGun.size + SkullGun.size_upgrade * upgrade;

            size = new Vector2(s, s);
        }

        public override void Dispose()
        {
            base.Dispose();

            attached_to = null;
        }

        public override void hit(Target t)
        {
            if (t == null) {
                explode();
                return;
            }

            if (t.hp <= 0) { return; }

            explosion_timeout = (int)(SkullGun.explosion_delay * 60);

            t.skulls.Add(this);
            attached_to = t;
            position = Util.fromPolar(Util.rng.NextFloat(0, (float)Math.PI * 2),
                                      Util.rng.NextFloat(0, Math.Min(t.size.x / 2, t.size.y / 2)));
            remove = true;
        }

        public void explode(bool hidden = false)
        {
            damage = special ? SkullGun.special_damage + upgrade * SkullGun.special_damage_upgrade :
                               SkullGun.damage + upgrade * SkullGun.damage_upgrade;

            AppMain.screenShake(upgrade * 5, 20);

            Bullet.explosion(position + (attached_to != null ? attached_to.position : Vector2.zero),
                             Gun.Ammo.SKULL, attached_to != null ? new Vector2 (attached_to.velocity.x, 0) : Vector2.zero,
                             SkullGun.explosion_size + upgrade * SkullGun.explosion_size_upgrade,
                             damage, targets, hidden);
        }

        public override void tick()
        {
            if (attached_to == null) {
                base.tick();
                return;
            }

            frame += 0.1f;
            if (frame > 2) { frame -= 2; }

            angle += Util.rng.NextFloat(-0.05f, 0.05f);
            position += new Vector2(Util.rng.NextFloat(-1f, 1f), Util.rng.NextFloat(-0.1f, 0.1f));

            bool hidden = false;
            if (--explosion_timeout <= 0) {
                for (int i = 0; i < attached_to.skulls.Count; ++i) {
                    attached_to.skulls[i].explode(hidden || Util.rng.NextDouble() < 0.2f);
                    hidden = true;
                }
            }
        }

        public override void draw()
        {
            AppMain.textures.skullbullet.draw((int)frame + (attached_to != null ? 2 : 0),
                                              position, Vector2.one * size.x / 40, angle, Vector4.one);
        }
    }

    public class BigBoomerang : Entity
    {
        public int sprite;
        public int time;
        public int frame = 0;
        public int upgrade;

        public BigBoomerang(int upgrade_, int row)
        {
            upgrade = upgrade_;

            size = new Vector2(400 / 400 * BoomerangGun.special_size + BoomerangGun.special_size_upgrade * upgrade, 160 / 400 * BoomerangGun.special_size + BoomerangGun.special_size_upgrade * upgrade);
            int y = House.lane_a;
            if (row == 1) { y = House.lane_b; }
            if (row == 2) { y = House.lane_c; }
            position = new Vector2(-size.x, y);
            velocity = new Vector2(12.0f, 0);
            sprite = Util.rng.Next(4);
            angle = 0;
            time = 0;
        }

        public override void tick()
        {
            frame += 10;

            velocity += new Vector2 (-0.06f, 0);
            position += velocity;
            if (position.x + size.x < 0) { remove = true; }

            if (time++ % 45 == 0) {
                Choom.PlayEffect(SoundAssets.BoomerangSpecial);
            }

            Game.instance.enemy_group.findCollisions (this, (Entity target) => {
                (target as Target).damage ((int)(BoomerangGun.special_damage + BoomerangGun.special_damage_upgrade * upgrade), Gun.Ammo.BOOMERANG);
            });
        }

        public override void draw ()
        {
            AppMain.textures.boomerang.draw("special", frame, position,
                                            new Vector2 (1.0f / 400.0f * size.x, 1.0f / 200.0f * size.y), 0, Vector4.one);
        }
    }

    public class Boomerang : Entity
    {
        public int sprite;
        public int time;
        public int frame = 0;
        public int upgrade;
        public Vector2 gravity;
        public float spin;

        public Boomerang (Vector2 position_, int upgrade_, int row)
        {
            upgrade = upgrade_;
            position = position_;
            gravity = new Vector2 (-0.06f, 0);
            sprite = Util.rng.Next (4);
            velocity = new Vector2 (8, -0.5f);
            if (row == 0) { velocity.y = 0.5f; }
            if (row == 1 && Util.rng.Next (2) == 0) { velocity.y = 0.5f; }
            size = Vector2.one * (BoomerangGun.size + BoomerangGun.size_upgrade * upgrade);
            angle = Util.rng.NextFloat (0, (float)Math.PI * 2);
            spin = (Util.rng.Next (2) == 0) ? -0.2f : 0.2f;
        }

        public override void tick ()
        {
            frame += 10;
            position += velocity;
            velocity += gravity;
            angle += spin;
            if (position.x + size.x < 0) {
                remove = true;
            }

            if (time++ % 20 == 0) {
                Choom.PlayEffect(SoundAssets.BoomerangShot, .35f);
            }

            Game.instance.enemy_group.findCollisions (this, (Entity target) => {
                (target as Target).damage ((int)(BoomerangGun.damage + BoomerangGun.damage_upgrade * upgrade), Gun.Ammo.BOOMERANG);
                remove = true;
            });
        }

        public override void draw ()
        {
            AppMain.textures.boomerang.draw ("bullet", frame, position, size / 100, angle, Vector4.one);
        }
    }

    public class Boomeranger
    {
        public static void bullet (Vector2 position, int upgrade, int row)
        {
            DataStorage.ShotsFired++;
            //Util.trace(DataStorage.ShotsFired);
            Game.instance.bullet_group.add (new Boomerang (position, upgrade, row));
        }
    }

    public class Flamer : Entity
    {
        public int upgrade;
        public int timeout;
        public int flame_count;
        public int frame = 0;

        public Flamer (int upgrade_)
        {
            upgrade = upgrade_;
            flame_count = (int)(FlameGun.special_flame_count + upgrade * FlameGun.special_flame_count_upgrade);
            timeout = (int)((FlameGun.special_flame_lifetime + upgrade * FlameGun.special_flame_lifetime_upgrade) * 60);

            Choom.PlayEffect(SoundAssets.FlameShot, true);
        }

        public override void tick ()
        {
            if (timeout < 30) {
                frame -= 10;
            }
            else {
                frame += 10;
                if (frame > 305) frame = 305;
            }

            if (--timeout <= 0) {
                remove = true;
                Choom.StopEffect(SoundAssets.FlameShot);
            }

            if (timeout % 10 != 0 || frame < 300) return;

            int flame_position = Puzzle.grid_left + Puzzle.piece_size * 3 + 120;
            int flame_spacing = (920 - flame_position) / flame_count;
            for (int i = 0; i < flame_count; i++) {
                emitFlame (new Vector2 (flame_position + 10, 450),
                    new Vector2 (Util.rng.NextFloat (-0.1f, 0.1f), -1), upgrade, true);
                flame_position += flame_spacing;
            }
        }

        public override void draw ()
        {
            int flame_position = Puzzle.grid_left + Puzzle.piece_size * 3 + 120;
            int flame_spacing = (920 - flame_position) / flame_count;
            for (int i = 0; i < flame_count; i++) {
                AppMain.textures.flames.draw ("special-full-sequence", frame, new Vector2 (flame_position, 505), Vector2.one / 2, 0, Vector4.one);
                flame_position += flame_spacing;
            }
        }

        public static void emitFlame (Vector2 position, Vector2 direction, int upgrade, bool special = false)
        {
            DataStorage.ShotsFired++;
            //Util.trace(DataStorage.ShotsFired);

            Particle gb = new Particle (AppMain.textures.flames.atlas);
            gb.frame = (int)flame.Sprites.gun_flame_bullet1 + Util.rng.Next (5);
            gb.loop = true;
            gb.loop_start = (int)flame.Sprites.gun_flame_bullet1;
            gb.loop_end = (int)flame.Sprites.gun_flame_bullet5;
            gb.frame_speed = 0.2f;
            gb.position = position;
            gb.timeout = (int)((FlameGun.lifetime + FlameGun.lifetime_upgrade * upgrade) * 60) + (special ? 30 : 0);
            gb.velocity = direction * FlameGun.velocity;
            gb.collides_with = Game.instance.enemy_group.entities;
            gb.scale = Vector2.one / 150 * (FlameGun.size + FlameGun.size_upgrade * upgrade);
            gb.grow = Vector2.one / 150;
            gb.collide_behavior = (ref Particle p, Entity e) => {
                if (e != null)
                    (e as Target).damage (FlameGun.damage + upgrade * FlameGun.damage_upgrade,
                        Gun.Ammo.FLAME);
            };

            AppMain.screenShake(upgrade * 5, 10);

            Game.instance.bullet_manager.add (gb);
        }
    }

    public class Forker
    {
        static Vector2 scale = new Vector2(0.5f, 0.5f);

        public static void bullet(Vector2 position, int upgrade)
        {
            for (int a = -1; a <= 1; a += 2) {
                DataStorage.ShotsFired += 2;

                Particle gb = new Particle(AppMain.textures.gun_fork);
                gb.frame = Util.rng.Next((int)gun_fork.Sprites.splat_0);
                gb.position = position + new Vector2(0, a * 5);
                gb.angle = ForkGun.angle / 180 * (float)Math.PI * a;
                gb.velocity = Util.fromPolar(gb.angle, ForkGun.velocity);
                gb.collides_with = Game.instance.enemy_group.entities;
                gb.scale = scale / 40 * (ForkGun.size + ForkGun.size_upgrade * upgrade);
                gb.ground_at = 480;
                gb.collide_behavior = (ref Particle p, Entity e) => {
                    p.remove = true;
                    if (e != null) {
                        (e as Target).damage(ForkGun.damage + ForkGun.damage_upgrade * upgrade, Gun.Ammo.FORK);
                    }
                    Particle splat = new Particle(AppMain.textures.gun_fork);
                    splat.frame = (int)gun_fork.Sprites.splat_0;
                    //splat.loop_end = (int)gun_fork.Sprites.splat_1 + 1;
                    splat.frame_speed = 0.2f;
                    splat.position = p.position;
                    splat.velocity = e != null ? new Vector2(e.velocity.x, 0) : Vector2.zero;
                    splat.scale = p.scale;
                    Game.instance.particle_manager.add(splat);
                };
                Game.instance.bullet_manager.add(gb);
            }
        }
    }

    public class Spike : Entity
    {
        public int upgrade, timeout;
        public float frame = 0;

        public Spike(int pos_x, int upgrade_)
        {
            upgrade = upgrade_;
            int s = (int)(SinGun.special_spike_size + SinGun.special_spike_size_upgrade * upgrade);
            size = new Vector2 (s, s);
            angle = Util.rng.NextFloat(-0.4f, 0.4f);
            position = new Vector2(pos_x, 500);
            timeout = (int)(60 * (SinGun.special_spike_lifetime + SinGun.special_spike_lifetime_upgrade * upgrade)) +
                           Util.rng.Next (-20, 40);

            for (int i = 0; i < Game.instance.enemy_group.entities.Count; ++i) {
                Target t = (Target)Game.instance.enemy_group.entities[i];

                if (t.position.y < 500 - s) { continue; }

                t.damage(SinGun.special_damage + SinGun.special_damage_upgrade * upgrade, Gun.Ammo.SIN);
                if (t.hp > 0) {
                    t.spike(timeout);
                }
            }
        }

        public override void draw()
        {
            int y_size = (int)(AppMain.textures.laserspecial.sprites [(int)frame].size.y * size.y / 500);
            Vector2 rot = new Vector2((float)Math.Cos(angle),
                                      (float)Math.Sin(angle - (Math.PI / 2))) * ((y_size - 30) / 2);
            AppMain.textures.laserspecial.draw((int)frame, position + rot, size / 500.0f, angle, Vector4.one);
        }

        public override void tick()
        {
            if (frame < 4.0) { frame += 0.15f; }

            if (--timeout <= 0) {
                remove = true;

                for (int i = 0; i < Game.instance.enemy_group.entities.Count; ++i) {
                    Target t = (Target)Game.instance.enemy_group.entities[i];

                    if (t.position.y < 500 - size.x) { continue; }
                    t.damage(SinGun.special_damage + SinGun.special_damage_upgrade * upgrade, Gun.Ammo.SIN);
                    if (t.hp > 0) {
                        t.spike(timeout);
                    }
                }

                Choom.PlayEffect(SoundAssets.SpikeBreak);
                AppMain.screenShake(upgrade * 5, 30);

                for (int i = 0; i < 5; i++) {
                    Particle p = new Particle (AppMain.textures.laserspecial);
                    p.frame = Util.rng.Next (5, 11);
                    Vector2 rot = new Vector2((float)Math.Cos (angle),
                                              (float)Math.Sin (angle - (Math.PI / 2))) * (size.y * i / 5);
                    p.position = position + rot;
                    p.velocity = new Vector2(Util.rng.NextFloat(-2, 2), Util.rng.NextFloat(0, 2));
                    p.scale = Vector2.one;
                    p.gravity = new Vector2(0, 0.25f);
                    p.angle = 0;
                    p.spin = Util.rng.NextFloat(-0.05f, 0.05f);
                    Game.instance.particle_manager.add(p);
                }
            }
        }
    }

    public class Lightning : Entity
    {
        public Entity dest, src;
        public int frame = 0;
        public int upgrade = 0;
        public List<Entity> victims;

        public Lightning(Entity dest_, Entity src_, int upgrade_, List<Entity> victims_)
        {
            dest = dest_;
            src = src_;
            upgrade = upgrade_;
            victims = victims_;
        }

        public override void Dispose()
        {
            base.Dispose();
            dest = null;
            src = null;
            victims = null;
        }

        public override void tick()
        {
            base.tick();

            frame++;
            if (frame == 15 && dest != null) lightningFrom(dest, upgrade, victims);
            if (frame == 50) remove = true;
        }

        public override void draw()
        {
            var src_pos = src.position;
            if (src is Gun) src_pos += new Vector2(130, 10);

            float scale = (LightningGun.range + upgrade * LightningGun.range_upgrade) / 2;
            float angle = 0;

            if (dest != null) {
                angle = Util.angle (dest.position - src_pos);
                scale = (src_pos - dest.position).magnitude;
            }

            AppMain.textures.lightning_strike.draw("bullet", frame * 400 / 50, src_pos,
                                                   Vector2.one * scale / 600, angle + (float)Math.PI, Vector4.one);
        }

        public static void lightningFrom(Entity src, int upgrade, List<Entity> victims = null)
        {
            DataStorage.ShotsFired++;
            //Util.trace(DataStorage.ShotsFired);

            if (victims == null) { victims = new List<Entity>(); }

            Target closest = null;

            var src_pos = src.position;
            if (src is Gun) { src_pos += new Vector2 (130, 10); }

            bool shoot_at_nothing = src is Gun;

            int range = LightningGun.range + upgrade * LightningGun.range_upgrade;

            for (int i = 0; i < Game.instance.enemy_group.entities.Count; ++i) {
                bool used = false;
                if (victims.IndexOf(Game.instance.enemy_group.entities[i]) != -1) { used = true; }

                float distance = (Game.instance.enemy_group.entities[i].position - src_pos).sqrMagnitude;
                if (!used && distance < range * range &&
                    (closest == null || distance < (closest.position - src_pos).sqrMagnitude)) {
                    closest = (Target)Game.instance.enemy_group.entities[i];
                }
            }

            if (closest != null) {
                victims.Add(closest);
                Game.instance.bullet_group.add(new Lightning(closest, src, upgrade, victims));
                closest.damage((int)(LightningGun.damage + upgrade * LightningGun.damage_upgrade), Gun.Ammo.LIGHTNING);
                shoot_at_nothing = false;
            }

            if (shoot_at_nothing) {
                Game.instance.bullet_group.add(new Lightning (null, src, upgrade, null));
            }
        }
    }

    public partial class DragonGun : Entity
    {
        public int rate, timeout, upgrade;
        public Vector2 vel;
        public bool derpy = false;
        Vector2 scale = new Vector2(-1, 1);

        public DragonGun(int upgrade_)
        {
            upgrade = upgrade_;
            position = new Vector2(0, -200);
            float cross_time = (special_time + special_time_upgrade * upgrade) * 60;
            vel = new Vector2 (960.0f / cross_time, 0);
            rate = (int)(60.0f / (special_fire_rate + special_fire_rate_upgrade * upgrade));
            timeout = 0;

            derpy = Util.rng.NextDouble () < special_derpy_chance;
        }

        public static void fireBullet(Vector2 position, Vector2 vector, int upgrade, List<Entity> target_list,
                                      bool special = false, bool derpy = false)
        {
            DataStorage.ShotsFired++;
            Choom.PlayEffect(SoundAssets.DragonShot);

            Particle db = new Particle(AppMain.textures.gun_dragon);
            db.position = position + vector.normalized * 100 * 1.5f;

            if (special) {
                var a = Util.fromPolar(special_fire_angle, 1);
                if (derpy) db.position = position + a * 90 + new Vector2(a.y, -a.x) * -30;
                else db.position = position + a * 120 + new Vector2(a.y, -a.x) * -50;
            }

            db.gravity = new Vector2(0, DragonGun.gravity);

            if (special) {
                db.velocity = vector;
            }
            else {
                db.velocity = vector * DragonGun.bullet_velocity;
            }

            db.collides_with = target_list;
            db.spin = 0.2f;
            db.angle = Util.rng.NextFloat(0, (float)Math.PI * 2);

            var size = DragonGun.bullet_size + DragonGun.bullet_size_upgrade * upgrade;
            db.scale = new Vector2(size / 12.0f, size / 12.0f);
            db.ground_at = 440;
            db.origin = new Vector2 (1.0f / 3, 2.0f / 5);

            db.collide_behavior = (ref Particle p, Entity e) => {
                p.remove = true;
                Choom.PlayEffect(SoundAssets.Explosion[Util.rng.Next(SoundAssets.Explosion.Length)]);

                AppMain.screenShake(upgrade * 5, 10);

                Bullet.explosion(p.position, Gun.Ammo.DRAGON,
                    e != null ? new Vector2(e.velocity.x, 0) : Vector2.zero,
                    DragonGun.explosion_size + upgrade * DragonGun.explosion_size_upgrade,
                    DragonGun.damage + upgrade * DragonGun.damage_upgrade,
                    Game.instance.enemy_group);
            };

            Game.instance.bullet_manager.add(db);
        }

        public override void draw()
        {
            if (derpy) {
                AppMain.textures.gun_dragon.draw((int)gun_dragon.Sprites.special_1,
                        position + new Vector2(-80, -70), scale, special_fire_angle, Vector4.one);
            }
            else {
                AppMain.textures.gun_dragon.draw((int)gun_dragon.Sprites.special_0,
                        position + new Vector2(-10, -10), scale, special_fire_angle, Vector4.one);
            }
        }

        public override void tick()
        {
            vel.y = (position.y * 0.95f - position.y);
            position += vel;
            if (position.x > 1100) remove = true;

            if (--timeout <= 0 && position.x > 100 && position.x < 1040) {
                timeout = rate;
                fireBullet(position, new Vector2 (vel.x * 1.2f, vel.y * 0.8f), upgrade,
                           Game.instance.enemy_group.entities, true, derpy);
            }
        }
    }

    public class Penguin : Entity
    {
        public float dest_y;
        public const float special_approach_coefficient = 0.97f;
        public int upgrade;
        public float frame = 0;

        public Penguin(int upgrade_)
        {
            upgrade = upgrade_;
            position = new Vector2(-100, Util.rng.NextFloat(-500, -50));
            dest_y = position.y + 550;

            int s = (int)(IglooGun.special_penguin_size + IglooGun.special_penguin_size_upgrade * upgrade);
            size = new Vector2 (s, s) / 100;
        }

        public override void draw()
        {
            AppMain.textures.penguinbullet.draw((int)frame, position, size, Util.angle(velocity), Vector4.one);
        }

        public override void tick()
        {
            frame += 0.1f;
            if (frame > 2) {
                frame -= 2;
            }

            var old_pos = position;
            position.x += IglooGun.special_penguin_speed;
            position.y = position.y * special_approach_coefficient + dest_y * (1 - special_approach_coefficient);
            velocity = position - old_pos;

            for (int i = 0; i < Game.instance.enemy_group.entities.Count; ++i) {
                if ((Game.instance.enemy_group.entities[i].position - position).sqrMagnitude <
                    size.magnitude + Game.instance.enemy_group.entities[i].size.magnitude) {
                    Target t = (Target)Game.instance.enemy_group.entities[i];

                    if (t.frozen <= 0) {
                        t.damage(IglooGun.special_damage + IglooGun.special_damage_upgrade * upgrade, Gun.Ammo.IGLOO);
                    }

                    t.freeze(IglooGun.special_penguin_freeze_time + IglooGun.special_penguin_freeze_time_upgrade * upgrade);

                    Choom.PlayEffect(SoundAssets.IglooFreeze);
                }
            }
        }
    }

    public class Penguiner : Entity
    {
        public int penguins_left, penguin_timeout, penguin_rate, upgrade;

        public Penguiner(int upgrade_)
        {
            Choom.PlayEffect(SoundAssets.PenguinSkullSpecial);
            AppMain.screenShake(upgrade * 5, 10);
            upgrade = upgrade_;
            penguins_left = IglooGun.special_penguin_count + IglooGun.special_penguin_count_upgrade * upgrade;
            penguin_rate = (int)(60 / (IglooGun.special_penguin_rate + IglooGun.special_penguin_rate_upgrade * upgrade));
            penguin_timeout = penguin_rate;
        }

        public static void penguin(Vector2 position, Vector2 velocity, bool special, int upgrade, List<Entity> target_list)
        {
            DataStorage.ShotsFired++;
            //Util.trace(DataStorage.ShotsFired);

            Particle pb = new Particle(AppMain.textures.penguinbullet);
            pb.frame_speed = 0.15f;
            pb.loop_start = 0;
            pb.loop_end = 2;
            pb.loop = true;
            pb.drawable_size /= 2;
            pb.position = position;
            pb.velocity = velocity;
            pb.angle = Util.angle(velocity);
            pb.collides_with = target_list;
            pb.ground_at = 450;

            float s = special ? IglooGun.special_penguin_size + IglooGun.special_penguin_size_upgrade * upgrade :
                                IglooGun.size + IglooGun.size_upgrade * upgrade;

            pb.scale = new Vector2 (s, s) / 100.0f;
            pb.collide_behavior = (ref Particle p, Entity e) => {
                p.remove = true;

                float freeze_time = special ? IglooGun.special_penguin_freeze_time +
                                              IglooGun.special_penguin_freeze_time_upgrade * upgrade :
                                              IglooGun.freeze_time + IglooGun.freeze_time_upgrade * upgrade;
                int damage = special ? IglooGun.special_damage + IglooGun.special_damage_upgrade * upgrade :
                                       IglooGun.damage + IglooGun.damage_upgrade * upgrade;

                AppMain.screenShake(upgrade * 5, 5);

                Choom.PlayEffect(SoundAssets.Explosion[Util.rng.Next(SoundAssets.Explosion.Length)]);

                if (e != null) {
                    (e as Target).freeze(freeze_time);
                    (e as Target).damage(damage, Gun.Ammo.IGLOO);
                }

                Particle eb = new Particle(AppMain.textures.penguinbullet);
                eb.frame = 2;
                eb.frame_speed = 0.15f;
                eb.velocity = e != null ? new Vector2(e.velocity.x, 0) : Vector2.zero;
                eb.position = p.position;

                eb.angle = p.angle - (float)Math.PI / 2;

                float es = special ? IglooGun.special_explosion_size + IglooGun.special_explosion_size_upgrade * upgrade :
                                     IglooGun.explosion_size + IglooGun.explosion_size_upgrade * upgrade;

                eb.scale = new Vector2(es, es) / 400.0f;
                eb.origin = new Vector2(1.0f / 2, 5.0f / 6);

                Game.instance.bullet_manager.add(eb);
            };

            Game.instance.bullet_manager.add(pb);
        }

        public override void tick()
        {
            if (--penguin_timeout > 0) { return; }

            if (--penguins_left <= 0) {
                remove = true;
            }

            penguin(new Vector2(Util.rng.Next(50, 900), -100),
                    new Vector2(1, 2).normalized * IglooGun.special_penguin_speed,
                    true, upgrade, Game.instance.enemy_group.entities);

            penguin_timeout = penguin_rate;
        }
    }

    public class Skuller : Entity
    {
        public int timeout = 5 * 60;

        public Skuller(int size)
        {
            Choom.PlayEffect(SoundAssets.PenguinSkullSpecial);

            for (int i = 0; i < SkullGun.special_skull_count + SkullGun.special_skull_count_upgrade * size; i++) {
                Game.instance.bullet_group.add(new SkullBullet(true,
                        new Vector2(Util.rng.NextFloat(150, 900), Util.rng.NextFloat(-SkullGun.special_max_altitude, 0)),
                        new Vector2 (2, 6), size, Game.instance.enemy_group));
            }
        }

        public static void addBullet(Vector2 position, Vector2 velocity, int size, EntityGroup target_list, Target target)
        {
            Game.instance.bullet_group.add(new SkullBullet(false, position, velocity, size, target_list, target));
        }

        public override void tick()
        {
            if (--timeout <= 0) {
                remove = true;
            }
        }
    }

    public class Lightninger : Entity
    {
        public int ticks_per_strike, motion_per_strike;
        public int timeout = 0, frame = -1;
        public int upgrade;
        public float scale = 470;

        public Lightninger(int upgrade_)
        {
            upgrade = upgrade_;
            position = new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3, (-20 + 450) / 2);
            size = new Vector2(scale / 4, scale);
            center_based = true;
            int strikes = (int)(LightningGun.special_strikes + upgrade * LightningGun.special_strike_upgrade);
            ticks_per_strike = 5 * 30 / strikes;
            motion_per_strike = (int)((960 - position.x) / strikes);
        }

        public override void draw()
        {
            if (frame < 0) return;

            AppMain.textures.lightning_strike.draw("bullet", frame * 400 / 50,
                                                   position + new Vector2(0, -470 / 2),
                                                   Vector2.one * 470 / 600, -(float)Math.PI / 2, Vector4.one);
        }

        public override void tick()
        {
            if (frame >= 0) {
                frame++;
                if (frame == 10) {
                    Game.instance.enemy_group.findCollisions(this, (Entity target) => {
                        Lightning.lightningFrom(target, upgrade);
                        (target as Target).damage((int)(LightningGun.damage + upgrade * LightningGun.damage_upgrade), Gun.Ammo.LIGHTNING);
                    });

                    Choom.PlayEffect(SoundAssets.LightningSpecial);
                    AppMain.screenShake(upgrade * 5, 20);
                }

                if (frame >= 50) frame = -1;
            }

            if (++timeout >= ticks_per_strike) {
                position.x += motion_per_strike;
                if (position.x > 960) {
                    remove = true;
                    return;
                }
                timeout = 0;
                frame = 0;
                Choom.PlayEffect(SoundAssets.LightningSpecial);
            }
        }
    }

    public class BigVegetable : Entity
    {
        public Vector2 origin;
        public int upgrade;
        public float spin = 0, acceleration = 0;
        public float frame = 6.99f;

        public BigVegetable(int upgrade_)
        {
            position = new Vector2(360, 490);
            upgrade = upgrade_;
            float s = VegetableGun.special_size + VegetableGun.special_size_upgrade * upgrade;
            size = new Vector2 (s, s);
            angle = 0;
            Choom.PlayEffect(SoundAssets.Vegsprout);
        }

        public override void tick()
        {
            if (frame >= 7) {
                frame += 0.15f;
                position = new Vector2(360, 490) + new Vector2(35 * size.y, 0);
                if (frame >= 11) { remove = true; }
            }
            else {
                frame -= 0.15f;
                position = new Vector2(360, 490) - new Vector2(0, 35 * size.y);
                origin = new Vector2(0.5f, 1.0f - 15 * size.y / AppMain.textures.veggie_special.sprites [(int)frame].size.y);
            }

            if (frame < 0) {
                frame = 0;
                acceleration += 0.00005f;
                spin += acceleration;
                angle += spin;

                if (angle > Math.PI / 2) {
                    int damage = VegetableGun.special_damage + VegetableGun.special_damage_upgrade * upgrade;
                    damage = (int)(damage * Gun.UpgradeMultiplier(Gun.Ammo.VEGETABLE));

                    AppMain.screenShake(upgrade * 5, 40);

                    for (int i = 0; i < Game.instance.enemy_group.entities.Count; ++i) {
                        ((Target)Game.instance.enemy_group.entities[i]).damage(damage, Gun.Ammo.VEGETABLE);
                    }

                    frame = 7.0f;
                    origin = new Vector2 (1 - origin.y, origin.x);
                    angle = 0;
                }
            }
        }

        public override void draw()
        {
            AppMain.textures.veggie_special.draw((int)frame, position, origin, size, angle, Vector4.one);
        }
    }

    public class BigBounce : Entity
    {
        public float radius;
        public int upgrade;
        public int sprite;
        Vector2 scaleInvertX = new Vector2(-0.5f, 0.5f);

        public BigBounce(int size)
        {
            Choom.PlayEffect(SoundAssets.BeachBallRoll, true);
            upgrade = size;
            radius = BeachBallGun.special_radius + size * BeachBallGun.special_radius_upgrade;
            position = new Vector2 (-radius, 500 - radius);
            sprite = Util.rng.Next(3);
        }

        public override void tick()
        {
            position.x += BeachBallGun.special_speed;
            var rotate_by = (float)(BeachBallGun.special_speed / (2 * Math.PI * radius) * 2 * Math.PI);
            angle += rotate_by;
            AppMain.screenShake(upgrade * 2, 20);

            if (position.x - radius > 960) {
                Choom.StopEffect(SoundAssets.BeachBallRoll);
                remove = true;
            }

            for (int i = 0; i < Game.instance.enemy_group.entities.Count; ++i) {
                if ((Game.instance.enemy_group.entities[i].position - position).sqrMagnitude < radius * radius) {
                    AppMain.screenShake(upgrade * 7, 5);
                    ((Target)Game.instance.enemy_group.entities[i]).damage((int)(BeachBallGun.special_damage +
                                                    BeachBallGun.special_damage_upgrade * upgrade), Gun.Ammo.BOUNCE);
                }
            }
        }

        public override void draw()
        {
            AppMain.textures.gun_beachball.draw((int)gun_beachball.Sprites.special_blue + sprite,
                                                position, scaleInvertX / 233 * radius, angle, Vector4.one);
            AppMain.textures.gun_beachball.draw((int)gun_beachball.Sprites.special_shine,
                position + new Vector2(-7, -7) / 233 * radius, scaleInvertX / 233 * radius, 0, Vector4.one);

            //AppMain.textures.beachball.draw((int)beachball.Sprites.gun_beachball_special_1 + sprite,
            //    position, new Vector2 (-1, 1) / 233 * radius, angle, Vector4.one);
            //AppMain.textures.beachball.draw((int)beachball.Sprites.gun_beachball_special_highlite,
                //position + new Vector2 (-7, -7) / 233 * radius, new Vector2 (-1, 1) / 233 * radius, 0, Vector4.one);
        }
    }

    public class Shield : Entity
    {
        public int time = 0;
        public int timeout = 0;
        public int upgrade = 0;
        public static Shield existing_shield = null;

        public Shield(int size)
        {
            timeout = (int)(ForkGun.shield_duration * 60);
            addTime (size);
            existing_shield = this;

            Choom.PlayEffect(SoundAssets.ForkSpecial);
        }

        public void addTime(int size)
        {
            timeout += (int)(size * ForkGun.shield_duration_upgrade * 60);
            upgrade += size;
            if (upgrade > 12) upgrade = 12;
        }

        public override void tick()
        {
            time++;
            if (--timeout <= 0) {
                remove = true;
                existing_shield = null;
            }

            int right = ForkGun.shield_right;
            int left = ForkGun.shield_left;

            for (int i = 0; i < Game.instance.enemy_group.entities.Count; ++i) {
                if (Game.instance.enemy_group.entities[i].position.x < right &&
                    Game.instance.enemy_group.entities[i].position.x > left) {
                    if (Game.instance.enemy_group.entities[i].position.x < (left + right) / 2) {
                        Game.instance.enemy_group.entities[i].position.x = left;
                    }
                    else {
                        Game.instance.enemy_group.entities[i].position.x = right;
                    }
                }
            }

            for (int i = 0; i < Game.instance.enemy_bullet_group.entities.Count; ++i) {
                if (Game.instance.enemy_bullet_group.entities[i].position.x < right &&
                    Game.instance.enemy_bullet_group.entities[i].position.x > left) {
                    if (Game.instance.enemy_bullet_group.entities[i] is EnemyBullet) {
                        ((EnemyBullet)Game.instance.enemy_bullet_group.entities[i]).kill();
                    }
                    else {
                        Game.instance.enemy_bullet_group.entities[i].remove = true;
                        Bullet.explosion(Game.instance.enemy_bullet_group.entities[i].position,
                                         Gun.Ammo.NONE, Vector2.zero, 15, 0, null);
                    }
                }
            }
        }

        public override void draw()
        {
            int frame = 299;
            if (time < 30) frame = time * 10;
            if (timeout < 30) frame = timeout * 10;
            AppMain.textures.forkspecial.draw("fork-special-spawn", frame,
                new Vector2((ForkGun.shield_left + ForkGun.shield_right) / 2, 544 / 2 - 30),
                new Vector2(-1, 1), 0, Vector4.one);
            
            if (frame >= 250) {
                AppMain.textures.forkspecial.atlas.draw (6,
                    new Vector2((ForkGun.shield_left + ForkGun.shield_right) / 2 + 15, 544 / 2 - 90),
                    new Vector2(-1, 1) * (0.6f + 0.4f / 12 * upgrade), 0, Vector4.one);
            }
        }
    }

    // GHint replaces int because Windows Phone 8 does not like arrays with generic types and system types...
    public struct GHint
    {
        public int v;

        public GHint (int val)
        {
            this.v = val;
        }

        public GHint (float val)
        {
            this.v = (int)val;
        }

        public static implicit operator GHint (int val)
        {
            return new GHint (val);
        }

        public static implicit operator GHint (float val)
        {
            return new GHint (val);
        }

        public static implicit operator int (GHint val)
        {
            return val.v;
        }
    }

    public partial class House : Entity
    {
        public float health;
        public float max_health;
        public int score;
        public float door_position = 1.0f;
        public bool isDoorClosed = true;
        public Tuple<Gun.Ammo, GHint>[] special_attacks = new Tuple<Gun.Ammo, GHint>[3];
        public static List<Gun.Ammo> ammo_available = new List<Gun.Ammo>();

        int time;
        int door_pixels_pulled;
        float special_timeout;
        int orphan_cry_timeout;
        int last_countdown_ping;
        float door_velocity;
        public float door_target_position = 1.0f;
        bool ready_to_open;
        float next_transition = 1;
        bool complained_about_health;
        int puzzle_release_timeout = 60 * 3;

        public bool tutorial_timer_once;

        public House()
        {
            max_health = health_per_heart * MetaState.hearts;
            health = max_health;
            Shield.existing_shield = null;

            // build list of equipped ammo
            Gun.Ammo[] equipped = new Gun.Ammo[3];
            if (!MetaState.hardcore_mode) {
                int equip_index = 0;
                for (int i = 0; i < DataStorage.NumberOfGuns; ++i) {
                    if (!DataStorage.GunEquipped[i]) { continue; }
                    equipped[equip_index++] = (Gun.Ammo)i;
                }
            }
            else {
                equipped[0] = Gun.Ammo.DRAGON;
                equipped[1] = Gun.Ammo.IGLOO;
                equipped[2] = Gun.Ammo.SKULL;
            }

            // choose among equpped ammo
            ammo_available.Clear();

            for (int i = 0; i < Game.ammoTypesPerWave(MetaState.wave_number); ++i) {
                Gun.Ammo a;
                do {
                    a = equipped[Util.rng.Next(equipped.Length)];
                } while(ammo_available.Contains(a));

                ammo_available.Add(a);
            }

            ammo_available.Sort();

            for (int i = 0; i < ammo_available.Count; ++i) {
                switch (ammo_available[i])
                {
                case Gun.Ammo.BOOMERANG: AppMain.textures.boomerang.touch(); break;
                case Gun.Ammo.BOUNCE: AppMain.textures.gun_beachball.touch(); break;
                case Gun.Ammo.DRAGON: AppMain.textures.gun_dragon.touch(); break;
                case Gun.Ammo.FLAME: AppMain.textures.flames.touch(); break;
                case Gun.Ammo.FORK:
                    AppMain.textures.gun_fork.touch();
                    AppMain.textures.forkspecial.touch();
                    break;
                case Gun.Ammo.IGLOO:
                    AppMain.textures.gun_penguin.touch();

                    AppMain.textures.penguinbullet.touch();
                    break;
                case Gun.Ammo.LIGHTNING:
                    AppMain.textures.lightning_strike.touch();
                    break;
                case Gun.Ammo.SIN:
                    AppMain.textures.gun_sin.touch();

                    AppMain.textures.laserspecial.touch();
                    break;
                case Gun.Ammo.SKULL:
                    AppMain.textures.gun_skull.touch();

                    AppMain.textures.skullbullet.touch();
                    break;
                case Gun.Ammo.VEGETABLE:
                    AppMain.textures.gun_vegetable.touch();

                    AppMain.textures.veggies.touch();
                    AppMain.textures.veggie_special.touch();
                    break;
                }

                AppMain.textures.house_guns.touch();
                AppMain.textures.ui_game.touch();
                AppMain.textures.gunpoof.touch();
                AppMain.textures.skeletonkingprojectile.touch();
                AppMain.textures.lightning_match.touch();
            }

            for (int i = 0; i < 3; ++i) {
                special_attacks[i] = new Tuple<Gun.Ammo, GHint>(Gun.Ammo.NONE, 0);
            }
        }

        public override void tick()
        {
            if (door_position < 1.0f) {
                puzzle_release_timeout = (int)(Puzzle.attack_round_length * 60);
            }

            if (puzzle_release_timeout > 0 && AppMain.tutorial.FreezeDoor) {
                puzzle_release_timeout--;
            }

            if (door_target_position >= door_position) {
                float time_left = (1.0f - door_position) / door_velocity / 60.0f;

                if (last_countdown_ping == 0) {
                    last_countdown_ping = 5;
                }

                if (Math.Ceiling(time_left) == last_countdown_ping) {
                    last_countdown_ping--;
                    Choom.PlayEffect(SoundAssets.CountDown);
                }
            }

            // only open the door after all bullets are gone
            ready_to_open = (puzzle_release_timeout <= 0) && Game.instance.win_timer <= 0 &&
                            (Game.instance.bullet_manager.particle_count + Game.instance.bullet_group.entities.Count) == 1 &&
                            AppMain.top_state == Game.instance;

            for (int i = 0; i < Game.instance.gun_group.entities.Count; ++i) {
                if (((Gun)Game.instance.gun_group.entities[i]).firing &&
                    ((Gun)Game.instance.gun_group.entities[i]).ammo_ct > 0) {
                    ready_to_open = false;
                }
            }

            if (ready_to_open && AppMain.top_state == Game.instance) {
                openDoor();

                if (AppMain.tutorial.ActivatedSpecial) {
                    AppMain.tutorial.SetLesson(Lesson.DOOR_OPENS);
                }

                Objectives.DoorOpen();
            }

            if (next_transition < 1.0) {
                next_transition = (float)Math.Min(next_transition + 0.01, 1.0);
            }

            // close door
            if (!AppMain.tutorial.hasFocus && AppMain.tutorial.Countdown &&
                door_target_position < 1.0f) {
                door_target_position += door_velocity;
            }

            if (door_position != door_target_position) {
                if (Math.Abs(door_target_position - door_position) < 0.05f) {
                    door_position = door_target_position;
                }
                else {
                    door_position += Math.Sign(door_target_position - door_position) * 0.05f;
                }

                if (door_position > 1.0f) {
                    door_position = 1.0f;
                    door_target_position = 1.0f;
                    closeDoor();

                    Choom.PlayEffect(SoundAssets.DoorClose);
                }
            }

            if (AppMain.top_state != Game.instance && !AppMain.IsPaused) {
                door_position = 1.0f;
                door_target_position = 1.0f;
                closeDoor();
            }

            orphan_cry_timeout--;
            time++;
        }

        public override void draw()
        {
            Vector2 scale = new Vector2(0.5f, 0.5f);
            Vector2 scaleInvertX = new Vector2(-0.5f, 0.5f);

            #region Orphan Meter Health

            float shake = 0;
            float meter_val = health / max_health;
            if (meter_val > health_complain_threshold) {
                complained_about_health = false;
            }

            if (meter_val < health_complain_threshold) {
                if (!complained_about_health) {
                    Choom.PlayEffect(SoundAssets.HealthLow);
                    complained_about_health = true;
                    shake = (1 - meter_val) * 5;
                }
            }

            // smart resize if the screen is too small
            float part_size = (AppMain.vscreen.x - 72.0f - (Puzzle.grid_left + Puzzle.piece_size * 3.0f + 200.0f + 32.0f)) / 6.0f;
            var heart_pos = new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3 + 200 + 32, 42) +
                Util.fromPolar(Util.rng.NextFloat((float)Math.PI * 2), Util.rng.Next((int)shake));

            float shown = 0;
            for (int i = 0; i < MetaState.hearts; ++i) {
                float size = 0;
                if (shown + House.health_per_heart < health) {
                    size = 1;
                }
                else if (shown < health) {
                    size = (health - shown) / House.health_per_heart;
                }

                AppMain.textures.puzzle.draw((int)puzzle.Sprites.heart_outline,
                                             heart_pos, scale / 96 * 58, Vector4.one);
                AppMain.textures.puzzle.draw((int)puzzle.Sprites.heart,
                                             heart_pos + new Vector2(2, 2), scale * size / 96 * 58, Vector4.one);

                heart_pos.x += part_size;
                shown += House.health_per_heart;
            }

            #endregion

            #region Weaknesses

            AppMain.textures.puzzle.draw((int)puzzle.Sprites.match_bonus_background,
                                         new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3 + 105, 40),
                                         new Vector2(-185.0f / 368, 115.0f / 250), Vector4.one);

            AppMain.textures.puzzle.draw(Puzzle.AmmoToSprite(Game.instance.next_bonuses[1]),
                                         new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3 + 140, 42.5f),
                                         scaleInvertX * next_transition, Vector4.one);

            if (door_position != 1.0f) {
                scaleInvertX *= (float)(Math.Sin(time / 10.0f) / 6 + 1);
            }
            AppMain.textures.puzzle.draw(Puzzle.AmmoToSprite(Game.instance.next_bonuses[0]),
                                         new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3 + 140 - 70 * next_transition, 42.5f),
                                         scaleInvertX, Vector4.one);

            #endregion

            #region Door Timer

            AppMain.textures.puzzle.draw((int)puzzle.Sprites.house_counter,
                                         new Vector2(Puzzle.grid_left + 112, Puzzle.grid_top - 45),
                                         new Vector2(-0.5f, 0.5f) * 3 / 4, Vector4.one);

            if (!AppMain.tutorial.hasFocus && AppMain.tutorial.Countdown &&
                door_target_position != 1.0f) {
                string open_time = string.Format("{0:00.0}", (1.0f - door_position) / door_velocity / 60.0f);

                Vector2 time_pos = new Vector2(Puzzle.grid_left + 110, Puzzle.grid_top - 45) + new Vector2(31.8f, -1);

                for (int i = 0; i < open_time.Length; ++i) {
                    int n = open_time[i] - '0' - 1;
                    if (n == -1) { n = 9; }

                    if (n >= 0 && n <= 9) {
                        AppMain.textures.counter_time.draw(n, time_pos, new Vector2(-1, 1) * 3 / 4, Vector4.one);
                        time_pos.x -= 32 * 3 / 4;
                    }
                    else {
                        time_pos.x -= 10 * 3 / 4;
                    }
                }
            }

            #endregion

            #region House

            AppMain.textures.house.draw(0, new Vector2(Puzzle.grid_left + 95, Puzzle.grid_top + 146), scale, Vector4.one);

            /* ================================================== */
            /* DOOR */

            float amount_open = visibleDoorPosition();

            if (amount_open != 0) {
                AtlasSprite door = AppMain.textures.puzzle.sprites[DataStorage.DisconcertingObjectivesSeen >= 20 ?
                                                                   (int)puzzle.Sprites.house_door_alt :
                                                                   (int)puzzle.Sprites.house_door];

                float doorcrop = (1.0f - amount_open) * (door.bounds.Point00.y - door.bounds.Point11.y);

                AppMain.renderer.addSprite(AppMain.textures.puzzle.texture,
                                           new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3, Puzzle.grid_top),
                                           new Vector2(Puzzle.grid_left, Puzzle.grid_top),
                                           new Vector2(Puzzle.grid_left, Puzzle.grid_top + visibleDoorPosition() *
                                Puzzle.piece_size * 6),
                                           new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3,
                                Puzzle.grid_top + visibleDoorPosition() *
                                Puzzle.piece_size * 6),
                                           new Necrosofty.Math.Bounds2(
                                               new Vector2(door.bounds.Point00.x, door.bounds.Point00.y - doorcrop),
                                               new Vector2(door.bounds.Point11.x, door.bounds.Point11.y)),
                                           Vector4.one);
            }

            AppMain.textures.puzzle.draw((int)puzzle.Sprites.house_border,
                                         new Vector2(Puzzle.grid_left + Puzzle.piece_size * 1.8f, Puzzle.grid_top + Puzzle.piece_size * 3),
                                         new Vector2(-0.5f, 0.5f), Vector4.one);

            #endregion

            #region Specials

            for (int i = 0; i < special_attacks.Length; ++i) {
                Vector2 vibrate = Vector2.zero;
                var pos = new Vector2(Puzzle.grid_left - 40,
                                      Puzzle.grid_top + (i * 2 + 1) * Puzzle.piece_size);

                if (special_attacks[i].Item1 != Gun.Ammo.NONE) {
                    Vector2 pulse = new Vector2(-0.5f, 0.5f);
                    if (door_position == 1.0f) {
                        pulse *= ((float)Math.Sin (i + time / 20.0f) / 10.0f + 1.05f);
                    }

                    vibrate = Util.fromPolar(Util.rng.NextFloat((float)Math.PI * 2),
                                             Util.rng.NextFloat(0, special_attacks[i].Item2)) / 4;

                    AppMain.textures.puzzle.draw(Puzzle.AmmoToSprite(special_attacks[i].Item1),
                                                 pos + vibrate, pulse, Vector4.one);
                }

                AppMain.textures.puzzle.draw((int)puzzle.Sprites.house_tank,
                                             new Vector2(Puzzle.grid_left - 40,
                                                         Puzzle.grid_top + (i * 2 + 1) * Puzzle.piece_size + 8) + vibrate,
                                             new Vector2(-0.5f, 0.5f), Vector4.one);

                bool selected = Game.instance.house.isDoorClosed &&
                                    Game.instance.puzzle.selected_weapon == new Vector2(0, i);

                if (selected && Gun.fireableExists() && AppMain.game_pad_active) {
                    AppMain.textures.ui_game.draw((int)ui_game.Sprites.arrow,
                                                  pos + new Vector2(64 + Mathf.Abs(Mathf.Sin(AppMain.frame / 30f)) * 16, 0), new Vector2(-1, 1), 0, Vector4.one);
                }
            }

            #endregion
        }

        public float visibleDoorPosition()
        {
            return Math.Max(door_position, 0);
        }

        public void closeDoor()
        {
            isDoorClosed = true;

            AppMain.tutorial.SetLesson(Lesson.ACTIVATE_GUNS);

            for (int i = Game.instance.gun_group.entities.Count - 1; i >= 0; --i) {
                ((Gun)Game.instance.gun_group.entities[i]).Dispose();
            }

            AppMain.reset_scene = true;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void openDoor()
        {
            isDoorClosed = false;

            AppMain.reset_scene = true;

            Target.damages = new Dictionary<Gun.Ammo, float>();
            Choom.PlayEffect(SoundAssets.DoorOpen);

            if (MetaState.wave_number == 0 && !MetaState.hardcore_mode && !tutorial_timer_once) {
                AppMain.overrideDoorTimer = true;
                AppMain.overrideDoorOpenTime = 5;
                tutorial_timer_once = true;
            }
            else { AppMain.overrideDoorTimer = false; }

            door_target_position = AppMain.overrideDoorTimer ? -AppMain.overrideDoorOpenTime : -Game.instance.door_close_time;
            door_velocity = Difficulty.door_close_speed;
            Game.instance.match_combo = 1;

            Input.clearTouches(); // workaround to clean up hypothetical stray touches
        }

        public void advanceNext()
        {
            next_transition = 0;
            Game.instance.next_bonuses.RemoveAt(0);
            Game.instance.next_bonuses.Add (Game.instance.puzzle.nextPiece());
            Game.instance.current_bonus = Game.instance.next_bonuses[0];
        }

        public void addScore(int size, Vector2 pos)
        {
            int new_points = (int)((size / 10) * MetaState.logCurve(MetaState.wave_number,
                                                                    Difficulty.money_initial,
                                                                    Difficulty.money_steepness,
                                                                    Difficulty.money_amplification));

            float hp_add = size / 6 * MetaState.healing_coefficient;

            if (Util.rng.NextFloat() > health / max_health && size < 3000) {
                Game.instance.particle_group.add(new Pickup(pos, hp_add, 0));
            }
            else {
                Game.instance.particle_group.add(new Pickup(pos, 0, new_points));
            }
        }

        public static int gunPositionFromY(float y)
        {
            return (int)Util.clamp((int)(y - Puzzle.grid_top) / Puzzle.piece_size / 2, 0, 2);
        }

        public void addSpecialAttack(int position, Gun.Ammo type, int size)
        {
            var current_type = special_attacks[position].Item1;
            var current_size = special_attacks[position].Item2;

            if (current_type == type) { current_size += size - 1; }
            else { current_size = size - 1; }

            special_attacks[position] = new Tuple<Gun.Ammo, GHint>(type, Util.clamp(current_size, 0, Puzzle.max_upgrade_level));
        }

        public void orphanCry()
        {
            if (orphan_cry_timeout > 0) { return; }

            orphan_cry_timeout = (int)(60 / House.orphan_cry_rate);
        }

        public void damage(float amt)
        {
            health -= amt * MetaState.monster_attack_coefficient;
        }

        public void specialAttackAt(Vector2 xy)
        {
            if (door_position != 1.0f) { return; }

            int n = House.gunPositionFromY(xy.y);

            if (special_attacks[n].Item1 == Gun.Ammo.NONE) { return; }

            Gun.Ammo type = special_attacks[n].Item1;
            GHint size = special_attacks[n].Item2;
            special_attacks[n] = new Tuple<Gun.Ammo, GHint>(Gun.Ammo.NONE, 0);

            Entity special_attacker = null;

            if (type == Gun.Ammo.SKULL) { special_attacker = new Skuller(size); }
            if (type == Gun.Ammo.DRAGON) { special_attacker = new DragonGun(size); }
            if (type == Gun.Ammo.IGLOO) { special_attacker = new Penguiner(size); }
            if (type == Gun.Ammo.VEGETABLE) { special_attacker = new BigVegetable(size); }
            if (type == Gun.Ammo.LIGHTNING) { special_attacker = new Lightninger(size); }
            if (type == Gun.Ammo.BOOMERANG) { special_attacker = new BigBoomerang(size, n); }
            if (type == Gun.Ammo.FLAME) { special_attacker = new Flamer(size); }

            if (type == Gun.Ammo.FORK) {
                if (Shield.existing_shield == null) {
                    special_attacker = new Shield(size);
                }
                else  {
                    special_attacker = null;
                    Shield.existing_shield.addTime(size);
                }
            }

            if (type == Gun.Ammo.SIN) {
                int spike_count = (int)(SinGun.special_spike_count + size * SinGun.special_spike_count_upgrade);
                int spike_position = Puzzle.grid_left + 120;
                int spike_spacing = (920 - spike_position) / spike_count;

                for (int i = 0; i < spike_count; ++i) {
                    Game.instance.bullet_group.add(new Spike(spike_position, size));
                    spike_position += spike_spacing;
                }

                Choom.PlayEffect(SoundAssets.MathSpecial);
                special_attacker = null;
            }

            if (type == Gun.Ammo.BOUNCE) {
                special_attacker = new BigBounce(size);
                Choom.PlayEffect(SoundAssets.BeachBallShot);
            }

            if (special_attacker != null) {
                Game.instance.bullet_group.add(special_attacker);
            }

            Objectives.FireSpecial(type);
        }
    }
}
