using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Necrosoft;

namespace Gunhouse
{
    public class PuzzlePiece : Entity
    {
        public Gun.Ammo type;
        public int taps_to_remove;
        public bool settled = false;
        public bool selected = false;
        public Vector2 settled_position;
        public int visual_shift = 0;

        public PuzzlePiece(Gun.Ammo type_, Vector2 position_)
        {
            type = type_;
            position = position_;
            size = new Vector2(Puzzle.piece_size, Puzzle.piece_size);
            center_based = false;
        }

        public void SetGridPosition(Vector2 p)
        {
            position = p * Puzzle.piece_size;
        }

        public void SetGridSize(Vector2 s)
        {
            size = new Vector2(s.x * Puzzle.piece_size, s.y * Puzzle.piece_size);
        }

        public Vector2 GridPosition()
        {
            return new Vector2((float)Math.Floor(position.x / Puzzle.piece_size + 0.5),
                               (float)Math.Floor(position.y / Puzzle.piece_size + 0.5));
        }

        public Vector2 GridSize()
        {
            return size / Puzzle.piece_size;
        }

        public override void tick()
        {
            base.tick();

            velocity += new Vector2(0, Puzzle.block_gravity);
            settled = false;

            Vector2 destination = position + velocity;
            Vector2 delta = new Vector2(Math.Sign(velocity.x), Math.Sign(velocity.y));

            while (delta.y > 0 && position.y < destination.y || delta.y < 0 && position.y > destination.y) {
                position.y += delta.y;
                if (parent.findCollision(this)) {
                    position.y = (float)Math.Round(position.y / Puzzle.piece_size) * Puzzle.piece_size;
                    velocity.y = 0;
                    settled = true;
                    settled_position = position;

                    break;
                }
            }
        }

        public override void draw()
        {
            int x_offset = 0;
            x_offset = visual_shift;

            if (position.y + Puzzle.piece_size > 0) {
                var pos = position + size / 64 * 32 + new Vector2(Puzzle.grid_left, Puzzle.grid_top) +
                    new Vector2(-x_offset, 0);

                AppMain.textures.block.draw_outline(Puzzle.ammoToBlockSprite(type), pos,
                                                    size / 64 / 4,
                                                    new Vector4(0.5f, 0.5f, 0.5f, 0.5f),
                                                    1.0f, selected);

                float square_size = System.Math.Min(size.x, size.y);
                AppMain.textures.block_logo.draw(Puzzle.ammoToLogoSprite(type), pos,
                                                 new Vector2(-1, 1) * square_size / 64 / 4,
                                                 new Vector4(0.5f, 0.5f, 0.5f, 0.5f),
                                                 1.0f);
            }
        }
    }
    
    public class PadMenuController
    {
        int selected_button = 0;
        Button[] live_buttons;

        public PadMenuController()
        {
            live_buttons = AppMain.menuOverlay.GetLiveButtons();
        }

        public void tick()
        {
            Vector2 direction = Util.keyRepeat("menuoverlay_pad_repeat", Vector2.zero, 20, 5, Input.Pad.Move);

            if (direction.x != 0) {
                selected_button += Util.sign(direction.x);
                selected_button = Util.clamp(selected_button, 0, live_buttons.Length - 1);
            }

            if (Input.Pad.Submit.WasPressed) { live_buttons[selected_button].onClick.Invoke(); }

            for (int i = 0; i < live_buttons.Length; ++i) {
                Button b = live_buttons[i];
                float pulse = 1 + Mathf.Sin(AppMain.frame / 30.0f) / 10;
                if (i != selected_button) { pulse = 1; }
                b.GetComponent<RectTransform>().localScale = new Vector3(pulse, pulse, 1);
            }
        }
    }

    public class EndWaveState : State
    {
        public int time = 0;
        public float alpha = 0.0f;
        public bool won;

        public string story = "";
        public string story_so_far;

        public int money_ticker;
        public int money_tick_amt;

        PadMenuController pmc;

        public EndWaveState(bool won_, State child_state_)
        {
            MetaState.end_game = false;
            AppMain.screenShake(0, 0);
            Choom.StopAllEffects();

            won = won_;
            child_state = child_state_;
            tick_child_state = false;

            AppMain.tutorial.SetDisplay(false);
            AppMain.menuOverlay.Show(won);

            if (!won) { DataStorage.TimesDefeated++; }

            if (!won && MetaState.hardcore_mode) {
                Game.instance.saveHardcoreScore();
                Platform.SaveHardcore();
            }

            pmc = new PadMenuController();

            money_ticker = DataStorage.Money;
            money_tick_amt = (DataStorage.Money - money_ticker) / 180;

            if (won && MetaState.wave_number + 1 > DataStorage.StartOnWave &&
                !MetaState.hardcore_mode) {

                if ((MetaState.wave_number % 3) == 2) { Objectives.BossDefeated(); }
                DataStorage.StartOnWave = MetaState.wave_number + 1;
            }

            Platform.SaveEndWave();
            Objectives.CheckAchievements();
        }

        override public void tick()
        {
            if (AppMain.top_state != this) { return; }

            base.tick();

            pmc.tick();

            MoneyGuy.me.tick();

            money_ticker += money_tick_amt;
            if (money_ticker >= DataStorage.Money) {
                money_ticker = DataStorage.Money;
            }
        }

        override public void draw()
        {
            base.draw();
        }
    }

    public class EndGameState : State
    {
        public int time = 0;
        public float alpha = 0.0f;

        public string story = "";
        public string story_so_far;

        PadMenuController pmc;

        public EndGameState(State child_state_)
        {
            MetaState.end_game = false;
            AppMain.screenShake(0, 0);

            Choom.StopAllEffects();

            child_state = child_state_;

            if (MetaState.wave_number + 1 > DataStorage.StartOnWave) {
                DataStorage.StartOnWave = MetaState.wave_number + 1;
            }

            Objectives.BossDefeated();
            Objectives.SurvivedFinalStage();
            Objectives.CheckAchievements();

            child_state = null;
            AppMain.top_state = new MenuState(Menu.MenuState.Credits);

            pmc = new PadMenuController();
        }

        public override void tick()
        {
            if (AppMain.top_state != this) { return; }

            pmc.tick();

            base.tick();
        }

        override public void draw()
        {
            if (this != AppMain.top_state) { return; }

            base.draw();
        }
    }

    public class WaveDefinition
    {
        public Dictionary<Type, float> enemy_list;
        public ArrayList backgrounds = new ArrayList();

        public float cluster_size_goal = 5.0f;
        public float cluster_size = 0.0f;
        public int enemy_palette = 0;

        public bool done = false;
        public bool boss_out = false;

        public int next_enemy_timeout = 0;
        public int next_cluster_timeout = 0;

        public WaveDefinition(Dictionary<Type, float> enemy_list_, Type background1, Type background2,
                              Type background3, int enemy_palette_)
        {
            enemy_palette = enemy_palette_;
            enemy_list = enemy_list_;
            backgrounds.Add(background1);
            backgrounds.Add(background2);
            backgrounds.Add(background3);
        }

        public void reset()
        {
            load();

            if (true) {
                AppMain.background = (Entity)Activator.CreateInstance(backgrounds[MetaState.wave_number % 3] as Type);
                //AppMain.background_fade = 0.0f;
                //AppMain.background_fade_delta = 1.0f / 120;
            }

            done = false;
            boss_out = false;
            MetaState.end_game = false;
            cluster_size_goal = Difficulty.min_cluster_size;
            cluster_size = 0.0f;
            next_cluster_timeout = Difficulty.time_per_cluster;
            next_enemy_timeout = 0;
        }

        public void tick()
        {
            if (done || --next_enemy_timeout > 0) { return; }

            next_enemy_timeout = Difficulty.time_per_enemy;

            if (cluster_size >= cluster_size_goal) {
                if (!done && Game.instance.enemy_group.entities.Count > 0) { return; }
                if (done && --next_cluster_timeout > 0) { return; }

                next_cluster_timeout = Difficulty.time_per_cluster;
                cluster_size = 0;

                if (!done) { cluster_size_goal += Difficulty.cluster_size_step; }
                if (cluster_size_goal > Difficulty.max_cluster_size +
                    Difficulty.cluster_size_step * (MetaState.wave_number % 3)) {
                    done = true;
                    cluster_size_goal *= Difficulty.final_cluster_multiplier;
                }
            }

            if ((MetaState.wave_number % 30) == 9 * 3 + 2) {
                cluster_size = 0;
                done = true;
            }

            int enemy_n = Util.rng.Next(enemy_list.Count - 1);

            if (!boss_out && done && cluster_size == 0 && MetaState.wave_number % 3 == 2) {
                enemy_n = enemy_list.Count - 1;
                boss_out = true;
            }

            Type enemy = null;
            foreach (Type t in enemy_list.Keys) {
                if (enemy_n-- == 0) { enemy = t; }
            }

            Game.instance.enemy_group.add((Entity)Activator.CreateInstance(enemy));

            cluster_size += enemy_list[enemy];
        }

        public void load()
        {
            AppMain.textures.orphan.touch();
            AppMain.textures.ui.touch();
            foreach (Type t in enemy_list.Keys) {
                t.GetMethod("loadAssets").Invoke(null, null);
            }
        }
    }

    public class Game : State
    {
        public EntityGroup bullet_group, gun_group, enemy_group, orphan_group,
        particle_group, house_group, enemy_bullet_group, dead_group;
        public ParticleManager particle_manager, bullet_manager, enemy_bullet_manager;

        MatrixType camera = new MatrixType { matrix = new float[][] { new float[Matrix.size], new float[Matrix.size], new float[Matrix.size] } };
        public float camera_mod = 0;

        public static Game instance;

        public int time = 0;

        public House house;

        public PuzzlePiece enemy_target_piece = null;
        public Vector2 enemy_target_offset = Vector2.zero;
        public float enemy_target_damage = 0;
        public int damage_frame;
        public float damage_angle;

        public float door_close_time;

        public List<Gun.Ammo> next_bonuses = new List<Gun.Ammo>();
        public const int next_bonus_count = 2;
        public int match_combo = 1;
        public int match_streak = 0;
        public Gun.Ammo current_bonus;

        public int win_timer = 0;

        public Puzzle puzzle;

        int startWaveTimer = 60 * 3;

        bool hidePauseButton = false;
        public static bool HidePauseButton { set { instance.hidePauseButton = value; } }

        public Game()
        {
            Tracker.LevelStart(MetaState.wave_number);

            instance = this;

            camera = Matrix.Identity(camera);

            AppMain.tutorial.SetDisplay(MetaState.wave_number == 0 && !MetaState.hardcore_mode);

            house = new House();

            bullet_group = new EntityGroup(EntityGroupID.Bullets);
            enemy_bullet_group = new EntityGroup(EntityGroupID.EnemyBullets);
            particle_group = new EntityGroup(EntityGroupID.Particles);
            orphan_group = new EntityGroup(EntityGroupID.Orphans);
            particle_group = new EntityGroup(EntityGroupID.Particles);
            enemy_group = new EntityGroup(EntityGroupID.Enemies);
            house_group = new EntityGroup(EntityGroupID.House);
            dead_group = new EntityGroup(EntityGroupID.Dead);

            house_group.add(house);
            house_group.add(MoneyGuy.me);

            particle_manager = new ParticleManager();
            particle_group.add(particle_manager);
            enemy_bullet_manager = new ParticleManager();
            enemy_bullet_group.add(enemy_bullet_manager);
            bullet_manager = new ParticleManager();
            bullet_group.add(bullet_manager);

            gun_group = new EntityGroup(EntityGroupID.Guns);

            for (int i = 1; i <= 5; i += 2)
            {
                Gun g = new Gun(new Vector2(Puzzle.grid_left + 200, Puzzle.grid_top + Puzzle.piece_size * i),
                                enemy_group, bullet_group);

                g.gun_index = gun_group.to_add.Count;
                gun_group.add(g);
            }

            puzzle = new Puzzle();

            next_bonuses.Clear();

            for (int i = 0; i < Game.next_bonus_count; i++) {
                Game.instance.next_bonuses.Add(Game.instance.puzzle.nextPiece());
            }

            house.advanceNext();

            entities.Add(enemy_group);
            entities.Add(enemy_bullet_group);
            entities.Add(dead_group);
            entities.Add(orphan_group);
            entities.Add(puzzle.piece_group);
            entities.Add(house_group);
            entities.Add(gun_group);
            entities.Add(bullet_group);
            entities.Add(particle_group);

            door_close_time = Difficulty.door_openness_start *
                                        (float)Math.Pow(Difficulty.door_openness_multiplier, MetaState.wave_number);
            door_close_time = Math.Max(door_close_time, Difficulty.door_openness_minimum);

            int boss_music = 0;
            int stage_music = 0;

            for (int i = 0; i < MetaState.wave_number; i++) {
                if (i % 3 == 2) { boss_music++; }
                else { stage_music++; }
            }

            string music = "/Stages/stage2";
            if (MetaState.wave_number % 3 == 2) {
                switch (boss_music % 2) {
                    case 0: music = "/Boss/boss"; break;
                    case 1: music = "/Boss/boss2"; break;
                }
            }
            else {
                switch (stage_music % 5) {
                    case 0: music = "/Stages/stage2"; break;
                    case 1: music = "/Stages/stage3"; break;
                    case 2: music = "/Stages/stage4"; break;
                    case 3: music = "/Stages/stage5"; break;
                    case 4: music = "/Stages/stage6"; break;
                }
            }

            if ((MetaState.wave_number % 30) == 9 * 3 + 2) { music = "/Boss/boss3"; }

            Choom.Play("Music" + music);

            AppMain.MainMenu.DisplayDayName();

            #if UNITY_SWITCH
            hidePauseButton = UnityEngine.Switch.Operation.mode == UnityEngine.Switch.Operation.OperationMode.Console;
            #endif
        }

        public override void Dispose()
        {
            base.Dispose();

            if (bullet_group != null) {
                bullet_group.Dispose();
                bullet_group = null;
            }
            if (gun_group != null) {
                gun_group.Dispose();
                gun_group = null;
            }
            if (enemy_group != null){
                enemy_group.Dispose();
                enemy_group = null;
            }
            if (orphan_group != null) {
                orphan_group.Dispose();
                orphan_group = null;
            }
            if (particle_group != null) {
                particle_group.Dispose();
                particle_group = null;
            }
            if (house_group != null) {
                house_group.Dispose();
                house_group = null;
            }
            if (enemy_bullet_group != null) {
                enemy_bullet_group.Dispose();
                enemy_bullet_group = null;
            }
            if (dead_group != null) {
                dead_group.Dispose();
                dead_group = null;
            }
            if (particle_manager != null) {
                particle_manager.Dispose();
                dead_group = null;
            }
            if (bullet_manager != null) {
                bullet_manager.Dispose();
                bullet_manager = null;
            }
            if (enemy_bullet_manager != null) {
                enemy_bullet_manager.Dispose();
                enemy_bullet_manager = null;
            }
            if (house != null) {
                house.Dispose();
                house = null;
            }
            if (next_bonuses != null) {
                next_bonuses.Clear();
                next_bonuses = null;
            }
            if (puzzle != null) {
                puzzle.Dispose();
                puzzle = null;
            }
        }

        public override void draw()
        {
            camera = Matrix.Scale(camera, new Vector2 (-1, 1));
            camera = Matrix.Scale(camera, new Vector2(960, 0));

            base.draw();

            if (AppMain.top_state == this) {
                if (!hidePauseButton) {
                    AppMain.textures.hud.draw((int)hud.Sprites.pause, new Vector2(AppMain.vscreen.x - 47, 47),
                                              new Vector2(-1, 1) * 64 / 97, Vector4.one);
                }

                if (time > startWaveTimer) {
                    AppMain.tutorial.SetLesson(Lesson.MAKE_BLOCKS);
                }
            }
        }

        public override void tick()
        {
            time++;

            #region Pause/Back

            if (AppMain.top_state == this) {
                for (int i = 0; i < Input.touches.Count; i++) {
                    if (Input.touches[i].status != Touch.Status.DOWN) { continue; }

                    Rect pauseButtonRect = new Rect(AppMain.vscreen.x - 100, 10, 90, 90);
                    if (pauseButtonRect.Contains(Input.touches[i].position)) {
                        AppMain.top_state = new MenuState(Menu.MenuState.Pause, this);
                    }
                }

                if (Input.Pad.Start.WasPressed) AppMain.top_state = new MenuState(Menu.MenuState.Pause, this);
            }

            if (AppMain.back) {
                AppMain.back = false;
                if (Input.touches.Count > 0 || MetaState.wave.done) return;

                AppMain.top_state = new MenuState(Menu.MenuState.Pause, this);
            }

            #endregion

            #region Value Resets

            if (house.door_position < 1.0f) {
                if (!MetaState.end_game) {
                    enemy_group.tickrate = 0;
                    enemy_bullet_group.tickrate = 0;
                }

                bullet_group.tickrate = 0;
                gun_group.tickrate = 0;
                orphan_group.tickrate = 0;
            }
            else {
                enemy_group.tickrate = 1;
                enemy_bullet_group.tickrate = 1;
                bullet_group.tickrate = 1;
                gun_group.tickrate = 1;
                orphan_group.tickrate = 1;
            }

            if (AppMain.top_state != this) {
                bullet_group.tickrate = 0;
                gun_group.tickrate = 0;
                orphan_group.tickrate = 0;
                enemy_group.tickrate = 0;
                enemy_bullet_group.tickrate = 0;
                bullet_group.tickrate = 0;
                gun_group.tickrate = 0;
                orphan_group.tickrate = 0;
            }

            #endregion

            if (AppMain.top_state == this) {
                base.tick();
                puzzle.tick();
            }

            if (AppMain.tutorial.hasFocus) { return; }

            if (time > startWaveTimer &&
                house.door_position == 1.0f &&
                AppMain.top_state == this) {
                MetaState.wave.tick();
            }

            #if UNITY_EDITOR

            if (Input.keys[(int)KeyCode.Delete]) { house.health = 0; }

            #endif

            if (house.health <= 0 && AppMain.top_state == this) {
                AppMain.top_state = new EndWaveState(false, this);
                return;
            }

            if (MetaState.wave.done && enemy_group.entities.Count == 0 && AppMain.top_state == this) {
                int win_goal = 150;

                /* NOTE(shane): this is a round about saying equals remainder 29 which is the final boss stage */
                if ((MetaState.wave_number % 30) == 9 * 3 + 2) {
                    win_goal = 200;
                }

                if (++win_timer > win_goal) {
                    particle_group.flushAddRemove();

                    for (int i = 0; i < particle_group.entities.Count; ++i) {
                        if (!(particle_group.entities[i] is Pickup)) { continue; }

                        particle_group.entities[i].remove = true;
                        MoneyGuy.me.addMoney(((Pickup)particle_group.entities[i]).money);
                    }

                    particle_group.flushAddRemove();

                    MoneyGuy.me.sign_v = 0;

                    //if (MetaState.end_game) { AppMain.top_state = new EndGameState(this); }
                    if (MetaState.wave_number == 29) { AppMain.top_state = new EndGameState(this); }
                    else { AppMain.top_state = new EndWaveState(true, this); }
                }
            }
            else {
                win_timer = 0;
            }
        }

        public void saveHardcoreScore()
        {
            DataStorage.BestHardcoreScores.Add(new Tuple<int, int>(MetaState.hardcore_score, MetaState.wave_number));
            DataStorage.BestHardcoreScores.Sort((Tuple<int, int> a, Tuple<int, int> b) => { return b.Item1.CompareTo(a.Item1); });
            if (DataStorage.BestHardcoreScores.Count > DataStorage.SCORES_TO_KEEP) {
                DataStorage.BestHardcoreScores.RemoveAt(DataStorage.SCORES_TO_KEEP);
            }
        }

        public static int ammoTypesPerWave(int wave_number)
        {
            return wave_number < 3 ? 2 : 3;
        }

        public void damageTargetPiece(float amt = 1.0f)
        {
            float house_armor_coefficient = MetaState.logCurve(DataStorage.Armor,
                                                               Difficulty.armor_initial,
                                                               Difficulty.armor_steepness,
                                                               Difficulty.armor_amplification);

            Game.instance.house.damage(amt * MetaState.monster_attack_coefficient / house_armor_coefficient);
        }

        public Vector2 targetDestination()
        {
            return new Vector2(Puzzle.grid_top + Puzzle.piece_size * 1.5f, Puzzle.grid_left + Puzzle.piece_size * 1.5f);
        }

        public void removeBoss()
        {
            for (int i = enemy_group.entities.Count - 1; i >= 0; --i)
            {
                ((Target)enemy_group.entities[i]).startDying();
            }
        }
    }

    public partial class Puzzle : State
    {
        public EntityGroup piece_group;
        public const int piece_size = 64;
        public const int rows = 6;
        public const int columns = 3;
        int prev_x;
        bool swiped = false;

        public Touch current_grab;
        public bool settling = true, kill_grab = false;

        public int cursor_y = 0;

        public int selected_row = -1;
        public Vector2 selected_weapon = Vector2.zero;

        public float shift_so_far = 0;

        public int wait_print;

        public Puzzle()
        {
            piece_group = new EntityGroup(EntityGroupID.PuzzlePieces);
            piece_group.tickrate = 0;
            piece_group.add(new Wall(new Vector2(0, rows * piece_size), new Vector2(columns * piece_size, piece_size)));
            piece_group.origin = new Vector2(Puzzle.grid_left, Puzzle.grid_top);

            for (int i = 0; i < columns; ++i) {
                for (int j = 0; j < rows; ++j) {
                    piece_group.add(new PuzzlePiece(nextPiece(), new Vector2(i * piece_size, -j * piece_size * 1.2f)));
                }
            }

            piece_group.flushAddRemove();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (piece_group != null)
            {
                piece_group.Dispose();
                piece_group = null;
            }
        }

        public override void tick()
        {
            if (settling) {
                if (piecesSettled()) {
                    for (int i = 0; i < piece_group.entities.Count; ++i) {
                        if (!(piece_group.entities[i] is PuzzlePiece)) { continue; }

                        PuzzlePiece p = (PuzzlePiece)piece_group.entities[i];

                        if (p.position.y > piece_size * 5 || p.position.y < 0) { p.remove = true; }
                    }

                    piece_group.flushAddRemove();

                    bool merges_found = false;
                    while (findMerges()) {
                        piece_group.tick();
                        merges_found = true;
                    }

                    if (merges_found && Game.instance.house.visibleDoorPosition () < 1) {
                        Choom.PlayEffect(SoundAssets.BlockFuse);
                    }

                    settling = false;
                }

                piece_group.tick();
            }

            if (AppMain.tutorial.hasFocus) { return; }

            if (AppMain.top_state == Game.instance) {
                if (AppMain.game_pad_active && Input.touches.Count > 0) {
                    clearPadInputState();
                    AppMain.game_pad_active = false;
                }
                else if (!AppMain.game_pad_active && Input.touches.Count == 0 &&
                         (Input.Pad.Move != Vector2.zero || Input.Pad.AnyWasPressed)) {
                    clearTouchInputState();
                    AppMain.game_pad_active = true;
                }

                if (AppMain.game_pad_active) { handlePadInput(); }
                else { handleTouchInput(); }
            }
        }

        public Gun.Ammo nextPiece()
        {
            return House.ammo_available[Util.rng.Next(House.ammo_available.Count)];
        }

        bool piecesSettled()
        {
            for (int i = 0; i < piece_group.entities.Count; ++i)
            {
                if (piece_group.entities[i] is PuzzlePiece &&
                    !((PuzzlePiece)piece_group.entities[i]).settled) { return false; }
            }

            return true;
        }

        public PuzzlePiece pieceAt(Vector2 at)
        {
            for (int i = 0; i < piece_group.entities.Count; ++i) {
                if (!(piece_group.entities[i] is PuzzlePiece)) { continue; }

                PuzzlePiece p = (PuzzlePiece)piece_group.entities[i];

                Vector2 tl = p.GridPosition();
                Vector2 br = tl + new Vector2(p.size.x / Puzzle.piece_size, p.size.y / Puzzle.piece_size);

                if (at.x >= tl.x && at.y >= tl.y && at.x < br.x && at.y < br.y) { return p; }
            }
            return null;
        }

        bool areaContiguous(Vector2 top_left, Vector2 bottom_right)
        {
            Gun.Ammo type = Gun.Ammo.NONE;

            for (int i = (int)top_left.x; i <= bottom_right.x; i++) {
                for (int j = (int)top_left.y; j <= bottom_right.y; j++) {

                    PuzzlePiece looking_at = pieceAt(new Vector2 (i, j));
                    if (looking_at == null) { return false; }

                    // check if piece in question is fully contained in area
                    var piece_tl = looking_at.GridPosition();
                    var piece_br = piece_tl + looking_at.GridSize() - new Vector2 (1, 1);
                    if (piece_tl.x < top_left.x || piece_tl.y < top_left.y ||
                        piece_br.x > bottom_right.x || piece_br.y > bottom_right.y) {
                        return false;
                    }

                    if (type == Gun.Ammo.NONE) { type = looking_at.type; }
                    else if (type != looking_at.type) { return false; }
                }
            }
            return true;
        }

        void removeArea(Vector2 top_left, Vector2 bottom_right)
        {
            for (int i = (int)top_left.x; i <= bottom_right.x; ++i) {
                for (int j = (int)top_left.y; j <= bottom_right.y; ++j) {
                    pieceAt(new Vector2(i, j)).remove = true;
                }
            }
        }

        bool findMerges()
        {
            for (int m = 0; m < piece_group.entities.Count; ++m) {
                if (!(piece_group.entities[m] is PuzzlePiece)) { continue; }

                PuzzlePiece p = (PuzzlePiece)piece_group.entities[m];

                if (p.GridSize() == Vector2.one) {
                    var pos = p.GridPosition();

                    for (int i = -1; i <= 0; ++i) {
                        for (int j = -1; j <= 0; ++j) {
                            if (areaContiguous(pos + new Vector2(i, j), pos +
                                               new Vector2(i, j) + Vector2.one)) {

                                removeArea(pos + new Vector2(i, j), pos + new Vector2 (i, j) + Vector2.one);
                                p.remove = false;
                                p.SetGridPosition(pos + new Vector2 (i, j));
                                p.SetGridSize(new Vector2(2, 2));

                                if (!Game.instance.house.isDoorClosed && AppMain.tutorial.MakeBlocks) {
                                    AppMain.tutorial.blocksCreated++;
                                    if (AppMain.tutorial.blocksCreated > AppMain.tutorial.repeatAmount) {
                                        AppMain.tutorial.SetLesson(Lesson.LOAD_GUNS);
                                    }
                                }
                                Objectives.CreatedBlockOfSize(2, 2);

                                if (Game.instance.enemy_target_piece != null &&
                                    Game.instance.enemy_target_piece.remove) {
                                    Game.instance.enemy_target_offset = new Vector2(
                                        (Game.instance.enemy_target_piece.position.x +
                                                     Game.instance.enemy_target_offset.x - p.position.x) / piece_size,
                                        (Game.instance.enemy_target_piece.position.x +
                                                     Game.instance.enemy_target_offset.x - p.position.x) / piece_size);
                                }

                                return true;
                            }
                        }
                    }
                }
                else {
                    var pos = p.GridPosition();
                    var size = p.GridSize();

                    Vector2 new_position = new Vector2(-1, -1),
                    new_size = new Vector2(-1, -1);

                    if (areaContiguous(pos + new Vector2(-1, 0), pos + size - Vector2.one)) {
                        new_position = pos + new Vector2(-1, 0);
                        new_size = size + new Vector2(1, 0);
                    }

                    if (areaContiguous(pos, pos + size + new Vector2 (1, 0) - Vector2.one)) {
                        new_position = pos;
                        new_size = size + new Vector2(1, 0);
                    }

                    for (int y = 1; y < rows; ++y) {
                        if (areaContiguous(pos + new Vector2(0, -y), pos + size - Vector2.one)) {
                            new_position = pos + new Vector2(0, -y);
                            new_size = size + new Vector2(0, y);
                        }

                        if (areaContiguous(pos, pos + size + new Vector2 (0, y) - Vector2.one)) {
                            new_position = pos;
                            new_size = size + new Vector2(0, y);
                        }
                    }

                    if (new_position.x != -1 || new_size.x != -1) {
                        removeArea(new_position, new_position + new_size - Vector2.one);
                        p.remove = false;
                        p.SetGridPosition(new_position);
                        p.SetGridSize(new_size);

                        if (Game.instance.enemy_target_piece != null && Game.instance.enemy_target_piece.remove) {
                            Game.instance.enemy_target_offset = (Game.instance.enemy_target_piece.position +
                                                                 Game.instance.enemy_target_offset - p.position) / piece_size;
                            Game.instance.enemy_target_piece = p;
                        }

                        Objectives.CreatedBlockOfSize(new_size.x, new_size.y);

                        return true;
                    }
                }
            }
            return false;
        }

        public void unSettle()
        {
            settling = true;

            for (int i = 0; i < piece_group.entities.Count; ++i) {
                if (!(piece_group.entities[i] is PuzzlePiece)) { continue; }

                ((PuzzlePiece)piece_group.entities[i]).settled = false;
            }
        }

        void fillGaps()
        {
            for (int i = 0; i < piece_group.entities.Count; ++i) {
                if (!(piece_group.entities[i] is PuzzlePiece)) { continue; }

                if (pieceAt(((PuzzlePiece)piece_group.entities[i]).GridPosition()) != piece_group.entities[i]) {
                    piece_group.entities[i].remove = true;
                }
            }

            piece_group.flushAddRemove();

            int[] added = { 0, 0, 0 };

            for (int y = 0; y < rows; ++y) {
                for (int x = 0; x < columns; ++x) {
                    added[x]++;
                    PuzzlePiece p = new PuzzlePiece(nextPiece(),
                                                    new Vector2(x * piece_size, -70 - piece_size * added[x] * 1.1f));

                    piece_group.add(p);
                }
            }

            piece_group.flushAddRemove();
        }

        void clearPadInputState()
        {
            selected_row = -1;
            shift_so_far = 0;

            for (int i = 0; i < piece_group.entities.Count; ++i) {
                PuzzlePiece p = piece_group.entities[i] as PuzzlePiece;
                if (p == null) continue;
                p.selected = false;
            }
        }

        void clearTouchInputState() { }

        void handlePadInput()
        {
            if (Game.instance.house.visibleDoorPosition() >= 1) {
                handlePadWeaponActivation();
                shift_so_far = 0;
                return;
            }

            Vector2 direction = Util.keyRepeat("puzzle_pad_repeat", Vector2.zero, 10, 5, Input.Pad.Move);

            if (direction.y != 0) {
                shift_so_far = 0;

                PuzzlePiece p = pieceAt(new Vector2(0, selected_row));
                if (p != null && p.GridSize().x == 3) {
                    while (pieceAt(new Vector2(0, selected_row + direction.y)) == p) {
                        if (direction.y > 0) direction.y++;
                        else direction.y--;
                    }
                }

                selected_row += (int)direction.y;
                if (selected_row < 0) selected_row = 0;
                if (selected_row > 5) selected_row = 5;
            }

            while (selected_row < 5 && (selected_row + 1) / 6.0f < Game.instance.house.door_position) {
                PuzzlePiece piece = pieceAt(new Vector2(shift_so_far > 0 ? 0 : 2, selected_row));
                if (piece != null) {
                    if (selected_row + 1 >= piece.GridPosition().y + (piece.size.y / Puzzle.piece_size) ||
                        (piece.size.y / Puzzle.piece_size) == 1.0f) {
                        shift_so_far = 0;
                    }
                }
                else {
                    shift_so_far = 0;
                }

                selected_row++;
            }

            float mx = Input.Pad.Move.x;
            if (Mathf.Abs(mx) < 0.5) { mx = 0; }
            int rx = Util.keyRepeat("block_shift_repeat", 0, 20, 10, Util.sign(mx));

            if (rx == -1) {
                if (shift_so_far > -192) shift_so_far -= 64; //Puzzle.stick_shift_speed;
            }
            else if (rx == 1) {
                if (shift_so_far < 192) shift_so_far += 64; //Puzzle.stick_shift_speed;
            }
            //else if (Mathf.Abs(shift_so_far) < 32)
            //{
            //    shift_so_far = 0;
            //}
            //else {
            if (Input.Pad.Submit.WasPressed) {
                // apply finalized move to piece grid
                Touch t = new Touch();
                t.position = new Vector2(grid_left + 32, grid_top + 64 * selected_row + 32);
                t.initial_position = t.position;
                t.last_position = t.position;
                t.really_initial_position = t.position;
                t.status = Touch.Status.DOWN;

                handleGrab(ref t);

                // don't need to involve handleMove here after all
                if (t.both_groups != null) {
                    t.position.x -= shift_so_far;
                    for (int i = 0; i < t.both_groups.Count; ++i) {
                        t.both_groups[i].position.x -= t.both_groups[i].visual_shift;
                    }
                }

                t.status = Touch.Status.UP;

                handleRelease(ref t);

                shift_so_far = 0;
            }

            // determine which pieces are being manipulated
            Touch ft = new Touch();
            ft.position = new Vector2(grid_left + 32, grid_top + 64 * selected_row + 32);
            handleGrab(ref ft);

            // reset piece shift and selection status
            for (int i = 0; i < piece_group.entities.Count; ++i) {
                PuzzlePiece p = piece_group.entities[i] as PuzzlePiece;
                if (p == null) continue;
                p.selected = false;
                p.visual_shift = 0;
            }

            // determine which pieces are selected
            for (int x = 0; x < 3; x++) {
                PuzzlePiece p = pieceAt(new Vector2(x, selected_row));
                if (p == null) continue;
                if (ft.left_group != null && ft.left_group .Contains(p) && shift_so_far > 0 ||
                    ft.right_group != null && ft.right_group.Contains(p) && shift_so_far < 0 ||
                    shift_so_far == 0) {
                    p.selected = true;
                }
            }

            // apply visual shift to moved pieces
            if (ft.left_group != null && ft.left_group .Count == 1 && ft.left_group[0].GridSize().x == 1 &&
                ft.right_group != null && ft.right_group.Count == 1 && ft.right_group[0].GridSize().x == 1) {
                if (shift_so_far < 0) ft.left_group[0].visual_shift = Mathf.FloorToInt(shift_so_far);
                else ft.right_group[0].visual_shift = Mathf.FloorToInt(shift_so_far);

                if (Mathf.Abs(shift_so_far) > 64) {
                    if(shift_so_far < 0) ft.right_group[0].visual_shift = Mathf.FloorToInt(shift_so_far + 64);
                    else ft.left_group[0].visual_shift = Mathf.FloorToInt(shift_so_far - 64);
                }
            }
            else {
                for (int x = 0; x < 3; x++) {
                    PuzzlePiece p = pieceAt(new Vector2(x, selected_row));
                    if (p == null) continue;
                    if (p.selected) p.visual_shift = Mathf.FloorToInt(shift_so_far);
                }
            }
        }

        void handleTouchInput()
        {
            bool block_move_touch_exists = false;

            for (int i = 0; i < Input.touches.Count; ++i) {
                Touch touchPosition = Input.touches[i];

                if (touchPosition.killed) { return; }

                if (kill_grab) {
                    if (touchPosition.left_group != null) {
                        for (int n = 0; i < touchPosition.left_group.Count; ++n) {
                            touchPosition.left_group[n].position = touchPosition.left_group[n].settled_position;
                        }
                    }

                    if (touchPosition.right_group != null) {
                        for (int n = 0; i < touchPosition.right_group.Count; ++n) {
                            touchPosition.right_group[n].position = touchPosition.right_group[n].settled_position;
                        }
                    }

                    touchPosition.killed = true;
                    kill_grab = false;
                }
                else {
                    if (touchPosition.status == Touch.Status.DOWN) {
                        if (Game.instance.house.visibleDoorPosition() >= 1)
                            fireWeapons(ref touchPosition);
                        else handleGrab(ref touchPosition);
                    }
                    if (touchPosition.status == Touch.Status.MOVE) { handleMove(ref touchPosition); }
                    if (touchPosition.status == Touch.Status.UP) {
                        handleRelease(ref touchPosition);
                        current_grab.left_group = null;
                    }

                    if (touchPosition.left_group != null) {
                        if (block_move_touch_exists) { touchPosition.killed = true; }
                        block_move_touch_exists = true;
                    }
                }

                current_grab = touchPosition;
                Input.touches[i] = touchPosition;
            }
        }

        void handleGrab(ref Touch t)
        {
            prev_x = (int)t.position.x;
            swiped = false;

            int column = (int)((t.position.x - grid_top) / piece_size);
            int row = (int)((t.position.y - grid_top) / piece_size);

            /* NOTE(shane): added 1 to columns to import ability to select blocks.
                this also a lot of magic numbers to control touch this is the quickest solution. */
            if (column < -1 || column >= (columns + 2) || row < 0 || row >= rows) { return; }

            if ((row + 1) / 6.0 < Game.instance.house.visibleDoorPosition()) { return; }

            /* NOTE(shane): I had disabled this to stop touches being ignored. */
            if (settling) { return; }  // check for settling before allowing a slide

            PuzzlePiece leftmost = null, rightmost = null;      // find leftmost and rightmost blocks in row

            for (int x = 0; x < columns; ++x) {
                PuzzlePiece p = pieceAt(new Vector2(x, row));

                if (p == null) {  continue; }

                p.selected = true;
                rightmost = p;

                if (leftmost == null) { leftmost = p; }
            }

            if (leftmost == null || rightmost == null) { return; }

            // add missing leftmost or rightmost piece from other rows if appropriate
            // if there is only one piece of 2x2 or greater
            if (rightmost == leftmost && rightmost.GridSize().y > 1) {
                var replace_left = true;
                var col = rightmost.GridPosition().x - 1;

                if (col == -1) {
                    col = 2;
                    replace_left = false;
                }

                for (int r = (int)rightmost.GridPosition().y; r <
                     rightmost.GridPosition().y + rightmost.GridSize().y; ++r) {
                    PuzzlePiece p = pieceAt(new Vector2(col, r));

                    if (p != null) {
                        if (replace_left) { leftmost = p; }
                        else { rightmost = p; }

                        break;
                    }
                }
            }

            t.right_group = new List<PuzzlePiece>();
            t.left_group = new List<PuzzlePiece>();
            t.both_groups = new List<PuzzlePiece>();

            t.right_group.Add(rightmost);
            t.left_group.Add(leftmost);

            // add pieces to groups
            if (leftmost.GridSize().y == 1) {
                for (int y = (int)leftmost.GridPosition().y;  y < leftmost.GridPosition().y + leftmost.GridSize().y; ++y) {
                    for (int x = (int)leftmost.GridPosition().x; x < columns; x++) {
                        PuzzlePiece piece = pieceAt(new Vector2 (x, y));

                        if (piece == null) { break; }
                        if (piece.GridSize() != Vector2.one) { break; }

                        if (!t.left_group.Contains(piece)) { t.left_group.Add(piece); }
                    }
                }
            }

            if (rightmost.GridSize ().y == 1) {
                for (int y = (int)rightmost.GridPosition().y; y < rightmost.GridPosition().y + rightmost.GridSize().y; ++y) {
                    for (int x = (int)rightmost.GridPosition().x; x >= 0; --x) {
                        PuzzlePiece piece = pieceAt(new Vector2(x, y));

                        if (piece == null) { break; }
                        if (piece.GridSize() != Vector2.one) { break; }

                        if (!t.right_group.Contains(piece)) { t.right_group.Add(piece); }
                    }
                }
            }

            t.both_groups.AddRange(t.left_group);

            for (int i = 0; i < t.right_group.Count; ++i) {
                if (!t.both_groups.Contains(t.right_group[i])) {
                    t.both_groups.Add(t.right_group[i]);
                }
            }
        }

        void handleMove(ref Touch t, bool ignore_swipe = false)
        {
            if (t.left_group == null) { return; }

            int accel = Math.Abs((int)t.position.x - prev_x);
            int move = (int)(t.position.x - t.initial_position.x);
            int move_f = Math.Abs(move) % Puzzle.piece_size;
            int move_d = 1 + (Math.Abs(move) / Puzzle.piece_size);

            if (Math.Abs(move_f) > (Puzzle.piece_size * 0.5f) && Math.Abs(move_f) < Puzzle.piece_size) {
                move = Math.Sign(move) * move_d * Puzzle.piece_size;
            }

            if (!ignore_swipe) {
                if (accel > Puzzle.piece_size) { swiped = true; }
                if (swiped) { move = Math.Sign(move) * 4 * Puzzle.piece_size; }
            }

            prev_x = (int)t.position.x;

            if (move == 0) { return; }
            move = Mathf.Clamp(move, -192, 192);

            for (int i = 0; i < t.both_groups.Count; ++i) {
                t.both_groups[i].position.x = t.both_groups[i].settled_position.x;
            }

            int move_step = 1;
            if (move > 0) {
                for (int i = 0; i < move; i += move_step) {
                    // move large pieces by themselves
                    if (t.left_group[0].GridSize().y > 1 || t.right_group[0].GridSize().y > 1) {
                        moveGroup(t.right_group, move_step);
                        if (groupBlocked(t.right_group, false)) { moveGroup(t.right_group, -move_step); }
                    }
                    else {
                        moveGroup(t.left_group, move_step);
                        if (groupBlocked(t.left_group, true)) {
                            moveGroup(t.left_group, -move_step);
                            moveGroup(t.right_group, move_step);
                            if (groupBlocked(t.right_group, false)) { moveGroup(t.right_group, -move_step); }
                        }
                    }
                }
            }
            else if (move < 0) {
                for (int i = 0; i > move; i -= move_step) {
                    // move large pieces by themselves
                    if (t.left_group[0].GridSize().y > 1 || t.right_group[0].GridSize().y > 1) {
                        moveGroup(t.left_group, -move_step);
                        if (groupBlocked(t.left_group, true)) { moveGroup(t.left_group, move_step); }
                    }
                    else {
                        moveGroup(t.right_group, -move_step);
                        if (groupBlocked(t.right_group, false)) {
                            moveGroup(t.right_group, move_step);
                            moveGroup(t.left_group, -move_step);
                            if (groupBlocked(t.left_group, true)) { moveGroup(t.left_group, move_step); }
                        }
                    }
                }
            }

            // merge two separate sametyped blocks when next to one another
            if (t.left_group[0] != t.right_group[0] &&
                t.left_group[0].GridSize().y == 1 && t.right_group [0].GridSize().y == 1 &&
                t.left_group.Count == 1 && t.right_group.Count == 1 &&
                t.left_group[0].type == t.right_group[0].type &&
                (t.left_group[0].position - t.right_group[0].position).magnitude <= Puzzle.piece_size + 1) {
                t.right_group[0].settled_position.x = t.left_group[0].settled_position.x + Puzzle.piece_size;
                t.right_group[0].position.x = t.left_group [0].position.x + Puzzle.piece_size;
                t.left_group.Add(t.right_group[0]);
                t.right_group = t.left_group;
                if (move < 0) { t.initial_position = t.position; }
            }
        }

        void handleRelease(ref Touch t)
        {
            swiped = false;

            if (t.left_group == null) { return; }

            Gun.Ammo feed_type = 0;
            int[] gun_feed_amt = { 0, 0, 0 };
            bool loading_specials = false;

            for (int i = 0; i < t.both_groups.Count; ++i) {
                t.both_groups[i].selected = false;
                Vector2 pos = t.both_groups[i].GridPosition();
                Vector2 gridsize = t.both_groups[i].GridSize();
                t.both_groups[i].SetGridPosition(pos);

                if (pos.x+(gridsize.x-1) >= columns || pos.x < 0) {
                    int grid_y = (int)t.both_groups[i].GridPosition().y;

                    if (pos.x >= columns) {
                        loading_specials = false;
                        Objectives.LoadGun(gridsize.x, gridsize.y);
                    }

                    if (pos.x < 0) {
                        loading_specials = true;
                        Objectives.LoadSpecial(gridsize.x, gridsize.y);
                    }

                    // only support blocks of 2x2 or higher
                    if (gridsize.y >= 2) {
                        for (int y = 0; y < gridsize.y; ++y) {
                            gun_feed_amt[(y + grid_y) / 2] += (int)gridsize.x;
                        }
                        recordStats(gridsize, t.both_groups[i].type);
                    }

                    feed_type = t.both_groups[i].type;

                    t.both_groups[i].remove = true;
                }
            }

            float multiplier = 1.0f;
            int amt = 0;
            for (int i = 0; i < 3; i++) { amt += gun_feed_amt[i]; }

            if (feed_type == Game.instance.current_bonus && amt > 3) {
                multiplier = current_match_bonus;
                Choom.PlayEffect(SoundAssets.BlockMatch);

                if (AppMain.tutorial.DoorOpen &&
                    AppMain.tutorial.loadMultiplierAmount >= AppMain.tutorial.repeatAmount) {
                    AppMain.tutorial.SetLesson(Lesson.GOODWILL);
                }
                else if (AppMain.tutorial.DoorOpen) {
                    AppMain.tutorial.loadMultiplierAmount++;
                }

                spawnFloatyText("Match! x" + Game.instance.match_combo,
                                new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3 + 130, 42.5f), 1.5f, Vector4.one);
                Game.instance.match_combo++;
                Game.instance.match_streak++;
                Game.instance.house.advanceNext();

                Objectives.LoadBonusElement(true);
            }
            else if (amt > 3) {
                Game.instance.match_combo = 1;
                Game.instance.match_streak = 0;
                Choom.PlayEffect(SoundAssets.EnableGun);

                Objectives.LoadBonusElement(false);
            }

            if (Game.instance.match_streak > DataStorage.MatchStreak) {
                DataStorage.MatchStreak = Game.instance.match_streak;
            }

            bool lightninged = false;
            bool recordOnce = false;

            for (int i = 0; i < 3; ++i) {
                if (gun_feed_amt[i] > 0) {
                    amt = gun_feed_amt[i];
                    if (amt <= 1) { continue; }

                    var splash_position = Vector2.zero;
                    amt = (int)(amt * multiplier);

                    if (loading_specials) {
                        Game.instance.house.addSpecialAttack(i, feed_type, amt);
                        splash_position = new Vector2 (Puzzle.grid_left, Puzzle.grid_top + (i * 2 + 1) * Puzzle.piece_size);

                        if (!recordOnce && AppMain.tutorial.LoadedSpecial &&
                            AppMain.tutorial.loadedSpecialAmount >= AppMain.tutorial.repeatAmount) {
                            AppMain.tutorial.SetLesson(Lesson.COUNTDOWN);
                        }
                        else if (!recordOnce && AppMain.tutorial.LoadedSpecial) {
                            AppMain.tutorial.loadedSpecialAmount++;
                            recordOnce = true;
                        }
                    }
                    else {
                        Gun g = Game.instance.gun_group.entities[i] as Gun;
                        splash_position = g.position;
                        g.feed(feed_type, amt);

                        if (!recordOnce && AppMain.tutorial.LoadedGun &&
                            AppMain.tutorial.loadedGunAmount >= AppMain.tutorial.repeatAmount) {
                            AppMain.tutorial.SetLesson(Lesson.LOAD_SPECIALS);
                        }
                        else if (!recordOnce && AppMain.tutorial.LoadedGun) {
                            AppMain.tutorial.loadedGunAmount++;
                            recordOnce = true;
                        }
                    }

                    if (multiplier > 1) {
                        Game.instance.particle_group.add(new LightningBetween(
                            new Vector2(Puzzle.grid_left + Puzzle.piece_size * 3 + 140 - 70, 42.5f),
                            splash_position, lightninged));
                    }

                    lightninged = true;
                    spawnFlash(splash_position, ammoColor(feed_type), 0.25f + 0.1f * amt);

                    spawnFloatyText("+" + gun_feed_amt[i].ToString() + (multiplier == 1 ? "" : "x" + multiplier.ToString()),
                                    splash_position, 1 + 1.0f * (float)Math.Log(amt), ammoColor(feed_type));
                }
            }

            // if a big piece snaps to only partly off the grid, snap it back on
            int move = (int)(t.position.x - t.initial_position.x);

            if (move < 0) {
                PuzzlePiece leftmost = t.left_group[0];

                if (leftmost.GridPosition().x < 0 && leftmost.GridPosition().x + leftmost.GridSize().x - 1 >= 0) {
                    int snap_by = -(int)leftmost.position.x;

                    for (int i = 0; i < t.left_group.Count; ++i) {
                        t.left_group[i].position.x += snap_by;
                    }
                }
            }
            else if (move > 0) {
                PuzzlePiece rightmost = t.right_group [0];

                if (rightmost.GridPosition().x < columns &&
                    rightmost.GridPosition().x + rightmost.GridSize().x - 1 >= columns) {
                    int snap_by = columns * piece_size - (int)rightmost.position.x - (int)rightmost.size.x;

                    for (int i = 0; i < t.right_group.Count; ++i) {
                        t.right_group[i].position.x += snap_by;
                    }
                }
            }

            t.left_group = null;
            t.right_group = null;
            t.both_groups = null;

            if ((t.really_initial_position - t.position).magnitude > 30) {
                fillGaps();
                unSettle();
            }
        }

        void moveGroup(List<PuzzlePiece> group, int by)
        {
            for (int i = 0; i < group.Count; ++i) { group[i].position.x += by; }
        }

        bool groupBlocked(List<PuzzlePiece> group, bool left_group)
        {
            for (int i = 0; i < group.Count; ++i) {
                if (piece_group.findCollision(group[i])) { return true; }
                if (left_group && group[i].position.x + group[i].size.x >= columns * piece_size) { return true; }
                if (!left_group && group[i].position.x < 0) { return true; }
            }

            return false;
        }

        void fireWeapons(ref Touch t)
        {
            // fire guns
            if (t.position.x > (grid_left + piece_size * 3) && t.position.x < (grid_left + piece_size * 6)) {
                if (Game.instance.house.visibleDoorPosition() >= 1) {
                    int y = House.gunPositionFromY(t.position.y);
                    Gun g = (Gun)Game.instance.gun_group.entities[y];

                    if (AppMain.tutorial.ActivatedGun && AppMain.tutorial.isReady) {
                        AppMain.tutorial.SetLesson(Lesson.ACTIVATE_SPECIALS);
                    }

                    g.startFiring();

                    Objectives.FireGun(g.ammo);
                }

                t.killed = true;
            }

            /* just for tutorial, ignored otherwise */
            if (AppMain.tutorial.ActivatedGun || !AppMain.tutorial.isReady) { return; }

            // fire specials
            if (t.position.x < grid_left) {
                Game.instance.house.specialAttackAt(t.position);

                if (AppMain.tutorial.ActivatedSpecial) {
                    AppMain.tutorial.activateSpecialFinished = true;
                }

                t.killed = true;
            }
        }

        public bool rowEmpty(int row)
        {
            if (row < 0 || row > 2) return true;
            return (Game.instance.gun_group.entities[row] as Gun).ammo == Gun.Ammo.GATLING &&
                                                                 Game.instance.house.special_attacks[row].Item1 == Gun.Ammo.NONE;
        }

        void handlePadWeaponActivation()
        {
            Vector2 dir = Util.keyRepeat("weapon_pad_repeat", Vector2.zero, 20, 10, Input.Pad.Move);
            selected_weapon.x -= Util.sign(dir.x);
            selected_weapon.y += Util.sign(dir.y);
            selected_weapon.x = Util.clamp(selected_weapon.x, 0, 1);
            selected_weapon.y = Util.clamp(selected_weapon.y, 0, 2);

            if (Input.Pad.Submit.WasPressed) {
                Touch t = new Touch();
                t.status = Touch.Status.DOWN;

                t.position.x = grid_left - 1 + selected_weapon.x * (1 + piece_size * 4.5f);
                t.position.y = grid_top + piece_size * (selected_weapon.y * 2);

                if (t.position != Vector2.zero) { fireWeapons(ref t); }
            }
        }

        void recordStats(Vector2 size, Gun.Ammo ammo_type)
        {
            int n = 0;
            for (int x = 2; x <= 3; x++) {
                for (int y = 2; y <= 6; y++) {
                    if (size.x == x && size.y == y) { DataStorage.BlocksLoaded[n]++; }
                    n++;
                }
            }

            int ammo = (int)ammo_type;
            if (ammo >= 0 && ammo <= 9) {
                DataStorage.AmmoLoaded[ammo] += Mathf.FloorToInt(size.x * size.y);
            }
        }

        public static void spawnFlash(Vector2 position, Vector4 color, float decay_time = 1.0f)
        {
            Particle flash = new Particle(AppMain.textures.circle);

            flash.position = position;
            flash.color = color;
            flash.scale = Vector2.one;
            flash.grow = Vector2.one / 10;
            flash.color_delta = new Vector4(0, 0, 0, -1.0f / (60 * decay_time));

            Game.instance.particle_manager.add(flash);
        }

        public static void spawnFloatyText(string text, Vector2 position, float size, Vector4 color)
        {
            Particle floaty = new Particle(AppMain.textures.font);

            floaty.position = position;
            floaty.velocity = new Vector2(0, -0.2f);
            floaty.scale = Vector2.one / 2 * size;
            floaty.color = color;
            floaty.color_delta = new Vector4(0, 0, 0, -0.25f / 60);
            floaty.text = text;
            floaty.position.x = Math.Max(floaty.position.x, 24 * floaty.scale.x * text.Length);

            Game.instance.particle_manager.add(floaty);
        }

        public static Vector4 ammoColor(Gun.Ammo type)
        {
            if (type == Gun.Ammo.DRAGON) { return new Vector4 (1.0f, 0.7f, 0.1f, 1); }
            if (type == Gun.Ammo.IGLOO) { return new Vector4 (0.1f, 0.5f, 1.0f, 1); }
            if (type == Gun.Ammo.SKULL) { return new Vector4 (0.8f, 0.2f, 0.7f, 1); }
            if (type == Gun.Ammo.VEGETABLE) { return new Vector4 (0.4f, 0.8f, 0.3f, 1); }
            if (type == Gun.Ammo.LIGHTNING) { return new Vector4 (1.0f, 0.3f, 0.3f, 1); }
            if (type == Gun.Ammo.FLAME) { return new Vector4 (1.0f, 0.7f, 0.1f, 1); }
            if (type == Gun.Ammo.FORK) { return new Vector4 (0.1f, 0.5f, 1.0f, 1); }
            if (type == Gun.Ammo.BOUNCE) { return new Vector4 (0.8f, 0.2f, 0.7f, 1); }
            if (type == Gun.Ammo.BOOMERANG) { return new Vector4 (0.4f, 0.8f, 0.3f, 1); }
            if (type == Gun.Ammo.SIN) { return new Vector4 (1.0f, 0.3f, 0.3f, 1); }
            return Vector4.one;
        }

        public static int ammoElementToSprite(Gun.Ammo a)
        {
            if (a == Gun.Ammo.DRAGON) { return 0; }
            if (a == Gun.Ammo.IGLOO) { return 1; }
            if (a == Gun.Ammo.SKULL) { return 2; }
            if (a == Gun.Ammo.VEGETABLE) { return 3; }
            if (a == Gun.Ammo.LIGHTNING) { return 4; }
            if (a == Gun.Ammo.FLAME) { return 5; }
            if (a == Gun.Ammo.FORK) { return 6; }
            if (a == Gun.Ammo.BOUNCE) { return 7; }
            if (a == Gun.Ammo.BOOMERANG) { return 8; }
            if (a == Gun.Ammo.SIN) { return 9; }
            return -1;
        }

        public static int ammoToBlockSprite(Gun.Ammo a)
        {
            switch (a)
            {
                case Gun.Ammo.DRAGON:
                case Gun.Ammo.FLAME: return (int)block.Sprites.orange;
                case Gun.Ammo.FORK:
                case Gun.Ammo.IGLOO: return (int)block.Sprites.blue;
                case Gun.Ammo.LIGHTNING:
                case Gun.Ammo.SIN: return (int)block.Sprites.red;
                case Gun.Ammo.SKULL:
                case Gun.Ammo.BOUNCE: return (int)block.Sprites.purple;
                case Gun.Ammo.VEGETABLE:
                case Gun.Ammo.BOOMERANG: return (int)block.Sprites.green;
            }

            return -1;
        }

        public static int ammoToLogoSprite(Gun.Ammo a)
        {
            switch (a)
            {
                case Gun.Ammo.DRAGON: return (int)block_logo.Sprites.dragon;
                case Gun.Ammo.FLAME: return (int)block_logo.Sprites.crown;
                case Gun.Ammo.FORK: return (int)block_logo.Sprites.forks;
                case Gun.Ammo.IGLOO: return (int)block_logo.Sprites.igloo;
                case Gun.Ammo.LIGHTNING: return (int)block_logo.Sprites.lightning;
                case Gun.Ammo.SIN: return (int)block_logo.Sprites.sign;
                case Gun.Ammo.SKULL: return (int)block_logo.Sprites.skull;
                case Gun.Ammo.BOUNCE: return (int)block_logo.Sprites.ball;
                case Gun.Ammo.VEGETABLE: return (int)block_logo.Sprites.carrot;
                case Gun.Ammo.BOOMERANG: return (int)block_logo.Sprites.boomerang;
            }

            return -1;
        }
    }
}
