namespace Gunhouse
{
#if BUNDLED
    public enum SceneIndex { Downloading, Main, Credits };
#else
    public enum SceneIndex { Main, Credits };
#endif
    public enum EntityGroupID { None, Bullets, EnemyBullets, Particles,
                                Orphans, Enemies, House, Dead, Guns, PuzzlePieces };

    public enum ControllerButton { PS_X, PS_CIRCLE, PS_SQUARE, PS_TRIANGLE,
                                   XBOX_X, XBOX_A, XBOX_B, XBOX_Y,
                                   SWITCH_X, SWITCH_A, SWITCH_B, SWITCH_Y, NONE };
}
