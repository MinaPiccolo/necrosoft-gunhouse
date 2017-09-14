namespace Gunhouse
{
#if BUNDLED
    public enum SceneIndex { Downloading, Main, Credits };
#else
    public enum SceneIndex { Main, Credits };
#endif
    public enum EntityGroupID { None, Bullets, EnemyBullets, Particles,
                                Orphans, Enemies, House, Dead, Guns, PuzzlePieces };
}