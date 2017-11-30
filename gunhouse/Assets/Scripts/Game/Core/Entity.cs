using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gunhouse
{
    public abstract class Entity
    {
        public Vector2 position, velocity, size;
        public bool center_based = true;
        public float angle;
        public bool remove = false;
        public EntityGroup parent;

        public Entity()
        {
            position = Vector2.zero;
            velocity = Vector2.zero;
            size = new Vector2(15, 15);
        }

        ~Entity()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            parent = null;
        }

        public virtual void draw() { }
        public virtual void tick() { }
    }

    public class Wall : Entity
    {
        public Wall (Vector2 position_, Vector2 size_)
        {
            position = position_;
            size = size_;
            center_based = false;
        }
    }

    public class EntityGroup
    {
        public List<Entity> entities = new List<Entity>();
        public List<Entity> to_add = new List<Entity>();
        public Vector2 origin = Vector2.zero;
        public EntityGroupID id;

        public bool autodraw = true;
        public int tickrate = 1;

        public EntityGroup(EntityGroupID groupID)
        {
            id = groupID;
        }

        ~EntityGroup()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (entities != null)
            {
                for (int i = entities.Count - 1; i >= 0; --i) { entities[i].Dispose(); }

                entities.Clear();
            }

            if (to_add != null)
            {
                for (int i = to_add.Count - 1; i >= 0; --i) { to_add[i].Dispose(); }
                to_add.Clear();
            }

            entities = null;
            to_add = null;
        }

        public void add(Entity e)
        {
            to_add.Add(e);
            e.parent = this;
        }

        public Target findClosest(Vector2 to, Gun.Ammo type = Gun.Ammo.NONE, int min_distance = 100)
        {
            Target closest = null;
            float distance = 0;

            for (int i = 0; i < entities.Count; ++i)
            {
                Target t = (Target)entities[i];

                if (type != Gun.Ammo.NONE) { continue; }
                if (t.hp < 0 || t.remove || t.position.x > 1000) { continue; }
                if ((t.position - to).sqrMagnitude < min_distance * min_distance) { continue; }

                if (closest == null || (t.position - to).sqrMagnitude < distance && t.hp > 0)
                {
                    closest = t;
                    distance = (float)(t.position - to).sqrMagnitude;
                }
            }

            if (type != Gun.Ammo.NONE && closest == null)
            {
                return findClosest(to, Gun.Ammo.NONE, min_distance);
            }

            return closest;
        }

        public bool findCollision(Entity collider)
        {
            Entity dummy;
            return findCollision(collider, out dummy);
        }

        public delegate void CollisionCallback (Entity with);

        public void findCollisions(Entity collider, CollisionCallback callback)
        {
            Vector2 tl = collider.position;
            Vector2 br = collider.position + collider.size;

            if (collider.center_based)
            {
                tl = collider.position - collider.size / 2;
                br = collider.position + collider.size / 2;
            }

            for (int i = entities.Count - 1; i >= 0; --i)
            {
                if (collider == entities[i] || entities[i].remove) { continue; }

                Vector2 etl = entities[i].position;
                Vector2 ebr = entities[i].position + entities[i].size;

                if (entities[i].center_based)
                {
                    etl = entities[i].position - entities[i].size / 2;
                    ebr = entities[i].position + entities[i].size / 2;
                }

                if (tl.x < ebr.x && tl.y < ebr.y && br.x > etl.x && br.y > etl.y)
                {
                    callback(entities[i]);
                }
            }
        }

        public bool findCollision(Entity collider, out Entity collidee)
        {
            Vector2 tl = collider.position;
            Vector2 br = collider.position + collider.size;

            if (collider.center_based)
            {
                tl = collider.position - collider.size / 2;
                br = collider.position + collider.size / 2;
            }

            for (int i = entities.Count - 1; i >= 0; --i)
            {
                if (collider == entities[i] || entities[i].remove) { continue; }
 
                Vector2 etl = entities[i].position;
                Vector2 ebr = entities[i].position + entities[i].size;

                if (entities[i].center_based)
                {
                    etl = entities[i].position - entities[i].size / 2;
                    ebr = entities[i].position + entities[i].size / 2;
                }

                if (tl.x < ebr.x && tl.y < ebr.y && br.x > etl.x && br.y > etl.y)
                {
                    collidee = entities[i];
                    return true;
                }
            }

            collidee = null;
            return false;
        }

        public virtual void tick()
        {
            for (int i = entities.Count - 1; i >= 0; --i)
            {
                if (entities[i].remove) { continue; }

                entities[i].tick();
            }

            flushAddRemove();
        }

        public void flushAddRemove()
        {
            for (int i = 0; i < entities.Count; ++i)
            {
                if (!entities[i].remove) { continue; }

                if (entities[i] is Target)
                {
                    Game.instance.house.damage((entities[i] as Target).held_orphans.Count);
                }

                entities[i].remove = false;
                entities[i].parent = null;
                entities.RemoveAt(i--);
            }

            for (int i = 0; i < to_add.Count; ++i) { entities.Add(to_add[i]); }

            to_add.Clear();
        }

        public virtual void draw()
        {
            for (int i = 0; i < entities.Count; ++i) { entities[i].draw(); }
        }
    }

    public class LightningBetween : Entity
    {
        public float f = 0;
        public int frame = 0;
        public Vector2 start, end;
        public bool skip_start_pop;

        public LightningBetween (Vector2 start_, Vector2 end_, bool skip_start_pop_)
        {
            start = start_;
            end = end_;
            skip_start_pop = skip_start_pop_;
        }

        public override void tick ()
        {
            f += 1.0f;
            frame = (int)f;
            if (++frame > 30)
                remove = true;
        }

        public override void draw ()
        {
//      var pos = (end+start)/2.0f;
            var angle = Util.angle (end - start);
            var scale = (start - end).magnitude;
            AppMain.textures.lightning_match.draw ("special1", frame * 10, start,
                new Vector2 (scale / 285, 0.2f), angle + (float)Math.PI, new Vector4 (1, 1, 1, 0.8f));
            var projectile_vomiting = (frame / 30.0f) * (frame / 30.0f) * 300;
            if (!skip_start_pop)
                AppMain.textures.skeletonkingprojectile.draw ("explosion", (int)projectile_vomiting, start, Vector2.one / 5, 0, new Vector4 (1, 1, 1, 0.8f));
            AppMain.textures.skeletonkingprojectile.draw ("explosion", (int)projectile_vomiting, end, Vector2.one / 5, 0, new Vector4 (1, 1, 1, 0.8f));
        }
    }

    public class Explosion : Entity
    {
        public float frame = 0;
        public int damage = 50;
        public float radius = 0;
        public EntityGroup target_group;
        Gun.Ammo type;

        public Explosion (Vector2 position_, Vector2 velocity_, float size_, int damage_, Gun.Ammo type_)
        {
            position = position_;
            velocity = velocity_;
            radius = size_ / 2;
            size = new Vector2(size_ / AppMain.textures.explosion.sprites[0].size.x,
                               size_ / AppMain.textures.explosion.sprites[0].size.y);
            damage = damage_;
            type = type_;
            target_group = Game.instance.enemy_group;
        }

        public override void Dispose()
        {
            base.Dispose();

            target_group = null;
        }

        public override void tick()
        {
            if (frame == 0)
            {
                for (int i = 0; i < target_group.entities.Count; ++i)
                {
                    Target t = (Target)target_group.entities[i];

                    if ((t.position - position).magnitude < radius + t.size.magnitude)
                    {
                        t.damage(damage, type);
                    }
                }
            }

            position += velocity;
            frame += 0.2f;
            if (frame >= 7) { remove = true; }
        }

        public override void draw()
        {
            var color = Vector4.one;
            if (Game.instance.house.visibleDoorPosition() < 1) { color.w = 0.4f; }
            AppMain.textures.explosion.draw((int)frame, position, size, 0, color);
        }
    }

    public abstract class EnemyBullet : Entity
    {
        public abstract void kill();
    }

    public struct Particle
    {
        public Vector2 position, origin, velocity, gravity, drag, scale, grow;
        public Vector4 color, color_delta;
        public float angle, spin;
        public float drawable_size;
        public bool remove;

        public float ground_at;
        public int timeout;

        public Atlas atlas;
        public float frame;
        public float frame_speed;
        public bool loop;
        public int loop_start, loop_end;
        public int time;

        public delegate void CollideBehavior (ref Particle p,Entity with);

        public CollideBehavior collide_behavior;

        public delegate void TickBehavior (ref Particle p);

        public TickBehavior tick_behavior;

        public List<Entity> collides_with;

        public void Dispose()
        {
            atlas = null;
            collides_with = null;
            collide_behavior = null;
            tick_behavior = null;
        }

        public Particle(Atlas a)
        {
            position = Vector2.zero;
            atlas = a;
            timeout = -1;
            drawable_size = a.sprites[0].size.magnitude;
            velocity = Vector2.zero;
            gravity = Vector2.zero;
            origin = Vector2.one / 2;
            drag = Vector2.one;
            scale = Vector2.one;
            grow = Vector2.zero;
            angle = 0;
            spin = 0;
            color = Vector4.one;
            color_delta = Vector4.zero;
            frame = 0;
            frame_speed = 0;
            loop = false;
            loop_start = 0;
            loop_end = a.n_sprites;
            remove = false;
            ground_at = 600;
            time = 0;
            collide_behavior = null;
            collides_with = null;
            tick_behavior = null;
        }
    }

    public class ParticleManager : Entity
    {
        private const int PARTICLE_MAX = 500;
        public Particle[] particles = new Particle[PARTICLE_MAX];
        public int particle_count = 0;

        public void add(Particle p)
        {
            if (particle_count >= PARTICLE_MAX) {
                particles[0] = particles[--particle_count];
            }

            particles[particle_count++] = p;
        }

        public override void Dispose()
        {
            base.Dispose();

            for (int i = 0; i < particle_count; i++) { particles[i].Dispose(); }
        }

        public override void tick()
        {
            for (int i = 0; i < particle_count; i++)
            {
                while (i < particle_count && particles[i].remove) {
                    for (int j = i + 1; j < particle_count; j++) { // shift to preserve ordering
                        particles[j - 1] = particles[j];
                        particles[j].Dispose();
                    }

                    particle_count--;
                }

                Particle p = particles[i];
                if (p.remove) { continue; }

                if (p.tick_behavior != null) { p.tick_behavior(ref p); }

                p.time++;

                if (p.collides_with != null && p.collides_with.Count == 0) { p.collides_with = null; }

                p.velocity += p.gravity;
                p.velocity.x *= p.drag.x;
                p.velocity.y *= p.drag.y;
                p.position += p.velocity;
                p.color += p.color_delta;
                p.scale += p.grow;
                p.angle += p.spin;
                p.frame += p.frame_speed;

                if (p.position.x > 1000 && p.velocity.x > 0 ||
                    p.position.x < -50 && p.velocity.x < 0 ||
                    p.position.y > 600 && p.velocity.y > 0 ||
                    p.position.y < -50 && p.velocity.y < 0)
                    p.remove = true;

                if (p.position.y > p.ground_at && p.velocity.y > 0) {
                    if (p.collide_behavior != null) {
                        p.collide_behavior(ref p, null);
                    }

                    p.remove = true;
                }

                if (p.timeout > 0 && --p.timeout == 0) { p.remove = true; }

                if (p.collide_behavior != null) {
                    Vector2 tl = p.position - p.scale * p.drawable_size / 2;
                    Vector2 br = p.position + p.scale * p.drawable_size / 2;

                    p.collides_with = Game.instance.enemy_group.entities;

                    for (int n = 0; n < p.collides_with.Count; ++n) {
                        Target e = (Target)p.collides_with[n];

                        Vector2 etl = e.position - e.size / 2;
                        Vector2 ebr = e.position + e.size / 2;

                        if (tl.x < ebr.x && tl.y < ebr.y && br.x > etl.x && br.y > etl.y) {
                            p.collide_behavior(ref p, e);
                        }
                    }
                }

                if (p.color.w <= 0) { p.remove = true; }
                if (p.scale.x <= 0 || p.scale.y <= 0) { p.remove = true; }
                if (p.frame >= p.loop_end) {
                    if (!p.loop) { p.remove = true; }
                    else { p.frame -= (p.loop_end - p.loop_start); }
                }

                particles[i] = p;
            }
        }

        public override void draw()
        {
            for (int i = 0; i < particle_count; i++) {
                Particle p = particles[i];
                if (p.remove) continue;

                p.atlas.draw((int)p.frame, p.position, p.origin, p.scale, p.angle, p.color);
            }
        }
    }

    public partial class Orphan : Entity
    {
        public bool escaping = false, mind_controlled = false;
        int state, type;
        float state_frame = 0;

        public Orphan(Vector2 pos)
        {
            position = pos;
            state = (int)orphan.Sprites.orphan1_grabbed1;
            type = Util.rng.Next (12);
        }

        public override void tick()
        {
            if (state == (int)orphan.Sprites.orphan1_grabbed1) {
                state_frame += 0.1f;
                if (state_frame >= 2) state_frame -= 2;
                Game.instance.house.orphanCry();
            }
            else { /* state == walking */
                state_frame += 0.1f;
                if (state_frame >= 3) state_frame -= 3.0f;
            }

            if (escaping) {
                velocity.y += gravity;
                position += velocity;

                if (Shield.existing_shield != null) {
                    int right = ForkGun.shield_right;
                    int left = ForkGun.shield_left;
                    if (position.x < right && position.x > left) {
                        if (position.x < (left + right) / 2) position.x = left;
                        else position.x = right;
                    }
                }

                if (position.y > 420 && velocity.y > 0) {
                    position.y = 420;
                    velocity = new Vector2 (velocity.x * 0.5f, -velocity.y * 0.5f);

                    if (Math.Abs(velocity.y) < 0.5) {
                        state = (int)orphan.Sprites.orphan1_walk1;
                        velocity = new Vector2(-Util.rng.NextFloat(escape_speed_min, escape_speed_max), 0);
                    }
                }

                if (position.x < escape_distance || position.x > 1000) { remove = true; }
            }
        }

        public override void draw()
        {
            AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 460),
                                             Vector2.one / 6, new Vector4(1, 1, 1, 0.5f));
            int frame = state + (int)state_frame + type * 5;
            AppMain.textures.orphan.draw(frame, position, new Vector2(velocity.x < 0 ? -1 : 1, 1), angle, Vector4.one);
        }

        public void enterWorld(Vector2 at)
        {
            if (escaping) return;

            position = at;
            velocity = new Vector2(Util.rng.NextFloat(-1.5f, 1.5f), Util.rng.NextFloat(-3, -1));
            escaping = true;
        }
    }

    public class MoneyGuy : Entity
    {
        public int home = 180;
        public float dest_x;
        public static MoneyGuy me;
        public bool facing_left = false;
        public string printed_score;
        public float sign_y = 0, sign_v = 0;
        public int time = 0;

        public MoneyGuy(bool by_house = false)
        {
            position = new Vector2(home, 510);
            addMoney(0);
        }

        public void addMoney(int money)
        {
            if (money != 0) {
                sign_v -= sign_v >= 0 ? 1.5f : 0.5f;
            }

            int stored_money = MetaState.hardcore_mode ? MetaState.hardcore_score : DataStorage.Money;
            stored_money += money;

            if (stored_money < 0) stored_money  = 0;

            if (stored_money > 999999) {
                printed_score = "999999";
            }
            else if (stored_money > 99999) {
                printed_score = System.String.Format("{0}", stored_money);
            }
            else {
                printed_score = System.String.Format("${0}", stored_money);
            }

            if (MetaState.hardcore_mode) {
                MetaState.hardcore_score = stored_money;
            }
            else {
                DataStorage.Money = stored_money;
            }
        }

        public override void tick()
        {
            time++;
            dest_x = home;

            if (Game.instance != null && Game.instance.house.visibleDoorPosition() < 1) {
                for (int i = Game.instance.particle_group.entities.Count - 1; i >= 0; --i) {
                    Entity e = Game.instance.particle_group.entities[i];

                    if (!e.remove && e.position.y > 480 && e is Pickup) {
                        Pickup p = (Pickup)e;

                        if (p.money > 0 && (dest_x == home || Math.Abs(p.position.x - position.x) <
                            Math.Abs(position.x - dest_x))) {
                            dest_x = p.position.x;
                        }
                    }
                }
            }

            sign_y += sign_v;
            sign_v += 0.1f;

            if (sign_y >= 0) {
                sign_y = 0;
                sign_v = 0;
            }

            velocity.x = Math.Sign(dest_x - position.x);
            facing_left = velocity.x < 0;
            position += velocity;

            addMoney(0);
        }

        public override void draw()
        {
            Vector4 color = Vector4.one;

            Vector2 offset = new Vector2(0, (time % 20 < 10) ? 0 : 2);
            AppMain.textures.money_guy.draw((int)moneyguy.Sprites.money_register, position + offset,
                                            new Vector2(facing_left ? 1 : -1, 1), color);
            AppMain.textures.money_guy.draw((int)moneyguy.Sprites.counter_money,
                                             position + new Vector2(0, -38 + sign_y), Vector2.one, color);
            AppMain.textures.shadowblob2.draw(0, new Vector2(position.x, 530),
                                              Vector2.one / 5, new Vector4(1, 1, 1, 0.5f));

            Vector2 money_pos = position + new Vector2(-56, -38 + sign_y) + new Vector2(printed_score.Length * 16, 0);

            for (int i = 0; i < printed_score.Length; ++i) {
                int n = printed_score[i] - '0' - 1;
                if (n == -1) { n = 9; }
                if (printed_score[i] == '$') { n = 10; }

                if (n >= 0 && n <= 10) {
                    AppMain.textures.counter_money.draw(n, money_pos, new Vector2(-1, 1), color);
                }
                money_pos.x -= 16;
            }
        }
    }

    public class Pickup : Entity
    {
        public float health = 0;
        public int money = 0;
        public Vector2 destination = Vector2.zero;
        public static int money_pickups = 0;

        public Pickup (Vector2 position_, float health_, int money_)
        {
            position = position_;
            velocity = Util.fromPolar(Util.rng.NextFloat((float)Math.PI, (float)Math.PI * 2), 1.5f);
            health = health_;
            money = money_;
            if (money > 0) {
                money_pickups++;
            }
        }

        public override void tick()
        {
            if (Game.instance.house.health <= 0) { remove = true; }

            if (money > 0 && (MoneyGuy.me.position - position).sqrMagnitude < 10000) {
                destination = MoneyGuy.me.position + new Vector2(0, 10);
            }
            else if (health > 0) {
                destination = new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3 + 230, 32);
            }
            else {
                destination = Vector2.zero;
            }

            if (destination == Vector2.zero) {
                velocity.y += 0.07f;
                position += velocity;
                if (position.y > 500) {
                    position.y = 500;
                    velocity = Vector2.zero;
                }
            }
            else {
                position = position * 0.95f + destination * 0.05f;
                if ((position - destination).sqrMagnitude < 400) {
                    remove = true;
                    if (money > 0) {
                        money_pickups--;
                        MoneyGuy.me.addMoney(money);
                    }
                    Game.instance.house.score += money;
                    Game.instance.house.health = Math.Min(Game.instance.house.health + health, Game.instance.house.max_health);
                }
            }
        }

        public override void draw()
        {
            int sprite;
            Vector2 scale = Vector2.one;
            Vector2 shadow_size = Vector2.one / 4;
            Vector2 offset = Vector2.zero;

            if (money > 150) {
                sprite = (int)pickups.Sprites.money1_large;
            }
            else if (money > 75) {
                offset.y = 5;
                sprite = (int)pickups.Sprites.money1_small;
                shadow_size = Vector2.one / 6;
            }
            else if (money > 0) {
                offset.y = 5;
                sprite = (int)pickups.Sprites.money2_small;
                shadow_size = Vector2.one / 6;
            }
            else {
                sprite = (int)pickups.Sprites.heart;
                scale /= 2;
            }

            float distance = Vector2.Distance(position, destination);
            if (distance < 100) {
                scale *= distance / 100;
                shadow_size *= distance / 100;
            }

            AppMain.textures.pickups.draw(sprite, position + offset, scale, Vector4.one);
            AppMain.textures.shadowblob.draw(0, new Vector2(position.x, 520),
                                                            shadow_size / (1.5f + (float)Math.Abs(position.y - 520) / 60),
                                             new Vector4 (1, 1, 1, 0.5f));
        }
    }

    public class Target : Entity
    {
        public float hp, max_hp = 0;
        public int damage_since_last_floaty = 0, floaty_timeout = 0;
        public Gun.Ammo damaged_by = Gun.Ammo.NONE;
        public int flashing = 0;
        public int flash_timeout = 0;
        public float hit_flash = 0.0f;
        public float frozen = 0;
        public int spiked = 0;
        public bool frozen_this_frame = false;
        public bool facing_left;
        public List<Orphan> held_orphans = new List<Orphan>();
        public List<SkullBullet> skulls = new List<SkullBullet>();

        static public Dictionary<Gun.Ammo, float> damages;

        public Target() { }

        public override void Dispose()
        {
            base.Dispose();

            if (held_orphans != null) {
                for (int i = held_orphans.Count - 1; i >= 0; --i) {
                    held_orphans[i].Dispose();
                }
            }

            if (skulls != null) {
                for (int i = skulls.Count - 1; i >= 0; --i) {
                    skulls[i].Dispose();
                }
            }

            held_orphans = null;
            skulls = null;
        }

        public virtual void damage(float by, Gun.Ammo type)
        {
            if (hp <= 0) { return; }

            max_hp = max_hp == 0 ? hp : max_hp;

            float amt = (by * Gun.UpgradeMultiplier(type));

            if (!Target.damages.ContainsKey(type)) {
                Target.damages[type] = 0;
            }
            Target.damages[type] += amt;

            hp -= amt;
            if (hp <= 0) {
                releaseOrphans();

                for (int i = 0; i < skulls.Count; ++i) {
                    skulls[i].explosion_timeout = 0;
                }
                frozen = 0;
                spiked = 0;
                startDying();
                Game.instance.house.addScore((int)max_hp, position);
                remove = true;
                Game.instance.dead_group.add(this);

                Objectives.DefeatedWithAmmo(type);
            }

            if (flash_timeout <= 0) {
                flashing = House.flash_length;
                float health = hp / (float)max_hp;
                flash_timeout = (int)(health * House.flash_max_timeout + (1 - health) * House.flash_min_timeout);
            }
        }

        public void releaseOrphans()
        {
            for (int i = 0; i < held_orphans.Count; ++i) {
                Game.instance.orphan_group.add(held_orphans[i]);
                held_orphans[i].enterWorld(position + held_orphans[i].position);
            }
            held_orphans.Clear();
        }

        virtual public void startDying()
        {
            finishDying();
        }

        public void finishDying()
        {
            remove = true;
        }

        public Vector4 flashColor()
        {
            if (flashing > 0) {
                flashing--;
                return new Vector4(1, 0, 0, 1);
            }

            if (frozen > 0) {
                return new Vector4(1 - (float)Math.Min(1, frozen) / 2, 1 - (float)Math.Min(1, frozen) / 2, 1, 1);
            }

            return Vector4.one;
        }

        public Vector2 facingScale()
        {
            return facingScale(Vector2.one);
        }

        public Vector2 facingScale(Vector2 base_scale)
        {
            return new Vector2(facing_left ? -base_scale.x : base_scale.x, base_scale.y);
        }

        public void freeze(float amt)
        {
            frozen = amt;
        }

        public void spike(int length)
        {
            spiked = Math.Max(spiked, length);
        }

        public bool frozenThisFrame()
        {
            return spiked > 0 || frozen_this_frame;
        }

        public override void tick()
        {
            if (--floaty_timeout <= 0 && damage_since_last_floaty > 0) {
                damage_since_last_floaty = 0;
                floaty_timeout = 20;
            }

            spiked -= spiked > 0 ? 1 : 0;

            if (frozen > 0) {
                frozen -= 1.0f / 60f;
                frozen_this_frame = !frozen_this_frame;
            } 
            else {
                frozen_this_frame = false;
            }

            flash_timeout -= flash_timeout > 0 ? 1 : 0;

            // don't update facing direction during "off" frame
            if(!frozenThisFrame())
                facing_left = velocity.x < 0 ? true : velocity.x > 0 ? false : facing_left;

            for (int i = 0; i < held_orphans.Count; ++i) {
                held_orphans[i].tick();
            }

            for (int i = 0; i < skulls.Count; ++i) {
                skulls[i].tick();
            }

            if (skulls.Count > 0 && skulls[0].explosion_timeout <= 0) {
                skulls.Clear();
            }

            base.tick();
        }

        public void drawSubOrphans()
        {
            for (int i = 0; i < held_orphans.Count; ++i) {
                held_orphans[i].position += position;
                held_orphans[i].draw();
                held_orphans[i].position -= position;
            }
        }

        public void drawSubBullets()
        {
            for (int i = 0; i < skulls.Count; ++i) {
                skulls[i].position += position;
                skulls[i].draw();
                skulls[i].position -= position;
            }
        }
    }
}
