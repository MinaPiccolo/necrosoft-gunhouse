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
        public const int size = 15;//33;
        public const int size_upgrade = 3;//5;
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
        public const int explosion_size = 5;//25;
        public const int explosion_size_upgrade = 5;//10;
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
