using System;
using System.Collections.Generic;

namespace Gunhouse
{
    public partial class DrDog
    {
        public const int health = 7000;
        public const float approach_speed = 1.5f;
        public const float jump_force = 3.0f;
        public const float jump_wait = 60.0f;
        public const float gravity = 0.05f;
        public const float punch_distance = 600.0f;
        public const float punch_wait = 90.0f;
        public const float punch_speed = 5.0f;
        public const float punch_chance = 0.5f;
        public const float missile_chance = 0.5f;
        public const int missile_distance = 800;
        public const float missile_max_speed = 2.5f;
        public const float missile_acceleration = 0.05f;
        public const float missile_rotation_speed = 0.025f;
        public const float missile_damage = 0.5f;
        public const int missiles_per_burst = 8;
        public const float missile_lifetime = 10.0f;
        public const float eat_wait = 120;
        public const int orphan_grab_min = 2;
        public const int orphan_grab_max = 4;
        public const float orphan_drop_damage = 500.0f;
        public const float eat_speed = 0.2f;
        public const float die_speed = 6.0f;
    }

    public partial class DrDogMinion
    {
        public const int health = 300;
        public const float approach_speed_min = 1.5f;
        public const float approach_speed_max = 2.0f;
        public const float gravity = 0.1f;
        public const int steal_distance = 300;
        public const float run_animation_speed_min = 0.0075f;
        public const float run_animation_speed_max = 0.0125f;
        public const float die_animation_speed = 0.15f;
    }

    public partial class DrDogMinion2
    {
        public const int health = 200;
        public const float approach_speed_min = 1.5f;
        public const float approach_speed_max = 2.0f;
        public const float destination_min = 650;
        public const float destination_max = 800;
        public const float run_animation_speed_min = 0.0075f;
        public const float run_animation_speed_max = 0.0125f;
        public const float die_animation_speed = 0.15f;
        public const float fire_wait_min = 1.0f;
        public const float fire_wait_max = 3.0f;
        public const float fire_damage = 0.5f;
        public const float fire_speed = 8.0f;
    }

    public partial class DrDogMiniboss
    {
        public const int health = 300;
        public const float approach_speed_min = 0.8f;
        public const float approach_speed_max = 1.0f;
        public const float walk_anim_speed = 5.0f;
        public const float fire_anim_speed = 5.0f;
        public const float die_anim_speed = 8.0f;
        public const float bullet_anim_speed = 5.0f;
        public const float bullet_speed = 5.0f;
        public const float bullet_damage = 1.0f;
        public const float aim_turn_rate = 0.005f;
        public const float attack_cooldown = 2.0f;
    }

    public partial class DrDogFlyer
    {
        public const int health = 300;
        public const float speed = 2.5f;
        public const float fly_anim_speed = 6.0f;
        public const float grab_anim_speed = 6.0f;
        public const float shoot_anim_speed = 6.0f;
        public const float die_anim_speed = 7.0f;
        public const float shoot_chance = 0.8f;
        public const int grab_distance = 370;
        public const float shot_speed = 4.0f;
        public const float shot_damage = 0.8f;
    }

    public partial class SkeletonKingFlyer
    {
        public const int health = 200;
        public const float gravity = 0.2f;
        public const float speed = 2.5f;
        public const float death_anim_speed = 1.0f;
        public const int strike_distance = 400;
        public const float strike_damage = 0.75f;
        public const int steal_distance = 350;
        public const int max_hits = 2;
        public const float runaway_speed = 1.3f;
    }

    public partial class SkeletonKingMinion
    {
        public const int health = 300;
        public const float gravity = 0.2f;
        public const float approach_speed_min = 1.5f;
        public const float approach_speed_max = 2.0f;
        public const float death_anim_speed = 1.0f;
        public const float run_anim_speed_min = 0.5f;
        public const float run_anim_speed_max = 0.7f;
        public const int steal_distance = 300;
    }

    public partial class SkeletonKingMinion2
    {
        public const int health = 200;
        public const float gravity = 0.2f;
        public const float approach_speed_min = 0.7f;
        public const float approach_speed_max = 1.2f;
        public const float death_anim_speed = 1.0f;
        public const float run_anim_speed_min = 0.5f;
        public const float run_anim_speed_max = 0.7f;
        public const int steal_distance = 300;
        public const float weapon_gravity = 0.1f;
        public const float weapon_damage = 1.0f;
        public const int max_throw_angle = 30;
        public const int min_throw_angle = 20;
        public const int throw_strength = 7;
    }

    public partial class SkeletonKingMiniboss
    {
        public const int health = 220;
        public const float air_resistance = 0.95f;
        public const float fly_anim_speed = 0.6f;
        public const float die_anim_speed = 0.75f;
        public const float idle_anim_speed = 0.6f;
        public const float attack_anim_speed = 0.9f;
        public const float frost_explode_anim_speed = 1.0f;
        public const float frost_speed = 3.0f;
        public const float frost_damage = 0.75f;
        public const float fly_acceleration = 0.025f;
        public const float fly_max_speed = 2.0f;
        public const int destination_switch_distance = 75;
        public const float after_flight_wait = 1.5f;
        public const float continue_flying_chance = 0.4f;
        public const float attack_chance = 0.75f;
    }

    public partial class SkeletonKing
    {
        public const int health = 7000;
        public const float walk_speed = 1.2f;
        public const float swing_chance = 0.5f;
        public const int min_distance = 550;
        public const int max_distance = 800;
        public const int idle_time_min = 120, idle_time_max = 240;
        public const float swing_damage = 2.0f;
        public const float orphan_drop_damage = 200.0f;
    }

    public partial class PenguinBoss
    {
        public const float x_destination = 750;
        public const int health = 8000;
        public const float walk_speed = 1.0f;
        public const int idle_time_min = 120, idle_time_max = 240;
        public const float shoot_chance = 0.5f;
        public const float bullet_damage = 2.0f;
        public const float bullet_speed = 4.0f;
    }

    public partial class PenguinTank
    {
        public const int walk_dest_min = 550, walk_dest_max = 900;
        public const float walk_speed = 2.0f;
        public const int idle_time_min = 60, idle_time_max = 180;
        public const float bullet_speed = 4.0f;
        public const int health = 500;
        public const float bullet_damage = 1.5f;
    }

    public partial class PenguinFlyer
    {
        public const int health = 200;
        public const float shoot_chance = 0.8f;
        public const float grab_position = 300.0f;
        public const float speed = 2.0f;
        public const int min_shoots = 5;
        public const int max_shoots = 10;
    }

    public partial class PenguinFlyer2
    {
        public const int health = 200;
        public const float speed = 2.0f;
    }

    public partial class PenguinMinion
    {
        public const int health = 300;
        public const float approach_speed_min = 1.1f;
        public const float approach_speed_max = 1.8f;
        public const float run_anim_speed_min = 0.5f;
        public const float run_anim_speed_max = 0.7f;
        public const int steal_distance = 300;
        public const float gravity = 0.25f;
    }

    public partial class DrHoot
    {
        public const int health = 7000;
        public const float orphan_drop_damage = 500.0f;
        public const float approach_speed = 1.0f;
        public const float grab_chance = 0.5f;
        public const float laser_damage = 0.08f;
        // per frame
        public const int min_orphans_grabbed = 3;
        public const int max_orphans_grabbed = 5;
        public const float idle_time_min = 1.0f;
        public const float idle_time_max = 2.5f;

    }

    public partial class HootTank
    {
        public const int walk_dest_min = 550, walk_dest_max = 900;
        public const float walk_speed = 2.0f;
        public const int idle_time_min = 60, idle_time_max = 180;
        public const float bullet_speed = 4.0f;
        public const float bullet_damage = 0.3f;
        public const int health = 500;
    }

    public partial class HootMinion
    {
        public const float approach_speed_min = 1.1f;
        public const float approach_speed_max = 1.8f;
        public const float run_anim_speed_min = 0.5f;
        public const float run_anim_speed_max = 0.7f;
        public const int steal_distance = 300;
        public const float gravity = 0.5f;
        public const float weapon_gravity = 0.1f;
        public const int health = 300;
        public const float weapon_damage = 1.5f;
        public const int max_throw_angle = 50;
        public const int min_throw_angle = 20;
        public const int throw_strength = 10;
    }

    public partial class HootMinion2
    {
        public const float approach_speed_min = 1.1f;
        public const float approach_speed_max = 1.8f;
        public const float run_anim_speed_min = 0.5f;
        public const float run_anim_speed_max = 0.7f;
        public const int steal_distance = 300;
        public const float gravity = 0.2f;
        public const float weapon_gravity = 0.08f;
        public const int health = 150;
        public const float jumpchance = 0.3f;
        public const float jump_strength = 5.0f;
        public const float weapon_damage = 1.5f;
        public const int max_throw_angle = 40;
        public const int min_throw_angle = 50;
        public const float throw_strength = 7;
    }

    public partial class HootFly
    {
        public const int health = 200;
        public const float shoot_chance = 0.8f;
        public const float grab_position = 350.0f;
        public const float speed = 1.3f;
        public const float idle_boost = 2.0f;
        public const float gravity = 0.1f;
        public const float bulet_speed = 7.0f;
        public const float bullet_damage = 1.0f;
        public const int min_shoots = 5;
        public const int max_shoots = 10;
    }

    public partial class IceCreamBoss
    {
        public const float eat_pos = 500;
        public const float max_pos = 600;
        public const float min_pos = 900;
        public const int health = 7000;
        public const float orphan_drop_damage = 500.0f;
        public const float walk_speed = 2.0f;
        public const int idle_time_min = 60, idle_time_max = 120;
        public const float bullet_damage = 2.5f;
        public const float bullet_speed = 5.0f;
        public const float prob_eating = 0.2f;
        public const float max_orphans = 3;
        public const float max_spit = 6;
    }

    public partial class IceCreamFlyer2
    {
        public const int health = 200;
        public const float max_speed = 6.0f;
        public const float max_climb_speed = 8.0f;
        public const float bullet_speed = 7.0f;
        public const float bullet_damage = 1.0f;
        public const float min_idle = 20.0f;
        public const float max_idle = 10.0f;
        public const float range_pos = 500;
    }

    public partial class IceCreamFlyer
    {
        public const int health = 300;
        public const float shoot_chance = 0.8f;
        public const float grab_position = 450.0f;
        public const float max_speed = 3.0f;
        public const float escape_speed = 6.0f;
        public const float escape_weight = 0.1f;
        public const float bulet_speed = 7.0f;
        public const float bullet_damage = 1.0f;
        public const int min_shoots = 2;
        public const int max_shoots = 5;
    }

    public partial class IceCreamMinion
    {
        public const float approach_speed_min = 1.1f;
        public const float approach_speed_max = 1.4f;
        public const float run_anim_speed_min = 0.5f;
        public const float run_anim_speed_max = 0.7f;
        public const int steal_distance = 300;
        public const float gravity = 0.5f;
        public const float jump_strength = 10.0f;
        public const int maximum_jumps = 4;
        public const int health = 300;
        public const float weapon_damage = 1.5f;
    }

    public partial class IceCreamTank
    {
        public const int walk_dest_min = 600, walk_dest_max = 800;
        public const float walk_speed = 1.5f;
        public const int idle_time_min = 1, idle_time_max = 2;
        public const float bullet_speed = 9.0f;
        public const float bullet_yspeed = 1.4f;
        public const float bullet_damage = 1.0f;
        public const float gravity = 0.15f;
        public const int health = 500;
    }

    public partial class PeterBoss
    {
        public enum State { START, IDLE, LASER, MISSILES, CAST, RAISE, LOWER, SHOOT, SLAM, DYING };

        public const int level1_health = 9000;
        public const int level2_health = 6000;
        public const int level3_health = 2500;

        public const int level2_heal_bonus = 300;
        public const int level3_heal_bonus = 300;

        // EXAMPLE public State[] level1_actions = { State.LASER, State.MISSILES, State.RAISE, State.SHOOT, State.SLAM };
        public State[] level1_actions = { State.LASER, State.MISSILES, State.RAISE, State.SHOOT, State.SLAM };
        public State[] level2_actions = { State.LASER, State.MISSILES, State.RAISE, State.SHOOT, State.SLAM };
        public State[] level3_actions = { State.LASER, State.MISSILES, State.RAISE, State.SHOOT, State.SLAM };

        public const float orphan_drop_damage = 400.0f;
        public const float approach_speed = 1.0f;
        public const float grab_chance = 0.3f;
        public const float laser_damage = 0.03f;
        // per frame
        public const int min_orphans_grabbed = 3;
        public const int max_orphans_grabbed = 5;
        public const float idle_time_min = 1.0f;
        public const float idle_time_max = 2.5f;

        public const float missile_chance = 0.5f;
        public const int missile_distance = 800;
        public const float missile_max_speed = 2.5f;
        public const float missile_acceleration = 1.0f;
        public const float missile_rotation_speed = 0.03f;
        public const float missile_damage = 0.2f;
        public const int missiles_per_burst = 8;
        public const float missile_lifetime = 10.0f;

        public const float bullet_damage = 1.0f;
        public const float bullet_speed = 4.0f;

        public const float slam_damage = 0.3f;
    }

    public partial class Orphan
    {
        public const float escape_speed_min = 1.4f;
        public const float escape_speed_max = 1.6f;
        public const int escape_distance = 200;
        public const float gravity = 0.1f;
        public const float run_animation_speed_min = 0.08f;
        public const float run_animation_speed_max = 0.12f;
    }

    public class SkullGun
    {
        public const float fire_rate = 1.0f;
        public const float fire_arc = 0.4f;
        public const float skulls_per_shot = 3;

        public const int damage = 7;
        public const int damage_upgrade = 0;
        public const int size = 15;
        public const int size_upgrade = 3;
        public const float velocity = 5.0f;
        public const float explosion_delay = 2.0f;
        public const int explosion_size = 20;
        public const int explosion_size_upgrade = 5;
        public const float aim_error = 0.025f;
        public const float turn_speed = 0.3f;
        public const int ammo = 4;
        public const int ammo_upgrade = 1;

        public const int special_skull_size = 20;
        public const int special_skull_size_upgrade = 3;
        public const int special_skull_count = 20;
        public const int special_skull_count_upgrade = 5;
        public const float special_min_velocity = 4.0f;
        public const float special_max_velocity = 6.0f;
        public const int special_damage = 6;
        public const int special_damage_upgrade = 1;
        public const int special_max_altitude = 400;
    }

    public class SinGun
    {
        public const int ammo = 20;
        public const int ammo_upgrade = 2;
        public const int size = 10;
        public const int size_upgrade = 2;
        public const float wavelength = 30.0f;
        public const int vertical_range = 70;
        public const int vertical_range_upgrade = 0;
        public const int damage = 5;
        public const int damage_upgrade = 2;

        public const float special_spike_size = 200;
        public const float special_spike_size_upgrade = 30;
        public const float special_spike_lifetime = 2.0f;
        public const float special_spike_lifetime_upgrade = 0.25f;
        public const int special_spike_count = 3;
        public const float special_spike_count_upgrade = 0.5f;
        public const int special_damage = 3;
        // per spike, applied twice
        public const int special_damage_upgrade = 2;
    }

    public class BeachBallGun
    {
        public const float ammo = 5;
        public const float ammo_upgrade = 1.0f;
        public const int size = 33;
        public const int size_upgrade = 5;
        public const int damage = 20;
        public const int damage_upgrade = 5;
        public const float gravity = 0.1f;
        public const int special_damage = 4;
        public const float special_damage_upgrade = 0.5f;
        public const float special_speed = 4;
        public const float special_radius = 130;
        public const float special_radius_upgrade = 10;

    }

    public class BoomerangGun
    {
        public const int ammo = 5;
        public const int ammo_upgrade = 1;
        public const int size = 35;
        public const int size_upgrade = 5;
        public const float damage = 12;
        public const float damage_upgrade = 4;
        public const int special_size = 150;
        public const int special_size_upgrade = 20;
        public const float special_damage = 2.5f;
        public const float special_damage_upgrade = 0.2f;
    }

    public class ForkGun
    {
        public const int ammo = 9;
        public const int ammo_upgrade = 1;
        public const int size = 17;
        public const int size_upgrade = 2;
        public const int damage = 8;
        public const int damage_upgrade = 1;
        public const float angle = 15;
        public const float velocity = 6;

        public const int shield_left = 340;
        public const int shield_right = 570;
        public const float shield_duration = 2.0f;
        public const float shield_duration_upgrade = 0.5f;
    }

    public class FlameGun
    {
        public const int ammo = 10;
        public const float ammo_upgrade = 0.5f;
        public const int size = 10;
        public const int size_upgrade = 3;
        public const float damage = 0.3f;
        public const float damage_upgrade = 0.05f;
        public const float turn_speed = 0.01f;
        public const float velocity = 4;
        public const float lifetime = 1.0f;
        public const float lifetime_upgrade = 0.15f;

        public const int special_flame_count = 4;
        public const float special_flame_count_upgrade = 0.2f;
        public const float special_flame_lifetime = 1.2f;
        public const float special_flame_lifetime_upgrade = 0.15f;

        /*    public const float special_velocity = 6;
    public const float special_drag = 0.985f;
    public const int special_flame_count = 3;
    public const float special_flame_count_upgrade = 0.25f;
    public const float special_flame_duration = 3.0f;
    public const float special_flame_duration_upgrade = 0.25f;*/
    }

    public partial class DragonGun
    {
        public const float fire_rate = 1.2f;
        public const float turn_speed = 0.3f;
        public const int ammo = 4;
        public const float gravity = 0.1f;
        public const float bullet_velocity = 8.0f;
        public const int bullet_size = 2;
        public const int bullet_size_upgrade = 1;
        public const int ammo_upgrade = 2;
        public const int damage = 12;
        public const int damage_upgrade = 3;
        public const int explosion_size = 25;
        public const int explosion_size_upgrade = 10;
        public const float aim_lead = 0.00035f;
        public const float aim_error = 0.025f;

        public const float special_derpy_chance = 0.15f;
        public const float special_time = 3.0f;
        public const float special_time_upgrade = 0.5f;
        public const float special_fire_rate = 2.0f;
        public const float special_fire_rate_upgrade = 0.2f;
        public const float special_fire_angle = 0;
        public const float special_velocity = 4.0f;
    }

    public class LightningGun
    {
        public const float ammo = 8;
        public const float ammo_upgrade = 0;
        public const int range = 240;
        public const int range_upgrade = 10;
        public const int damage = 20;
        public const float damage_upgrade = 2;
        public const int special_strikes = 3;
        public const float special_strike_upgrade = 0.25f;
    }

    public class IglooGun
    {
        public const float fire_rate = 4.5f;
        public const float turn_speed = 0.3f;
        public const int ammo = 8;
        public const int ammo_upgrade = 4;
        public const float velocity = 5.2f;
        public const float aim_error = 0.01f;
        public const int damage = 5;
        public const int damage_upgrade = 1;
        public const float size = 35.0f;
        public const float size_upgrade = 5.0f;
        public const float freeze_time = 2.0f;
        public const float freeze_time_upgrade = 1.0f;
        public const float explosion_size = 32.0f;
        public const float explosion_size_upgrade = 16.0f;

        public const int special_damage = 6;
        public const int special_damage_upgrade = 2;
        public const float special_penguin_freeze_time = 1.5f;
        public const float special_penguin_freeze_time_upgrade = 0.75f;
        public const int special_penguin_count = 15;
        public const int special_penguin_count_upgrade = 5;
        public const float special_penguin_size = 45.0f;
        public const float special_penguin_size_upgrade = 5.0f;
        public const float special_penguin_speed = 15.0f;
        public const float special_penguin_rate = 8.0f;
        public const float special_penguin_rate_upgrade = 0.5f;
        public const float special_explosion_size = 64.0f;
        public const float special_explosion_size_upgrade = 32.0f;
    }

    public class VegetableGun
    {
        public const float fire_rate = 6.0f;
        public const float velocity = 8.5f;
        public const float turn_speed = 0.02f;
        public const int ammo = 20;
        public const int ammo_upgrade = 5;
        public const float size = 2.5f;
        public const float size_upgrade = 0.5f;
        public const float damage = 12;
        public const float damage_upgrade = 0.5f;
        public const float aim_error = 0.05f;

        public const int special_damage = 60;
        public const int special_damage_upgrade = 20;
        public const float special_size = 0.5f;
        public const float special_size_upgrade = 0.15f;
    }

    public partial class Puzzle
    {
        public const int grid_top = 85;
        public const int grid_left = 72;
        public const float block_gravity = 1.2f; // originally was 0.8f;
        public const float current_match_bonus = 2.0f;
        public const float attack_round_length = 6.0f;
        public const float gun_burst_length = 3.0f;
        public const int max_upgrade_level = 12;
    }

    public class Story
    {
        public static string[] tips = {
            "Protip: Match big blocks to the pulsing element to get a power bonus!",
            "Protip: Get health and money by killing enemies.",
            "Protip: New guns are automatically equipped. Make sure to curate your loadout.",
            "Protip: Spend dollar units at the company store. Ask your parents for permission.",
            "Protip: Loading tiny blocks has no effect on your guns, but gets them out of the way.",
            "Protip: The boomerang special attack stays in the lane from which it was fired.",
            "Protip: Loading a new ammo type replaces your existing gun.",
            "Protip: Figure out your favorite guns and only use those forever.",
            "Protip: Call somebody you love today. Tell them I said hi.",
            "Protip: Load guns to the left and special attacks to the right!",
            "Protip: Alien robots are jerks.",
            "Protip: The more a gun jiggles, the more powerful it is!",
            "Protip: Defend your orphans! They are so precious!",
            "Protip: I know where you live.",
            "Protip: Bet you I could beat up a wolverine.",
            "Protip: Look out for tetrominos! They are easy to turn into big blocks.",
            "Protip: Let's be friends forever!",
            "Protip: Necrosoft Games loves you.",
            "Protip: To donate money to help actual orphans, we recommend www.savethechildren.org.",
            "Protip: If you bought this game you are probably very attractive.",
            "Protip: The fork gun's shield special blocks most projectiles, but has trouble with ghosts.",
            "Protip: Beachball bullets bounce back to the height from which they were shot.",
            "Protip: Frogs aren't actually that good at math.",
            "Protip: The laser special not only locks enemies in place, it damages them when it breaks apart.",
            "Protip: Skull gun bullets only do damage when they explode.",
            "Protip: \"You live in a little house made of guns. You need many guns to fight invaders but also need to keep a roof on top of your many children\" ~Peter Molydeux",
            "Protip: Ground enemies stick to a certain path, but flying enemies go wherever they like.",
            "Protip: Hit enemies to make them drop orphans!",
            "Protip: If you're having trouble with a particular wave, remember to upgrade your guns and health!",
            "Protip: If you chew with your mouth open you are just the worst.",
            "Protip: The money collector robot is saving up your cast-off pennies to open a gluten-free bakery."
        };

        public static string[] story = {
            "Caretaker's note: I know not whence these clanging metal men come, nor the whyfors of their appearing, but they have caused the children a great deal of unease. I am not one to suffer fools.",
            "Caretaker's note: And still they persist. Never have there been such carryings-on at my doorstep. They may go with my blessing so long as they never return.",
            "Caretaker's note: Billy lost his first infant's tooth in to-day's commotion. I gently explained to him that a new one would grow back in its place, but he would not be consoled. The other children made sport of him, and were negatively affected by his ill humor. They took to misdeeds throughout the evening.",
            "Caretaker's note: A day's passing has not assuaged the metal howlers at my gate. The children have succumbed to unease, and though I am not inclined to grouse, it is a trying task managing both their emotional states and the affairs outside our walls.",
            "Caretaker's note: I have not the proclivity nor a tolerance for violence. It is why I took up as a caretaker of the young and parentless. But when a body is so thoroughly vexed, as I have been these two days past, to what other path can we turn?",
            "Caretaker's note: It appears we shall have another evening without rest. I shall remain diligent for as long as I am able, but I begin to consider the situation outside the orphanage. With so many aggressors at our doors, what of the rest of the town? Why does no-one come to our aid?",
            "Caretaker's note: As this morning dawned, I knew our situation would be unchanged, and none would come to help. It is up to me. It is a lucky thing our stores are full of provisions, that we may last out several days of isolation. I pray for a swift end to these troubles.",
            "Caretaker's note: The children have taken to naming our foes, singing limericks to mock their horrid visages. Rhyming is the devil's game, but these are desperate times, and I shall let it pass. This world is not so black and white.",
            "Caretaker's note: Through it all, daily chores must be dealt with. Victuals must be prepared, the orphanage's leaky pipes demand a constant tending to, and the beds want making. Hard times shall not remove from us our civility.",
            "Caretaker's note: I have resolved to educate the children in matters of the orphanage's upkeep and defense. Should I be unable to continue my duties, they will have to carry on in my stead. Whatever the case, we shall not waver.",
            "Caretaker's note: Round noontime, little Jillian got the devil in her, and fell ill. I worry she is like to expire. Salves and tinctures can't protect her from the terrible din out-of-doors. It's rest she's needing, above all.",
            "Caretaker's note: Our fair Jillian did not make the night through. Had I the time, had I the supplies, I might have offered her a smoother passing, but she went with a wail and a terrible violence. The others shuffle about under a sombre malaise.",
            "Caretaker's note: The constant repairs and day-to-days of the orphanage are more than I can manage. Evermoreso, the children help where they can. They grow older daily. Would that I could grant them a carefree youth.",
            "Caretaker's note: There are times when I would simply give up. But the children are my strength. I will carry on forever if needs must, for their sake if not for my own. I cannot protect their innocence, but mayhaps I can guard their lives.",
            "Caretaker's note: I miss the sunshine. I can see it from outside our windows, but that is a pale replacement for letting it beam down upon my face, in the meadows. But I mustn't leave the orphanage. I mustn't.",
            "Caretaker's note: Some of the children have taken to wearing the crockery on their heads as helmets. I remind them that we still need to cook on occasion, but they seem to be taking our conflict more seriously than their dietary needs.",
            "Caretaker's note: Watching my young charges at their posts recalls me bittersweetly to the days when they would roll about with their toys, and it was only scuffed knees or bruised feelings that elicited their tears. The challenges of to-day are far more grave.",
            "Caretaker's note: The children grow accustomed to these skirmishes, alarmingly so. I admit, there is little time to feel sorry for ourselves. We must battle on. I fear for their future.",
            "Caretaker's note: These last events rocked the children's toychest for a powerful spell. Though I bound it with twine, it did split apart into many pieces. Scattered as they were, I could not help but consider the many children we have missed to the inexplicable happenings of these dark days. Well may they rest.",
            "Caretaker's note: The long hours harry me. I started with a fright when our Timothy tried to hold my hand as I went about my duties. It frightened him so, and he would not be quieted until I gave him a biscuit as a calmative. I do confess it was the last of them.",
            "Caretaker's note: Though it has been ages since I touched the stuff, I happened upon some brandy down in the cellar as I ventured deeper into our provisions. Just a sip to calm the nerves. I haven't the stomach for more.",
            "Caretaker's note: I took ken to it the night prior, but feared to write it down, making it real. Our stores begin to run low. Though we have rationed carefully, there are too many mouths to feed. Too many are working too hard to go without nourishment. Actions must be taken.",
            "Caretaker's note: We have been isolated for so long now. We have heard nothing from the outside world, and I fear we may be all that is left of it. I must get word from the village, and we must have more supplies. I will broach the subject after our sparse meal.",
            "Caretaker's note: As agreed, I stole away in secret whilst the children kept the metal men at bay. I have divined that those we now face are only the vanguard of a much larger syndicate. I hear tell of cities falling, and it is the children they are after. The children!",
            "Caretaker's note: During last evening's sojourn, I managed to scrounge a small variety of provisions, with which to supplement our dwindling stores. This morn I distributed a parcel of sweet biscuits among my hungry charges. Such smiles they boasted!",
            "Caretaker's note: To-day I fell asleep for a spell as I kept watch. I woke with a start, worried something might have befallen the children. In truth, they had taken over my post, and put a blanket round my shoulders besides. The silly dears.",
            "Caretaker's note: From beyond the reaches of the village rubble, I can hear klaxons sounding. Someone is warning us. But who, and of what? What horrors could be worse than those we have already faced? Why this terrible din?",
            "Caretaker's note: Something stirs, out there, amid the ruins of our once-populous village. In the furthest reaches of our vision. Something massive. It comes for My Innocent Little Orphans' souls. It shall not have them. Where others have fallen, we shall resist.",
            "Caretaker's note: It feeds on us - on the children. I see that now. With each precious stolen life it grows stronger. I know not the means of stopping it. What could do this, if not a god? Gods be damned, then, and may the devil take them.",
            "Caretaker's note: It was no god. Just another empty metal shell. Our orphanage stands strong, for the time being. But what of others? Who will help them? We, who survive. We shall help. We shall write our own fable. Where evil roams, we shall resist. Sally forth, my children, to victory! ~Genevieve Severine Mercy Goppert"
        };
    }

    public partial class Shop
    {
        const int base_heart_price = 1200;
        const float heart_price_multiplier = 1.5f;
        const int base_armor_price = 3000;
        const float armor_price_multiplier = 1.5f;
        const int base_healing_price = 400;
        const float healing_price_multiplier = 1.5f;
        const int base_upgrade_price = 400;
        const float upgrade_price_multiplier = 1.65f;

        public static int max_equipped = 3;

        public static GunUpgrade[] guns = {
            // row 1
            new GunUpgrade(Gun.Ammo.DRAGON, "Dragon Gun", "Upgrade to a dragon friend!", 0),
            new GunUpgrade(Gun.Ammo.IGLOO, "Penguin Gun", "Your enemies will chill out.", 0),
            new GunUpgrade(Gun.Ammo.SKULL, "Skull Gun", "These guys are champing at the bit.", 0),
            new GunUpgrade(Gun.Ammo.VEGETABLE, "Vegetable Gun", "Veggies make you a straight shooter.", 1500),
            new GunUpgrade(Gun.Ammo.LIGHTNING, "Lightning Gun", "Tesla-style chain attacks.", 2000),
            // row 2
            new GunUpgrade(Gun.Ammo.FLAME, "Flame Gun", "The Dragon Gun's enthusiastic cousin.", 2500),
            new GunUpgrade(Gun.Ammo.FORK, "Fork Gun", "Defensive, with a split personality.", 3500),
            new GunUpgrade(Gun.Ammo.BOUNCE, "Beach Ball Gun", "Bouncy, bouncy, bouncy!", 1500),
            new GunUpgrade(Gun.Ammo.BOOMERANG, "Boomerang Gun", "There and back again.", 1000),
            new GunUpgrade(Gun.Ammo.SIN, "Sine Wave Gun", "Sine on the line which is dotted!", 2500)
        };
    }

    public class Difficulty
    {
        public const float door_openness_start         = 2.5f;
        public const float door_openness_multiplier    = 0.85f;
        public const float door_openness_minimum       = 2.5f;
        public const float door_close_speed = 0.003f;

        public const float healing_upgrade_base          = 0.05f;
        public const float healing_upgrade_steepness     = 5.5f;
        public const float healing_upgrade_amplification = 0.09f;

        public const float armor_initial       = 1.5f;
        public const float armor_steepness     = 0.1f;
        public const float armor_amplification = 9.0f;

        public const float money_initial = 0.5f;
        public const float money_steepness = 0.7f;
        public const float money_amplification = 4;

        public const float gun_upgrade_base          = 1.0f;
        public const float gun_upgrade_steepness     = 0.7f;
        public const float gun_upgrade_amplification = 5.0f;

        public const float monster_armor_initial       = 1.0f;
        public const float monster_armor_steepness     = 0.1f;
        public const float monster_armor_amplification = 12.0f;

        public const float monster_attack_initial       = 1.0f;
        public const float monster_attack_steepness     = 0.1f;
        public const float monster_attack_amplification = 2.5f;

        public const float peter_armor_initial       = 2.0f;
        public const float peter_armor_steepness     = 0.1f;
        public const float peter_armor_amplification = 5.75f;

        public const float peter_attack_initial       = 1.55f;
        public const float peter_attack_steepness     = 0.1f;
        public const float peter_attack_amplification = 0.9f;

        public const int time_per_enemy = 30;
        public const int time_per_cluster = 5*30;
        public const float min_cluster_size = 8.0f;
        public const float max_cluster_size = 12.0f;
        public const float cluster_size_step = 2.0f;
        public const float final_cluster_multiplier = 2.0f;

        public static WaveDefinition[] waves = {
            new WaveDefinition(new Dictionary<Type, float> {            /* 1 */
                                { typeof(DrDogMinion), 0.5f },
                                { typeof(DrDogMinion2), 0.5f },
                                { typeof(DrDogMiniboss), 4.0f },
                                { typeof(DrDogFlyer), 2.0f },
                                { typeof(DrDog), 10.0f } },
                               typeof(DrDogBackgroundDay), typeof(DrDogBackgroundDusk), typeof(DrDogBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 2 */
                                { typeof(IceCreamMinion), 0.01f },
                                { typeof(IceCreamFlyer), 1.0f },
                                { typeof(IceCreamFlyer2), 3.0f },
                                { typeof(IceCreamTank), 4.0f },
                                { typeof(IceCreamBoss), 10.0f } },
                               typeof(PyramidBackgroundDay), typeof(PyramidBackgroundDusk), typeof(PyramidBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 3 */
                                { typeof(SkeletonKingMinion), 0.5f },
                                { typeof(SkeletonKingMiniboss), 3.0f },
                                { typeof(SkeletonKingFlyer), 2.0f },
                                { typeof(SkeletonKing), 10.0f } },
                               typeof(SkeletonKingBackgroundDay), typeof(SkeletonKingBackgroundDusk), typeof(SkeletonKingBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 4 */
                                { typeof(PenguinMinion), 0.5f },
                                { typeof(PenguinFlyer), 1.0f },
                                { typeof(PenguinFlyer2), 1.0f },
                                { typeof(PenguinTank), 4.0f },
                                { typeof(PenguinBoss), 10.0f } },
                               typeof(PenguinPirateBackgroundDay), typeof(PenguinPirateBackgroundDusk), typeof(PenguinPirateBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 5 */
                                { typeof(HootFly), 3.0f },
                                { typeof(HootMinion), 1.0f },
                                { typeof(HootMinion2), 1.0f },
                                { typeof(HootTank), 3.0f },
                                { typeof(DrHoot), 10.0f } },
                               typeof(ForestBackgroundDay), typeof(ForestBackgroundDusk), typeof(ForestBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 6 */
                                { typeof(IceCreamMinion), 0.01f },
                                { typeof(IceCreamFlyer), 1.0f },
                                { typeof(IceCreamFlyer2), 3.0f },
                                { typeof(IceCreamTank), 4.0f },
                                { typeof(IceCreamBoss), 10.0f } },
                               typeof(StageOaklandBackgroundDay), typeof(StageOaklandBackgroundDusk), typeof(StageOaklandBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 7 */
                                { typeof(DrDogMinion), 0.5f },
                                { typeof(DrDogMiniboss), 4.0f },
                                { typeof(DrDogFlyer), 2.0f },
                                { typeof(DrDog), 10.0f } },
                               typeof(SkeletonKingBackgroundDay), typeof(SkeletonKingBackgroundDusk), typeof(SkeletonKingBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 8 */
                                { typeof(PenguinMinion), 0.5f  },
                                { typeof(PenguinFlyer), 1.0f  },
                                { typeof(PenguinFlyer2), 1.0f  },
                                { typeof(PenguinTank), 4.0f  },
                                { typeof(PenguinBoss), 10.0f } },
                               typeof(ForestBackgroundDay), typeof(ForestBackgroundDusk), typeof(ForestBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 9 */
                                { typeof(HootFly), 3.0f },
                                { typeof(HootMinion), 1.0f },
                                { typeof(HootMinion2), 1.0f },
                                { typeof(HootTank), 3.0f },
                                { typeof(DrHoot), 10.0f } },
                               typeof(DrDogBackgroundDay), typeof(DrDogBackgroundDusk), typeof(DrDogBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 10 */
                                { typeof(HootFly), 2.0f },
                                { typeof(IceCreamFlyer2), 2.0f },
                                { typeof(DrDogFlyer), 2.0f },
                                { typeof(SkeletonKingFlyer), 2.0f },
                                { typeof(PenguinFlyer2), 2.0f },
                                { typeof(PeterBoss), 99999.0f }, },
                               typeof(SpaceBackground), typeof(SpaceBackground), typeof(SpaceBackground),
                               0),

            new WaveDefinition(new Dictionary<Type, float> {            /* 11 */
                                { typeof(SkeletonKingMinion), 0.5f },
                                { typeof(SkeletonKingMinion2), 0.5f },
                                { typeof(SkeletonKingMiniboss), 3.0f },
                                { typeof(SkeletonKingFlyer), 2.0f },
                                { typeof(SkeletonKing), 10.0f } },
                               typeof(PyramidBackgroundDay), typeof(PyramidBackgroundDusk), typeof(PyramidBackgroundNight),
                               1),
            new WaveDefinition(new Dictionary<Type, float> {            /* 12 */
                                { typeof(PenguinMinion), 0.5f },
                                { typeof(PenguinFlyer), 1.0f },
                                { typeof(PenguinFlyer2), 1.0f },
                                { typeof(PenguinTank), 4.0f },
                                { typeof(PenguinBoss), 10.0f } },
                               typeof(SkeletonKingBackgroundDay), typeof(SkeletonKingBackgroundDusk), typeof(SkeletonKingBackgroundNight),
                               1),
            new WaveDefinition(new Dictionary<Type, float> {            /* 13 */
                                { typeof(IceCreamMinion), 0.01f },
                                { typeof(IceCreamFlyer), 1.0f },
                                { typeof(IceCreamFlyer2), 3.0f },
                                { typeof(IceCreamTank), 4.0f },
                                { typeof(IceCreamBoss), 10.0f } },
                               typeof(PenguinPirateBackgroundDay), typeof(PenguinPirateBackgroundDusk), typeof(PenguinPirateBackgroundNight),
                               2),
            new WaveDefinition(new Dictionary<Type, float> {            /* 14 */
                                { typeof(SkeletonKingMinion), 0.5f },
                                { typeof(SkeletonKingMinion2), 0.5f },
                                { typeof(SkeletonKingMiniboss), 3.0f },
                                { typeof(SkeletonKingFlyer), 2.0f },
                                { typeof(SkeletonKing), 10.0f } },
                               typeof(DrDogBackgroundDay), typeof(DrDogBackgroundDusk), typeof(DrDogBackgroundNight),
                               0),
            new WaveDefinition(new Dictionary<Type, float> {            /* 15 */
                                { typeof(DrDogMinion), 0.5f },
                                { typeof(DrDogMinion2), 0.5f },
                                { typeof(DrDogMiniboss), 4.0f },
                                { typeof(DrDogFlyer), 2.0f },
                                { typeof(DrDog), 10.0f } },
                               typeof(ForestBackgroundDay), typeof(ForestBackgroundDusk), typeof(ForestBackgroundNight),
                               2),
            new WaveDefinition(new Dictionary<Type, float> {            /* 16 */
                                { typeof(HootFly), 3.0f },
                                { typeof(HootMinion), 1.0f },
                                { typeof(HootMinion2), 1.0f },
                                { typeof(HootTank), 3.0f },
                                { typeof(DrHoot),10.0f } },
                               typeof(PyramidBackgroundDay), typeof(PyramidBackgroundDusk), typeof(PyramidBackgroundNight),
                               2),
            new WaveDefinition(new Dictionary<Type, float> {            /* 17 */
                                { typeof(IceCreamMinion), 0.01f },
                                { typeof(IceCreamFlyer), 1.0f },
                                { typeof(IceCreamFlyer2), 3.0f },
                                { typeof(IceCreamTank), 4.0f },
                                { typeof(IceCreamBoss), 10.0f } },
                               typeof(SkeletonKingBackgroundDay), typeof(SkeletonKingBackgroundDusk), typeof(SkeletonKingBackgroundNight),
                               2),
            new WaveDefinition(new Dictionary<Type, float> {            /* 18 */
                                { typeof(SkeletonKingMinion), 0.5f },
                                { typeof(SkeletonKingMinion2), 0.5f },
                                { typeof(SkeletonKingMiniboss), 3.0f },
                                { typeof(SkeletonKingFlyer), 2.0f },
                                { typeof(SkeletonKing), 10.0f } },
                               typeof(ForestBackgroundDay), typeof(ForestBackgroundDusk), typeof(ForestBackgroundNight),
                               3),
            new WaveDefinition(new Dictionary<Type, float> {            /* 19 */
                                { typeof(PenguinMinion), 0.5f },
                                { typeof(PenguinFlyer), 1.0f },
                                { typeof(PenguinFlyer2), 1.0f },
                                { typeof(PenguinTank),4.0f },
                                { typeof(PenguinBoss), 10.0f } },
                               typeof(DrDogBackgroundDay), typeof(DrDogBackgroundDusk), typeof(DrDogBackgroundNight),
                               2),
            new WaveDefinition(new Dictionary<Type, float> {           /* 20 */
                                { typeof(HootFly), 2.0f },
                                { typeof(IceCreamFlyer2), 2.0f },
                                { typeof(DrDogFlyer), 2.0f },
                                { typeof(SkeletonKingFlyer), 2.0f },
                                { typeof(PenguinFlyer2), 2.0f },
                                { typeof(PeterBoss), 99999.0f }, },
                               typeof(SpaceBackground),typeof(SpaceBackground), typeof(SpaceBackground),
                               3),
        };
  }

    public partial class House
    {
        public const float health_complain_threshold = 0.4f;

        public const float orphan_count_flash_time = 1.5f;
        public const float orphan_count_flash_rate = 20.0f;
        public const float orphan_cry_rate = 1.0f;
        public const int health_per_heart = 10;
        public static Gun.Ammo[] ammo_types = { Gun.Ammo.DRAGON, Gun.Ammo.SKULL, Gun.Ammo.IGLOO, Gun.Ammo.VEGETABLE };
        public const float step_speed = 1.0f;
        public const int max_frameskip = 4;
        public const int flash_length = 3;
        public const int flash_max_timeout = 30;
        public const int flash_min_timeout = 9;

        public const int lane_a = 85 + 64, lane_b = 85 + 64 * 3, lane_c = 85 + 64 * 5;
    }
}
