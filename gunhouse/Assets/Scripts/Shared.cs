namespace Gunhouse
{
#if BUNDLED
    public enum SceneIndex { Downloading, Main };
#else
    public enum SceneIndex { Main };
#endif
    public enum EntityGroupID { None, Bullets, EnemyBullets, Particles,
                                Orphans, Enemies, House, Dead, Guns, PuzzlePieces };

    public static class ScreenPoints {
        public const int VisibleBounds = 960;
    }

    //public enum StageIndex { town_noon = 1, town_dusk, town_night,
    //                         pyramid_noon, pyramid_dusk, pyramid_night,
    //                         forest_noon, forest_dusk, forest_night,
    //                         skull_noon, skull_dusk, skull_night,
    //                         oakland_noon, oakland_dusk, oakland_night,
    //                         boat_noon, boat_dusk, boat_night,
    //                         space
    //};
}
