using UnityEngine;

namespace Necrosoft
{
    public class GarbageCollector : MonoBehaviour
    {
        float counter = 0;
        [SerializeField] [Range(0.0F, 5.0F)] float limit;

        void Update()
        {
            counter += Time.deltaTime;

            if (counter < limit) { return; }

            counter = 0;
            System.GC.Collect();
        }
    }
}