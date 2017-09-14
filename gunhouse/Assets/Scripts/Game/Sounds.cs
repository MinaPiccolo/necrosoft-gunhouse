using UnityEngine;

namespace Gunhouse
{
    public class SoundInfo {
        public string file;
        public float volume;
        public int lastPlayed;
    }

    public class SoundAssets
    {
        public static SoundInfo[] Gatling = new SoundInfo[] {
            new SoundInfo { file = "gatling1", volume = .4f }, new SoundInfo { file = "gatling2", volume = .4f} };
        public static SoundInfo[] Explosion = new SoundInfo[] {
            new SoundInfo { file = "explosion1", volume = .3f }, new SoundInfo { file = "explosion2", volume = .3f },
            new SoundInfo { file = "explosion3", volume = .3f }, new SoundInfo { file = "explosion4", volume = .3f } };
        public static SoundInfo[] HitHouse = new SoundInfo[] {
            new SoundInfo { file = "hithouse1", volume = .8f }, new SoundInfo { file = "hithouse2", volume = .8f },
            new SoundInfo { file = "hithouse3", volume = .8f }, new SoundInfo { file = "hithouse4", volume = .8f } };

        public static SoundInfo EnableGun = new SoundInfo { file = "enablegun", volume = .5f };
        public static SoundInfo DragonShot  = new SoundInfo { file = "dragonshot",volume = .4f };
        public static SoundInfo FlameShot = new SoundInfo { file = "flameshot",volume = .4f };
        public static SoundInfo ForkShot = new SoundInfo { file = "forkshot", volume = .2f };
        public static SoundInfo ForkSpecial = new SoundInfo { file = "forkspecial", volume = .4f };
        public static SoundInfo SkullShot = new SoundInfo { file = "skullshot", volume = .4f };
        public static SoundInfo IceShot = new SoundInfo { file = "iceshot", volume = .3f };
        public static SoundInfo PenguinSkullSpecial = new SoundInfo { file = "penguin-skull-special", volume = 1f };
        public static SoundInfo LightningShot = new SoundInfo { file = "lightningshot", volume = .3f };
        public static SoundInfo LightningSpecial = new SoundInfo { file = "lightningspecial", volume = .5f };
        public static SoundInfo MathShot  = new SoundInfo { file = "mathshot", volume = .7f };
        public static SoundInfo MathSpecial = new SoundInfo { file = "mathspecial", volume = .4f };
        public static SoundInfo SpikeBreak = new SoundInfo { file = "spikebreak", volume = .4f };
        public static SoundInfo BoomerangShot = new SoundInfo { file = "boomerangshot", volume = .4f };
        public static SoundInfo BoomerangSpecial = new SoundInfo { file = "boomerangspecial", volume = .8f };
        public static SoundInfo BeachBallShot = new SoundInfo { file = "beachballshot", volume = .5f };
        public static SoundInfo BeachBallBounce = new SoundInfo { file = "beachballbounce", volume = .5f };
        public static SoundInfo BeachBallRoll = new SoundInfo { file = "beachballroll", volume = .4f };
        public static SoundInfo VegShot = new SoundInfo { file = "vegshot", volume = .2f };
        public static SoundInfo Vegsprout = new SoundInfo { file = "vegsprout", volume = .4f };
        public static SoundInfo IglooFreeze = new SoundInfo { file = "igloofreeze", volume = .3f };
        public static SoundInfo SonicBoom = new SoundInfo { file = "sonicboom", volume = .5f };
        public static SoundInfo LaserShot = new SoundInfo { file = "lasershot", volume = .5f };
        public static SoundInfo HootLaser = new SoundInfo { file = "hootlaser", volume = .5f };

        public static SoundInfo CountDown = new SoundInfo { file = "countdown", volume = .3f };
        public static SoundInfo BlockLand = new SoundInfo { file = "blockland", volume = .4f };
        public static SoundInfo BlockMatch = new SoundInfo { file = "blockmatch", volume = .5f };
        public static SoundInfo BlockFuse = new SoundInfo { file = "blockfuse", volume = .4f };
        public static SoundInfo DoorOpen =  new SoundInfo { file = "dooropen", volume = .4f };
        public static SoundInfo DoorClose = new SoundInfo { file = "doorclose", volume = .4f };
        public static SoundInfo OrphanTake = new SoundInfo { file = "orphantake", volume = .4f };
        public static SoundInfo HealthLow = new SoundInfo { file = "health_low", volume = .6f };
        public static SoundInfo UIConfirm = new SoundInfo { file = "ui_confirm", volume = .4f };
        public static SoundInfo UISelect = new SoundInfo { file = "ui_select", volume = .3f };

        public static SoundInfo EnemyDie = new SoundInfo { file = "enemydie", volume = .9f };
        public static SoundInfo BossDie1 = new SoundInfo { file = "boss_die_1", volume = 1 };
        public static SoundInfo BossDie2 = new SoundInfo { file = "boss_die_2", volume = 1 };
    }
}