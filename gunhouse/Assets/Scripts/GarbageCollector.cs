using UnityEngine;
using System.Collections;

namespace Gunhouse
{
    public class GarbageCollector : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 5.0f)] float limit = 1;
        float counter = 0;

        void Update()
        {
            counter += Time.deltaTime;

            if (counter < limit) { return; }

            counter = 0;
            System.GC.Collect();
        }
    }
}