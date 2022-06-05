using UnityEngine;

namespace Osiris
{
    public static class CustomRandom
    {
        public static void SetSeed(int seed) {
            Random.InitState(seed);
        }

        /// <summary>
        /// Return a random float number between 0 [inclusive] and 1 [inclusive].
        /// </summary>
        public static float Binomial() {
            return Random.Range(0f, 1f);
        }

        /// <summary>
        /// Return a random float number between min [inclusive] and max [inclusive].
        /// </summary>
        public static float Range(float min, float max) {
            return Random.Range(min, max);
        }

        /// <summary>
        /// Return a random integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int Range(int min, int max) {
            return Random.Range(min, max);
        }
    }
}