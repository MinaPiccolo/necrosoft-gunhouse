using UnityEngine;

namespace Necrosoft
{
    public class Poolable : MonoBehaviour 
    {
        [HideInInspector] public int id;

        public void ReturnToPool() { Pool.Return(this); }

        #if UNITY_EDITOR
        [HideInInspector] public int poolSize;
        #endif
    }
}