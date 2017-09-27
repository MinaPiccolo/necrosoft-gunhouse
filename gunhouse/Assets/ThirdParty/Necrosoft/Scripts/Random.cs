using System.Collections.Generic;

namespace Necrosoft.Math
{
    public static class Random
    {
        public static int UniqueRandom(int min, int max, HashSet<int> previousNumbers)
        {
            if (max == previousNumbers.Count) previousNumbers.Clear();

            int number = UnityEngine.Random.Range(min, max);
            while (previousNumbers.Contains(number)) number = UnityEngine.Random.Range(min, max);

            previousNumbers.Add(number);
            return number;
        }
    }
}
