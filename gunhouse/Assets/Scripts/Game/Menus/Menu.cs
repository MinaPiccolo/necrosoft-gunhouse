using Necrosoft;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Gunhouse
{
    public enum MenuOptions { Title, Continue, Hardcore, Shop, Options, Stats };

    public struct MenuOption
    {
        public delegate void Selected();

        public Selected selected;
        public Vector2 position;
        public Vector2 size;
        public string text;
        public Vector2 text_size;
        public float font_scale;
        public Gun.Ammo ammo;
        public ControllerButton button;

        public MenuOption(string text_, Gun.Ammo ammo_, Selected s,
                          ControllerButton button = ControllerButton.NONE, float font_scale_ = 0.48f)
        {
            this.button = button;
            font_scale = font_scale_;
            text = Text.Wrap(text_, (int)(64 * 3 / (24 * font_scale)));
            text_size = Text.Size(AppMain.textures.font, text);
            text_size.x = -text_size.x;
            ammo = ammo_;
            selected = s;
            position = Vector2.zero;
            size = new Vector2(Puzzle.piece_size * 3, Puzzle.piece_size * 2);
        }
    }

    public class MenuHouse : Entity
    {
        public MenuOption[] options = new MenuOption[6];
        public int n_options = 0;
        public int n_options_pos = 0;
        public int held_option = -1;
        public bool ignore_pad = false;
        public bool was_down;

        public override void tick()
        {
            for (int i = 0; i < Input.touches.Count; ++i) {
                if (Input.touches[i].status == Touch.Status.DOWN ||
                    Input.touches[i].status == Touch.Status.MOVE) {
                    Touch t = Input.touches[i];
                    for (int o = 0; o < n_options; ++o) {
                        if (t.position.x > Puzzle.grid_left &&
                            t.position.x < Puzzle.grid_left + Puzzle.piece_size * 3 &&
                            t.position.y > options[o].position.y &&
                            t.position.y < options[o].position.y + options[o].size.y) {
                            held_option = o;
                            was_down = true;
                            return;
                        }
                    }

                    was_down = false;
                    held_option = -1;
                }
                else if (Input.touches[i].status == Touch.Status.UP) {
                    if (was_down && held_option != -1) {
                        if (options[held_option].selected == null) {
                            held_option = -1;
                            was_down = false;
                            return;
                        }

                        options[held_option].selected();
                        Choom.PlayEffect(SoundAssets.UISelect);
                        held_option = -1;
                        was_down = false;
                    }
                }
            }

            if (!ignore_pad) {
                if (Input.Pad.Submit.IsPressed) { was_down = true; }

                if (Input.Pad.Submit.WasPressed) {
                    if (was_down && options[held_option].selected != null) {
                        options[held_option].selected();
                        Choom.PlayEffect(SoundAssets.UISelect);
                        held_option = -1;
                        was_down = false;
                    }
                }

                Vector2 direction = Util.keyRepeat("menu_house", Vector2.zero, 20, 10, Input.Pad.Move);
                if (direction.y < 0) {
                    held_option = Mathf.Clamp(held_option - 1, 0, n_options - 1);

                    if ((AppMain.top_state is TitleState) &&
                        options[held_option].selected == null) {
                        held_option -= 1;
                    }
                }
                if (direction.y > 0) {
                    held_option = Mathf.Clamp(held_option + 1, 0, n_options - 1);
                    if ((AppMain.top_state is TitleState) &&
                        options[held_option].selected == null) {
                        held_option += 1;
                    }
                }
            }
        }

        public override void draw()
        {
            for (int i = 0; i < n_options; ++i) {
                bool is_selected = held_option == i && options[i].selected != null;

                AppMain.textures.block.draw_outline(Puzzle.ammoToBlockSprite(options[i].ammo),
                                                     options[i].position +
                                                     new Vector2(Puzzle.piece_size * 1.5f,
                                                                 options[i].size.y * .5f),
                                                     (options[i].size / 64) * .25f,
                                                     new Vector4(.5f, .5f, .5f, .5f),
                                                     1, is_selected);

                Text.Draw(options[i].position + new Vector2(Puzzle.piece_size * 1.5f, options[i].size.y * .5f) -
                          (options[i].text_size * .5f) * options[i].font_scale +
                          new Vector2(-12, 22) * options[i].font_scale, options[i].text,
                          Vector2.one * options[i].font_scale, Vector4.one);

                draw_button(i);
            }
        }

        public void addOption(MenuOption mo, int size = 2)
        {
            AppMain.textures.font.touch();
            options[n_options] = mo;
            options[n_options].position = new Vector2(Puzzle.grid_left, Puzzle.grid_top + n_options_pos);
            options[n_options].size = new Vector2(Puzzle.piece_size * 3, Puzzle.piece_size * size);
            n_options_pos += Puzzle.piece_size * size;
            n_options++;
        }

        void draw_button(int index)
        {
            if (options[index].button == ControllerButton.NONE) { return; }

            int button_sprite = -1;
            switch (options[index].button)
            {
            case ControllerButton.PS_X: button_sprite = (int)store.Sprites.ps_x; break;
            case ControllerButton.PS_CIRCLE: button_sprite = (int)store.Sprites.ps_circle; break;
            case ControllerButton.PS_TRIANGLE: button_sprite = (int)store.Sprites.ps_triangle; break;
            case ControllerButton.PS_SQUARE: button_sprite = (int)store.Sprites.ps_square; break;
            }

            Vector2 buttonPosition = options[index].position +
                                     new Vector2(Puzzle.piece_size * 1.5f, options[index].size.y * .5f) -
                                     (options[index].text_size * .5f) * options[index].font_scale +
                                     new Vector2(-30, 20) * options[index].font_scale;

            AppMain.textures.store.draw(button_sprite, buttonPosition, Vector2.one * 0.64f, Vector4.one);
        }
    }

    public class TitleState : State
    {
        public MenuHouse menu_house = new MenuHouse();
        bool delayDisplay;

        public TitleState(MenuOptions selected_item)
        {
            Tracker.ScreenVisit(SCREEN_NAME.MAIN_MENU);

            AppMain.menuAchievements.DisplayButtons(AppMain.background_fade);
            MetaState.hardcore_mode = false;

            string name = DataStorage.StartOnWave == 0 ? MenuText.PlayGunhouse :
                            MenuText.ContinueOn + Game.dayName(DataStorage.StartOnWave);

            menu_house.addOption(new MenuOption(name, Gun.Ammo.DRAGON, () => {
                #if TRACKING
                string[] equippedNames = new string[Shop.max_equipped];
                int equip_index = 0;
                for (int i = 0; i < DataStorage.NumberOfGuns; ++i) {
                    if (!DataStorage.GunEquipped[i]) { continue; }
                    equippedNames[equip_index++] = Shop.guns[i].name;
                }
                Tracker.StartMode(Game.dayName(MetaState.wave_number), equippedNames, DataStorage.Money);
                #endif

                AppMain.menuAchievements.HideButtons();
                AppMain.top_state.Dispose();
#if LOADING_SCREEN
                AppMain.top_state = new LoadState(() => {
                    MetaState.reset();
                    MetaState.reset(DataStorage.StartOnWave);
                    return new Game();
                }, DataStorage.StartOnWave / 3 % 5);
#else
                MetaState.reset();
                MetaState.reset(DataStorage.StartOnWave);
                AppMain.top_state = new Game();
#endif
            }), 1);

            if (DataStorage.StartOnWave == 0) {
                menu_house.addOption(new MenuOption("", Gun.Ammo.IGLOO, null), 1);
            }
            else {
                menu_house.addOption(new MenuOption(MenuText.StartOnEarlierDay,
                                     Gun.Ammo.IGLOO, () => {
                    AppMain.menuAchievements.HideButtons();
                    AppMain.top_state.Dispose();
                    AppMain.top_state = new PickDayState();
                }), 1);
            }

            menu_house.addOption(new MenuOption(MenuText.HardcoreMode, Gun.Ammo.VEGETABLE, () => {
                AppMain.menuAchievements.HideButtons();
                AppMain.top_state.Dispose();
#if LOADING_SCREEN
                AppMain.top_state = new LoadState(() => {
                    MetaState.hardcore_mode = true;
                    MetaState.hardcore_score = 0;
                    MetaState.reset(0);
                    return new Game();
                }, DataStorage.StartOnWave / 3 % 5);
#else
                MetaState.hardcore_mode = true;
                MetaState.hardcore_score = 0;
                MetaState.reset(0);
                AppMain.top_state = new Game();
#endif
            }), 1);

            menu_house.addOption(new MenuOption(MenuText.Shop, Gun.Ammo.SKULL, () => {
                AppMain.menuAchievements.HideButtons();
                AppMain.top_state.Dispose();
                AppMain.top_state = new Shop();
            }), 1);

            menu_house.addOption(new MenuOption(MenuText.Options, Gun.Ammo.DRAGON, () => {
                AppMain.menuAchievements.HideButtons();
                AppMain.top_state.Dispose();
                AppMain.top_state = new Options();
            }), 1);

            menu_house.addOption(new MenuOption(MenuText.Stats, Gun.Ammo.LIGHTNING, () => {
                AppMain.menuAchievements.HideButtons();
                AppMain.top_state.Dispose();
                AppMain.top_state = new StatsState();
            }), 1);

            menu_house.held_option = (int)selected_item;
        }

        public bool first_tick = true;

        public override void tick()
        {
            if (first_tick) {
                Choom.Play("Music/title");
                Game.instance = null;
                first_tick = false;
            }

            base.tick();
            menu_house.tick();
            MoneyGuy.me.tick();

            if (AppMain.back) {
                AppMain.back = false;
                QuitMenu quit = new QuitMenu();
                AppMain.top_state.Dispose();
                AppMain.top_state = quit;
            }

            #if UNITY_EDITOR

            int key = Util.keyRepeat("level select", 0, 20, 5, Input.last_key);

            if (key == (int)KeyCode.I) {
                DataStorage.StartOnWave++;
                MetaState.setCoefficients(DataStorage.StartOnWave);
                AppMain.top_state.Dispose();
                AppMain.top_state = new TitleState(MenuOptions.Title);
            }

            if (key == (int)KeyCode.K) {
                DataStorage.StartOnWave--;
                if (DataStorage.StartOnWave < 0) DataStorage.StartOnWave = 0;
                MetaState.setCoefficients(DataStorage.StartOnWave);
                AppMain.top_state.Dispose();
                AppMain.top_state = new TitleState(MenuOptions.Title);
            }

            if (key == (int)KeyCode.J) {
                MoneyGuy.me.addMoney(-100000);
            }

            if (key == (int)KeyCode.L) {
                MoneyGuy.me.addMoney(100000);
            }

            #endif
        }

        public override void draw()
        {
            base.draw();

            MoneyGuy.me.draw();

            AppMain.textures.title.draw(0, new Vector2 (title_position, 544 / 2 - 65 + (float)Math.Sin(AppMain.frame / 50.0f) * 5),
                                        new Vector2 (-1, 1), Vector4.one);

            AppMain.textures.house.draw(0, new Vector2(Puzzle.grid_left + 95, Puzzle.grid_top + 144), Vector2.one, Vector4.one);

            menu_house.draw();
        }
    }

    public class Options : State
    {
        public MenuHouse menu_house = new MenuHouse();
        int selected = 1;

        public float wheel_angle = -6.749995f;
        public int shop_position = 620;
        public string cat_anim = "idle";
        public int cat_frame;

        public Options()
        {
            Tracker.ScreenVisit(SCREEN_NAME.OPTIONS);
            menu_house = new MenuHouse();
            menu_house.ignore_pad = true;
            resetMenu();
        }

        public void resetMenu()
        {
            menu_house.addOption(new MenuOption(MenuText.WatchCredits,
                                                Gun.Ammo.DRAGON, () => {
                MoneyGuy.me.addMoney(100);
                AppMain.top_state.Dispose();
                AppMain.top_state = new CreditState();
            }, Input.Pad.Alt_Button));
            menu_house.addOption(new MenuOption("", Gun.Ammo.SKULL, null));
            menu_house.addOption(new MenuOption(MenuText.ReturnToTile, Gun.Ammo.VEGETABLE,
            () => {
                Platform.SaveOptions();
                AppMain.top_state.Dispose();
                AppMain.top_state = new TitleState(MenuOptions.Options);
            }, Input.Pad.Cancel_Button));
        }

        bool first_tick = true;

        public override void tick()
        {
            if (first_tick)
            {
                Game.instance = null;
                first_tick = false;
            }

            menu_house.tick();

            cat_frame += 10;
            if (cat_frame > 500) {
                cat_frame = 0;
                switch (Util.rng.Next (4))
                {
                case 0: cat_anim = "idle"; break;
                case 1: cat_anim = "blink"; break;
                case 2: cat_anim = "paw"; break;
                case 3: cat_anim = "tail"; break;
                }
            }

            MoneyGuy.me.tick();

            if (AppMain.back) {
                AppMain.back = false;
                AppMain.top_state.Dispose();
                AppMain.top_state = new TitleState(MenuOptions.Options);
            }

            Vector2 directions = Util.keyRepeat("options_pad_repeat", Vector2.zero, 20, 5, Input.Pad.Move);
            if (directions.y < 0) selected = 1;
            if (directions.y > 0) selected = 0;
            if (directions.x != 0) {
                if (selected == 0) { Choom.MusicVolume  += Util.sign(directions.x) * 0.05f; }
                if (selected == 1) { Choom.EffectVolume += Util.sign(directions.x) * 0.05f; }
                Choom.MusicVolume = Util.clamp(Choom.MusicVolume, 0, 1);
                Choom.EffectVolume = Util.clamp(Choom.EffectVolume, 0, 1);
            }

            if (Input.Pad.Y.WasPressed && menu_house.options[0].selected != null) { menu_house.options[0].selected(); }
            if (Input.Pad.Cancel.WasPressed && menu_house.options[2].selected != null) { menu_house.options[2].selected(); }

            for (int t = 0; t < Input.touches.Count; ++t) {
                Vector2 pos = Input.touches[t].position;
                float x_pos;
                if (pos.y > 110 && pos.y < 150) { selected = 1; }
                if (pos.y > 170 && pos.y < 210) { selected = 0; }
                if (pos.x > 350 && pos.x < 765) {
                    x_pos = 1-Util.clamp((pos.x-390)/(735f-390), 0, 1);
                    if(selected == 0) Choom.MusicVolume  = x_pos;
                    if(selected == 1) Choom.EffectVolume = x_pos;
                }

                if (Input.touches[t].status == Touch.Status.UP) { selected = -1; }
            }
        }

        public override void draw()
        {
            base.draw();

            MoneyGuy.me.draw();

            AppMain.textures.house.draw(0, new Vector2(Puzzle.grid_left + 95, Puzzle.grid_top + 144), Vector2.one, Vector4.one);

            AppMain.textures.store_board.draw(0, new Vector2(shop_position, 255), new Vector2(-1, 1), Vector4.one);
            AppMain.textures.cat.draw(cat_anim, cat_frame, new Vector2(shop_position + 250, 50), new Vector2(-1, 1), 0, Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.wheel, new Vector2(shop_position + 250, 475), Vector2.one, wheel_angle, Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.wheel, new Vector2(shop_position - 250, 475), Vector2.one, wheel_angle + 1, Vector4.one);

            menu_house.draw();

            Text.Draw(new Vector2(880, 130), "SOUND:", Vector2.one * 2 / 3, Vector4.one);
            Text.Draw(new Vector2(880, 190), "MUSIC:", Vector2.one * 2 / 3, Vector4.one);

            AppMain.textures.store.draw((int)store.Sprites.slider_blue,
                                                new Vector2(570.0f, 130.0f), new Vector2(0.5f, 0.5f), Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.slider_blue,
                                                new Vector2(570.0f, 190.0f), new Vector2(0.5f, 0.5f), Vector4.one);

            float pulse = (float)(1.5f + Math.Sin(AppMain.frame / 10.0f) / 4);
            AppMain.textures.store.draw((int)store.Sprites.skull,
                                                new Vector2(735 - ((Choom.MusicVolume * 4) * 84), 190.0f),
                                                new Vector2(0.5f, 0.5f) * ((selected == 0) ? pulse : 1), Vector4.one);

            AppMain.textures.store.draw((int)store.Sprites.skull,
                                                new Vector2 (735 - ((Choom.EffectVolume * 4) * 84), 130.0f),
                                                new Vector2(0.5f, 0.5f) * ((selected == 1) ? pulse : 1), Vector4.one);

            Text.Draw(new Vector2(880, 280), "About:", Vector2.one * 2 / 3, Vector4.one);
            Text.Draw(new Vector2(880, 320), "Gunhouse", Vector2.one * 2 / 3, Vector4.one);
            Text.Draw(new Vector2(880, 342), "Copyright Necrosoft Games 2017", Vector2.one * 1 / 3, Vector4.one);
            Text.Draw(new Vector2(880, 362), "Version: " + Application.version, Vector2.one * 1 / 3, Vector4.one);
            Text.Draw(new Vector2(880, 382), "support/contact: feedback@necrosoftgames.com", Vector2.one * 1 / 3, Vector4.one);
        }
    }

    public class StatsState : State
    {
        public MenuHouse menu_house;
        bool first_tick = true;

        float wheel_angle = -6.749995f;
        int shop_position = 620;
        string cat_anim = "idle";
        int cat_frame;

        int text_left_edge = 960 - 80;
        Vector2 title_size = new Vector2(0.7f, 0.7f);

        int held_option = -1;
        int page_index = 0;
        int pages = 2;

        StringBuilder builder = new StringBuilder(100);

        public StatsState()
        {
            Tracker.ScreenVisit(SCREEN_NAME.STATS);
            menu_house = new MenuHouse();
            menu_house.ignore_pad = true;

            menu_house.addOption(new MenuOption("", Gun.Ammo.SKULL, null));
            menu_house.addOption(new MenuOption("", Gun.Ammo.SKULL, null));
            menu_house.addOption(new MenuOption(MenuText.ReturnToTile,
                                                Gun.Ammo.VEGETABLE, () => {
                AppMain.top_state.Dispose();
                AppMain.top_state = new TitleState(MenuOptions.Stats);
            }, Input.Pad.Cancel_Button));
        }

        public override void tick()
        {
            update_input();
            menu_house.tick();
            update_shop();

            MoneyGuy.me.tick();

            if (AppMain.back) {
                AppMain.back = false;
                AppMain.top_state.Dispose();
                AppMain.top_state = new TitleState(MenuOptions.Stats);
            }
        }

        public override void draw()
        {
            base.draw();

            MoneyGuy.me.draw();
            AppMain.textures.house.draw(0, new Vector2(Puzzle.grid_left + 95, Puzzle.grid_top + 144), Vector2.one, Vector4.one);
            menu_house.draw();
            draw_shop();
            draw_stats();
        }

        void update_shop()
        {
            if (first_tick) {
                Game.instance = null;
                first_tick = false;
            }

            cat_frame += 10;
            if (cat_frame > 500) {
                cat_frame = 0;
                switch (Util.rng.Next (4))
                {
                    case 0: cat_anim = "idle"; break;
                    case 1: cat_anim = "blink"; break;
                    case 2: cat_anim = "paw"; break;
                    case 3: cat_anim = "tail"; break;
                }
            }
        }

        void update_input()
        {
            Vector2 size = new Vector2(48, 48);
            Vector2[] buttons = { new Vector2(text_left_edge - 60, 390),
                                  new Vector2(text_left_edge - 470, 390) };

            for (int t = 0; t < Input.touches.Count; ++t) {
                if (Input.touches[t].status == Touch.Status.DOWN ||
                    Input.touches[t].status == Touch.Status.MOVE) {
                    for (int i = 0; i < buttons.Length; ++i) {
                        if (Input.touches[t].position.x > buttons[i].x - size.x &&
                            Input.touches[t].position.x < buttons[i].x + size.x &&
                            Input.touches[t].position.y > buttons[i].y - size.y &&
                            Input.touches[t].position.y < buttons[i].y + size.y) {
                            held_option = i;
                            return;
                        }
                        held_option = -1;
                    }
                }
                else if (Input.touches[t].status == Touch.Status.UP) {
                    for (int i = 0; i < buttons.Length; ++i) {
                        if (held_option == -1) { return; }

                        page_index = Mathf.Clamp(page_index + (held_option == 0 ? -1 : 1), 0, pages);
                        held_option = -1;
                        Choom.PlayEffect(SoundAssets.UISelect);
                    }
                }
            }

            if (Input.Pad.Cancel.WasPressed && menu_house.options[2].selected != null) {
                menu_house.options[2].selected();
            }

            Vector2 direction = Util.keyRepeat("stats_repeat", Vector2.zero, 20, 10, Input.Pad.Move);
            if (direction.x < 0) { page_index = Mathf.Clamp(page_index - 1, 0, pages); }
            if (direction.x > 0) { page_index = Mathf.Clamp(page_index + 1, 0, pages); }
        }

        void draw_shop()
        {
            AppMain.textures.store_board.draw(0, new Vector2(shop_position, 255), new Vector2(-1, 1), Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.leaderboard, new Vector2(shop_position, 60), new Vector2(-1, 1), Vector4.one);
            AppMain.textures.cat.draw(cat_anim, cat_frame,
                                      new Vector2(shop_position + 250, 50),
                                      new Vector2(-1, 1), 0, Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.wheel,
                                            new Vector2(shop_position + 250, 475),
                                            Vector2.one, wheel_angle, Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.wheel,
                                         new Vector2(shop_position - 250, 475),
                                         Vector2.one, wheel_angle + 1, Vector4.one);
        }

        void draw_stats()
        {
            builder.Length = 0;

            builder.Append(MenuText.StatsTitle);
            Text.Draw(new Vector2(text_left_edge - 180, 140), builder.ToString(), Vector2.one, Vector4.one);

            switch (page_index)
            {
                case 0: {
                    int y_position = 180;

                    builder.Length = 0;
                    builder.Append(MenuText.BestHardcoreScore);
                    Text.Draw(new Vector2(text_left_edge, y_position), builder.ToString(), title_size, Vector4.one);

                    y_position = 218;
                    int y_spacing = 32;

                    for (int i = 0; i < 5; i++) {
                        int amount;
                        int day;

                        builder.Length = 0;
                        builder.Append((i + 1).ToString()).Append(": $");
                        if (i < DataStorage.BestHardcoreScores.Count) {
                            amount = DataStorage.BestHardcoreScores[i].Item1;
                            day = DataStorage.BestHardcoreScores[i].Item2;
                            builder.Append(amount > 999999 ? "999999+" : amount.ToString())
                                   .Append(", ").Append(Game.dayName(day));
                        }
                        else {
                            builder.Append("0, Day X");
                        }

                            Text.Draw(new Vector2(text_left_edge - 56, y_position + (y_spacing * i)),
                                      builder.ToString(), new Vector2(0.6f, 0.6f), Vector4.one);
                    }

                    AppMain.textures.arrow.draw(0, new Vector2(text_left_edge - 470, 390), new Vector2(-0.8f, 0.8f), Vector4.one);
               } break;
                case 1: {
                    int most = -1;
                    int max = -1;
                    for (int i = 0; i < 10; i++) {
                        if (DataStorage.AmmoLoaded[i] > max) {
                            most = i;
                            max = DataStorage.AmmoLoaded[i];
                        }
                    }

                    int y_position = 145;
                    int y_spacing = 50;
                    int y_counter = 0;

                    builder.Length = 0;
                    builder.Append("Favorite Gun: ").Append(Shop.guns[most].name);
                    Text.Draw(new Vector2(text_left_edge, y_position + (y_spacing * (++y_counter))),
                            builder.ToString(), title_size, Vector4.one);

                    builder.Length = 0;
                    builder.Append("Shots Fired: ").Append(DataStorage.ShotsFired > 9999 ? "9999+" : DataStorage.ShotsFired.ToString());
                    Text.Draw(new Vector2(text_left_edge, y_position + (y_spacing * (++y_counter))),
                            builder.ToString(), title_size, Vector4.one);

                    builder.Length = 0;
                    builder.Append("Times Defeated: ").Append(DataStorage.TimesDefeated > 9999 ? "9999+" : DataStorage.TimesDefeated.ToString());
                    Text.Draw(new Vector2(text_left_edge, y_position + (y_spacing * (++y_counter))),
                            builder.ToString(), title_size, Vector4.one);

                    builder.Length = 0;
                    builder.Append("Best Match Streak: ").Append(DataStorage.MatchStreak > 9999 ? "9999+" : DataStorage.MatchStreak.ToString());
                    Text.Draw(new Vector2(text_left_edge, y_position + (y_spacing * (++y_counter))),
                            builder.ToString(), title_size, Vector4.one);

                    AppMain.textures.arrow.draw(0, new Vector2(text_left_edge - 470, 390), new Vector2(-0.8f, 0.8f), Vector4.one);
                    AppMain.textures.arrow.draw(0, new Vector2(text_left_edge - 60, 390), new Vector2(0.8f, 0.8f), Vector4.one);
                } break;
                case 2: {
                    builder.Length = 0;
                    builder.Append("Blocks Loaded:");
                    Text.Draw(new Vector2(text_left_edge, 195), builder.ToString(), title_size, Vector4.one);

                    int text_left_start = 1160;
                    int x_spacing = -200;
                    int y_position = 165;
                    int y_spacing = 40;
                    int n = 0;
                    for (int x = 2; x <= 3; x++) {
                        for (int y = 2; y <= 6; y++) {
                            builder.Length = 0;
                            builder.Append(x.ToString()).Append("x").Append(y.ToString()).Append(": ")
                                   .Append(DataStorage.BlocksLoaded[n] > 9999 ? "9999+" : DataStorage.BlocksLoaded[n].ToString());
                            Text.Draw(new Vector2(text_left_start + (x_spacing * x), y_position + (y_spacing * y)),
                                    builder.ToString(), title_size, Vector4.one);
                            n++;
                        }
                    }

                    AppMain.textures.arrow.draw(0, new Vector2(text_left_edge - 60, 390), new Vector2(0.8f, 0.8f), Vector4.one);
                } break;
            }
        }
    }

    public class QuitMenu : State
    {
        public MenuHouse menu_house = new MenuHouse();

        public QuitMenu()
        {
            Tracker.ScreenVisit(SCREEN_NAME.QUIT_MENU);
            menu_house = new MenuHouse();
            menu_house.held_option = 1;
            resetMenu();
        }

        public void resetMenu()
        {
            menu_house.addOption(new MenuOption(MenuText.Exit, Gun.Ammo.SKULL, null));
            menu_house.addOption(new MenuOption(MenuText.Yes, Gun.Ammo.VEGETABLE, () => {
                Platform.Quit();
            }));
            menu_house.addOption(new MenuOption(MenuText.No, Gun.Ammo.DRAGON,
            () => {
                AppMain.top_state.Dispose();
                AppMain.top_state = new TitleState(MenuOptions.Title);
            }));
        }

        bool first_tick = true;

        public override void tick()
        {
            if (first_tick)
            {
                Game.instance = null;
                first_tick = false;
            }

            menu_house.tick();
            MoneyGuy.me.tick();

            if (AppMain.back)
            {
                AppMain.back = false;
                Platform.Quit();
            }
        }

        public override void draw()
        {
            base.draw();

            MoneyGuy.me.draw();

            AppMain.textures.house.draw(0, new Vector2(Puzzle.grid_left + 95, Puzzle.grid_top + 144), Vector2.one, Vector4.one);
            menu_house.draw();
        }
    }

    public class PickDayState : State
    {
        public MenuHouse menu_house = new MenuHouse();
        int selected_day;
        int max_day;
        int held_option = -1;
        bool play_pressed;

        bool first_tick = true;
        float wheel_angle = -6.749995f;
        int shop_position = 620;
        string cat_anim = "idle";
        int cat_frame;

        bool left_select = false;
        int arrow_y = 390;
        int text_left_edge = 960 - 80;
        Vector2 title_size = new Vector2(0.8f, 0.8f);
        StringBuilder builder = new StringBuilder(100);

        public PickDayState()
        {
            Tracker.ScreenVisit(SCREEN_NAME.EARLIER_DAY);

            menu_house = new MenuHouse();
            menu_house.ignore_pad = true;

            menu_house.addOption(new MenuOption(MenuText.Start, Gun.Ammo.DRAGON, () => {
                #if TRACKING
                string[] equippedNames = new string[Shop.max_equipped];
                int equip_index = 0;
                for (int i = 0; i < DataStorage.NumberOfGuns; ++i) {
                    if (!DataStorage.GunEquipped[i]) { continue; }
                    equippedNames[equip_index++] = Shop.guns[i].name;
                }
                Tracker.StartMode(Game.dayName(MetaState.wave_number), equippedNames, DataStorage.Money);
                #endif

                AppMain.top_state.Dispose();
#if LOADING_SCREEN
                AppMain.top_state = new LoadState(() => {
                    MetaState.reset(selected_day);
                    return new Game();
                }, DataStorage.StartOnWave / 3 % 5);
#else

                MetaState.reset(selected_day);
                AppMain.top_state = new Game();
#endif
            }, Input.Pad.Submit_Button));
            menu_house.addOption(new MenuOption("", Gun.Ammo.SKULL, null));
            menu_house.addOption(new MenuOption(MenuText.ReturnToTile, Gun.Ammo.VEGETABLE, () => {
                AppMain.top_state.Dispose();
                AppMain.top_state = new TitleState(MenuOptions.Continue);
            }, Input.Pad.Cancel_Button));

            max_day = DataStorage.StartOnWave;
            selected_day = max_day;
        }

        public override void tick()
        {
            update_input();
            menu_house.tick();
            update_shop();

            MoneyGuy.me.tick();

            if (AppMain.back) {
                AppMain.back = false;
                AppMain.top_state.Dispose();
                AppMain.top_state = new TitleState(MenuOptions.Continue);
            }
        }

        public override void draw()
        {
            base.draw();

            MoneyGuy.me.draw();
            AppMain.textures.house.draw(0, new Vector2(Puzzle.grid_left + 95, Puzzle.grid_top + 144), Vector2.one, Vector4.one);
            menu_house.draw();
            draw_shop();
            draw_selection();
        }

        void update_shop()
        {
            if (first_tick) {
                Game.instance = null;
                first_tick = false;
            }

            cat_frame += 10;
            if (cat_frame > 500) {
                cat_frame = 0;
                switch (Util.rng.Next (4))
                {
                    case 0: cat_anim = "idle"; break;
                    case 1: cat_anim = "blink"; break;
                    case 2: cat_anim = "paw"; break;
                    case 3: cat_anim = "tail"; break;
                }
            }
        }

        void draw_shop()
        {
            AppMain.textures.store_board.draw(0, new Vector2(shop_position, 255), new Vector2(-1, 1), Vector4.one);
            AppMain.textures.cat.draw(cat_anim, cat_frame,
                                      new Vector2(shop_position + 250, 50),
                                      new Vector2(-1, 1), 0, Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.wheel,
                                        new Vector2(shop_position + 250, 475),
                                        Vector2.one, wheel_angle, Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.wheel,
                                        new Vector2(shop_position - 250, 475),
                                        Vector2.one, wheel_angle + 1, Vector4.one);
        }

        void draw_selection()
        {
            builder.Length = 0;
            builder.Append(MenuText.PickADay);
            Text.Draw(new Vector2(text_left_edge - 125, 140), builder.ToString(), Vector2.one, Vector4.one);

            AppMain.textures.pick_a_day.draw(0, new Vector2(text_left_edge - 260, 270),
                                             Vector2.one,
                                             !play_pressed ? Vector4.one :
                                             new Vector4(.75f, .75f, .75f, .5f));

            int y_position = 270;

            builder.Length = 0;
            builder.Append(Game.dayName(selected_day));

            int offset = 0;
            switch (selected_day % 3) { case 2: { offset = 10; } break; }
            if (selected_day > 296) { offset += 19; }
            else if (selected_day > 26) { offset += 11; }
            Text.Draw(new Vector2(text_left_edge - 150 + offset, y_position),
                      builder.ToString(), title_size, Vector4.one);

            float ease = Mathf.Abs(Mathf.Sin(AppMain.frame / 10f)) * 16;

            if (selected_day != 0) {
                AppMain.textures.arrow.draw(0, new Vector2(text_left_edge - 60 + (left_select ? ease : 0), arrow_y),
                                            new Vector2(0.8f, 0.8f),
                                            held_option == 1 ? new Vector4(.9f, .9f, .9f, .9f) :
                                                               new Vector4(.5f, .5f, .5f, .5f),
                                            1);
            }

            if (selected_day != max_day) {
                AppMain.textures.arrow.draw(0, new Vector2(text_left_edge - 470 + (!left_select ? ease : 0), arrow_y),
                                            new Vector2(-0.8f, 0.8f),
                                            held_option == 0 ? new Vector4(.9f, .9f, .9f, .9f) :
                                                               new Vector4(.2f, .2f, .2f, .2f),
                                            1);
            }
        }

        void update_input()
        {
            Vector2 play = new Vector2(text_left_edge - 260, 270);
            Vector2 play_size = new Vector2(158, 53);
            Vector2 size = new Vector2(48, 48);
            Vector2[] buttons = { new Vector2(text_left_edge - 60, 390),
                                  new Vector2(text_left_edge - 470, 390) };

            for (int t = 0; t < Input.touches.Count; ++t) {
                if (Input.touches[t].status == Touch.Status.DOWN ||
                    Input.touches[t].status == Touch.Status.MOVE) {
                    for (int i = 0; i < buttons.Length; ++i) {
                        if (Input.touches[t].position.x > buttons[i].x - size.x &&
                              Input.touches[t].position.x < buttons[i].x + size.x &&
                              Input.touches[t].position.y > buttons[i].y - size.y &&
                              Input.touches[t].position.y < buttons[i].y + size.y) {
                            held_option = i;
                            play_pressed = false;
                            return;
                        }

                        held_option = -1;
                    }

                    if (Input.touches[t].position.x > play.x - play_size.x &&
                                            Input.touches[t].position.x < play.x + play_size.x &&
                                            Input.touches[t].position.y > play.y - play_size.y &&
                                            Input.touches[t].position.y < play.y + play_size.y) {
                        play_pressed = true;
                        held_option = -1;
                        return;
                    }
                    play_pressed = false;
                }
                else if (Input.touches[t].status == Touch.Status.UP) {
                    for (int i = 0; i < buttons.Length; ++i) {
                        if (held_option != -1) {
                            selected_day = Mathf.Clamp(selected_day + (held_option == 0 ? -1 : 1), 0, max_day);
                            left_select = held_option == 0;

                            held_option = -1;
                            Choom.PlayEffect(SoundAssets.UISelect);
                        }

                        if (play_pressed) { menu_house.options[0].selected(); return; }
                    }
                }
            }

            Vector2 direction = Util.keyRepeat("pick_day_repeat", Vector2.zero, 20, 3, Input.Pad.Move);
            if (direction.x < 0 && selected_day > 0)  {
                selected_day--;
                left_select = true;
                held_option = 1;
            }
            else if (direction.x > 0 && selected_day < max_day) {
                selected_day++;
                left_select = false;
                held_option = 0;
            }

            if (Input.Pad.Submit.WasPressed && menu_house.options[0].selected != null) { menu_house.options[0].selected(); }
            if (Input.Pad.Cancel.WasPressed && menu_house.options[2].selected != null) { menu_house.options[2].selected(); }
        }
    }

    public partial class Shop : State
    {
        public MenuHouse menu_house = new MenuHouse();
        public float wheel_angle = 0;
        public bool switch_gun = false;
        public int swap_slot = 0;

        public int shop_position = 1500;
        public int selected_gun = 0;

        public string cat_anim = "idle";
        public int cat_frame = 0;

        public MoveGrid move_grid;
        bool first_tick = true;

        StringBuilder builder = new StringBuilder(100);

        public Shop()
        {
            Tracker.ScreenVisit(SCREEN_NAME.SHOP);
            Choom.Play("Music/title");
            selectGun(0);

            rebuildMoveGrid();
        }

        public override void tick()
        {
            if (first_tick) {
                Game.instance = null;
                first_tick =  false;
            }

            MoneyGuy.me.tick();

            if (!AppMain.back && shop_position > 620) {
                shop_position -= 20;
                wheel_angle -= 0.15f;
            }

            base.tick();

            if (!AppMain.back) { menu_house.tick(); }

            if (Input.Pad.Submit.WasPressed && menu_house.options[0].selected != null) { menu_house.options[0].selected(); }
            if (Input.Pad.Y.WasPressed && menu_house.options[1].selected != null) { menu_house.options[1].selected(); }
            if (Input.Pad.Cancel.WasPressed && menu_house.options[2].selected != null) { menu_house.options[2].selected(); }

            cat_frame += 10;
            if (cat_frame > 500) {
                cat_frame = 0;
                switch (Util.rng.Next(4))
                {
                case 0: cat_anim = "idle"; break;
                case 1: cat_anim = "blink"; break;
                case 2: cat_anim = "paw"; break;
                case 3: cat_anim = "tail"; break;
                }
            }

            Vector2 direction = Util.keyRepeat("shop_pad_repeat", Vector2.zero, 20, 10, Input.Pad.Move);
            int? new_pos = null;
            if (direction.x > 0) new_pos = move_grid.get(selected_gun).right;
            if (direction.x < 0) new_pos = move_grid.get(selected_gun).left;
            if (direction.y > 0) new_pos = move_grid.get(selected_gun).down;
            if (direction.y < 0) new_pos = move_grid.get(selected_gun).up;
            if (new_pos != null) selectGun(new_pos.Value);

            for (int t = 0; t < Input.touches.Count; ++t) {
                if (Input.touches[t].status == Touch.Status.DOWN) {
                    for (int i = -3; i < DataStorage.NumberOfGuns; ++i) {
                        Vector2 pos = buttonPosition(i);
                        Vector2 size = new Vector2((i == -1 || i == -2 || i == -3)? 64 : 32, i == -1 ? 64 : 32);

                        if (!(Input.touches[t].position.x > pos.x - size.x &&
                              Input.touches[t].position.x < pos.x + size.x &&
                              Input.touches[t].position.y > pos.y - size.y &&
                              Input.touches[t].position.y < pos.y + size.y)) { continue; }

                        if (switch_gun && i < 0) { continue; }
                        if (switch_gun && !DataStorage.GunEquipped[i]) { continue; }

                        selectGun(i);
                    }
                }
            }

            if (AppMain.back) {
                if (shop_position < 1500) {
                    shop_position += 20;
                    wheel_angle += 0.15f;
                }
                else {
                    AppMain.back = false;
                    AppMain.top_state.Dispose();
                    AppMain.top_state = new TitleState(MenuOptions.Shop);
                }
            }
        }

        public override void draw()
        {
            base.draw();

            MoneyGuy.me.draw();

            AppMain.textures.title.draw(0, new Vector2(title_position, 544 / 2 - 65 + (float)Math.Sin(AppMain.frame / 50.0f) * 5),
                                        new Vector2 (-1, 1), Vector4.one);
            AppMain.textures.house.draw(0, new Vector2(Puzzle.grid_left + 95, Puzzle.grid_top + 144),
                                        Vector2.one, Vector4.one);

            if (switch_gun) {
                Text.Draw(new Vector2(shop_position, 544 / 2) + new Vector2(250, -20),
                          builder.ToString(), Vector2.one * 0.52f, Vector4.one,
                          new Vector4(0.99f, 0.99f, 0.39f, 1), 67, true);
            }
            else {
                Text.Draw(new Vector2(shop_position, 544 / 2) + new Vector2(250, -20),
                          builder.ToString(), Vector2.one * 0.52f, Vector4.one);
            }

            AppMain.textures.store_board.draw(0, new Vector2(shop_position, 255), new Vector2(-1, 1), Vector4.one);
            AppMain.textures.cat.draw(cat_anim, cat_frame, new Vector2(shop_position + 250, 50), new Vector2(-1, 1), 0, Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.wheel, new Vector2(shop_position + 250, 475), Vector2.one, wheel_angle, Vector4.one);
            AppMain.textures.store.draw((int)store.Sprites.wheel, new Vector2(shop_position - 250, 475), Vector2.one, wheel_angle + 1, Vector4.one);

            for (int i = -1; i < DataStorage.NumberOfGuns; ++i)
            {
                Vector2 pos = buttonPosition(i);

                if (i == -1)
                {
                    AppMain.textures.hud.draw((int)hud.Sprites.heart, buttonPosition(i), Vector2.one / 96 * 128, Vector4.one);
                }
                else if (i == -2)
                {
                    //AppMain.textures.menu_shop.draw((int)store.Sprites.gunhouse_credits_button, buttonPosition(i),
                                                    //new Vector2 (-0.5f, 0.5f) / 96 * 128, Vector4.one);
                }
                else if (i == -3)
                {
                    //AppMain.textures.menu_shop.draw((int)store.Sprites.gunhouse_options_button1, buttonPosition (i),
                    //                                new Vector2(-0.5f, 0.5f) / 96 * 128, Vector4.one);
                }
                else
                {
                    AppMain.textures.elements.draw(i + 10, pos, new Vector2(-1, 1), Vector4.one);

                    if (!DataStorage.GunOwned[i] || (switch_gun && !DataStorage.GunEquipped[i] && i != swap_slot))
                    {
                        AppMain.textures.elements.draw((int)elements.Sprites.desaturate, pos, Vector2.one, new Vector4(0, 0, 0, 0.6f));
                    }

                    if (DataStorage.GunEquipped[i] && !switch_gun)
                    {
                        AppMain.textures.store.draw((int)store.Sprites.shield,
                                                    pos + new Vector2(-15.0f, 15.0f), new Vector2(0.25f, 0.25f), Vector4.one);
                    }
                }

                if (switch_gun)
                {
                    if (i == swap_slot)
                    {
                        AppMain.textures.store.draw((int)store.Sprites.square_small,
                                                    pos, Vector2.one, Vector4.one);
                    }

                    if (i == selected_gun)
                    {
                        AppMain.textures.store.draw((int)store.Sprites.reticle, pos,
                                                    Vector2.one * (1.05f + (float)Math.Sin(AppMain.frame / 10.0f) * 0.05f),
                                                    Vector4.one);
                    }
                }
                else if (i == selected_gun)
                {
                    AppMain.textures.store.draw(i < -1 ? (int)store.Sprites.square_small :
                                                i < 0 ? (int)store.Sprites.square_large : (int)store.Sprites.square_small,
                        pos, i < -1 ? Vector2.one * (1.5f + (float)Math.Sin(AppMain.frame / 10.0f) * 0.05f)
                        : Vector2.one * (1.05f + (float)Math.Sin(AppMain.frame / 10.0f) * 0.05f), Vector4.one);
                }
            }

            menu_house.draw();
        }

        public void rebuildMoveGrid()
        {
            move_grid = new MoveGrid();
            if (switch_gun) {
                int? last = null;
                for (int x = 0; x < 5; x++) {
                    if (DataStorage.GunEquipped[x]) {
                        move_grid.addNode(new MoveGrid.Node(x, last));
                        last = x;
                    }
                    else if (DataStorage.GunEquipped[x + 5]) {
                        move_grid.addNode(new MoveGrid.Node(x + 5, last));
                        last = x + 5;
                    }
                }
            }
            else {
                for (int x = 0; x < 5; x++) {
                    for (int y = 0; y < 2; y++) {
                        int? left = null, up = null;
                        if (x > 0) { left = y * 5 + x - 1; }
                        if (y > 0) { up = y * 5 + x - 5; }
                        move_grid.addNode(new MoveGrid.Node(y * 5 + x, left, null, up));
                    }
                }

                /* NOTE(shane): for some reason the order here matters. */
                move_grid.addNode(new MoveGrid.Node(-1, 4));
                //move_grid.addNode(new MoveGrid.Node(-2, 9, null, -1, null));
                move_grid.get(9).right = -1;
                //move_grid.get(-1).down = -2;
            }
        }

        public void resetMenu()
        {
            menu_house = new MenuHouse();
            menu_house.ignore_pad = true;

            if (selected_gun == -1) {
                if (upgradeCost(-1) > DataStorage.Money) {
                    if (DataStorage.Hearts >= 6) {
                        menu_house.addOption(new MenuOption(MenuText.AddArmor +
                                                            UpgradeAmount(-1), Gun.Ammo.DRAGON, null, Input.Pad.Submit_Button));
                    }
                    else {
                        menu_house.addOption(new MenuOption(MenuText.AddHeart +
                                                            UpgradeAmount(-1), Gun.Ammo.DRAGON, null, Input.Pad.Submit_Button));
                    }
                }
                else {
                    if (DataStorage.Hearts >= 6) {
                        menu_house.addOption(new MenuOption(MenuText.AddArmor + UpgradeAmount(-1),
                                                            Gun.Ammo.DRAGON,
                                                            () => {
                            MoneyGuy.me.addMoney(-upgradeCost(-1));
                            DataStorage.Armor++;
                            selectGun(selected_gun);
                            Tracker.ShopItemUpgrade("armor", DataStorage.Armor);
                        }, Input.Pad.Submit_Button));
                    }
                    else {
                        menu_house.addOption(new MenuOption(MenuText.AddHeart + UpgradeAmount(-1),
                                                            Gun.Ammo.DRAGON,
                                                            () => {
                            MoneyGuy.me.addMoney(-upgradeCost(-1));
                            DataStorage.Hearts++;
                            selectGun(selected_gun);
                            Tracker.ShopItemUpgrade("heart", DataStorage.Hearts);
                        }, Input.Pad.Submit_Button));
                    }
                }

                if (upgradeCost(-2) > DataStorage.Money) {
                    menu_house.addOption(new MenuOption(MenuText.AddHealing +
                                                        UpgradeAmount(-2), Gun.Ammo.IGLOO, null, Input.Pad.Alt_Button));
                }
                else {
                    menu_house.addOption(new MenuOption(MenuText.AddHealing + UpgradeAmount(-2),
                                                        Gun.Ammo.IGLOO,
                                                        () => {
                        MoneyGuy.me.addMoney(-upgradeCost(-2));
                        DataStorage.Healing++;
                        selectGun(selected_gun);
                        Tracker.ShopItemUpgrade("healing", DataStorage.Healing);
                    }, Input.Pad.Alt_Button));
                }
            }
            else if (selected_gun == -2) {
                //menu_house.addOption(new MenuOption(MenuText.WatchCredits,
                //                                    Gun.Ammo.DRAGON, () => {
                //    MoneyGuy.me.addMoney(100);
                //    AppMain.top_state.Dispose();
                //    AppMain.top_state = new CreditState();
                //}));

                //menu_house.addOption(new MenuOption("", Gun.Ammo.SKULL, null));
            }
            else if (selected_gun == -3) {
                /* NOTE(shane): this option is not here anymore. */
                //menu_house.addOption(new MenuOption("A: Options", Gun.Ammo.SKULL, () => {
                //    AppMain.top_state.Dispose();
                //    AppMain.top_state = new Options(false);
                //}));

                //menu_house.addOption(new MenuOption("", Gun.Ammo.VEGETABLE, null));
            }
            else if (DataStorage.GunOwned[selected_gun] && !switch_gun) {
                if (upgradeCost(selected_gun) > DataStorage.Money) {
                    menu_house.addOption(new MenuOption(MenuText.Upgrade + UpgradeAmount(selected_gun),
                                                        Gun.Ammo.DRAGON, null, Input.Pad.Submit_Button));
                }
                else {
                    menu_house.addOption(new MenuOption(MenuText.Upgrade + UpgradeAmount(selected_gun),
                                                        Gun.Ammo.DRAGON,
                                                        () => {
                        MoneyGuy.me.addMoney(-upgradeCost(selected_gun));
                        DataStorage.GunPower[selected_gun]++;
                        selectGun(selected_gun);
                        Tracker.ShopItemUpgrade(guns[selected_gun].name, DataStorage.GunPower[selected_gun]);
                    }, Input.Pad.Submit_Button));
                }

                if (DataStorage.GunEquipped[selected_gun]) {
                    if (DataStorage.GunPower[selected_gun] > 1) {
                        menu_house.addOption(new MenuOption(MenuText.Refund +
                                                            upgradeRefund(selected_gun).ToString(),
                                                            Gun.Ammo.IGLOO,
                                                            () => {
                            MoneyGuy.me.addMoney(upgradeRefund(selected_gun));
                            DataStorage.GunPower[selected_gun]--;
                            selectGun(selected_gun);
                            Tracker.ShopItemDowngrade(guns[selected_gun].name, DataStorage.GunPower[selected_gun]);
                        }, Input.Pad.Alt_Button));
                    }
                    else {
                        menu_house.addOption(new MenuOption("", Gun.Ammo.SKULL, null));
                    }
                }
                else {
                    menu_house.addOption(new MenuOption(MenuText.Equip, Gun.Ammo.IGLOO, () => { equipGun(); }, Input.Pad.Alt_Button));
                }
            }
            else {
                if (!switch_gun) {
                    int purchase_cost = guns[selected_gun].cost;

                    if (purchase_cost > DataStorage.Money) {
                        menu_house.addOption(new MenuOption(MenuText.Purchase + purchase_cost.ToString(), Gun.Ammo.DRAGON, null, Input.Pad.Submit_Button));
                    }
                    else {
                        menu_house.addOption(new MenuOption(MenuText.Purchase + purchase_cost.ToString(),
                                                            Gun.Ammo.DRAGON,
                                                            () => {
                            Tracker.ShopItemPurchased(guns[selected_gun].name);
                            MoneyGuy.me.addMoney(-purchase_cost);
                            DataStorage.GunOwned[selected_gun] = true;
                            equipGun();
                            resetMenu();
                        }, Input.Pad.Submit_Button));
                    }
                    menu_house.addOption(new MenuOption("", Gun.Ammo.SKULL, null));
                }
                else {
                    menu_house.addOption(new MenuOption(MenuText.Swap, Gun.Ammo.DRAGON, () => {
                        DataStorage.GunEquipped[selected_gun] = false;
                        DataStorage.GunEquipped[swap_slot] = true;
                        switch_gun = false;
                        selectGun(swap_slot);
                        resetMenu();
                        rebuildMoveGrid();
                    }, Input.Pad.Submit_Button));

                    menu_house.addOption(new MenuOption(MenuText.Cancel, Gun.Ammo.IGLOO, () => {
                        switch_gun = false;
                        selectGun(swap_slot);
                        resetMenu();
                        rebuildMoveGrid();
                    }, Input.Pad.Alt_Button));
                }
            }

            menu_house.addOption(new MenuOption(MenuText.ReturnToTile, Gun.Ammo.VEGETABLE, () => {
                AppMain.back = true;
                Platform.SaveStore();
            }, Input.Pad.Cancel_Button));
        }

        public int upgradeCost(int n)
        {
            if (n == -1) {
                if (DataStorage.Hearts < 6) {
                    return (int)(Math.Pow(heart_price_multiplier, DataStorage.Hearts - 1) * base_heart_price);
                }
                return (int)(Math.Pow(armor_price_multiplier, DataStorage.Armor) * base_armor_price);
            }
            else if (n == -2) {
                return (int)(Math.Pow(healing_price_multiplier, DataStorage.Healing)) * base_healing_price;
            }
            else if (DataStorage.GunOwned[n]) {
                return (int)(Math.Pow(upgrade_price_multiplier, DataStorage.GunPower[n] - 1) * base_upgrade_price);
            }
            else {
                return guns[selected_gun].cost;
            }
        }

        public string UpgradeAmount(int id)
        {
            return upgradeCost(id) > 9999999 ? "9999999+" : upgradeCost(id).ToString();
        }

        public int upgradeRefund(int n)
        {
            return (int)(Math.Pow(upgrade_price_multiplier, DataStorage.GunPower[n] - 2) * base_upgrade_price);
        }

        public void equipGun()
        {
            if (selected_gun >= DataStorage.NumberOfGuns / 2 &&
                DataStorage.GunEquipped[selected_gun - DataStorage.NumberOfGuns / 2]) {
                DataStorage.GunEquipped[selected_gun - DataStorage.NumberOfGuns / 2] = false;
            }
            else if (selected_gun < DataStorage.NumberOfGuns / 2 &&
                     DataStorage.GunEquipped[selected_gun + DataStorage.NumberOfGuns / 2]) {
                DataStorage.GunEquipped[selected_gun + DataStorage.NumberOfGuns / 2] = false;
            }
            else {
                switch_gun = true;
                rebuildMoveGrid();
                swap_slot = selected_gun;

                int i = 0;
                while (!DataStorage.GunEquipped[i]) { i++; }
                selected_gun = i;
            }

            DataStorage.GunEquipped[selected_gun] = true;

            selectGun(selected_gun);
            resetMenu();
        }

        public void selectGun(int n)
        {
            builder.Length = 0;

            selected_gun = n;

            switch (n)
            {
            case -1: {
                builder.Append(MenuText.Hearts).Append(DataStorage.Hearts.ToString())
                       .Append(MenuText.HealingLevel).Append(DataStorage.Healing.ToString());
                if (DataStorage.Armor > 0) {
                    builder.Append(MenuText.ArmorLevel).Append(DataStorage.Armor.ToString());
                }
            } break;
            //case -2: { builder.Append(MenuText.GameByPeople); } break;
            //case -3: { builder.Append(MenuText.GetOptions); } break;
            default: {
                builder.Append(guns[selected_gun].name);

                if (!DataStorage.GunOwned[n]) {
                    builder.Append(MenuText.Cost).Append(guns[selected_gun].cost.ToString());
                }
                else {
                    if (DataStorage.GunEquipped[n]) {
                        builder.Append(MenuText.EquippedLevel).Append(DataStorage.GunPower[n]);
                    }
                    else {
                        builder.Append(MenuText.OwnedLevel).Append(DataStorage.GunPower[n]);
                    }
                }

                builder.Append("\n\n").Append(Text.Wrap(guns[selected_gun].description, 37));
                if (switch_gun) { builder.Append(MenuText.MoveX); }
            } break;
            }

            resetMenu();
        }

        public Vector2 buttonPosition(int n)
        {
            if (n == -1) { return new Vector2(shop_position, 544 / 2) + new Vector2(-200, -150 + 67 / 2); }
            //if (n == -2) { return new Vector2(shop_position, 544 / 2) + new Vector2(-200, -150 + 67 / 2 + 200); }
            //if (n == -3) { return new Vector2(shop_position, 544 / 2) + new Vector2(-200, -150 + 67 / 2 + 200); }

            int row = n / (DataStorage.NumberOfGuns / 2);
            int col = n % (DataStorage.NumberOfGuns / 2);

            return new Vector2(shop_position, 544 / 2) + new Vector2(225, -150) + new Vector2(-col * 70, row * 67);
        }

        public int buttonSprite(int n)
        {
            return n + 5;
        }
    }

    public struct GunUpgrade
    {
        public Gun.Ammo ammo;
        public string name;
        public string description;
        public int cost;

        public GunUpgrade(Gun.Ammo ammo_, string name_, string description_, int cost_)
        {
            ammo = ammo_;
            name = name_;
            description = description_;
            cost = cost_;
        }

        public static float upgradeMultiplier(Gun.Ammo ammo)
        {
            if (MetaState.hardcore_mode) {
                return MetaState.logCurve(1,
                                          Difficulty.gun_upgrade_base,
                                          Difficulty.gun_upgrade_steepness,
                                          Difficulty.gun_upgrade_amplification) / MetaState.monster_armor_coefficient;
            }

            for (int i = 0; i < Shop.guns.Length; ++i) {
                if (Shop.guns[i].ammo != ammo) { continue; }

                return MetaState.logCurve(DataStorage.GunPower[i],
                                          Difficulty.gun_upgrade_base,
                                          Difficulty.gun_upgrade_steepness,
                                          Difficulty.gun_upgrade_amplification) / MetaState.monster_armor_coefficient;
            }

            return 1.0f;
        }
    }

    public class MoveGrid
    {
        public class Node
        {
            public int number;
            public int? left, right, up, down;

            public Node(int number, int? left=null, int? right=null, int? up=null, int? down=null)
            {
                this.number = number;
                this.left = left;
                this.right = right;
                this.up = up;
                this.down = down;
            }
        }

        List<Node> nodes = new List<Node>();

        public Node addNode(Node n)
        {
            nodes.Add(n);

            for (int i = 0; i < nodes.Count; ++i) {
                if (n.left == nodes[i].number) { nodes[i].right = n.number; }
                if (n.right == nodes[i].number) { nodes[i].left = n.number; }
                if (n.up == nodes[i].number) { nodes[i].down = n.number; }
                if (n.down == nodes[i].number) { nodes[i].up = n.number; }
            }

            return n;
        }

        public Node get(int index)
        {
            for (int i = 0; i < nodes.Count; ++i) {
                if (nodes[i].number == index) return nodes[i];
            }

            return null;
        }

        public void deleteNode(Node n)
        {
            nodes.Remove(n);

            for (int i = 0; i < nodes.Count; ++i) {
                if (nodes[i].left == n.number) { nodes[i].left = n.left; }
                if (nodes[i].right == n.number) { nodes[i].right = n.right; }
                if (nodes[i].up == n.number) { nodes[i].up = n.up; }
                if (nodes[i].down == n.number) { nodes[i].down  = n.down; }
            }
        }
    }

    public class CreditState : State
    {
        #if LOADING_SCREEN
        int loadscreen;
        string protip;
        float textWidth;
        #endif

        public CreditState(bool autoMove = true)
        {
            Tracker.ScreenVisit(SCREEN_NAME.CREDITS);
            MetaState.end_game = false;

            #if LOADING_SCREEN
            loadscreen = Util.rng.Next(5);
            protip = Text.Wrap(Story.tips[Util.rng.Next(Story.tips.Length)], 50);
            textWidth = Text.Size(AppMain.textures.font, protip).x;
            #endif
            AppEntry.LoadCreditsSceneAsync(autoMove);
        }

        public override void tick() { }
        public override void draw()
        {
            #if LOADING_SCREEN
            AppMain.textures.loading.draw(loadscreen, new Vector2(AppMain.vscreen.x * 0.5f, 545 * 0.5f - 50), new Vector2(-1, 1), Vector4.one);
            Text.Draw(new Vector2(AppMain.vscreen.x * 0.5f, 390) + new Vector2(textWidth * 0.25f, 0), protip, Vector2.one * 0.5f, Vector4.one);
            #endif
        }
    }

#if LOADING_SCREEN
    public class LoadState : State
    {
        public delegate State ToLoad();

        public ToLoad new_state;
        int time;
        int loadscreen;

        string protip;
        float textWidth;

        public LoadState(ToLoad new_state_, int loadscreen_)
        {
            new_state = new_state_;
            loadscreen = loadscreen_;

            if (loadscreen == 5) return;    /* 5 is the launch loading screen */

            AppMain.wipe_textures = true;
            protip = Text.Wrap(Story.tips[Util.rng.Next(Story.tips.Length)], 50);
            textWidth = Text.Size(AppMain.textures.font, protip).x;
        }

        public override void tick()
        {
            if (AppMain.wipe_textures) return;

            /* CHECK WHAT GUNS ARE EQUIPED AND LOAD TEXTURES, LOAD SCORE TEXTURE */

            if (time > (loadscreen == 5 ? 30 : 1)) AppMain.top_state = new_state();
        }

        public override void draw()
        {
            if (AppMain.wipe_textures) return;
            time++;

            AppMain.textures.loading.draw(loadscreen, new Vector2(AppMain.vscreen.x * 0.5f, 545 * 0.5f - 50), new Vector2(-1, 1), Vector4.one);

            if (loadscreen == 5) return;    /* 5 is the launch loading screen */

            Text.Draw(new Vector2(AppMain.vscreen.x * 0.5f, 390) + new Vector2(textWidth * 0.25f, 0), protip, Vector2.one * 0.5f, Vector4.one);
        }
    }
#endif
}
