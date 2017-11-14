using UnityEngine;

namespace Gunhouse.Menu
{
    public class OnClickStoreItem : MonoBehaviour { public StoreItem item; }

    public enum StoreItem { StoreDragon, StoreIce, StoreSkull, StoreVegetable,
                            StoreLightning, StoreFlame, StoreFork, StoreBall,
                            StoreBoomerang, StoreWave, StoreHeart, StoreArmor };
}
