﻿namespace Gunhouse
{
#if BUNDLED
    public enum SceneIndex { Downloading, Main };
#else
    public enum SceneIndex { Main };
#endif
    public enum EntityGroupID { None, Bullets, EnemyBullets, Particles,
                                Orphans, Enemies, House, Dead, Guns, PuzzlePieces };
}
